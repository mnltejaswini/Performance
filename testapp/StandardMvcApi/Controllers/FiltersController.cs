// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Mvc;

namespace StandardMvcApi.Controllers
{
    /// <summary>
    /// There is no performance test scenario runs through this contoller yet. The solo purpose of this controller
    /// is to fill out the routing/action-selection tables.
    /// </summary>
    public class FiltersController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            ViewBag.Message = "Filters/Index";
            
            return View();
        }

        public IActionResult Next()
        {
            ViewBag.Message = "Filters/Next";

            return View("Index");
        }
    }
}
