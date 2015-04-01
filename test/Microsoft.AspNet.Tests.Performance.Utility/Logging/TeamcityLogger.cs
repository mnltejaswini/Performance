// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Framework.Logging;
using Microsoft.Framework.Logging.Internal;

namespace Microsoft.AspNet.Tests.Performance.Utility.Logging
{
    public class TeamcityLogger : ILogger
    {
        private readonly string _dataMessageTemplate;
        private readonly string _name;

        public TeamcityLogger(string name)
        {
            _name = name;
            _dataMessageTemplate = "##teamcity[buildStatisticValue key='" + _name + ".{0}' value='{1}']";
        }

        public IDisposable BeginScope(object state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log(LogLevel logLevel, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
        {
            var data = LoggerHelper.RetrivePerformanceData(state);
            if (data != null)
            {
                Console.WriteLine(_dataMessageTemplate, data.Item1, data.Item2);
            }
            else
            {
                var logValues = state as FormattedLogValues;
                if (logValues != null)
                {
                    Console.WriteLine("[{0}] {1}", logLevel, logValues.Format());
                }
                else
                {
                    Console.WriteLine("[{0}] {1}", logLevel, state);
                }
            }
        }
    }
}