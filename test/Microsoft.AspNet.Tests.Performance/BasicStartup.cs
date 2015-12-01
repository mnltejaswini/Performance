// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using Benchmarks.Utility.Helpers;
using Benchmarks.Utility.Logging;
using Benchmarks.Utility.Measurement;
using Microsoft.Framework.Logging;
using Xunit;

namespace Microsoft.AspNet.Tests.Performance
{
    public class BasicStartup
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly int _iterationCount = 10;

        private readonly DnxHelper _dnx = new DnxHelper("perf");

        public BasicStartup()
        {
            _loggerFactory = LoggerHelper.GetLoggerFactory();
        }

        [Theory]
        [InlineData("BasicKestrel", "clr", "kestrel", 5001)]
        [InlineData("BasicKestrel", "coreclr", "kestrel", 5001)]
        [InlineData("StarterMvc", "clr", "web", 5000)]
        [InlineData("StarterMvc", "coreclr", "web", 5000)]
        public void DevelopmentScenario(string sampleName, string framework, string command, int port)
        {
            var logger = _loggerFactory.CreateLogger<BasicStartup>(nameof(DevelopmentScenario), sampleName, framework);
            using (logger.BeginScope("root"))
            {
                var samplePath = PathHelper.GetTestAppFolder(sampleName);

                Assert.NotNull(samplePath);

                var restoreResult = _dnx.Restore(samplePath, framework, quiet: true);
                Assert.True(restoreResult, "Failed to restore packages");

                var prepare = EnvironmentHelper.Prepare();
                Assert.True(prepare, "Failed to prepare the environment");

                var testAppStartInfo = _dnx.BuildStartInfo(samplePath, framework: framework, argument: command);
                var runner = new WebApplicationFirstRequest(
                    new StartupRunnerOptions { ProcessStartInfo = testAppStartInfo, Logger = logger, IterationCount = _iterationCount },
                    port: port, path: "/", timeout: TimeSpan.FromSeconds(60));

                var errors = new List<string>();
                var result = runner.Run();

                Assert.True(result, "Fail:\t" + string.Join("\n\t", errors));
            }

        }

        [Theory]
        [InlineData("BasicKestrel", "clr", "kestrel", 5001)]
        [InlineData("BasicKestrel", "coreclr", "kestrel", 5001)]
        [InlineData("StarterMvc", "clr", "web", 5000)]
        [InlineData("StarterMvc", "coreclr", "web", 5000)]
        public void ProductionScenario(string sampleName, string framework, string command, int port)
        {
            var logger = _loggerFactory.CreateLogger<BasicStartup>(nameof(ProductionScenario), sampleName, framework);
            using (logger.BeginScope("root"))
            {
                var samplePath = PathHelper.GetTestAppFolder(sampleName);

                Assert.NotNull(samplePath);

                var restoreResult = _dnx.Restore(samplePath, framework, quiet: true);
                Assert.True(restoreResult, "Failed to restore packages");

                var output = Path.Combine(PathHelper.GetArtifactFolder(), "publish", $"{nameof(ProductionScenario)}_{sampleName}_{framework}");
                if (Directory.Exists(output))
                {
                    Directory.Delete(output, recursive: true);
                }
                var publishResult = _dnx.Publish(samplePath, framework, output, nosource: true, quiet: true);
                Assert.True(publishResult, "Failed to publish application");

                var prepare = EnvironmentHelper.Prepare();
                Assert.True(prepare, "Failed to prepare the environment");

                // --project "%~dp0packages\BasicKestrel\1.0.0\root"
                var root = Path.Combine(output, "approot", "packages", sampleName, "1.0.0", "root");
                var testAppStartInfo = _dnx.BuildStartInfo(output, framework, $"--project {root} {command}");
                var runner = new WebApplicationFirstRequest(
                    new StartupRunnerOptions { ProcessStartInfo = testAppStartInfo, Logger = logger, IterationCount = _iterationCount },
                    port: port, path: "/", timeout: TimeSpan.FromSeconds(60));

                var errors = new List<string>();
                var result = runner.Run();

                Assert.True(result, "Fail:\t" + string.Join("\n\t", errors));
            }
        }
    }
}
