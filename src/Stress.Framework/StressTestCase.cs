// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Server.Testing;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Stress.Framework
{
    public class StressTestCase : StressTestCaseBase
    {
        public StressTestCase(
            string testApplicationName,
            long iterations,
            string variation,
            ServerType serverType,
            IMethodInfo warmupMethod,
            IMessageSink diagnosticMessageSink,
            ITestMethod testMethod,
            object[] testMethodArguments)
            : base(
            testApplicationName,
            variation,
            serverType,
            warmupMethod,
            diagnosticMessageSink,
            testMethod,
            testMethodArguments)
        {
            Iterations = iterations;
            MetricCollector = new StressMetricCollector();
            MetricReporter = new TimedMetricReporter();
        }

        public override IStressMetricCollector MetricCollector { get; protected set; }

        public virtual IStressMetricReporter MetricReporter { get; protected set; }

        public long Iterations { get; protected set; }

        public override Task<RunSummary> RunAsync(
            IMessageSink diagnosticMessageSink,
            IMessageBus messageBus,
            object[] constructorArguments,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
        {
            return new StressTestCaseRunner(
                this,
                DisplayName,
                SkipReason,
                constructorArguments,
                TestMethodArguments,
                messageBus,
                aggregator,
                cancellationTokenSource,
                DiagnosticMessageSink).RunAsync();
        }
    }
}
