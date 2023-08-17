using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Threading.Tasks;

[assembly: OwinStartup(typeof(MVCPractice.Startup))]

namespace MVCPractice
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Login/Index"), // Redirect to login page if not authenticated
                ExpireTimeSpan = TimeSpan.FromMinutes(30),   // Set the cookie expiration time
                SlidingExpiration = true,                   // Extend cookie expiration on each request
                CookieHttpOnly = true,                      // The cookie is only accessible on the server-side
                CookieSecure = CookieSecureOption.SameAsRequest, // Use SameAsRequest for development, .Always for production with HTTPS
            });
        }
    }
}
