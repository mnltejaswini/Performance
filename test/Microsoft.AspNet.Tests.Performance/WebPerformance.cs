// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Benchmarks.Framework;
using Benchmarks.Utility.Helpers;
using Benchmarks.Utility.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Xunit;

namespace Microsoft.AspNet.Tests.Performance
{
    public class WebPerformance : IBenchmarkTest, IClassFixture<SampleManager>
    {
        private readonly SampleManager _sampleManager;
        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(60);
        private readonly int _retry = 10;

        public WebPerformance(SampleManager sampleManager)
        {
            _sampleManager = sampleManager;
        }

        public IMetricCollector Collector { get; set; } = new NullMetricCollector();

        [Benchmark(Iterations = 10, WarmupIterations = 0)]
        [BenchmarkVariation("BasicKestrel_DevelopmentScenario", "BasicKestrel")]
        [BenchmarkVariation("StarterMvc_DevelopmentScenario", "StarterMvc")]
        public void Development_Startup(string sampleName)
        {
            var framework = PlatformServices.Default.Runtime.RuntimeType;
            var testName = $"{sampleName}.{framework}.{nameof(Development_Startup)}";
            var logger = LogUtility.LoggerFactory.CreateLogger(testName);

            var testProject = _sampleManager.GetRestoredSample(sampleName);
            Assert.True(testProject != null, $"Fail to set up test project.");
            logger.LogInformation($"Test project is set up at {testProject}");

            var testAppStartInfo = DnxHelper.GetDefaultInstance().BuildStartInfo(testProject, framework, "run");

            RunStartup(5000, logger, testAppStartInfo);
        }

        [Benchmark(Iterations = 10, WarmupIterations = 0)]
        [BenchmarkVariation("BasicKestrel_ProductionScenario", "BasicKestrel")]
        [BenchmarkVariation("StarterMvc_ProductionScenario", "StarterMvc")]
        public void Production_Startup(string sampleName)
        {
            var framework = PlatformServices.Default.Runtime.RuntimeType;
            var appliationFramework = GetFrameworkName(framework);
            var testName = $"{sampleName}.{framework}.{nameof(Production_Startup)}";
            var logger = LogUtility.LoggerFactory.CreateLogger(testName);

            var testProject = _sampleManager.GetDnxPublishedSample(sampleName, appliationFramework);
            Assert.True(testProject != null, $"Fail to set up test project.");
            logger.LogInformation($"Test project is set up at {testProject}");

            // --project "%~dp0packages\BasicKestrel\1.0.0\root"
            var root = Path.Combine(testProject, "approot", "packages", sampleName, "1.0.0", "root");
            var testAppStartInfo = DnxHelper.GetDefaultInstance().BuildStartInfo(testProject, framework, $"--project {root} run");

            RunStartup(5000, logger, testAppStartInfo);
        }

        [Benchmark(Iterations = 10, WarmupIterations = 0)]
        [BenchmarkVariation("BasicKestrel_DotNet_ProductionScenario", "BasicKestrel")]
        public void Production_DotNet_Startup(string sampleName)
        {
            var framework = PlatformServices.Default.Runtime.RuntimeType;
            var appliationFramework = GetFrameworkName(framework);
            var testName = $"{sampleName}.{framework}.{nameof(Production_DotNet_Startup)}";
            var logger = LogUtility.LoggerFactory.CreateLogger(testName);

            var testProject = _sampleManager.GetDotNetPublishedSample(sampleName, appliationFramework);
            Assert.True(testProject != null, $"Fail to set up test project.");
            logger.LogInformation($"Test project is set up at {testProject}");

            var startInfo = new ProcessStartInfo(Path.Combine(testProject, $"{sampleName}.exe")) { UseShellExecute = false };
            RunStartup(5000, logger, startInfo);
        }

        private static string GetFrameworkName(string runtimeType)
        {
            if (string.Equals(runtimeType, "clr", StringComparison.OrdinalIgnoreCase))
            {
                return "dnx451";
            }
            else if (string.Equals(runtimeType, "coreclr", StringComparison.OrdinalIgnoreCase))
            {
                return "dnxcore50";
            }
            else
            {
                Assert.False(true, $"Unknown framework {runtimeType}");
                return null;
            }
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
