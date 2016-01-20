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

        private long _previousRequestCount;

        public TimedMetricReporter()
        {
        }

        private void Report(object state)
        {
            var collector = _testCase.MetricCollector;
            var elapsed = collector.Time.Elapsed;
            var requests = collector.Requests;

            if (elapsed.TotalMilliseconds == 0)
            {
                return;
            }

            var rps = Math.Round(requests / elapsed.TotalSeconds, 1);

            var momentalRps = Math.Round((requests - _previousRequestCount)/_reportInterval.TotalSeconds);
            _previousRequestCount = requests;

            // TODO: Fix Memory collection and then report it.
            _messageBus.QueueMessage(
                new StressTestProgressMessage($"Test {_testCase.DisplayName} ran {requests} requests @ {momentalRps}/{rps} RPS in {(int)Math.Round(elapsed.TotalSeconds)}s."));

            _messageBus.QueueMessage(new StressTestStatisticsMessage(_testCase, "TIME", collector.Time.ElapsedMilliseconds));
            _messageBus.QueueMessage(new StressTestStatisticsMessage(_testCase, "MEM", collector.MemoryDelta / 1000));
            _messageBus.QueueMessage(new StressTestStatisticsMessage(_testCase, "REQ", requests));
            _messageBus.QueueMessage(new StressTestStatisticsMessage(_testCase, "RPS", rps));
            _messageBus.QueueMessage(new StressTestStatisticsMessage(_testCase, "RPSM", momentalRps));
        }

        public void Start(IMessageBus messageBus, StressTestCase stressTestCase)
        {
            if (_reportInterval.Equals(TimeSpan.Zero))
            {
                return;
            }

            _messageBus = messageBus;
            _testCase = stressTestCase;
            _previousRequestCount = 0;
            _timer = new Timer(Report, null, _reportInterval, _reportInterval);
        }

        public void Stop()
        {
            _timer?.Dispose();
        }
    }
}
