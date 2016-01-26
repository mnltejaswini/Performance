// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO;

namespace Benchmarks.Utility.Helpers
{
    public class PathHelper
    {
        private static readonly string ArtifactFolder = "artifacts";
        private static readonly string TestAppFolder = "testapp";
        private static readonly string ScriptFolder = "scripts";

        public static string GetNuGetConfig()
        {
            var testFolder = Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory()));

            return Path.Combine(testFolder, "NuGet.config");
        }

        public static string GetTestAppFolder(string sampleName)
        {
            var testFolder = Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory()));
            var sampleFolder = Path.Combine(testFolder, TestAppFolder, sampleName);

            if (Directory.Exists(sampleFolder))
            {
                return sampleFolder;
            }
            else
            {
                return null;
            }
        }

        public static string GetScript(string scriptName)
        {
            var testFolder = Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory()));
            var script = Path.Combine(testFolder, ScriptFolder, scriptName);

            if (File.Exists(script))
            {
                return script;
            }
            else
            {
                return null;
            }
        }

        public static string GetArtifactFolder()
        {
            var testFolder = Directory.GetCurrentDirectory();
            var result = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(testFolder)), ArtifactFolder);

            if (!Directory.Exists(result))
            {
                Directory.CreateDirectory(result);
            }

            return result;
        }

        public static string GetNewTempFolder()
        {
            var result = Path.GetTempFileName();
            File.Delete(result);
            Directory.CreateDirectory(result);

            return result;
        }
    }
}