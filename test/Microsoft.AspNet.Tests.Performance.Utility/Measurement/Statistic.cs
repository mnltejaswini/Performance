// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNet.Tests.Performance.Utility.Measurement
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
    }
}