// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using Benchmarks.Utility.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Benchmarks.Utility.Azure
{
    public class AzureCli
    {
        private readonly CommandLineRunner _runner;
        private readonly string _workingDirectory;

        private AzureCli(string azureCmd, string workingDirectory)
        {
            _runner = new CommandLineRunner(azureCmd)
            {
                RedirectOutput = true,
                Timeout = TimeSpan.FromMinutes(1)
            };

            _workingDirectory = workingDirectory;
        }

        public static AzureCli Create(string workingDirectory)
        {
            var cmdRunner = new CommandLineRunner()
            {
                RedirectOutput = true,
                Timeout = TimeSpan.FromSeconds(1)
            };

            if (cmdRunner.Execute("where azure") == -1)
            {
                return null;
            }

            var azureCmd = cmdRunner.LastOutput.Trim(' ', '\n', '\r');
            if (!File.Exists(azureCmd))
            {
                return null;
            }

            return new AzureCli(azureCmd, workingDirectory);
        }

        public string GetDefaultAccountName()
        {
            var accounts = RunCommandForJson("account list");

            var defaultAccount = accounts.FirstOrDefault(acct => acct["isDefault"].Value<bool>());
            if (defaultAccount != null)
            {
                return defaultAccount["name"].Value<string>();
            }
            else
            {
                return null;
            }
        }

        public JObject GetWebSiteInformation(string sitename)
        {
            var sites = RunCommandForJson($"site list {sitename}");

            return (JObject)sites.SingleOrDefault();
        }

        public JObject CreateWebSiteGit(string location, string sitename, string siteaccount)
        {
            _runner.Execute($"site create --location \"{location}\" --git --gitusername \"{siteaccount}\" {sitename}", _workingDirectory);
            return GetWebSiteInformation(sitename);
        }

        public void SetWebSiteScaleMode(string sitename, string mode)
        {
            _runner.Execute($"site scale mode --mode {mode} {sitename}");
        }

        public void DeleteWebSite(string sitename)
        {
            _runner.Execute($"site delete {sitename} -q", _workingDirectory);
        }

        public void ResetCredential(string username, string password)
        {
            _runner.Execute($"site deployment user set -u \"{username}\" -p \"{password}\"", _workingDirectory);
        }

        private JArray RunCommandForJson(string arguments)
        {
            if (!arguments.EndsWith("--json"))
            {
                arguments = $"{arguments.Trim()} --json";
            }

            if (_runner.Execute(arguments, _workingDirectory) == -1)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<JArray>(_runner.LastOutput);
        }

        public void AddAppSetting(string sitename, string key, string value)
        {
            _runner.Execute($"site appsetting add {key}={value} {sitename}");
        }
    }
}
