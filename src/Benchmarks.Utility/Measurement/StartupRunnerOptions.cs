// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using Benchmarks.Framework;
using Microsoft.Framework.Logging;

namespace Benchmarks.Utility.Measurement
{
    public class StartupRunnerOptions
    {
        public ProcessStartInfo ProcessStartInfo { get; set; }

        public int IterationCount { get; set; }

        public ILogger Logger { get; set; }

        public BenchmarkRunSummary Summary { get; set; }
    }
}