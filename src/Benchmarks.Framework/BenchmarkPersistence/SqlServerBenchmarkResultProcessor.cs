// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Benchmarks.Framework.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Benchmarks.Framework.BenchmarkPersistence
{
    public class SqlServerBenchmarkResultProcessor
    {
        private readonly DbContextOptions _contextOptions;

        public SqlServerBenchmarkResultProcessor(string connectionString)
        {

            var contextOptionsBuilder = new DbContextOptionsBuilder();
            contextOptionsBuilder.UseSqlServer(connectionString);
            _contextOptions = contextOptionsBuilder.Options;

            InitializeDatabase();
        }

        internal void InitializeDatabase()
        {
            using (var context = new BenchmarkContext(_contextOptions))
            {
                context.Database.EnsureCreated();
            }
        }

        public void SaveSummary(BenchmarkRunSummary summary)
        {
            using (var context = new BenchmarkContext(_contextOptions))
            {
                context.Runs.Add(new Run()
                {
                    TestClassFullName = summary.TestClassFullName,
                    TestClass = summary.TestClass,
                    TestMethod = summary.TestMethod,
                    Variation = summary.Variation,
                    MachineName = summary.MachineName,
                    ProductReportingVersion = summary.ProductReportingVersion,
                    Framework = summary.Framework,
                    Architecture = summary.Architecture,
                    CustomData = summary.CustomData,
                    RunStarted = summary.RunStarted,
                    WarmupIterations = summary.WarmupIterations,
                    Iterations = summary.Iterations,
                    TimeElapsedAverage = summary.TimeElapsedAverage,
                    TimeElapsedPercentile99 = summary.TimeElapsedPercentile99,
                    TimeElapsedPercentile95 = summary.TimeElapsedPercentile95,
                    TimeElapsedPercentile90 = summary.TimeElapsedPercentile90,
                    MemoryDeltaAverage = summary.MemoryDeltaAverage,
                    MemoryDeltaPercentile99 = summary.MemoryDeltaPercentile99,
                    MemoryDeltaPercentile95 = summary.MemoryDeltaPercentile95,
                    MemoryDeltaPercentile90 = summary.MemoryDeltaPercentile90,
                    MemoryDeltaStandardDeviation = summary.MemoryDeltaStandardDeviation,
                });
                context.SaveChanges();
            }
        }
    }
}
