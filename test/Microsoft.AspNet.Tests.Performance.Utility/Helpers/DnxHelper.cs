// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.IO;

namespace Microsoft.AspNet.Tests.Performance.Utility.Helpers
{
    public class DnxHelper
    {
        private readonly string DnxHome = ".dnx";
        private readonly string _alias;

        public DnxHelper() : this("default") { }

        public DnxHelper(string alias)
        {
            _alias = alias;
        }

        public ProcessStartInfo BuildStartInfo(string appbasePath,
                                               string framework,
                                               string argument)
        {
            var dnxPath = GetDnxExecutable(framework);
            var psi = new ProcessStartInfo(dnxPath, argument)
            {
                WorkingDirectory = appbasePath,
                UseShellExecute = true
            };

            return psi;
        }

        public bool Restore(string workingDir, string framework, bool quiet = false)
        {
            var dnu = GetDnuExecutable(framework);

            var psi = new ProcessStartInfo(dnu)
            {
                Arguments = "restore" + (quiet ? " --quiet" : ""),
                WorkingDirectory = workingDir,
                UseShellExecute = false
            };

            var proc = Process.Start(psi);

            var exited = proc.WaitForExit(300 * 1000);

            return exited && proc.ExitCode == 0;
        }

        public bool Publish(string workingDir, string framework, string outputDir, bool nosource, bool quiet)
        {
            var dnu = GetDnuExecutable(framework);

            var psi = new ProcessStartInfo(dnu)
            {
                Arguments = $"publish --out {outputDir}" + (nosource ? " --no-source" : "") + (quiet ? " --quiet" : ""),
                WorkingDirectory = workingDir,
                UseShellExecute = false
            };

            var proc = Process.Start(psi);

            var exited = proc.WaitForExit(300 * 1000);

            return exited && proc.ExitCode == 0;
        }

        public string GetDnxPath(string framework = "clr")
        {
            var home = Environment.GetEnvironmentVariable("USERPROFILE");
            var aliasFile = Path.Combine(home, DnxHome, "alias", _alias + ".txt");
            var dnxRuntimes = Path.Combine(home, DnxHome, "runtimes");

            if (!File.Exists(aliasFile))
            {
                // fall back to use default alias
                aliasFile = Path.Combine(home, DnxHome, "alias", "default.txt");
                if (!File.Exists(aliasFile))
                {
                    return null;
                }
            }

            var aliasFileContent = File.ReadAllLines(aliasFile);
            if (aliasFileContent.Length < 1)
            {
                return null;
            }

            // split into name and version
            var parts = aliasFileContent[0].Split(new char[] { '.' }, 2);
            if (parts.Length < 2)
            {
                return null;
            }

            // split into 'dnx', framework, system, arch
            var dnxNameParts = parts[0].Split(new char[] { '-' }, 4);
            if (dnxNameParts.Length < 4)
            {
                // mono will be illegal for now
                return null;
            }

            dnxNameParts[1] = framework;

            var fullname = string.Format("{0}.{1}", string.Join("-", dnxNameParts), parts[1]);
            var result = Path.Combine(dnxRuntimes, fullname);
            if (Directory.Exists(result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        public string GetDnxExecutable(string framework = "clr")
        {
            return GetExecutable(framework, "dnx.exe");
        }

        public string GetDnuExecutable(string framework = "clr")
        {
            return GetExecutable(framework, "dnu.cmd");
        }

        private string GetExecutable(string framework, string executable)
        {
            var dnxPath = GetDnxPath(framework);
            if (dnxPath != null)
            {
                return Path.Combine(dnxPath, "bin", executable);
            }
            else
            {
                return null;
            }
        }
    }
}
