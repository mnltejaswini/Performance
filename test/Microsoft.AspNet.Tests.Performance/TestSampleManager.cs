using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Benchmarks.Utility.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;

namespace Microsoft.AspNet.Tests.Performance
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
        }

        public ILoggerFactory LoggerFactory { get; }

        public DnxHelper DnxHelper { get; }

        public string PrepareSample(string testName, string sampleName)
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

            _runner.Execute("git clean -xdff .", source);
            _runner.Execute($"robocopy {source} {target} /E /S /XD node_modules");

            if (!DnxHelper.Restore(target, "coreclr", quiet: true))
            {
                Directory.Delete(target, recursive: true);
                return null;
            }

            _cache[testName] = target;

            return target;
        }

        public string PrepareSample(string testName, string sampleName, bool publish)
        {
            if (!publish)
            {
                return PrepareSample(testName, sampleName);
            }

            var key = $"{testName}_out";
            string result;
            if (_cache.TryGetValue(key, out result))
            {
                return result;
            }

            var source = PrepareSample(testName, sampleName);
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