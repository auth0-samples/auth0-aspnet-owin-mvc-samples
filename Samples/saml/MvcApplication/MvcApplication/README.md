# SAML ASP.NET MVC Sample

This sample shows how to integrate your ASP.NET MVC application with Auth0 using SAML (without using the Auth0 SDKs). 

## Configuring the application in Auth0

1. Create a new application in Auth0
2. Enable the SAML addon on the Addons tab
3. In the settings of the SAML addon, specify the callback url `http://localhost:3000/callback` and these settings:

```json
{
  "audience":  "urn:MyApp",
}
```

## Configuring the ASP.NET MVC application. 

This sample uses the **Sustainsys.Saml2.Owin** NuGet package for SAML support. The library needs to be configured at application startup:

```csharp
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
```

Also be sure to update the `web.config` with the correct Auth0 settings:

```xml
<appSettings>
  <add key="auth0:Domain" value="YOUR_AUTH0_DOMAIN" />
  <add key="auth0:ClientId" value="YOUR_AUTH0_CLIENT_ID" />
  <add key="auth0:ReturnUrl" value="http://localhost:3000/" />
  <add key="auth0:AppName" value="MyApp" />
</appSettings>
```

The Account controller will start the Login flow to Auth0 by challenging the SAML2 middleware.

```csharp
public ActionResult Login(string returnUrl)
{
    return new ChallengeResult("Saml2", returnUrl ?? Url.Action("Index", "Home"));
}
```
