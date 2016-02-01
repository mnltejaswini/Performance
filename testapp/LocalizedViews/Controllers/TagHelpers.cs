// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Mvc;

namespace LocalizedViews.Controllers
{
    public class TagHelpers : Controller
    {
        public IActionResult Index(Person person)
        {
            return View(person);
        }
    }
}
