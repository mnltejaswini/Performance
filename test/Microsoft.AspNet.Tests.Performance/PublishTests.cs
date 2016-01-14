// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO;
using Benchmarks.Framework;
using Benchmarks.Utility.Helpers;
using Microsoft.Extensions.PlatformAbstractions;
using Xunit;

namespace Microsoft.AspNet.Tests.Performance
{
    public class PublishTests : BenchmarkTestBase, IClassFixture<TestSampleManager>
    {
        private readonly TestSampleManager _sampleManager;

        public PublishTests(TestSampleManager sampleManager)
        {
            _sampleManager = sampleManager;
        }

        [Benchmark(Iterations = 5)]
        [BenchmarkVariation("DnuPublish_BasicKestrel", "BasicKestrel")]
        [BenchmarkVariation("DnuPublish_StarterMvc", "StarterMvc")]
        public void DnuPublish(string projectName)
        {
            var framework = PlatformServices.Default.Runtime.RuntimeType;
            var testName = $"{projectName}.{framework}.{nameof(DnuPublish)}";
            var testProject = _sampleManager.PrepareSample(testName, projectName, true);
            Assert.True(testProject != null, $"Fail to set up test project.");

            var testOutput = Path.Combine(PathHelper.GetNewTempFolder(), testName);
            Directory.CreateDirectory(testOutput);

            using (Collector.StartCollection())
            {
                _sampleManager.DnxHelper.Publish(
                    workingDir: testProject,
                    outputDir: testOutput,
                    framework: framework,
                    nosource: false,
                    quiet: true);
            }
        }

        [Benchmark(Iterations = 5)]
        [BenchmarkVariation("DnuPublish_BasicKestrel", "BasicKestrel")]
        [BenchmarkVariation("DnuPublish_StarterMvc", "StarterMvc")]
        public void DnuPublishNoSource(string projectName)
        {
            var framework = PlatformServices.Default.Runtime.RuntimeType;
            var testName = $"{projectName}.{framework}.{nameof(DnuPublishNoSource)}";
            var testProject = _sampleManager.PrepareSample(testName, projectName, true);
            Assert.True(testProject != null, $"Fail to set up test project.");

            var testOutput = Path.Combine(PathHelper.GetNewTempFolder(), testName);
            Directory.CreateDirectory(testOutput);

            using (Collector.StartCollection())
            {
                _sampleManager.DnxHelper.Publish(
                    workingDir: testProject,
                    outputDir: testOutput,
                    framework: framework,
                    nosource: true,
                    quiet: true);
            }
        }

        [Benchmark(Iterations = 5)]
        [BenchmarkVariation("DotnetPublish_BasicKestrel", "BasicKestrel")]
        [BenchmarkVariation("DotnetPublish_StarterMvc", "StarterMvc")]
        public void DotnetPublish(string projectName)
        {
            var framework = PlatformServices.Default.Runtime.RuntimeType;
            var testName = $"{projectName}.{framework}.{nameof(DotnetPublish)}";
            var testProject = _sampleManager.PrepareSample(testName, projectName, true);
            Assert.True(testProject != null, $"Fail to set up test project.");

            var testOutput = Path.Combine(PathHelper.GetNewTempFolder(), testName);
            Directory.CreateDirectory(testOutput);

            using (Collector.StartCollection())
            {
                _sampleManager.DotnetHelper.Publish(
                    workingDir: testProject,
                    outputDir: testOutput);
            }
        }
    }
}

