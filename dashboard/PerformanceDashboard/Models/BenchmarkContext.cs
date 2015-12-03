// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Data.Entity;
using PerformanceDashboard.ViewModels;

namespace PerformanceDashboard.Models
{
    public class BenchmarkContext : DbContext
    {
        public DbSet<Run> Runs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Run>().ToTable("Runs");
        }
    }
}
