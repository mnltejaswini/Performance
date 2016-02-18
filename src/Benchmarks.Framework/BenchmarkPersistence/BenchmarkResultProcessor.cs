// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Benchmarks.Framework.BenchmarkPersistence
{
    public class BenchmarkResultProcessor : IDisposable
    {
        private Lazy<List<SqlServerBenchmarkResultProcessor>> _connections =
            new Lazy<List<SqlServerBenchmarkResultProcessor>>(
                () => BenchmarkConfig.Instance.ResultDatabases.Where(CanConnect).Select(
                    connection => new SqlServerBenchmarkResultProcessor(connection)).ToList());

        private static Lazy<BenchmarkResultProcessor> _singleton = new Lazy<BenchmarkResultProcessor>(() => new BenchmarkResultProcessor());

        private BenchmarkResultProcessor() { }

        public static BenchmarkResultProcessor Instance => _singleton.Value;

        internal static void ReleaseInstance()
        {
            _singleton = null;
        }

        public void SaveSummary(BenchmarkRunSummary summary)
        {
            Console.WriteLine(summary.ToString());
            foreach (var connection in _connections.Value)
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_connections != null)
                {
                    var connections = _connections.Value;
                    connections.ForEach(c => c.Dispose());
                    connections.Clear();
                    _connections = null;
                }
            }
        }
    }
}
