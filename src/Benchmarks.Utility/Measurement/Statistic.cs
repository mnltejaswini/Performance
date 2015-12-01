// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Benchmarks.Utility.Measurement
{
    public static class Statistic
    {
        public static double Mean(this IEnumerable<double> samples)
        {
            if (samples.Count() == 0)
            {
                throw new ArgumentException("Zero samples", nameof(samples));
            }

            return samples.Sum() / samples.Count();
        }

        public static double StandardDeviation(this IEnumerable<double> samples)
        {
            var mean = samples.Mean();

            return Math.Sqrt(samples.Sum(v => Math.Pow(v - mean, 2)));
        }
    }
}