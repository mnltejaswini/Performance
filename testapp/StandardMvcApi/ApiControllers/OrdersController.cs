// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Mvc;
using StandardMvcApi.ViewData;

namespace StandardMvcApi.ApiControllers
{
    /// <summary>
    /// There is no performance test scenario runs through this contoller yet. The solo purpose of this controller
    /// is to fill out the routing/action-selection tables.
    /// </summary>
    [Route("api/Orders")]
    public class OrdersController : Controller
    {
        [HttpGet("Case1")]
        [Produces("application/xml", "application/json", "text/plain", "text/json", "text/xml")]
        public Order Case1()
        {
            return new Order
            {
                Id = 1,
                ProductName = "Jon Case One",
                Unit = 20
            };
        }

        [HttpGet("Case2")]
        [Produces("application/xml")]
        public Order Case2()
        {
            return new Order
            {
                Id = 2,
                ProductName = "Ron Case Two",
                Unit = 30
            };
        }

        [HttpGet("Case3")]
        public Order Case3()
        {
            return new Order
            {
                Id = 3,
                ProductName = "Don Case Three",
                Unit = 40
            };
        }

        [HttpPost("Case4")]
        [Produces("application/xml", "application/json", "text/plain", "text/json", "text/xml")]
        public Order Case4()
        {
            return new Order
            {
                Id = 4,
                ProductName = "Zon Case Four",
                Unit = 50
            };
        }

        [HttpPost("Case5")]
        [Produces("application/xml")]
        public Order Case5()
        {
            return new Order
            {
                Id = 5,
                ProductName = "Yon Case Five",
                Unit = 60
            };
        }

        [HttpPost("Case6")]
        public Order Case6()
        {
            return new Order
            {
                Id = 6,
                ProductName = "Bon Case Six",
                Unit = 60
            };
        }
    }
}