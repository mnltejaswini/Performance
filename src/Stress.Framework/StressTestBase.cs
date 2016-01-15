// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
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
                        try
                        {
                            await iterate(client);
                        }
                        catch (Exception ex) when (StressConfig.Instance.FailDebuggerTimeout > 0)
                        {
                            Console.Error.WriteLine($"Caught exception: {ex}");
                            var timeoutSpan = TimeSpan.FromSeconds(10);
                            var timeoutLeft = TimeSpan.FromMilliseconds(StressConfig.Instance.FailDebuggerTimeout);
                            while (timeoutLeft > TimeSpan.Zero)
                            {
                                Console.WriteLine($"Waiting for debugger attach, {timeoutLeft} left");
                                timeoutLeft = timeoutLeft - timeoutSpan;
                            }
                            throw;
                        }
                    }
                }
            }
        }

    }
}
