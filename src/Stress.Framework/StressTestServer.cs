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
        private ILogger _logger;

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
            var testProject = _sampleManager.PreparePublishingSample(fullTestName, _testName, publish: StressConfig.Instance.RunIterations);
            Assert.True(testProject != null, $"Fail to set up test project.");

            _logger = _sampleManager.LoggerFactory.CreateLogger(fullTestName);
            _logger.LogInformation($"Test project is set up at {testProject}");

            var serverStartInfo = _sampleManager.DnxHelper.BuildStartInfo(testProject, framework, _command);

            _serverProcess = Process.Start(serverStartInfo);
            _metricCollector.TrackMemoryFor(_serverProcess);

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
                    _logger.LogInformation($"Pinging {serverVerificationClient.BaseAddress} to ensure server booted properly");
                    response = await serverVerificationClient.GetAsync(serverVerificationClient.BaseAddress);
                    break;
                }
                catch (TimeoutException)
                {
                    _logger.LogError("Http client timeout.");
                    break;
                }
                catch (Exception)
                {
                    _logger.LogInformation("Failed to ping server. Retrying...");
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    continue;
                }
            }

            if (_serverProcess != null && _serverProcess.HasExited)
            {
                _logger.LogError($"Server exited unexpectedly: {_serverProcess.Id}");
            }

            if (response != null)
            {
                _logger.LogInformation($"Response {response.StatusCode}");
                response.EnsureSuccessStatusCode();
                _logger.LogInformation("Server started successfully");
                result.SuccessfullyStarted = true;
                ClientFactory = () => new RequestTrackingHttpClient(baseAddress, _metricCollector);
            }

            return result;
        }

        public void Dispose()
        {
            ClientFactory = null;
            if (!_serverProcess.HasExited)
            {
                _logger.LogInformation("Terminating server.");
                _serverProcess?.Kill();
            }
        }

        public Func<HttpClient> ClientFactory { get; private set; }

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
