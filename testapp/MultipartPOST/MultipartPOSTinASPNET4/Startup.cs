﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(MultipartPOSTinASPNET4.Startup))]

namespace MultipartPOSTinASPNET4
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
