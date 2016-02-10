// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Benchmarks.Framework.Model
{
    public class Run
    {
        public int Id { get; set; }

        // Dimensions
        public string TestClassFullName { get; set; }
        public string TestClass { get; set; }
        public string TestMethod { get; set; }
        public string Variation { get; set; }
        public string MachineName { get; set; }
        public string ProductReportingVersion { get; set; }
        public string Framework { get; set; }
        public string Architecture { get; set; }
        public string CustomData { get; set; }
        public DateTime RunStarted { get; set; }
        public int WarmupIterations { get; set; }
        public int Iterations { get; set; }

        // Metrics
        public long TimeElapsedAverage { get; set; }
        public long TimeElapsedPercentile99 { get; set; }
        public long TimeElapsedPercentile95 { get; set; }
        public long TimeElapsedPercentile90 { get; set; }
        public double TimeElapsedStandardDeviation { get; set; }

        public long MemoryDeltaAverage { get; set; }
        public long MemoryDeltaPercentile99 { get; set; }
        public long MemoryDeltaPercentile95 { get; set; }
        public long MemoryDeltaPercentile90 { get; set; }
        public double MemoryDeltaStandardDeviation { get; set; }
    }
}
