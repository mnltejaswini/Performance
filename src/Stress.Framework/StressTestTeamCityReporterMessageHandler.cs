// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;
using Xunit.Runner.Reporters;

namespace Stress.Framework
{
    public class StressTestTeamCityReporterMessageHandler: TeamCityReporterMessageHandler
    {
        private Func<string, string> _escape;

        private Func<string, string> _toFlowId;

        private IRunnerLogger _logger;

        public StressTestTeamCityReporterMessageHandler(IRunnerLogger logger,
            Func<string, string> flowIdMapper = null,
            TeamCityDisplayNameFormatter displayNameFormatter = null) : base(logger, flowIdMapper, displayNameFormatter)
        {
            _logger = logger;

            // I know, I know, but I want them so bad and ITestProgressMessage and ITestStatisticsMessage won't
            // be written too often, so should not hurt repformance.

            var escapeMethodInfo = typeof(TeamCityReporterMessageHandler).GetMethod("Escape", BindingFlags.Static | BindingFlags.NonPublic);
            _escape = (s) => (string) escapeMethodInfo.Invoke(this, new[] {s});

            var toFlowIdMethodInfo = typeof(TeamCityReporterMessageHandler).GetMethod("ToFlowId", BindingFlags.Instance | BindingFlags.NonPublic);
            _toFlowId = (s) => (string)toFlowIdMethodInfo.Invoke(this, new[] { s });

        }

        public override bool OnMessage(IMessageSinkMessage message)
        {
            return DoVisit<ITestProgressMessage>(message, m => Visit(m)) &&
                   DoVisit<ITestStatisticsMessage>(message, m => Visit(m)) &&
                   base.OnMessage(message);
        }

        protected virtual bool Visit(ITestProgressMessage message)
        {
            _logger.LogImportantMessage($"##teamcity[progressMessage '{_escape(message.Message)}' ]");
            return true;
        }

        protected virtual bool Visit(ITestStatisticsMessage message)
        {
            _logger.LogImportantMessage($"##teamcity[buildStatisticValue key='{_escape(message.TestCase.DisplayName + "_" + message.Key)}' value='{_escape(Convert.ToString(message.Value))}' flowId='{_toFlowId(message.TestCollection.DisplayName)}' ]");
            return true;
        }
    }
}