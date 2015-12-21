// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Benchmarks.Framework;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Xunit;

namespace Microsoft.AspNet.Tests.Performance
{
    public class BasicStartup : IBenchmarkTest, IClassFixture<TestSampleManager>
    {
        private readonly TestSampleManager _sampleManager;
        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(60);
        private readonly int _retry = 10;

        public BasicStartup(TestSampleManager sampleManager)
        {
            _sampleManager = sampleManager;
        }

        public IMetricCollector Collector { get; set; } = new NullMetricCollector();

        [Benchmark(Iterations = 10, WarmupIterations = 0)]
        [BenchmarkVariation("BasicKestrel_DevelopmentScenario", "BasicKestrel")]
        [BenchmarkVariation("StarterMvc_DevelopmentScenario", "StarterMvc")]
        public void DevelopmentScenario(string sampleName)
        {
            var framework = PlatformServices.Default.Runtime.RuntimeType;
            var testName = $"{sampleName}.{framework}.{nameof(DevelopmentScenario)}";
            var testProject = _sampleManager.PrepareSample(testName, sampleName);
            Assert.True(testProject != null, $"Fail to set up test project.");

            var logger = _sampleManager.LoggerFactory.CreateLogger(testName);
            logger.LogInformation($"Test project is set up at {testProject}");

            var testAppStartInfo = _sampleManager.DnxHelper.BuildStartInfo(testProject, framework, "run");

            RunStartup(5000, logger, testAppStartInfo);
        }

        [Benchmark(Iterations = 10, WarmupIterations = 0)]
        [BenchmarkVariation("BasicKestrel_ProductionScenario", "BasicKestrel")]
        [BenchmarkVariation("StarterMvc_ProductionScenario", "StarterMvc")]
        public void ProductionScenario(string sampleName)
        {
            var framework = PlatformServices.Default.Runtime.RuntimeType;
            var testName = $"{sampleName}.{framework}.{nameof(ProductionScenario)}";
            var testProject = _sampleManager.PrepareSample(testName, sampleName, publish: true);
            Assert.True(testProject != null, $"Fail to set up test project.");

            var logger = _sampleManager.LoggerFactory.CreateLogger(testName);
            logger.LogInformation($"Test project is set up at {testProject}");

            // --project "%~dp0packages\BasicKestrel\1.0.0\root"
            var root = Path.Combine(testProject, "approot", "packages", testName, "1.0.0", "root");
            var testAppStartInfo = _sampleManager.DnxHelper.BuildStartInfo(testProject, framework, $"--project {root} run");

            RunStartup(5000, logger, testAppStartInfo);
        }

        private void RunStartup(int port, ILogger logger, ProcessStartInfo testAppStartInfo)
        {
            Task<HttpResponseMessage> webtask = null;
            Process process = null;
            var responseRetrived = false;
            var url = $"http://localhost:{port}/";

            var client = new HttpClient();

            using (Collector.StartCollection())
            {
                process = Process.Start(testAppStartInfo);
                for (int i = 0; i < _retry; ++i)
                {
                    try
                    {
                        webtask = client.GetAsync(url);

                        if (webtask.Wait(_timeout))
                        {
                            responseRetrived = true;
                            break;
                        }
                        else
                        {
                            logger.LogError("Http client timeout.");
                            break;
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }

            if (process != null && !process.HasExited)
            {
                logger.LogDebug($"Kill process {process.Id}");
                process.Kill();
            }

            if (responseRetrived)
            {
                var response = webtask.Result;
                logger.LogInformation($"Response {response.StatusCode}");
                response.EnsureSuccessStatusCode();
            }
        }
    }
}
