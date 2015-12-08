using System;
using Benchmarks.Framework;
using Benchmarks.Utility.Helpers;
using Microsoft.Framework.Logging;

namespace Microsoft.AspNet.Tests.Performance
{
    public abstract class StartupTestBase
    {
        protected readonly BenchmarkRunSummary _summary;
        protected readonly int _iterationCount = 10;
        protected readonly ILoggerFactory _loggerFactory;
        protected readonly DnxHelper _dnx = new DnxHelper("perf");

        public StartupTestBase(int iterations)
        {
            _loggerFactory = new LoggerFactory();
            _loggerFactory.AddConsole();

            _iterationCount = iterations;

            _summary = new BenchmarkRunSummary
            {
                TestClassFullName = GetType().FullName,
                TestClass = GetType().Name,
                RunStarted = DateTime.Now,
                Iterations = iterations
            };
        }

        protected void SaveSummary(ILogger logger)
        {
            foreach (var database in BenchmarkConfig.Instance.ResultDatabases)
            {
                try
                {
                    new SqlServerBenchmarkResultProcessor(database).SaveSummary(_summary);
                }
                catch (Exception ex)
                {
                    logger.LogError($"Failed to save results to {database} {ex}");
                    throw;
                }
            }
        }
    }
}
