// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Mvc;

namespace StandardMvc.Controllers
{
    /// <summary>
    /// There is no performance test scenario runs through this contoller yet. The solo purpose of this controller
    /// is to fill out the routing/action-selection tables.
    /// </summary>
    public class LinkController : Controller
    {
        public ActionResult Details()
        {
            return View();
        }

        public string About()
        {
            return Url.Action(null);
        }

        public string Get()
        {
            // Creates a url like: http://localhost/Home/Create#CoolBeans!
            return Url.RouteUrl(null, new { controller = "Home", action = "Create" }, protocol: "http", host: null, fragment: "CoolBeans!");
        }

        public string Link1()
        {
            return Url.Action("Index", "Home");
        }

        public string Link2()
        {
            return Url.Action("Link2");
        }
    }
}