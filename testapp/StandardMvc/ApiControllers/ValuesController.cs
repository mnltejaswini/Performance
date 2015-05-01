// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Mvc;

namespace StandardMvc.ApiControllers
{
    /// <summary>
    /// There is no performance test scenario runs through this contoller yet. The solo purpose of this controller
    /// is to fill out the routing/action-selection tables.
    /// </summary>
    [Route("api/Values")]
    public class ValuesController : Controller
    {
        [HttpGet]
        public string ThisIsAGetMethod()
        {
            return "GET api/Values";
        }

        [HttpGet("[action]")]
        public string GetOtherThing()
        {
            // Will be GetOtherThing
            return (string)ActionContext.RouteData.Values["action"];
        }

        [HttpGet("Link")]
        public string GenerateLink(string action = null, string controller = null)
        {
            return Url.Action(action, controller);
        }
    }
}