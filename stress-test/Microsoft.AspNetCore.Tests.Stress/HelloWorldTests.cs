// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Net.Http;
using System.Threading.Tasks;
using Stress.Framework;

namespace Microsoft.AspNetCore.Tests.Stress
{
    public class HelloWorldTests : StressTestBase
    {
        public static void HelloWorld_Warmup(HttpClient client)
        {
            var response = client.GetAsync("/").GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
        }

        [Stress("BasicKestrel", WarmupMethodName = nameof(HelloWorld_Warmup))]
        public void Middleware_HelloWorld()
        {
            IterateAsync(client =>
            {
                var response = client.GetAsync("/").GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();
            });
        }

        [Stress("HelloWorldMvc", WarmupMethodName = nameof(HelloWorld_Warmup))]
        public void Mvc_HelloWorld()
        {
            IterateAsync(client =>
            {
                var response = client.GetAsync("/").GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();
            });
        }
    }
}
