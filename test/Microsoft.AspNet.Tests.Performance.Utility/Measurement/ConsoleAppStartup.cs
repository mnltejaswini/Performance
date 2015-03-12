// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using Microsoft.Framework.Logging;

namespace Microsoft.AspNet.Tests.Performance.Utility.Measurement
{
    public class ConsoleAppStartup
    {
        private readonly ProcessStartInfo _processInfo;
        private readonly ILogger _logger;

        public ConsoleAppStartup(ProcessStartInfo processInfo, ILogger logger)
        {
            _processInfo = processInfo;
            _logger = logger;
        }

        public bool Run()
        {
            var sw = new Stopwatch();
            sw.Start();

            var process = Process.Start(_processInfo);

            var timeout = !process.WaitForExit(300 * 100);
            sw.Stop();

            if (timeout)
            {
                _logger.LogError("Test case timeout");
                return false;
            }

            if (process.ExitCode != 0)
            {
                _logger.LogError("Test sample exit code is not zero.");
                return false;
            }

            _logger.LogData("Measurer", typeof(ConsoleAppStartup).Name, infoOnly: true);
            _logger.LogData("ExitCode", process.ExitCode, infoOnly: true);
            _logger.LogData("CommandFilename", _processInfo.FileName, infoOnly: true);
            _logger.LogData("CommandArguments", _processInfo.Arguments, infoOnly: true);

            _logger.LogData("Time", sw.ElapsedMilliseconds);

            return true;
        }
    }
}