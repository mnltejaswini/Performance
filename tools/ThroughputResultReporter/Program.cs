using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Benchmarks.Framework;

namespace ThroughputResultReporter
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logLocation = args[0];
            var logFiles = Directory.GetFiles(logLocation, "log.xml", SearchOption.AllDirectories);

            foreach (var file in logFiles)
            {
                var name = Path.GetFileName(Path.GetDirectoryName(file));
                var parts = name.Split('.').ToArray();
                Console.WriteLine($"Sample {parts[0]}, Framework {parts[1]}, Concurrency {parts[2]}.");

                var summary = new BenchmarkRunSummary
                {
                    TestClassFullName = $"Microsoft.AspNetCore.Tests.Throughput.BasicThroughputTests",
                    TestClass = $"BasicThroughputTests",
                    TestMethod = $"BasicTest",
                    ProductReportingVersion = BenchmarkConfig.Instance.ProductReportingVersion,
                    Iterations = 1,
                    WarmupIterations = 0,
                    Architecture = "x86",
                    CustomData = string.Empty,
                    Framework = parts[1]
                };

                using (var fs = File.OpenRead(file))
                {
                    var xdoc = XDocument.Load(fs);

                    var info = xdoc.Descendants("section")
                                   .First(elem => elem.Attribute("name").Value == "header")
                                   .Descendants("data")
                                   .ToDictionary(elem => elem.Attribute("name").Value, elem => elem.Value);

                    summary.RunStarted = DateTime.Parse(info["start"]);
                    summary.MachineName = info["host"];
                    summary.Variation = $"Sample={parts[0]} Concurrency={parts[2]}";

                    var data = xdoc.Descendants("section")
                                   .First(elem => elem.Attribute("name").Value == "summary")
                                   .Descendants("item").First()
                                   .Descendants("data")
                                   .ToDictionary(elem => elem.Attribute("name").Value, elem => elem.Value);

                    var rps = (long)double.Parse(data["rps"]);
                    summary.Aggregate(new BenchmarkIterationSummary { TimeElapsed = rps });
                    summary.PopulateMetrics();

                    foreach (var database in BenchmarkConfig.Instance.ResultDatabases)
                    {
                        try
                        {
                            new SqlServerBenchmarkResultProcessor(database).SaveSummary(summary);
                        }
                        catch (Exception ex)
                        {
                            Console.Error.WriteLine($"Failed to save results to {ex}");
                            throw;
                        }
                    }
                }
            }
        }
    }
}
