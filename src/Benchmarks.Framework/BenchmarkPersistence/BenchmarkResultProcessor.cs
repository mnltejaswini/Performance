// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Benchmarks.Framework.BenchmarkPersistence
{
    public class BenchmarkResultProcessor
    {
        private static readonly Lazy<List<SqlServerBenchmarkResultProcessor>> Connections =
            new Lazy<List<SqlServerBenchmarkResultProcessor>>(
                () => BenchmarkConfig.Instance.ResultDatabases.Where(CanConnect).Select(
                    connection => new SqlServerBenchmarkResultProcessor(connection)).ToList());

        public static void SaveSummary(BenchmarkRunSummary summary)
        {
            Console.WriteLine(summary.ToString());
            foreach (var connection in Connections.Value)
            {
                connection.SaveSummary(summary);
            }
        }

        private static bool CanConnect(string connectionString)
        {
            var csb = new SqlConnectionStringBuilder(connectionString);
            try
            {
                csb.InitialCatalog = "master";
                var connection = new SqlConnection(csb.ConnectionString);
                connection.Open();
                connection.Close();
                return true;
            }
            catch
            {
                Console.Error.WriteLine($"Can't connect to the specified datasource { csb.DataSource }");
            }
            return false;
        }
    }
}
