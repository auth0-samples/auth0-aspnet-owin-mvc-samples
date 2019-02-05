# SAML ASP.NET MVC Sample

This sample shows how to integrate your ASP.NET MVC application with Auth0 using SAML (without using the Auth0 SDKs). 

## Configuring the application in Auth0

1. Create a new application in Auth0
2. Enable the WS-FED addon on the Addons tab
3. In the settings of the WS-FED addon, specify the callback url `http://localhost:3000/callback` (or whatever the actual URL for your applications is,
and set the Realm to `urn:MyApp`.

## Configuring the ASP.NET MVC application. 

This sample uses the **Microsoft.Owin.Security.WsFederation** NuGet package for WS-FED support, so install the package:

```
Install-Package Microsoft.Owin.Security.WsFederation
```

The library needs to be configured at application startup:

```csharp
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
```

Also be sure to update the `web.config` with the correct Auth0 settings:

```xml
<appSettings>
  <add key="auth0:Domain" value="YOUR_AUTH0_DOMAIN" />
  <add key="auth0:ClientId" value="YOUR_AUTH0_CLIENT_ID" />
  <add key="auth0:AppName" value="MyApp" />
</appSettings>
```

The Account controller will start the Login flow to Auth0 by challenging the SAML2 middleware.

```csharp
public ActionResult Login(string returnUrl)
{
    return new ChallengeResult("Federation", returnUrl ?? Url.Action("Index", "Home"));
}
```
