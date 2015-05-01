// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;

namespace Microsoft.AspNet.Tests.Performance.Utility.Helpers
{
    public class EnvironmentHelper
    {
        public static bool Prepare()
        {
            var envPrepare = Environment.GetEnvironmentVariable("PERF_PREPARE");

            if (envPrepare == null)
            {
                return true;
            }

            var process = Process.Start(envPrepare);

            // timeout after 10 minutes
            var timeout = !process.WaitForExit(600 * 1000);

            return !timeout && (process.ExitCode == 0);
        }
    }
}