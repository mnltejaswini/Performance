// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace PerformanceDashboard.ViewModels
{
    public class MetricHelper
    {
        private readonly static Func<double, string> millisecondFormatter = r => string.Format("{0:0}ms", r);
        private readonly static Func<double, string> byteFormatter = r => string.Format("{0:n0}K", r / 1000);
        private readonly static Dictionary<string, Metrics> _displayList = new Dictionary<string, Metrics>
        {
            { "Time 90th Percentile", Metrics.TimeElapsedPercentile90 },
            { "Time 95th Percentile", Metrics.TimeElapsedPercentile95 },
            { "Time 99th Percentile", Metrics.TimeElapsedPercentile99 },
            { "Memory 90th Percentile", Metrics.MemoryDeltaPercentile90 },
            { "Memory 95th Percentile", Metrics.MemoryDeltaPercentile95 },
            { "Memory 99th Percentile", Metrics.MemoryDeltaPercentile99 }
        };

        private readonly Func<Run, double> _selector;
        private readonly Func<double, string> _formatter;

        private MetricHelper(Func<Run, double> selector, Func<double, string> formatter)
        {
            _selector = selector;
            _formatter = formatter;
        }

        public double GetMetric(Run run)
        {
            return _selector(run);
        }

        public string DisplayMetric(double metric)
        {
            return _formatter(metric);
        }

        public static MetricHelper Create(Metrics metric)
        {
            switch (metric)
            {
                case Metrics.TimeElapsedPercentile90:
                    return new MetricHelper(run => run.TimeElapsedPercentile90, millisecondFormatter);

                case Metrics.TimeElapsedPercentile95:
                    return new MetricHelper(run => run.TimeElapsedPercentile95, millisecondFormatter);

                case Metrics.TimeElapsedPercentile99:
                    return new MetricHelper(run => run.TimeElapsedPercentile99, millisecondFormatter);

                case Metrics.MemoryDeltaPercentile90:
                    return new MetricHelper(run => run.MemoryDeltaPercentile90, byteFormatter);

                case Metrics.MemoryDeltaPercentile95:
                    return new MetricHelper(run => run.MemoryDeltaPercentile95, byteFormatter);

                case Metrics.MemoryDeltaPercentile99:
                    return new MetricHelper(run => run.MemoryDeltaPercentile99, byteFormatter);

                default:
                    throw new NotSupportedException($"{metric} is not a supported metric.");
            }
        }

        public static Dictionary<string, Metrics> DisplayList => _displayList;
    }
}
