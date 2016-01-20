// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Threading;

namespace Stress.Framework
{
    public class StressMetricCollector : IStressMetricCollector
    {
        private bool _collecting;
        private readonly Scope _scope;
        private readonly Stopwatch _timer = new Stopwatch();
        private long _memoryOnCurrentCollectionStarted;
        private long _requestCount;
        private Process _process;

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
            Interlocked.Increment(ref _requestCount);
        }

        public void Reset()
        {
            _collecting = false;
            _timer.Reset();
            MemoryDelta = 0;
            _requestCount = 0;
        }

        public void TrackMemoryFor(Process process)
        {
            _process = process;
        }

        public Stopwatch Time => _timer;

        public long MemoryDelta { get; private set; }

        public long Requests => _requestCount;

        private long GetCurrentMemory()
        {
            return _process?.WorkingSet64 ?? 0;
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
