// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNet.Mvc;

namespace StandardMvcApi.Controllers
{
    /// <summary>
    /// Used in memory test case to force GC
    /// </summary>
    public class Diagnostics : Controller
    {
        public bool Collect()
        {
            GC.Collect();

            return true;
        }
    }
}