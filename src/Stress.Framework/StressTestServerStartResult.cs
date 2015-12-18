// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Stress.Framework
{
    public class StressTestServerStartResult
    {
        public IDisposable ServerHandle { get; set; }

        public bool SuccessfullyStarted { get; set; }
    }
}
