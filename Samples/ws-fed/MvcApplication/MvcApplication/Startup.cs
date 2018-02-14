using System;
using System.Configuration;
using System.IdentityModel.Metadata;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.WsFederation;
using Owin;

[assembly: OwinStartup(typeof(MvcApplication.Startup))]

namespace MvcApplication
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            string auth0Domain = ConfigurationManager.AppSettings["auth0:Domain"];
            string auth0ClientId = ConfigurationManager.AppSettings["auth0:ClientId"];
            string auth0AppName = ConfigurationManager.AppSettings["auth0:AppName"];

            // Enable Kentor Cookie Saver middleware
            app.UseKentorOwinCookieSaver();

            // Set Cookies as default authentication type
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = CookieAuthenticationDefaults.AuthenticationType,
                LoginPath = new PathString("/Account/Login")
            });

            // Configure Auth0 authentication via SAML
            app.UseWsFederationAuthentication(new WsFederationAuthenticationOptions
            {
                MetadataAddress = String.Format("https://{0}/wsfed/{1}/FederationMetadata/2007-06/FederationMetadata.xml", 
                    auth0Domain, 
                    auth0ClientId),
                Wtrealm = "urn:" + auth0AppName,
                Notifications = new WsFederationAuthenticationNotifications
                {
                    SecurityTokenValidated = notification =>
                    {
                        notification.AuthenticationTicket.Identity.AddClaim(new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "Auth0"));
                        return Task.FromResult(true);
                    }
                }
            });
        }
    }
}
