// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Net.Http;
using System.Threading.Tasks;
using Stress.Framework;

namespace Microsoft.AspNet.Tests.Stress
{
    public class KestrelTests : StressTestBase
    {
        public static async Task HelloWorldMiddleware_Warmup(HttpClient client)
        {
            var response = await client.GetAsync("/");
            response.EnsureSuccessStatusCode();
        }

        [Stress("BasicKestrel", WarmupMethodName = nameof(HelloWorldMiddleware_Warmup))]
        public async Task HelloWorldMiddleware()
        {
            await IterateAsync(async client =>
            {
                var response = await client.GetAsync("/");
                response.EnsureSuccessStatusCode();
            });
        }
    }
}
