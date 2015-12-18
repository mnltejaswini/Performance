// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;

namespace Stress.Framework
{
    public class StressMetricCollector : IStressMetricCollector
    {
        private bool _collecting;
        private readonly Scope _scope;
        private readonly Stopwatch _timer = new Stopwatch();
        private long _memoryOnCurrentCollectionStarted;
        private long _requestCount;

        public StressMetricCollector()
        {
            _scope = new Scope(this);
        }

        public IDisposable StartCollection()
        {
            _collecting = true;
            _memoryOnCurrentCollectionStarted = GetCurrentMemory();
            _timer.Start();
            return _scope;
        }

        public void StopCollection()
        {
            if (_collecting)
            {
                _timer.Stop();
                _collecting = false;
                var currentMemory = GetCurrentMemory();
                MemoryDelta += currentMemory - _memoryOnCurrentCollectionStarted;
            }
        }

        public void NewRequest()
        {
            _requestCount++;
        }

        public void Reset()
        {
            _collecting = false;
            _timer.Reset();
            MemoryDelta = 0;
            _requestCount = 0;
        }

        public Stopwatch Time => _timer;

        public long MemoryDelta { get; private set; }

        public long Requests => _requestCount;

        private static long GetCurrentMemory()
        {
            for (var i = 0; i < 5; i++)
            {
                GC.GetTotalMemory(forceFullCollection: true);
            }

            return GC.GetTotalMemory(forceFullCollection: true);
        }

        private class Scope : IDisposable
        {
            private readonly IStressMetricCollector _collector;

            public Scope(IStressMetricCollector collector)
            {
                _collector = collector;
            }

            public void Dispose()
            {
                _collector.StopCollection();
            }
        }
    }
}
