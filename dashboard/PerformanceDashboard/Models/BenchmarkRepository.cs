// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Entity;
using PerformanceDashboard.ViewModels;

namespace PerformanceDashboard.Models
{
    public class BenchmarkRepository
    {
        private readonly BenchmarkContext _context;
        private static readonly string _latestResultsSql =
@"SELECT Runs.*
FROM
	dbo.Runs AS Runs
	INNER JOIN (SELECT TestClass, TestMethod, Variation, ProductReportingVersion, Framework, MAX(RunStarted) LastRun
				FROM dbo.Runs
                WHERE RunStarted > GETDATE() - 30
				GROUP BY TestClass, TestMethod, Variation, ProductReportingVersion, Framework) AS LastRuns
	ON
		Runs.TestClass = LastRuns.TestClass
		AND Runs.TestMethod = LastRuns.TestMethod
		AND Runs.Variation = LastRuns.Variation
		AND Runs.ProductReportingVersion = LastRuns.ProductReportingVersion
		AND Runs.Framework = LastRuns.Framework
		AND Runs.RunStarted = LastRuns.LastRun";

        public BenchmarkRepository(BenchmarkContext context)
        {
            _context = context;
        }

        public IEnumerable<Run> GetHistory(int days)
        {
            if (days > 0)
            {
                return _context.Runs
                   .Where(r => r.RunStarted > DateTime.Today.AddDays(days * -1))
                   .ToArray();
            }
            else
            {
                return _context.Runs
                    .FromSql(_latestResultsSql)
                    .ToArray();
            }
        }

        public IEnumerable<Run> GetTestHistory(string testClass, string testMethod, int days)
        {
            return _context.Runs
                .Where(r => r.TestClass == testClass
                            && r.TestMethod == testMethod
                            && r.RunStarted > DateTime.Today.AddDays(days * -1))
                .OrderByDescending(r => r.RunStarted)
                .ToArray();
        }
    }
}
