// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Xml;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Net.Http.Headers;

namespace StandardMvcApi
{
    public class Startup
    {
        private static readonly MediaTypeHeaderValue JsonMediaType = MediaTypeHeaderValue.Parse("application/json");
        private static readonly MediaTypeHeaderValue XmlSerializerMediaType = MediaTypeHeaderValue.Parse("application/xml+xmlserializer");
        private static readonly MediaTypeHeaderValue XmlDataContractSerializerMediaType = MediaTypeHeaderValue.Parse("application/xml+datacontractserializer");

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.Configure<MvcOptions>(options =>
            {
                options.InputFormatters.Clear();

                var jsonInputFormatter = new JsonInputFormatter();
                jsonInputFormatter.SupportedMediaTypes.Clear();
                jsonInputFormatter.SupportedMediaTypes.Add(JsonMediaType);
                options.InputFormatters.Add(jsonInputFormatter);

                var xmlSerializerInputFormatter = new XmlSerializerInputFormatter();
                xmlSerializerInputFormatter.SupportedMediaTypes.Clear();
                xmlSerializerInputFormatter.SupportedMediaTypes.Add(XmlSerializerMediaType);
                options.InputFormatters.Add(xmlSerializerInputFormatter);

                var xmlDataContractSerializerInputFormatter = new XmlDataContractSerializerInputFormatter();
                xmlDataContractSerializerInputFormatter.SupportedMediaTypes.Clear();
                xmlDataContractSerializerInputFormatter.SupportedMediaTypes.Add(XmlDataContractSerializerMediaType);
                options.InputFormatters.Add(xmlDataContractSerializerInputFormatter);

                options.OutputFormatters.Clear();

                var jsonOutputFormatter = new JsonOutputFormatter();
                jsonOutputFormatter.SupportedMediaTypes.Clear();
                jsonOutputFormatter.SupportedMediaTypes.Add(JsonMediaType);
                options.OutputFormatters.Add(jsonOutputFormatter);

                var xmlSerializerOutputFormatter = new XmlSerializerOutputFormatter();
                xmlSerializerOutputFormatter.SupportedMediaTypes.Clear();
                xmlSerializerOutputFormatter.SupportedMediaTypes.Add(XmlSerializerMediaType);
                options.OutputFormatters.Add(xmlSerializerOutputFormatter);

                var xmlDataContractSerializerOutputFormatter = new XmlDataContractSerializerOutputFormatter();
                xmlDataContractSerializerOutputFormatter.SupportedMediaTypes.Clear();
                xmlDataContractSerializerOutputFormatter.SupportedMediaTypes.Add(XmlDataContractSerializerMediaType);
                options.OutputFormatters.Add(xmlDataContractSerializerOutputFormatter);
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}