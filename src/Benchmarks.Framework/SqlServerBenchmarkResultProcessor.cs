// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Data;
using System.Data.SqlClient;

namespace Benchmarks.Framework
{
    public class SqlServerBenchmarkResultProcessor
    {
        private readonly string _connectionString;

        private static readonly string _insertCommand =
            $@"INSERT INTO [dbo].[Runs]
           ([{nameof(BenchmarkRunSummary.TestClassFullName)}]
            ,[{nameof(BenchmarkRunSummary.TestClass)}]
            ,[{nameof(BenchmarkRunSummary.TestMethod)}]
            ,[{nameof(BenchmarkRunSummary.Variation)}]
            ,[{nameof(BenchmarkRunSummary.MachineName)}]
            ,[{nameof(BenchmarkRunSummary.ProductReportingVersion)}]
            ,[{nameof(BenchmarkRunSummary.Framework)}]
            ,[{nameof(BenchmarkRunSummary.Architecture)}]
            ,[{nameof(BenchmarkRunSummary.CustomData)}]
            ,[{nameof(BenchmarkRunSummary.RunStarted)}]
            ,[{nameof(BenchmarkRunSummary.WarmupIterations)}]
            ,[{nameof(BenchmarkRunSummary.Iterations)}]
            ,[{nameof(BenchmarkRunSummary.TimeElapsedAverage)}]
            ,[{nameof(BenchmarkRunSummary.TimeElapsedPercentile99)}]
            ,[{nameof(BenchmarkRunSummary.TimeElapsedPercentile95)}]
            ,[{nameof(BenchmarkRunSummary.TimeElapsedPercentile90)}]
            ,[{nameof(BenchmarkRunSummary.TimeElapsedStandardDeviation)}]
            ,[{nameof(BenchmarkRunSummary.MemoryDeltaAverage)}]
            ,[{nameof(BenchmarkRunSummary.MemoryDeltaPercentile99)}]
            ,[{nameof(BenchmarkRunSummary.MemoryDeltaPercentile95)}]
            ,[{nameof(BenchmarkRunSummary.MemoryDeltaPercentile90)}]
            ,[{nameof(BenchmarkRunSummary.MemoryDeltaStandardDeviation)}])
     VALUES
           (@{nameof(BenchmarkRunSummary.TestClassFullName)}
           ,@{nameof(BenchmarkRunSummary.TestClass)}
           ,@{nameof(BenchmarkRunSummary.TestMethod)}
           ,@{nameof(BenchmarkRunSummary.Variation)}
           ,@{nameof(BenchmarkRunSummary.MachineName)}
           ,@{nameof(BenchmarkRunSummary.ProductReportingVersion)}
           ,@{nameof(BenchmarkRunSummary.Framework)}
           ,@{nameof(BenchmarkRunSummary.Architecture)}
           ,@{nameof(BenchmarkRunSummary.CustomData)}
           ,@{nameof(BenchmarkRunSummary.RunStarted)}
           ,@{nameof(BenchmarkRunSummary.WarmupIterations)}
           ,@{nameof(BenchmarkRunSummary.Iterations)}
           ,@{nameof(BenchmarkRunSummary.TimeElapsedAverage)}
           ,@{nameof(BenchmarkRunSummary.TimeElapsedPercentile99)}
           ,@{nameof(BenchmarkRunSummary.TimeElapsedPercentile95)}
           ,@{nameof(BenchmarkRunSummary.TimeElapsedPercentile90)}
           ,@{nameof(BenchmarkRunSummary.TimeElapsedStandardDeviation)}
           ,@{nameof(BenchmarkRunSummary.MemoryDeltaAverage)}
           ,@{nameof(BenchmarkRunSummary.MemoryDeltaPercentile99)}
           ,@{nameof(BenchmarkRunSummary.MemoryDeltaPercentile95)}
           ,@{nameof(BenchmarkRunSummary.MemoryDeltaPercentile90)}
           ,@{nameof(BenchmarkRunSummary.MemoryDeltaStandardDeviation)})";

