// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Benchmarks.Framework.ResultsLogging
{
    public class BenchmarkResultProcessor
    {
        public void SaveSummary(BenchmarkRunSummary summary,
                                IEnumerable<string> connectionStrings,
                                Action<Exception, string> exceptionHandler = null,
                                bool printToConsole = false,
                                bool onErrorThrow = false,
                                bool onErrorPrintToConsole = true)
        {
            foreach (var database in connectionStrings)
            {
                try
                {
                    new SqlServerBenchmarkResultProcessor(database).SaveSummary(summary);
                }
                catch (Exception ex)
                {
                    if (onErrorPrintToConsole)
                    {
                        printToConsole = true;
                    }
                    exceptionHandler?.Invoke(ex, database);
                    if (onErrorThrow)
                    {
                        throw;
                    }
                }
            }
            if (printToConsole)
            {
                Console.WriteLine(summary.ToString());
            }
        }
    }
}
