// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using StandardMvcApi.ViewData;

namespace StandardMvcApi.Controllers
{
    /// <summary>
    /// There is no performance test scenario runs through this controller yet. The solo purpose of this controller
    /// is to fill out the routing/action-selection tables.
    /// </summary>
    public class RandomController : Controller
    {
        // GET: /<controller>/
        public ActionResult PlainContent()
        {
            return new ContentResult
            {
                Content = "Hello World from Content"
            };
        }

        public ActionResult JsonContent()
        {
            var jsonResult = new JsonResult(new { Name = "A json object", Value = 1234, Content = "Content" });

            return jsonResult;
        }

        public void DirectlyWrite()
        {
            Response.WriteAsync("Content is written directly");
        }

        public Order ObjectContent()
        {
            return new Order
            {
                Id = 1234,
                ProductName = "An order",
                Unit = 100
            };
        }

        public ActionResult ValidationSummary()
        {
            ModelState.AddModelError("something", "Something happened, show up in validation summary.");

            return View("ValidationSummary");
        }
        
        public ActionResult NotFound()
        {
            return HttpNotFound();
        }
    }
}
