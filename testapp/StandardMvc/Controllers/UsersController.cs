// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using StandardMvc.ViewData;
using StandardMvc.Filters;

namespace StandardMvc
{
    [DummyAuthorization]
    public class UsersController : Controller
    {
        private static readonly IEnumerable<SelectListItem> _ages = CreateAges();

        [DummyAction]
        [HttpGet]
        // Fetch a user's informance base on its id
        public IActionResult Index(int id)
        {
            // no db operation, just pretending...
            ViewBag.Id = id;

            return View(SiteUser.GetUser(id));
        }

        [DummyAction]
        [DummyException]
        [HttpGet]
        public IActionResult EditUser(int id)
        {
            ViewBag.Ages = _ages;

            return View(SiteUser.GetUser(id));
        }

        [DummyAction]
        [DummyException]
        [HttpPost]
        public IActionResult EditUser(SiteUser user)
        {
            if (ModelState.IsValid)
            {
                ViewBag.Message = "Successfully edited data";
                return View("Confirmation", user);
            }
            else
            {
                ViewBag.Ages = _ages;
                return View(user);
            }
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            ViewBag.Ages = _ages;
            return View(SiteUser.CreateNewUser());
        }

        [HttpPost]
        public IActionResult CreateUser(SiteUser user)
        {
            if (ModelState.IsValid)
            {
                ViewBag.Message = "Successfully created data";
                return View("Confirmation", user);
            }
            else
            {
                return View(user);
            }
        }

        private static IEnumerable<SelectListItem> CreateAges()
        {
            var ages = Enumerable
                .Range(27, 75 - 27)
                .Select(age => new { Age = age, Display = age.ToString("####"), });

            return new SelectList(ages, dataValueField: "Age", dataTextField: "Display");
        }
    }
}