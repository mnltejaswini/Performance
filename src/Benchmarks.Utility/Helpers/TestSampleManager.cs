// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;

namespace Benchmarks.Utility.Helpers
{
    public class TestSampleManager
    {
        private readonly Dictionary<string, string> _cache = new Dictionary<string, string>();
        private readonly CommandLineRunner _runner;

        public TestSampleManager()
        {
            _runner = new CommandLineRunner();

            LoggerFactory = new LoggerFactory();
            LoggerFactory.AddConsole();

            DnxHelper = new DnxHelper("perf");
            DotnetHelper = new DotnetHelper();
        }

        public ILoggerFactory LoggerFactory { get; }

        public DnxHelper DnxHelper { get; }

        public DotnetHelper DotnetHelper { get; }

        public string EnsureRestoredSample(string sampleName)
        {
            var source = PathHelper.GetTestAppFolder(sampleName);
            if (source == null)
            {
                return null;
            }

            if (!DnxHelper.Restore(source, "coreclr", quiet: true))
            {
                return null;
            }

            return source;
        }

        public string PrepareSample(string testName, string sampleName, bool cleanProject)
        {
            string result;
            if (_cache.TryGetValue(testName, out result))
            {
                return result;
            }

            var source = PathHelper.GetTestAppFolder(sampleName);
            if (source == null)
            {
                return null;
            }

            var target = Path.Combine(PathHelper.GetNewTempFolder(), testName);
            Directory.CreateDirectory(target);

            if (cleanProject)
            {
                _runner.Execute("git clean -xdff .", source);
            }
            _runner.Execute($"robocopy {source} {target} /E /S /XD node_modules");

            if (!DnxHelper.Restore(target, "coreclr", quiet: true))
            {
                Directory.Delete(target, recursive: true);
                return null;
            }

            _cache[testName] = target;

            return target;
        }

        public string PreparePublishingSample(string testName, string sampleName, bool publish)
        {
            if (!publish)
            {
                return PrepareSample(testName, sampleName, cleanProject: true);
            }

            var key = $"{testName}_out";
            string result;
            if (_cache.TryGetValue(key, out result))
            {
                return result;
            }

            var source = PrepareSample(testName, sampleName, cleanProject: true);
            if (source == null)
            {
                return null;
            }

            var target = Path.Combine(PathHelper.GetNewTempFolder(), testName);
            Directory.CreateDirectory(target);

            var framework = PlatformServices.Default.Runtime.RuntimeType;
            if (!DnxHelper.Publish(source, framework, target, nosource: true, quiet: true))
            {
                return null;
            }

            _cache[key] = target;
            return target;
        }
    }
}