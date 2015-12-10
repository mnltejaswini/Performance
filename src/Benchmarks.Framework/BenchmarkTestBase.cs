// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Benchmarks.Framework
{
    public interface IBenchmarkTest
    {
        IMetricCollector Collector { get; set; }
    }

    public abstract class BenchmarkTestBase : IBenchmarkTest
    {
        public IMetricCollector Collector { get; set; } = new NullMetricCollector();
    }
}
