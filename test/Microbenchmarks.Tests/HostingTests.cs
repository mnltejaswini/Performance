// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Benchmarks.Framework;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Hosting.Server;
using Microsoft.AspNet.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microbenchmarks.Tests
{
    public class HostingTests : BenchmarkTestBase
    {
        private const string HostingJsonFile = "hosting.json";
        private const string EnvironmentVariablesPrefix = "ASPNET_";
        private const string ConfigFileKey = "config";

        [Benchmark]
        [BenchmarkVariation("Kestrel", "Microsoft.AspNet.Server.Kestrel")]
        [BenchmarkVariation("WebListener", "Microsoft.AspNet.Server.WebListener")]
        public void MainToConfigureOverhead(string variationServer)
        {
            var args = new[] { "--server", variationServer };

            using (Collector.StartCollection())
            {
                // Duplicating the work done in WebApplication.Run. We can't call directly into it because it'd
                // normally perform a blocking operation preventing the tests from continuing.
                var tempBuilder = new ConfigurationBuilder().AddCommandLine(args);
                var tempConfig = tempBuilder.Build();
                var configFilePath = tempConfig[ConfigFileKey] ?? HostingJsonFile;
                var config = new ConfigurationBuilder()
                    .AddJsonFile(configFilePath, optional: true)
                    .AddEnvironmentVariables()
                    .AddEnvironmentVariables(prefix: EnvironmentVariablesPrefix)
                    .AddCommandLine(args)
                    .Build();

                var hostBuilder = new WebHostBuilder(config, captureStartupErrors: true);
                hostBuilder
                    .UseStartup(typeof(TestStartup))
                    .UseServices(ConfigureTestServices);

                var host = hostBuilder.Build();
                host.Start().Dispose();
            }
        }

        private void ConfigureTestServices(IServiceCollection services)
        {
            services.AddTransient<IServerLoader, TestServerLoader>();
            services.AddSingleton(Collector);
        }

        private class TestStartup
        {
            public void ConfigureServices(IServiceCollection services)
            {
            }

            public void Configure(IApplicationBuilder app, IMetricCollector collector)
            {
                collector.StopCollection();
            }

            public static void Main(string[] args) => WebApplication.Run<TestStartup>(args);
        }

        private class TestServerLoader : IServerLoader
        {
            private readonly IServerLoader _wrappedServerLoader;

            public TestServerLoader(IServiceProvider services)
            {
                _wrappedServerLoader = new ServerLoader(services);
            }

            public IServerFactory LoadServerFactory(string assemblyName)
            {
                var factory = _wrappedServerLoader.LoadServerFactory(assemblyName);

                return new TestServerFactory(factory);
            }

            private class TestServerFactory : IServerFactory
            {
                private readonly IServerFactory _wrappedServerFactory;

                public TestServerFactory(IServerFactory wrappedServerFactory)
                {
                    _wrappedServerFactory = wrappedServerFactory;
                }

                public IServer CreateServer(IConfiguration configuration)
                {
                    var server = _wrappedServerFactory.CreateServer(configuration);

                    return new TestServer(server);
                }

                private class TestServer : IServer
                {
                    private readonly IServer _wrappedServer;

                    public TestServer(IServer wrappedServer)
                    {
                        _wrappedServer = wrappedServer;
                    }

                    public IFeatureCollection Features => _wrappedServer.Features;

                    public void Dispose() => _wrappedServer.Dispose();

                    public void Start<TContext>(IHttpApplication<TContext> application)
                    {
                        // No-op, we don't want to actually start the server.
                    }
                }
            }
        }
    }
}
