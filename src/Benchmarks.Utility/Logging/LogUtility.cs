// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Extensions.Logging;

namespace Benchmarks.Utility.Logging
{
    public class LogUtility
    {
        public static ILoggerFactory LoggerFactory { get; } = BuildLoggerFactory();

        private static ILoggerFactory BuildLoggerFactory()
        {
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddConsole();

            return loggerFactory;
        }
    }
}
