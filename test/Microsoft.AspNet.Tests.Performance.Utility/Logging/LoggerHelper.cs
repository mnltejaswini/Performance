// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Framework.Logging;

namespace Microsoft.AspNet.Tests.Performance.Utility.Logging
{
    public static class LoggerHelper
    {
        public static ILoggerFactory GetLoggerFactory()
        {
            var factory = new LoggerFactory();
            factory.AddProvider(new ArchiveLoggerProvider());

            if (Environment.GetEnvironmentVariable(TeamcityLoggerProvider.EnvTeamcityProjectName) != null)
            {
                factory.AddProvider(new TeamcityLoggerProvider());
            }
            else
            {
                factory.AddConsole();
            }

            return factory;
        }

        public static ILogger CreateLogger(this ILoggerFactory self,
            Type testclassType,
            string testSample,
            string testMethod,
            string testFramework)
        {
            var categoryName = string.Format("Perf.{0}.{1}.{2}_{3}", testclassType.Name, testSample, testMethod, testFramework);
            return self.CreateLogger(categoryName);
        }

        public static Tuple<string, string> RetrivePerformanceData(object state)
        {
            return ParsePerformanceData(PerformanceLoggerExtensions.PerfDataPrefix, state);
        }

        public static Tuple<string, string> RetrivePerformanceInfo(object state)
        {
            return ParsePerformanceData(PerformanceLoggerExtensions.PerfInfoPrefix, state);
        }

        private static Tuple<string, string> ParsePerformanceData(string prefix, object state)
        {
            var message = state as string;
            if (message == null)
            {
                return null;
            }

            if (message.IndexOf(prefix) == 0)
            {
                var parts = message.Substring(PerformanceLoggerExtensions.PerfDataPrefix.Length).Split('=');
                if (parts.Length >= 2)
                {
                    return new Tuple<string, string>(parts[0], parts[1]);
                }
            }

            return null;
        }
    }
}