        private static readonly string _tableCreationCommand =
            $@"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='runs' and xtype='U')
	CREATE TABLE [dbo].[Runs](
		[Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
		[{nameof(BenchmarkRunSummary.TestClassFullName)}] [nvarchar](max) NULL,
		[{nameof(BenchmarkRunSummary.TestClass)}] [nvarchar](max) NULL,
		[{nameof(BenchmarkRunSummary.TestMethod)}] [nvarchar](max) NULL,
		[{nameof(BenchmarkRunSummary.Variation)}] [nvarchar](max) NULL,
		[{nameof(BenchmarkRunSummary.MachineName)}] [nvarchar](max) NULL,
		[{nameof(BenchmarkRunSummary.ProductReportingVersion)}] [nvarchar](max) NULL,
		[{nameof(BenchmarkRunSummary.Framework)}] [nvarchar](max) NULL,
		[{nameof(BenchmarkRunSummary.Architecture)}] [nvarchar](max) NULL,
		[{nameof(BenchmarkRunSummary.CustomData)}] [nvarchar](max) NULL,
		[{nameof(BenchmarkRunSummary.RunStarted)}] [datetime2](7) NOT NULL,
		[{nameof(BenchmarkRunSummary.WarmupIterations)}] [int] NOT NULL,
		[{nameof(BenchmarkRunSummary.Iterations)}] [int] NOT NULL,
		[{nameof(BenchmarkRunSummary.TimeElapsedAverage)}] [bigint] NOT NULL,
		[{nameof(BenchmarkRunSummary.TimeElapsedPercentile90)}] [bigint] NOT NULL,
		[{nameof(BenchmarkRunSummary.TimeElapsedPercentile95)}] [bigint] NOT NULL,
		[{nameof(BenchmarkRunSummary.TimeElapsedPercentile99)}] [bigint] NOT NULL,
		[{nameof(BenchmarkRunSummary.TimeElapsedStandardDeviation)}] [float] NOT NULL,
		[{nameof(BenchmarkRunSummary.MemoryDeltaAverage)}] [bigint] NOT NULL,
		[{nameof(BenchmarkRunSummary.MemoryDeltaPercentile90)}] [bigint] NOT NULL,
		[{nameof(BenchmarkRunSummary.MemoryDeltaPercentile95)}] [bigint] NOT NULL,
		[{nameof(BenchmarkRunSummary.MemoryDeltaPercentile99)}] [bigint] NOT NULL,
		[{nameof(BenchmarkRunSummary.MemoryDeltaStandardDeviation)}] [float] NOT NULL)
ELSE IF NOT EXISTS (SELECT * FROM syscolumns WHERE name = 'Architecture')
	ALTER TABLE [dbo].[Runs] ADD [Architecture] nvarchar(max) NULL";

        public SqlServerBenchmarkResultProcessor(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void SaveSummary(BenchmarkRunSummary summary)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    EnsureRunsTableCreated(conn);
                    WriteSummaryRecord(summary, conn);
                }
            }
            catch
            {
                throw new Exception("Could not save results into the database.");
            }
        }

        private void EnsureRunsTableCreated(SqlConnection conn)
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = _tableCreationCommand;
            cmd.ExecuteNonQuery();
        }

        private static void WriteSummaryRecord(BenchmarkRunSummary summary, SqlConnection conn)
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = _insertCommand;
            cmd.Parameters.AddWithValue($"@{nameof(summary.TestClassFullName)}", summary.TestClassFullName);
            cmd.Parameters.AddWithValue($"@{nameof(summary.TestClass)}", summary.TestClass);
            cmd.Parameters.AddWithValue($"@{nameof(summary.TestMethod)}", summary.TestMethod);
            cmd.Parameters.AddWithValue($"@{nameof(summary.Variation)}", summary.Variation);
            cmd.Parameters.AddWithValue($"@{nameof(summary.MachineName)}", summary.MachineName);
            cmd.Parameters.AddWithValue($"@{nameof(summary.ProductReportingVersion)}", summary.ProductReportingVersion);
            cmd.Parameters.AddWithValue($"@{nameof(summary.Framework)}", summary.Framework);
            cmd.Parameters.AddWithValue($"@{nameof(summary.Architecture)}", summary.Architecture);
            cmd.Parameters.Add($"@{nameof(summary.CustomData)}", SqlDbType.NVarChar).Value = (object)summary.CustomData ?? DBNull.Value;
            cmd.Parameters.AddWithValue($"@{nameof(summary.RunStarted)}", summary.RunStarted);
            cmd.Parameters.AddWithValue($"@{nameof(summary.WarmupIterations)}", summary.WarmupIterations);
            cmd.Parameters.AddWithValue($"@{nameof(summary.Iterations)}", summary.Iterations);
            cmd.Parameters.AddWithValue($"@{nameof(summary.TimeElapsedAverage)}", summary.TimeElapsedAverage);
            cmd.Parameters.AddWithValue($"@{nameof(summary.TimeElapsedPercentile99)}", summary.TimeElapsedPercentile99);
            cmd.Parameters.AddWithValue($"@{nameof(summary.TimeElapsedPercentile95)}", summary.TimeElapsedPercentile95);
            cmd.Parameters.AddWithValue($"@{nameof(summary.TimeElapsedPercentile90)}", summary.TimeElapsedPercentile90);
            cmd.Parameters.AddWithValue($"@{nameof(summary.TimeElapsedStandardDeviation)}", summary.TimeElapsedStandardDeviation);
            cmd.Parameters.AddWithValue($"@{nameof(summary.MemoryDeltaAverage)}", summary.MemoryDeltaAverage);
            cmd.Parameters.AddWithValue($"@{nameof(summary.MemoryDeltaPercentile99)}", summary.MemoryDeltaPercentile99);
            cmd.Parameters.AddWithValue($"@{nameof(summary.MemoryDeltaPercentile95)}", summary.MemoryDeltaPercentile95);
            cmd.Parameters.AddWithValue($"@{nameof(summary.MemoryDeltaPercentile90)}", summary.MemoryDeltaPercentile90);
            cmd.Parameters.AddWithValue($"@{nameof(summary.MemoryDeltaStandardDeviation)}", summary.MemoryDeltaStandardDeviation);

            cmd.ExecuteNonQuery();
        }
    }
}
