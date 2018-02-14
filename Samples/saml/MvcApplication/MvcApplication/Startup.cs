using System;
using System.Configuration;
using System.IdentityModel.Metadata;
using System.Net;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Owin;
using Sustainsys.Saml2;
using Sustainsys.Saml2.Configuration;
using Sustainsys.Saml2.Owin;
using Sustainsys.Saml2.WebSso;

[assembly: OwinStartup(typeof(MvcApplication.Startup))]

namespace MvcApplication
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {

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
            app.UseSaml2Authentication(CreateAuthServicesOptions());
        }

        private static Saml2AuthenticationOptions CreateAuthServicesOptions()
        {
            // Configure Auth0 parameters
            string auth0Domain = ConfigurationManager.AppSettings["auth0:Domain"];
            string auth0ClientId = ConfigurationManager.AppSettings["auth0:ClientId"];
            string auth0ReturnUrl = ConfigurationManager.AppSettings["auth0:ReturnUrl"];
            string auth0AppName = ConfigurationManager.AppSettings["auth0:AppName"];

            var authServicesOptions = new Saml2AuthenticationOptions(false)
            {
                SPOptions = new SPOptions
                {
                    EntityId = new EntityId($"urn:{auth0AppName}"), 
                    ReturnUrl = new Uri(auth0ReturnUrl),
                    MinIncomingSigningAlgorithm = "http://www.w3.org/2000/09/xmldsig#rsa-sha1"
                }
            };

            authServicesOptions.IdentityProviders.Add(new IdentityProvider(new EntityId($"urn:{auth0Domain}"), authServicesOptions.SPOptions)
            {
                AllowUnsolicitedAuthnResponse = true,
                MetadataLocation  = String.Format("https://{0}/samlp/metadata/{1}", auth0Domain, auth0ClientId),
                Binding = Saml2BindingType.HttpPost
            });

            return authServicesOptions;
        }
    }
}
