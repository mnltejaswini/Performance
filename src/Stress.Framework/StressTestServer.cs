// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Benchmarks.Utility.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Xunit;
using Microsoft.AspNet.Server.Testing;

namespace Stress.Framework
{
    public class StressTestServer : IDisposable
    {
        private readonly string _testName;
        private readonly int _port;
        private readonly IStressMetricCollector _metricCollector;
        private readonly TestSampleManager _sampleManager;
        private readonly string _command;
        private ILogger _logger;
        private readonly string _testMethodName;
        private readonly ServerType _serverType;

        private IApplicationDeployer _applicationDeployer;

        public StressTestServer(
            ServerType serverType,
            string testName,
            string testMethodName,
            int port,
            string command,
            IStressMetricCollector metricCollector)
        {
            _serverType = serverType;
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
            var fullTestName = $"{_testMethodName}.{_testName}.{framework}";
            fullTestName = fullTestName.Replace('_', '.');

            _logger = _sampleManager.LoggerFactory.CreateLogger(fullTestName);

            var baseAddress = $"http://localhost:{_port}/";

            var p = new DeploymentParameters(PathHelper.GetTestAppFolder(_testName), _serverType, RuntimeFlavor.Clr, RuntimeArchitecture.x86)
            {
                SiteName = _testName,
                ApplicationBaseUriHint = baseAddress,
                Command = _command,
            };
            _applicationDeployer = ApplicationDeployerFactory.Create(p, _logger);
            var deploymentResult = _applicationDeployer.Deploy();
            baseAddress = deploymentResult.ApplicationBaseUri;

            _logger.LogInformation($"Test project is set up at {deploymentResult.WebRootLocation}");

            var result = new StressTestServerStartResult
            {
                ServerHandle = this
            };
            var serverVerificationClient = new HttpClient
            {
                BaseAddress = new Uri(baseAddress)
            };

            HttpResponseMessage response = null;
            for (int i = 0; i < 20; ++i)
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

            if (response != null)
            {
                _logger.LogInformation($"Response {response.StatusCode}");
                response.EnsureSuccessStatusCode();
                _logger.LogInformation("Server started successfully");
                result.SuccessfullyStarted = true;
                ClientFactory = () => new RequestTrackingHttpClient(baseAddress, _metricCollector);
            }
            else
            {
                //_applicationDeployer.Dispose();
                throw new InvalidOperationException("Server could not start");
            }

            return result;
        }

        public void Dispose()
        {
            ClientFactory = null;
            _applicationDeployer?.Dispose();
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
