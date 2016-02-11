// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Benchmarks.Framework.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PerformanceDashboard.Models
{
    public class BenchmarkRepository
    {
        private readonly BenchmarkContext _context;

        public BenchmarkRepository(BenchmarkContext context)
        {
            _context = context;
        }

        public IEnumerable<Run> GetHistory(int days)
        {
            if (days > 0)
            {
                return _context.Runs
                   .Where(r => r.RunStarted > DateTime.Today.AddDays(-days))
                   .ToList();
            }

            var thirtyDaysAgo = DateTime.Today.AddDays(-30);
            return _context.Runs
                .Where(r => r.RunStarted > thirtyDaysAgo)
                .GroupBy(
                    r => 
                        new
                        {
                            r.TestClass,
                            r.TestMethod,
                            r.Variation,
                            r.ProductReportingVersion,
                            r.Framework
                        })
                .Select(
                    g =>
                        new
                        {
                            g.Key.TestClass,
                            g.Key.TestMethod,
                            g.Key.Variation,
                            g.Key.ProductReportingVersion,
                            g.Key.Framework,
                            LastRun = g.Max(r => r.RunStarted)
                        })
                .SelectMany(x => _context.Runs.Where(r =>
                    r.TestClass == x.TestClass &&
                    r.Variation == x.Variation &&
                    r.ProductReportingVersion == x.ProductReportingVersion &&
                    r.Framework == x.Framework &&
                    r.RunStarted == x.LastRun
                    ).Select(r => r)).ToList();
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
