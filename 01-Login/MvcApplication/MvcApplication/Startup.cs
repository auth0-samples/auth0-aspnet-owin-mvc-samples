using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

[assembly: OwinStartup(typeof(MvcApplication.Startup))]

namespace MvcApplication
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login")
            });

            // Use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Configure Auth0 authentication
            app.UseAuth0Authentication(
                clientId: System.Configuration.ConfigurationManager.AppSettings["auth0:ClientId"],
                clientSecret: System.Configuration.ConfigurationManager.AppSettings["auth0:ClientSecret"],
                domain: System.Configuration.ConfigurationManager.AppSettings["auth0:Domain"]);

        }
    }
}
