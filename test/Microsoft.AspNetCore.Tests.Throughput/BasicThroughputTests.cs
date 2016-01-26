using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Benchmarks.Utility.Helpers;
using Xunit;

namespace Microsoft.AspNetCore.Tests.Throughput
{
    public class BasicThroughputTests
    {
        private readonly int _clientCount;
        private readonly string _wcatClients;
        private readonly string _wcatControllerName;
        private readonly string _passwordFile;
        private readonly string _server;
        private readonly string _username;
        private readonly string _stagingFolder;

        public BasicThroughputTests()
        {
            _server = Environment.GetEnvironmentVariable("server.name");
            Assert.NotNull(_server);
            _username = Environment.GetEnvironmentVariable("username");
            Assert.NotNull(_username);
            _passwordFile = Environment.GetEnvironmentVariable("password_file");
            Assert.NotNull(_passwordFile);
            _wcatClients = Environment.GetEnvironmentVariable("client.names");
            Assert.NotNull(_wcatClients);
            _clientCount = _wcatClients.Split(',').Length;
            Assert.True(_clientCount > 0);
            _wcatControllerName = _wcatClients.Split(',')[0];
            Assert.NotNull(_wcatControllerName);

            _stagingFolder = PathHelper.GetNewTempFolder();
        }

        [Theory]
        [InlineData("BasicKestrel", "clr", 128)]
        [InlineData("BasicKestrel", "coreclr", 128)]
        [InlineData("BasicKestrel", "clr", 512)]
        [InlineData("BasicKestrel", "coreclr", 512)]
        [InlineData("BasicKestrelJson", "clr", 128)]
        [InlineData("BasicKestrelJson", "coreclr", 128)]
        [InlineData("BasicKestrelJson", "clr", 512)]
        [InlineData("BasicKestrelJson", "coreclr", 512)]
        [InlineData("HelloWorldMvc", "clr", 128)]
        [InlineData("HelloWorldMvc", "coreclr", 128)]
        [InlineData("HelloWorldMvc", "clr", 512)]
        [InlineData("HelloWorldMvc", "coreclr", 512)]
        [InlineData("PlainTextMvc", "clr", 128)]
        [InlineData("PlainTextMvc", "coreclr", 128)]
        [InlineData("PlainTextMvc", "clr", 512)]
        [InlineData("PlainTextMvc", "coreclr", 512)]
        [InlineData("LargePageMvc", "clr", 128)]
        [InlineData("LargePageMvc", "coreclr", 128)]
        [InlineData("LargePageMvc", "clr", 512)]
        [InlineData("LargePageMvc", "coreclr", 512)]
        public void BasicTest(string sampleName, string framework, int concurrency)
        {
            var testname = $"{sampleName}.{framework}.{concurrency}";

            var appsource = PathHelper.GetTestAppFolder(sampleName);
            Assert.NotNull(appsource);

            var powershell = new CommandLineRunner("powershell") { Timeout = TimeSpan.FromMinutes(5) };

            var deployScript = PathHelper.GetScript("deploy-site.ps1");
            var tempLocation = Path.Combine(_stagingFolder, testname);
            var deployArguments = $"{deployScript} -server {_server} -appsource {appsource} -username {_username} -password_file {_passwordFile} -framework {framework} -temp {tempLocation}";
            Console.WriteLine(deployArguments);

            var result = powershell.Execute(deployArguments);
            Assert.Equal(0, result);

            // Sleep for 5 seconds to allow the server to start.
            Thread.Sleep(TimeSpan.FromSeconds(5));

            var testScript = PathHelper.GetScript("throughput.ps1");
            var scenario = PathHelper.GetScript("basic_scenario.wcat");
            var settings = WriteWcatSettings(scenario, concurrency);
            var output = Path.Combine(PathHelper.GetArtifactFolder(), testname);
            Directory.CreateDirectory(output);
            var testArguments = $"{testScript} -controller {_wcatControllerName} -clients {_wcatClients} -settings {settings} -scenario {scenario} -username {_username} -password_file {_passwordFile} -output {output}\\";
            Console.WriteLine(testArguments);

            result = powershell.Execute(testArguments);
            Assert.Equal(0, result);
        }

        private string WriteWcatSettings(string scenario, int concurrency)
        {
            var content = new List<string>();
            content.Add("settings");
            content.Add("{");
            content.Add($"  clientfile = \"scenario.ubr\";");
            content.Add($"  server = \"{_server}\";");
            content.Add($"  virtualclients = {concurrency / _clientCount};");
            content.Add($"  clients = {_clientCount};");
            content.Add("}");

            var filepath = Path.Combine(PathHelper.GetNewTempFolder(), "settings.wcat");
            File.WriteAllLines(filepath, content);

            Console.WriteLine(File.ReadAllText(filepath));

            return filepath;
        }
    }
}