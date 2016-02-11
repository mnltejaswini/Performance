// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Benchmarks.Framework.Model;

namespace PerformanceDashboard.ViewModels.Dashboard
{
    public class IndexViewModel
    {
        private readonly string[] _frameworks;
        private readonly List<TestClassResult> _results = new List<TestClassResult>();

        public IndexViewModel(IEnumerable<Run> runs, int days, Metrics metric)
        {
            _frameworks = runs
                .Select(run => run.Framework)
                .Distinct()
                .OrderBy(framework => framework)
                .ToArray();

            Days = days;
            Metric = metric;

            foreach (var testClassRuns in runs.GroupBy(run => run.TestClass).OrderBy(grouping => grouping.Key))
            {
                _results.Add(new TestClassResult(testClassRuns, MetricHelper.Create(metric), _frameworks));
            }
        }

        public string[] Frameworks => _frameworks;
        public IEnumerable<TestClassResult> Results => _results;
        public int Days { get; private set; }
        public Metrics Metric { get; private set; }

        public class TestClassResult
        {
            private readonly List<TestVariationResult> _results = new List<TestVariationResult>();

            public TestClassResult(IEnumerable<Run> runs, MetricHelper metricHelper, string[] frameworkMap)
            {
                TestClassName = runs.Select(run => run.TestClass).Distinct().Single();

                foreach (var variationRuns in runs
                    .GroupBy(@class => new { @class.TestMethod, @class.Variation })
                    .OrderBy(grouping => grouping.Key.TestMethod + grouping.Key.Variation))
                {
                    _results.Add(new TestVariationResult(variationRuns, metricHelper, frameworkMap));
                }
            }

            public string TestClassName { get; private set; }
            public string GitHubLink => $"https://github.com/aspnet/Performance/search?q=class%20{TestClassName}";
            public IEnumerable<TestVariationResult> Results => _results;
        }

        public class TestVariationResult
        {
            private readonly FrameworkResult[] _results;

            public TestVariationResult(IEnumerable<Run> runs, MetricHelper metricHelper, string[] frameworkMap)
            {
                TestMethodName = runs.Select(run => run.TestMethod).Distinct().Single();
                VariationName = runs.Select(run => run.Variation).Distinct().Single();

                _results = new FrameworkResult[frameworkMap.Length];
                for (int i = 0; i < frameworkMap.Length; i++)
                {
                    var frameworkRuns = runs.Where(variation => variation.Framework == frameworkMap[i]);
                    _results[i] = new FrameworkResult(frameworkRuns, metricHelper);
                }
            }

            public string TestMethodName { get; }
            public string VariationName { get; }

            public FrameworkResult[] Results => _results;
        }

        public class FrameworkResult
        {
            public FrameworkResult(IEnumerable<Run> runs, MetricHelper metricHelper)
            {
                var defaultRuns = runs.Where(run => run.ProductReportingVersion == "Default");

                var result = defaultRuns.Any() ? (double?)defaultRuns.Average(run => metricHelper.GetMetric(run)) : null;
                if (result != null)
                {
                    Result = metricHelper.DisplayMetric(result.Value);
                }
            }

            public string Result { get; }
        }
    }
}
