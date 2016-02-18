// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Benchmarks.Framework.BenchmarkPersistence;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Benchmarks.Framework
{
    public class BenchmarkTestFramework : XunitTestFramework, IDisposable
    {
        public BenchmarkTestFramework(IMessageSink messageSink) : base(messageSink) {}

        public new virtual void Dispose()
        {
            BenchmarkResultProcessor.ReleaseInstance();
            base.Dispose();
        }
    }
}
