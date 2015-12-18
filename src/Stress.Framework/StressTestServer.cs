// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Benchmarks.Utility.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Xunit;

namespace Stress.Framework
{
    public class StressTestServer : IDisposable
    {
        private Process _serverProcess;
        private readonly string _testName;
        private readonly string _testMethodName;
        private readonly int _port;
        private readonly IStressMetricCollector _metricCollector;
        private readonly TestSampleManager _sampleManager;
        private readonly string _command;

        public StressTestServer(
            string testName,
            string testMethodName,
            int port,
            string command,
            IStressMetricCollector metricCollector)
        {
            _testName = testName;
            _testMethodName = testMethodName;
            _port = port;
            _command = command;
            _metricCollector = metricCollector;
            _sampleManager = new TestSampleManager();
        }

        public async Task<StressTestServerStartResult> StartAsync()
        {
            var framework = PlatformServices.Default.Runtime.RuntimeType;
            var fullTestName = $"{_testName}.{framework}.{nameof(_testMethodName)}";
            var testProject = _sampleManager.PrepareSample(fullTestName, _testName);
            Assert.True(testProject != null, $"Fail to set up test project.");

            var logger = _sampleManager.LoggerFactory.CreateLogger(fullTestName);
            logger.LogInformation($"Test project is set up at {testProject}");

            var serverStartInfo = _sampleManager.DnxHelper.BuildStartInfo(testProject, framework, _command);

            _serverProcess = Process.Start(serverStartInfo);

            var result = new StressTestServerStartResult
            {
                ServerHandle = this
            };
            var baseAddress = $"http://localhost:{_port}/";
            var serverVerificationClient = new HttpClient
            {
                BaseAddress = new Uri(baseAddress)
            };

            HttpResponseMessage response = null;
            for (int i = 0; i < 10; ++i)
            {
                try
                {
                    logger.LogInformation($"Pinging {serverVerificationClient.BaseAddress} to ensure server booted properly");
                    response = await serverVerificationClient.GetAsync(serverVerificationClient.BaseAddress);
                    break;
                }
                catch (TimeoutException)
                {
                    logger.LogError("Http client timeout.");
                    break;
                }
                catch (Exception)
                {
                    logger.LogInformation("Failed to ping server. Retrying...");
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    continue;
                }
            }

            if (_serverProcess != null && _serverProcess.HasExited)
            {
                logger.LogError($"Server exited unexpectedly: {_serverProcess.Id}");
            }

            if (response != null)
            {
                logger.LogInformation($"Response {response.StatusCode}");
                response.EnsureSuccessStatusCode();
                logger.LogInformation("Server started successfully");
                result.SuccessfullyStarted = true;
                Client = new RequestTrackingHttpClient(baseAddress, _metricCollector);
            }

            return result;
        }

        public void Dispose()
        {
            Client = null;
            if (!_serverProcess.HasExited)
            {
                _serverProcess?.Kill();
            }
        }

        public HttpClient Client { get; private set; }

        private class RequestTrackingHttpClient : HttpClient
        {
            public RequestTrackingHttpClient(string baseAddress, IStressMetricCollector metricCollector)
                : base(new RequestTrackingHandler(metricCollector))
            {
                BaseAddress = new Uri(baseAddress);
            }

            private class RequestTrackingHandler : HttpClientHandler
            {
                private readonly IStressMetricCollector _metricCollector;

                public RequestTrackingHandler(IStressMetricCollector metricCollector)
                {
                    _metricCollector = metricCollector;
                }

                protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
                {
                    _metricCollector.NewRequest();
                    return base.SendAsync(request, cancellationToken);
                }
            }
        }
    }
}
