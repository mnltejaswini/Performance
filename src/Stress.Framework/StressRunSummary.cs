// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Xunit.Sdk;

namespace Stress.Framework
{
    public class StressRunSummary : RunSummary
    {
        // Dimensions
        public string TestClassFullName { get; set; }
        public string TestClass { get; set; }
        public string TestMethod { get; set; }
        public string Variation { get; set; }
        public string MachineName { get; set; }
        public string Framework { get; set; }
        public string Architecture { get; set; }
        public DateTime RunStarted { get; set; }
        public long Iterations { get; set; }

        // Metrics
        public TimeSpan TimeElapsed { get; private set; }

        public long MemoryDelta { get; private set; }

        public double RequestsPerSecond { get; private set; }

        public long RequestCount { get; set; }

        public void PopulateMetrics(IStressMetricCollector collector)
        {
            TimeElapsed = collector.Time.Elapsed;
            MemoryDelta = collector.MemoryDelta;
            RequestCount = collector.Requests;

            var elapsedSeconds = TimeElapsed.TotalSeconds == 0 ? 1 : collector.Time.Elapsed.TotalSeconds;
            RequestsPerSecond = RequestCount / elapsedSeconds;
        }

        public void PublishOutput()
        {
            Console.WriteLine("Iterations: " + Iterations);
            Console.WriteLine("Total time elapsed: " + TimeElapsed);
            Console.WriteLine("Memory Delta: " + string.Format("{0:n0}K", MemoryDelta / 1000));
            Console.WriteLine("Number of requests: " + RequestCount);
            Console.WriteLine("Average Requests per Second: " + RequestsPerSecond);
        }

        public override string ToString()
        {
            return $@"{TestClass}.{TestMethod} (Variation={Variation})
    Run Iterations: {Iterations}
    Time Elapsed: {TimeElapsed}s
    Memory Delta: {MemoryDelta}";
        }
    }
}
