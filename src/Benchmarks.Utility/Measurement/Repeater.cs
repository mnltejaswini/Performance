// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Benchmarks.Utility.Measurement
{
    public class Repeater<TResult>
        where TResult : new()
    {
        private readonly Action<int, TResult> _body;
        private readonly Action<Exception, int, TResult> _exceptionHandler;

        public Repeater(Action<int, TResult> body, Action<Exception, int, TResult> exceptionHandler)
        {
            _body = body;
            _exceptionHandler = exceptionHandler;
        }

        public IList<TResult> Execute(int count)
        {
            var results = new List<TResult>();

            for (int i = 0; i < count; ++i)
            {
                var result = new TResult();

                try
                {
                    _body(i, result);
                }
                catch (Exception ex)
                {
                    _exceptionHandler(ex, i, result);
                }
                finally
                {
                    results.Add(result);
                }
            }

            return results;
        }
    }
}