// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Mvc;

namespace Microsoft.AspNet.Test.Perf.WebFx.Apps.HelloMvc.Areas.Travel.Controllers
{
    /// <summary>
    /// There is no performance test scenario runs through this contoller yet. The solo purpose of this controller
    /// is to fill out the routing/action-selection tables.
    /// </summary>
    [Area("Travel")]
    public class FlightController : Controller
    {
        public IActionResult Fly()
        {
            return View();
        }
    }
}