// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Net.Http;
using System.Threading.Tasks;
using Benchmarks.Utility.Helpers;

namespace Stress.Framework
{
    public abstract class StressTestBase
    {
        private readonly TestSampleManager _sampleManager;

        public StressTestBase()
        {
            _sampleManager = new TestSampleManager();
        }

        internal IStressMetricCollector Collector { get; set; } = new StressMetricCollector();

        internal long Iterations { get; set; }

        internal Func<HttpClient> ClientFactory { get; set; }

        public async Task IterateAsync(Func<HttpClient, Task> iterate)
        {
            using (Collector.StartCollection())
            {
                using (var client = ClientFactory())
                {
                    for (var i = 0; i < Iterations; i++)
                    {
                        await iterate(client);
                    }
                }
            }
        }

    }
}
