using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Auth0.Owin;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Newtonsoft.Json.Linq;
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
            var options = new Auth0AuthenticationOptions
            {
                ClientId = System.Configuration.ConfigurationManager.AppSettings["auth0:ClientId"],
                ClientSecret = System.Configuration.ConfigurationManager.AppSettings["auth0:ClientSecret"],
                Domain = System.Configuration.ConfigurationManager.AppSettings["auth0:Domain"],
                RedirectPath = new PathString("/Auth0Account/ExternalLoginCallback"),
                Provider = new Auth0AuthenticationProvider
                {
                    OnAuthenticated = context =>
                    {
                        JToken countryObject = null;
                        if (context.User.TryGetValue("country", out countryObject))
                        {
                            string country = countryObject.ToObject<string>();

                            context.Identity.AddClaim(new Claim("country", country, ClaimValueTypes.String, context.Connection));
                        }

                        return Task.FromResult(0);
                    }
                }
            };
            app.UseAuth0Authentication(options);
        }
    }
}
