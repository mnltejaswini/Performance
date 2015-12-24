// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using Xunit.Sdk;

namespace Stress.Framework
{
    public class TimedMetricReporter : IStressMetricReporter
    {
        private TimeSpan _reportInterval = TimeSpan.FromSeconds(StressConfig.Instance.MetricReportInterval);

        private IMessageBus _messageBus;

        private Timer _timer;

        private StressTestCase _testCase;

        public TimedMetricReporter()
        {
        }

        private void Report(object state)
        {
            var collector = _testCase.MetricCollector;
            var elapsed = collector.Time.Elapsed;

            if (elapsed.TotalMilliseconds == 0)
            {
                return;
            }

            var rps = Math.Round(collector.Requests / elapsed.TotalSeconds, 1);

            // TODO: Fix Memory collection and then report it.
            _messageBus.QueueMessage(
                new StressTestProgressMessage($"Test {_testCase.TestMethod.Method.Name} ran {collector.Requests} requests @ {rps} RPS in {(int)Math.Round(elapsed.TotalSeconds)} seconds."));
        }

        public void Start(IMessageBus messageBus, StressTestCase stressTestCase)
        {
            if (_reportInterval.Equals(TimeSpan.Zero))
            {
                return;
            }

            _messageBus = messageBus;
            _testCase = stressTestCase;
            _timer = new Timer(Report, null, _reportInterval, _reportInterval);
        }

        public void Stop()
        {
            _timer?.Dispose();
        }
    }
}
