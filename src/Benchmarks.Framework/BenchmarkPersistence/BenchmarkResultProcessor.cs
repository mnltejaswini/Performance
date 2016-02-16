// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Benchmarks.Framework.BenchmarkPersistence
{
    public static class BenchmarkResultProcessor
    {
        private static readonly Lazy<List<string>> GoodConnectionStrings =
            new Lazy<List<string>>(() => BenchmarkConfig.Instance.ResultDatabases.Where(CanConnect).ToList());

        private static readonly Dictionary<string, SqlServerBenchmarkResultProcessor> Connections =
            new Dictionary<string, SqlServerBenchmarkResultProcessor>();

        public static void SaveSummary(BenchmarkRunSummary summary)
        {
            Console.WriteLine(summary.ToString());
            foreach (var connectionString in GoodConnectionStrings.Value)
            {
                ObtainConnection(connectionString).SaveSummary(summary);
            }
        }

        private static bool CanConnect(string connectionString)
        {
            var canConnect = false;
            var csb = new SqlConnectionStringBuilder(connectionString);
            try
            {
                csb.InitialCatalog = "master";
                var connection = new SqlConnection(csb.ConnectionString);
                connection.Open();
                connection.Close();
                canConnect = true;
            }
            catch
            {
                Console.Error.WriteLine($"Can't connect to the specified datasource { csb.DataSource }");
            }
            return canConnect;
        }

        private static SqlServerBenchmarkResultProcessor ObtainConnection(string connectionString)
        {
            SqlServerBenchmarkResultProcessor result;
            if (!Connections.TryGetValue(connectionString, out result))
            {
                Connections[connectionString] = result = new SqlServerBenchmarkResultProcessor(connectionString);
            }
            return result;
        }
    }
}
