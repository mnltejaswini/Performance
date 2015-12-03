// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Mvc;
using PerformanceDashboard.Models;
using PerformanceDashboard.ViewModels;
using PerformanceDashboard.ViewModels.Dashboard;

namespace PerformanceDashboard.Controllers
{
    public class DashboardController : Controller
    {
        private readonly BenchmarkRepository _repository;

        public DashboardController(BenchmarkRepository repository)
        {
            _repository = repository;
        }

        public IActionResult Index(int days = 5, Metrics metric = Metrics.TimeElapsedPercentile95)
        {
            return View(new IndexViewModel(_repository.GetHistory(days), days, metric));
        }

        public IActionResult History(
            string testClass,
            string testMethod,
            int days = 30,
            Metrics metric = Metrics.TimeElapsedPercentile95)
        {
            return View(new HistoryViewModel
            {
                TestClass = testClass,
                TestMethod = testMethod,
                Days = days,
                Metric = metric,
                Runs = _repository.GetTestHistory(testClass, testMethod, days)
            });
        }

        public IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml");
        }
    }
}
