// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.AspNet.Tests.Performance.Utility.Helpers;
using Microsoft.AspNet.Tests.Performance.Utility.Logging;
using Microsoft.AspNet.Tests.Performance.Utility.Measurement;
using Microsoft.Framework.Logging;
using Xunit;

namespace Microsoft.AspNet.Tests.Performance
{
    public class BasicStartup
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly int _iterationCount = 10;

        public BasicStartup()
        {
            _loggerFactory = LoggerHelper.GetLoggerFactory();
        }

        [Theory]
        [InlineData("BasicConsole", "coreclr")]
        [InlineData("BasicConsole", "clr")]
        [InlineData("HeavyConsole", "coreclr")]
        [InlineData("HeavyConsole", "clr")]
        public void Console_DesignTime(string sampleName, string framework)
        {
            var logger = _loggerFactory.CreateLogger(GetType(), "run", sampleName, nameof(Console_DesignTime), framework);
            using (logger.BeginScope(null))
            {
                var samplePath = PathHelper.GetTestAppFolder(sampleName);

                logger.LogInformation("Probe application under " + samplePath);
                Assert.NotNull(samplePath);

                var restoreResult = DnuHelper.RestorePackage(samplePath, framework, quiet: true);
                Assert.True(restoreResult, "Failed to restore packages");

                var prepare = EnvironmentHelper.Prepare();
                Assert.True(prepare, "Failed to prepare the environment");

                var options = new StartupRunnerOptions
                {
                    ProcessStartInfo = DnxHelper.BuildStartInfo(samplePath, framework: framework),
                    Logger = logger,
                    IterationCount = _iterationCount
                };
                var runner = new ConsoleAppStartup(options);

                var errors = new List<string>();
                var result = runner.Run();

                Assert.True(result, "Fail:\t" + string.Join("\n\t", errors));
            }
        }

        [Theory]
        [InlineData("BasicWeb", "clr", "web", 5000)]
        [InlineData("BasicWeb", "coreclr", "web", 5000)]
        [InlineData("BasicKestrel", "clr", "kestrel", 5010)]
        [InlineData("BasicKestrel", "coreclr", "kestrel", 5010)]
        [InlineData("StandardMvc", "clr", "web", 5001)]
        [InlineData("StandardMvc", "coreclr", "web", 5001)]
        [InlineData("StandardMvc", "clr", "kestrel", 5011)]
        [InlineData("StandardMvc", "coreclr", "kestrel", 5011)]
        [InlineData("StandardMvcApi", "clr", "web", 5002)]
        [InlineData("StandardMvcApi", "coreclr", "web", 5002)]
        [InlineData("StandardMvcApi", "clr", "kestrel", 5012)]
        [InlineData("StandardMvcApi", "coreclr", "kestrel", 5012)]
        public void SelfhostWeb_Designtime(string sampleName, string framework, string command, int port)
        {
            var logger = _loggerFactory.CreateLogger(this.GetType(), sampleName, command, nameof(SelfhostWeb_Designtime), framework);
            using (logger.BeginScope(null))
            {
                var samplePath = PathHelper.GetTestAppFolder(sampleName);

                Assert.NotNull(samplePath);

                var restoreResult = DnuHelper.RestorePackage(samplePath, framework, quiet: true);
                Assert.True(restoreResult, "Failed to restore packages");

                var prepare = EnvironmentHelper.Prepare();
                Assert.True(prepare, "Failed to prepare the environment");

                var testAppStartInfo = DnxHelper.BuildStartInfo(samplePath, framework: framework, command: command);
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