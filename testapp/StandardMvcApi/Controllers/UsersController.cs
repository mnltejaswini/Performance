// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Mvc;
using StandardMvcApi.ViewData;
using StandardMvcApi.Filters;

namespace StandardMvcApi
{
    [Route("Users")]
    [DummyAuthorization]
    public class UsersController : Controller
    {
        [DummyAction]
        [HttpGet]
        // Fetch a user's information base on its id
        public IActionResult Get(int id)
        {
            // no db operation, just pretending...
            return new ObjectResult(SiteUser.GetUser(id));
        }

        [DummyAction]
        [DummyException]
        [HttpPost]
        public IActionResult Edit([FromBody] SiteUser user)
        {
            if (ModelState.IsValid && user != null)
            {
                return new ObjectResult(user);
            }
            else
            {
                return new HttpStatusCodeResult(400);
            }
        }
    }
}