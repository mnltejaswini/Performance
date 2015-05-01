// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.IO;

namespace Microsoft.AspNet.Tests.Performance.Utility.Helpers
{
    public class DnxHelper
    {
        private static readonly string DnxHome = ".dnx";
        private static readonly string StartCommandtemplate = "--appbase \"{0}\" \"{1}\" {2}";

        public static ProcessStartInfo BuildStartInfo(string appbasePath,
                                                      string dnxAlias = "default",
                                                      string framework = "clr",
                                                      string command = "run",
                                                      string host = "Microsoft.Framework.ApplicationHost")
        {
            var dnxPath = GetDnxExecutable(alias: dnxAlias, framework: framework);
            var arguments = string.Format(StartCommandtemplate, appbasePath, host, command);

            var psi = new ProcessStartInfo(dnxPath, arguments)
            {
                WorkingDirectory = appbasePath,
                UseShellExecute = true
            };

            return psi;
        }

        public static string GetDnxPath(string alias = "default", string framework = "clr")
        {
            var home = Environment.GetEnvironmentVariable("USERPROFILE");
            var aliasFile = Path.Combine(home, DnxHome, "alias", alias + ".txt");
            var dnxRuntimes = Path.Combine(home, DnxHome, "runtimes");

            if (!File.Exists(aliasFile))
            {
                return null;
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

        public static string GetDnxExecutable(string alias = "default", string framework = "clr")
        {
            return GetExecutable(alias, framework, "dnx.exe");
        }

        public static string GetDnuExecutable(string alias = "default", string framework = "clr")
        {
            return GetExecutable(alias, framework, "dnu.cmd");
        }

        private static string GetExecutable(string alias, string framework, string executable)
        {
            var dnxPath = DnxHelper.GetDnxPath(alias, framework);
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