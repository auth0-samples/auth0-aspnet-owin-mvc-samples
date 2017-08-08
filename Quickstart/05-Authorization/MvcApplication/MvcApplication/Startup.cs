using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Claims;
using System.Threading.Tasks;
using Auth0.Owin;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;
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
            // Configure Auth0 parameters
            string auth0Domain = ConfigurationManager.AppSettings["auth0:Domain"];
            string auth0ClientId = ConfigurationManager.AppSettings["auth0:ClientId"];
            string auth0ClientSecret = ConfigurationManager.AppSettings["auth0:ClientSecret"];

            // Enable Kentor Cookie Saver middleware
            app.UseKentorOwinCookieSaver();

            // Set Cookies as default authentication type
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = CookieAuthenticationDefaults.AuthenticationType,
                LoginPath = new PathString("/Account/Login")
            });

            // Configure Auth0 authentication
            var options = new Auth0AuthenticationOptions()
            {
                Domain = auth0Domain,
                ClientId = auth0ClientId,
                ClientSecret = auth0ClientSecret,

                Provider = new Auth0AuthenticationProvider
                {
                    OnAuthenticated = context =>
                    {
                        // Get the user's country
                        JToken countryObject = context.User["https://schemas.quickstarts.com/country"];
                        if (countryObject != null)
                        {
                            string country = countryObject.ToObject<string>();

                            context.Identity.AddClaim(new Claim("country", country, ClaimValueTypes.String, context.Connection));
                        }

                        // Get the user's roles
                        var rolesObject = context.User["https://schemas.quickstarts.com/roles"];
                        if (rolesObject != null)
                        {
                            string[] roles = rolesObject.ToObject<string[]>();
                            foreach (var role in roles)
                            {
                                context.Identity.AddClaim(new Claim(ClaimTypes.Role, role, ClaimValueTypes.String, context.Connection));
                            }
                        }


                        return Task.FromResult(0);
                    }
                }
            };
            app.UseAuth0Authentication(options);
        }
    }
}