// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#if DNX451
using System;
using System.Collections.Generic;
using Benchmarks.Utility.Helpers;
using Microsoft.AspNet.Server.Testing;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNet.Tests.Performance
{
    public class IISTestManager : IDisposable
    {
        private bool _initalized = false;
        private readonly TestSampleManager _sampleManager = new TestSampleManager();
        private readonly List<IDisposable> _deployer = new List<IDisposable>();
        private readonly Dictionary<Tuple<string, RuntimeFlavor>, DeploymentResult> _deployments
                   = new Dictionary<Tuple<string, RuntimeFlavor>, DeploymentResult>();

        public void Initialize(ILoggerFactory loggerFactory)
        {
            if (_initalized)
            {
                return;
            }

            var sampleList = new Tuple<string, RuntimeFlavor>[]
            {
                Tuple.Create("StarterMvc", RuntimeFlavor.Clr),
                Tuple.Create("StarterMvc", RuntimeFlavor.CoreClr)
            };

            foreach (var sample in sampleList)
            {
                var source = _sampleManager.RestoreSampleInPlace(sample.Item1);
                var parameters = new DeploymentParameters(source, ServerType.IIS, sample.Item2, RuntimeArchitecture.x64);

                // This is a quick fix to turn around the build before the fix in Hosting eventually goes online
                parameters.ApplicationBaseUriHint = "http://localhost:0";

                var deployer = ApplicationDeployerFactory.Create(parameters, loggerFactory.CreateLogger<IISTestManager>());

                var result = deployer.Deploy();
                _deployments[sample] = result;
                _deployer.Add(deployer);
            }

            _initalized = true;
        }

        public void Dispose()
        {
            _deployer.ForEach(d => d.Dispose());
        }

        public string GetSite(string sampleName, RuntimeFlavor runtimeFlavor, bool restart)
        {
            // restart site is not implemented

            DeploymentResult deployment;
            if (_deployments.TryGetValue(Tuple.Create(sampleName, runtimeFlavor), out deployment))
            {
                return deployment.ApplicationBaseUri;
            }
            else
            {
                return null;
            }
        }
    }
}
#endif
