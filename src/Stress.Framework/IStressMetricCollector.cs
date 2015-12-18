// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;

namespace Stress.Framework
{
    public interface IStressMetricCollector
    {
        IDisposable StartCollection();
        void StopCollection();
        void NewRequest();
        void Reset();
        Stopwatch Time { get; }
        long MemoryDelta { get; }
        long Requests { get; }
    }
}
