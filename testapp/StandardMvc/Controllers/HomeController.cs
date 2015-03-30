// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Test.Perf.WebFx.Apps.HelloMvc.ViewData;

namespace Microsoft.AspNet.Test.Perf.WebFx.Apps.HelloMvc.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View(SiteUser.CreateNewUser());
        }

        public IActionResult About()
        {
            return View();
        }
    }
}