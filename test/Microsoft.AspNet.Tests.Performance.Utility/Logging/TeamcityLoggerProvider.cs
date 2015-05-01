// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Framework.Logging;

namespace Microsoft.AspNet.Tests.Performance.Utility.Logging
{
    public class TeamcityLoggerProvider : ILoggerProvider
    {
        public static readonly string EnvTeamcityProjectName = "TEAMCITY_PROJECT_NAME";

        public ILogger CreateLogger(string name)
        {
            return new TeamcityLogger(name);
        }
    }
}