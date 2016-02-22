// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Concurrent;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;

namespace Microsoft.AspNetCore.Test.Perf.WebFx.Apps.LowAlloc
{
    public class PooledHttpContextFactory : IHttpContextFactory
    {
        private readonly int _maxPooledContexts;
        private readonly ConcurrentQueue<PooledHttpContext> _pool = new ConcurrentQueue<PooledHttpContext>();

        public PooledHttpContextFactory(IConfiguration configuration)
        {
            _maxPooledContexts = GetPooledCount(configuration["hosting.maxPooledContexts"]);
        }

        public HttpContext Create(IFeatureCollection featureCollection)
        {
            PooledHttpContext httpContext = null;
            if (_pool.TryDequeue(out httpContext))
            {
                httpContext.Initialize(featureCollection);
            }
            else
            {
                httpContext = new PooledHttpContext(featureCollection);
            }

            return httpContext;
        }

        public void Dispose(HttpContext httpContext)
        {
            var pooled = httpContext as PooledHttpContext;
            if (pooled != null)
            {
                // approximation due to race condition on count
                if (_pool.Count < _maxPooledContexts)
                {
                    pooled.Uninitialize();
                    _pool.Enqueue(pooled);
                }
            }
        }

        private static int GetPooledCount(string countString)
        {
            if (string.IsNullOrEmpty(countString))
            {
                return 0;
            }

            int count;
            if (int.TryParse(countString, NumberStyles.Integer, CultureInfo.InvariantCulture, out count))
            {
                return count > 0 ? count : 0;
            }

            return 0;
        }
    }
}
