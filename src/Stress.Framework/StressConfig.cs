// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Stress.Framework
{
    public class StressConfig
    {
        private static readonly Lazy<StressConfig> _instance = new Lazy<StressConfig>(() =>
            {
                var config = new ConfigurationBuilder()
                    .SetBasePath(".")
                    .AddJsonFile("config.json")
                    .AddEnvironmentVariables()
                    .Build();

                return new StressConfig
                {
                    RunIterations = bool.Parse(config["Stress:RunIterations"]),
                    Iterations = long.Parse(config["Stress:Iterations"]),
                    MetricReportInterval = int.Parse(config["Stress:MetricReportInterval"]),
                };
            });

        private StressConfig()
        {
        }

        public static StressConfig Instance => _instance.Value;

        public bool RunIterations { get; private set; }

        public long Iterations { get; private set; }

        public int MetricReportInterval { get; private set; }
    }
}
