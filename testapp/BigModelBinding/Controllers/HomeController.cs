// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Mvc;

namespace BigModelBinding.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index(Model model)
        {
            if (ModelState.IsValid)
            {
                return Ok(model);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
    }
}
