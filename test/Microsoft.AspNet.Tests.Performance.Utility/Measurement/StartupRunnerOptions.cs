// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using Microsoft.Framework.Logging;

namespace Microsoft.AspNet.Tests.Performance.Utility.Measurement
{
    public class StartupRunnerOptions
    {
        public ProcessStartInfo ProcessStartInfo { get; set; }

        public int IterationCount { get; set; }

        public ILogger Logger { get; set; }
    }
}