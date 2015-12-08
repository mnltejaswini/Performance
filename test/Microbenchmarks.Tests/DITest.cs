// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Benchmarks.Framework;
using Microsoft.Extensions.DependencyInjection;

namespace Microbenchmarks.Tests
{
    public class DITest : BenchmarkTestBase
    {
        [Benchmark]
        public void BuildServiceProvider()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddScoped<DependOnNonexistentService>();
            serviceCollection.AddTransient<ClassDependsOnPrivateConstructorClass>();
            serviceCollection.AddTransient<AbstractClass, ImplementedAbstractClass>();
            serviceCollection.AddTransient<EmptyInterface, ImplementedEmptyInterface>();
            serviceCollection.AddSingleton<ClassWithPrivateCtor>();

            using (Collector.StartCollection())
            {
                serviceCollection.BuildServiceProvider();
            }
        }

        [Benchmark]
        public void BuildServiceProvider_MvcServices()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddMvc(); ;

            using (Collector.StartCollection())
            {
                serviceCollection.BuildServiceProvider();
            }
        }

        private interface IFakeService
        {
        }

        private class DependOnNonexistentService
        {
            public DependOnNonexistentService(IFakeService nonExistentService)
            {
            }
        }

        private class ClassDependsOnPrivateConstructorClass
        {
            public ClassDependsOnPrivateConstructorClass(ClassWithPrivateCtor value)
            {

            }
        }

        private abstract class AbstractClass
        {
            public AbstractClass()
            {
            }
        }

        private class ImplementedAbstractClass : AbstractClass
        {
        }

        private class ClassWithPrivateCtor
        {
            private ClassWithPrivateCtor()
            {
            }
        }

        private interface EmptyInterface
        {
        }

        private class ImplementedEmptyInterface : EmptyInterface
        {
        }
    }
}
