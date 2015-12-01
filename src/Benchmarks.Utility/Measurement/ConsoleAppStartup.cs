// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Framework.Logging;

namespace Benchmarks.Utility.Measurement
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
            _options.Logger.LogData("Measurer", typeof(ConsoleAppStartup).Name, infoOnly: true);
            _options.Logger.LogData("CommandFilename", _options.ProcessStartInfo.FileName, infoOnly: true);
            _options.Logger.LogData("CommandArguments", _options.ProcessStartInfo.Arguments, infoOnly: true);

            var repeater = new Repeater<RunResult>(
                body: (iteration, result) =>
                {
                    var sw = new Stopwatch();
                    sw.Start();

                    var process = Process.Start(_options.ProcessStartInfo);

                    var timeout = !process.WaitForExit(300 * 100);
                    sw.Stop();

                    if (timeout)
                    {
                        _options.Logger.LogError("Test case timeout. [Iteration {0}]", iteration);
                        return;
                    }

                    if (process.ExitCode != 0)
                    {
                        _options.Logger.LogError("Test sample exit code is not zero. [iteration {0}]", iteration);
                        return;
                    }

                    result.Elapsed = sw.ElapsedMilliseconds;
                    result.ExitCode = process.ExitCode;
                },
                exceptionHandler: (ex, iteration, result) =>
                {
                    result.Elapsed = -1;
                    result.ExitCode = -1;
                    result.Exception = ex;

                    _options.Logger.LogError("Unexpected exception at iteration " + iteration, ex);
                });

            var results = repeater.Execute(_options.IterationCount);
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