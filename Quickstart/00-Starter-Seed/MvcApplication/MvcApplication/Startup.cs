using Microsoft.Owin;
using Microsoft.Owin.Host.SystemWeb;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using MvcApplication.Support;
using Owin;

[assembly: OwinStartup(typeof(MvcApplication.Startup))]

namespace MvcApplication
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Set Cookies as default authentication type
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = CookieAuthenticationDefaults.AuthenticationType,
                LoginPath = new PathString("/Account/Login"),
                CookieSameSite = SameSiteMode.Lax,
                CookieManager = new SameSiteCookieManager(new SystemWebCookieManager()),
                CookieSecure = CookieSecureOption.SameAsRequest
            });

        }
    }
}
