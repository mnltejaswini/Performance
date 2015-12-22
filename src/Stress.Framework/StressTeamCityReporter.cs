// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Xunit;
using Xunit.Abstractions;

namespace Stress.Framework
{
    public class TeamCityReporter : IRunnerReporter
    {
        public string Description
            => "forces TeamCity Stress Test mode (normally auto-detected)";

        public bool IsEnvironmentallyEnabled => Environment.GetEnvironmentVariable("TEAMCITY_STRESS_TEST_FORMATTER") == "1";

        public string RunnerSwitch
            => "teamcity_st";

        public IMessageSink CreateMessageHandler(IRunnerLogger logger)
            => new StressTestTeamCityReporterMessageHandler(logger);
    }
}
