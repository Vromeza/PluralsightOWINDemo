using Owin;
using Pluralsight_OWIN.Demo.WebApp.Middleware;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Nancy.Owin;
using Nancy;
using System.Web.Http;

namespace Pluralsight_OWIN.Demo.WebApp
{
    public class Startup
    {
        public static void Configuration(IAppBuilder app) {
            app.UseDebugMiddleware(new DebugMiddlewareOptions {
                OnIncomingRequest = (ctx) => {
                    var watch = new Stopwatch();
                    watch.Start();
                    ctx.Environment["DebugStopwatch"] = watch;
                },
                OnOutgoingRequest = (ctx) => {
                    var watch = (Stopwatch)ctx.Environment["DebugStopwatch"];
                    watch.Stop();
                    Debug.WriteLine($"Request took {watch.ElapsedMilliseconds} ms to complete");
                }
            });

            app.UseCookieAuthentication(new Microsoft.Owin.Security.Cookies.CookieAuthenticationOptions {
                AuthenticationType = "ApplicationCookie",
                LoginPath = new Microsoft.Owin.PathString("/Auth/Login")
            });

            app.Use(async (ctx, next) => {
                if (ctx.Authentication.User.Identity.IsAuthenticated) {
                    Debug.WriteLine("User: " + ctx.Authentication.User.Identity.Name);
                }
                else {
                    Debug.WriteLine("User not authenticated");
                }
            });

            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            app.UseWebApi(config);

            app.Map("/nancy", mappedApp => { mappedApp.UseNancy(); });
            //app.UseNancy(conf => {
            //    conf.PassThroughWhenStatusCodesAre(HttpStatusCode.NotFound);
            //});

            //app.Use(async(ctx, next) => {
            //    await ctx.Response.WriteAsync("Hello World");
            //});
        }
    }
}