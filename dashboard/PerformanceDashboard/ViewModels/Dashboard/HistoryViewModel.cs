// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Benchmarks.Framework.Model;

namespace PerformanceDashboard.ViewModels.Dashboard
{
    public class HistoryViewModel
    {
        public string TestClass { get; set; }
        public string TestMethod { get; set; }
        public int Days { get; set; }
        public Metrics Metric { get; set; }
        public IEnumerable<Run> Runs { get; set; }
    }
}
