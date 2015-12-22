// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Xunit.Sdk;

namespace Stress.Framework
{
    public interface IStressMetricReporter
    {
        void Start(IMessageBus messageBus, StressTestCase stressTestCase);
        void Stop();
    }
}