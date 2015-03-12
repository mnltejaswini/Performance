// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.Framework.Logging
{
    public static class PerformanceLoggerExtensions
    {
        public static readonly string PerfDataPrefix = "#Performance Data: ";
        public static readonly string PerfInfoPrefix = "#Performance Info: ";

        public static void LogData<T>(this ILogger logger, string name, T data, bool infoOnly = false)
        {
            logger.LogInformation(string.Format("{0}{1}={2}", infoOnly ? PerfInfoPrefix : PerfDataPrefix, name, data));
        }
    }
}