// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Framework.Logging;

namespace Microsoft.AspNet.Tests.Performance.Utility.Measurement
{
    public class ConsoleAppStartup
    {
        private readonly StartupRunnerOptions _options;

        public ConsoleAppStartup(StartupRunnerOptions options)
        {
            _options = options;
        }

        public bool Run()
        {
            var results = new List<RunResult>();

            _options.Logger.LogData("Measurer", typeof(ConsoleAppStartup).Name, infoOnly: true);
            _options.Logger.LogData("CommandFilename", _options.ProcessStartInfo.FileName, infoOnly: true);
            _options.Logger.LogData("CommandArguments", _options.ProcessStartInfo.Arguments, infoOnly: true);

            for (int i = 0; i < _options.IterationCount; ++i)
            {
                var result = new RunResult();

                try
                {
                    var sw = new Stopwatch();
                    sw.Start();

                    var process = Process.Start(_options.ProcessStartInfo);

                    var timeout = !process.WaitForExit(300 * 100);
                    sw.Stop();

                    if (timeout)
                    {
                        _options.Logger.LogError("Test case timeout. [Iteration {0}]", i);
                        return false;
                    }

                    if (process.ExitCode != 0)
                    {
                        _options.Logger.LogError("Test sample exit code is not zero. [iteration {0}]", i);
                        return false;
                    }

                    result.Elapsed = sw.ElapsedMilliseconds;
                    result.ExitCode = process.ExitCode;
                }
                catch (Exception ex)
                {
                    result.Elapsed = -1;
                    result.ExitCode = -1;
                    result.Exception = ex;

                    _options.Logger.LogError("Unexpected exception at iteration " + i, ex);
                }
                finally
                {
                    results.Add(result);
                }
            }

            var successful = results.Where(r => r.ExitCode == 0);

            _options.Logger.LogData("Successful rate", successful.Count() / results.Count(), infoOnly: true);
            _options.Logger.LogData("Successful iteration", successful.Count(), infoOnly: true);
            _options.Logger.LogData("Time", successful.Average(r => r.Elapsed));

            return true;
        }

        private class RunResult
        {
            public long Elapsed { get; set; }

            public int ExitCode { get; set; }

            public Exception Exception { get; set; }
        }
    }
}