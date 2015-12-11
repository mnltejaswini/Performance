// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.IO;

namespace Benchmarks.Utility.Helpers
{
    public class CommandLineRunner
    {
        private readonly string _command;

        public CommandLineRunner(string command)
        {
            _command = command;

            Timeout = TimeSpan.FromMinutes(1);
        }

        public CommandLineRunner() : this("cmd") { }

        public bool RedirectOutput { get; set; }

        public string LastOutput { get; set; }

        public int LastExitCode { get; set; }

        public TimeSpan Timeout { get; set; }

        public int Execute(string arguments)
        {
            return Execute(arguments, null);
        }

        public int Execute(string arguments, string workingDirectory)
        {
            var startinfo = new ProcessStartInfo(_command, ProcessArguments(arguments))
            {
#if DNX451
                UseShellExecute = !RedirectOutput,
#else
                UseShellExecute = false,
#endif
                RedirectStandardOutput = RedirectOutput,
                WorkingDirectory = workingDirectory ?? Directory.GetCurrentDirectory()
            };

            var proc = Process.Start(startinfo);
            var exit = proc.WaitForExit((int)Timeout.TotalMilliseconds);

            LastExitCode = proc.ExitCode;

            if (RedirectOutput)
            {
                LastOutput = proc.StandardOutput.ReadToEnd();
            }

            return exit ? LastExitCode : -1;
        }

        private string ProcessArguments(string arguments)
        {
            if (_command == "cmd")
            {
                return $"/C \"{arguments}\"";
            }
            else
            {
                return arguments;
            }
        }
    }
}
