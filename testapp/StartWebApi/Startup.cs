using System;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Routing;
using Microsoft.Framework.DependencyInjection;

namespace StartWebApi
{
    public class Startup
    {
        // Configure is called after ConfigureServices is called.
        public void Configure(IApplicationBuilder app)
        {
            app.UseStaticFiles();

            app.UseServices(services =>
            {
                services.AddMvc();
            });

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
