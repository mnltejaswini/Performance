// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Stress.Framework
{
    public abstract class StressTestBase
    {
        internal IStressMetricCollector Collector { get; set; } = new StressMetricCollector();

        internal long Iterations { get; set; }

        internal int Clients { get; set; }

        internal Func<HttpClient> ClientFactory { get; set; }

        public void IterateAsync(Action<HttpClient> iterate)
        {
            var iterationsPerClient = Iterations / Clients;
            var iterations = Enumerable.Repeat(iterationsPerClient, Clients).ToArray();
            for (int i = 0; i < Iterations - iterationsPerClient * Clients; i++)
            {
                iterations[i]++;
            }

            var clientRange = Enumerable.Range(0, Clients);
            var clients = clientRange.Select(i => ClientFactory()).ToArray();
            var data = clientRange.Select(i => Tuple.Create(iterations[i], clients[i]));

            using (Collector.StartCollection())
            {
                var tasks = clientRange.Select(i => Task.Run(()=>IterateAsync(iterations[i], clients[i], iterate)));
                Task.WhenAll(tasks).Wait();
            }
        }

        private void IterateAsync(long iterations, HttpClient client, Action<HttpClient> iterate)
        {
            for (var i = 0; i < iterations; i++)
            {
                try
                {
                    iterate(client);
                }
                catch (Exception ex) when (StressConfig.Instance.FailDebugger)
                {
                    Console.Error.WriteLine($"Caught exception: {ex}");
                    var timeoutSpan = TimeSpan.FromMinutes(1);
                    while (!Debugger.IsAttached)
                    {
                        Thread.Sleep((int)timeoutSpan.TotalMilliseconds);
                        Console.WriteLine($"Waiting for debugger attach");
                    }
                    throw;
                }
            }
        }

    }
}
