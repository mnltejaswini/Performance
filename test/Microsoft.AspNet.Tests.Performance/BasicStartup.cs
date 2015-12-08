// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using Benchmarks.Utility.Helpers;
using Benchmarks.Utility.Measurement;
using Xunit;

namespace Microsoft.AspNet.Tests.Performance
{
    public class BasicStartup : StartupTestBase
    {
        public BasicStartup() : base(10)
        {
            _summary.TestClassFullName = GetType().FullName;
            _summary.TestClass = GetType().Name;
        }

        [Theory]
        [InlineData("BasicKestrel", "clr", "kestrel", 5001)]
        [InlineData("BasicKestrel", "coreclr", "kestrel", 5001)]
        [InlineData("StarterMvc", "clr", "web", 5000)]
        [InlineData("StarterMvc", "coreclr", "web", 5000)]
        public void DevelopmentScenario(string sampleName, string framework, string command, int port)
        {
            _summary.TestMethod = nameof(DevelopmentScenario);
            _summary.Variation = sampleName;
            _summary.Framework = framework;

            var samplePath = PathHelper.GetTestAppFolder(sampleName);
            Assert.NotNull(samplePath);

            var restoreResult = _dnx.Restore(samplePath, framework, quiet: true);
            Assert.True(restoreResult, "Failed to restore packages");

            var prepare = EnvironmentHelper.Prepare();
            Assert.True(prepare, "Failed to prepare the environment");

            var logger = _loggerFactory.CreateLogger($"{nameof(BasicStartup)}.{nameof(DevelopmentScenario)}.{sampleName}.{framework}");
            var testAppStartInfo = _dnx.BuildStartInfo(samplePath, framework: framework, argument: command);
            var options = new StartupRunnerOptions
            {
                ProcessStartInfo = testAppStartInfo,
                Logger = logger,
                IterationCount = _iterationCount,
                Summary = _summary
            };

            var runner = new WebApplicationFirstRequest(options, port: port, path: "/", timeout: TimeSpan.FromSeconds(60));

            var errors = new List<string>();
            var result = runner.Run();

            SaveSummary(logger);

            Assert.True(result, "Fail:\t" + string.Join("\n\t", errors));
        }

        [Theory]
        [InlineData("BasicKestrel", "clr", "kestrel", 5001)]
        [InlineData("BasicKestrel", "coreclr", "kestrel", 5001)]
        [InlineData("StarterMvc", "clr", "web", 5000)]
        [InlineData("StarterMvc", "coreclr", "web", 5000)]
        public void ProductionScenario(string sampleName, string framework, string command, int port)
        {
            _summary.TestMethod = nameof(DevelopmentScenario);
            _summary.Variation = sampleName;
            _summary.Framework = framework;

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
            var logger = _loggerFactory.CreateLogger($"{nameof(BasicStartup)}.{nameof(ProductionScenario)}.{sampleName}.{framework}");
            var root = Path.Combine(output, "approot", "packages", sampleName, "1.0.0", "root");
            var testAppStartInfo = _dnx.BuildStartInfo(output, framework, $"--project {root} {command}");
            var options = new StartupRunnerOptions
            {
                ProcessStartInfo = testAppStartInfo,
                Logger = logger,
                IterationCount = _iterationCount,
                Summary = _summary
            };

            var runner = new WebApplicationFirstRequest(options, port: port, path: "/", timeout: TimeSpan.FromSeconds(60));

            var errors = new List<string>();
            var result = runner.Run();

            SaveSummary(logger);

            Assert.True(result, "Fail:\t" + string.Join("\n\t", errors));
        }
    }
}
