// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Xunit.Abstractions;

namespace Stress.Framework
{
    public class StressTestSummaryStatisticsMessage : StressTestStatisticsMessage, ITestSummaryStatisticsMessage
    {
        public StressTestSummaryStatisticsMessage(ITestCase testCase, string key, object value) : base(testCase, key, value)
        {
        }
    }
}