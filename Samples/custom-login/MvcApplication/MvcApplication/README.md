# Auth0 - ASP.NET (OWIN) MVC sample

## Running the example

In order to run this project, you will need to add `http://localhost:3000/callback` to the list of **Allowed Callback URLs** for your Auth0 Client, and `http://localhost:3000/` to the list of **Allowed Logout URLs**.

Specify the correct name of your DB connection in line 51 of [AccountController.cs](AccountController.cs).

Also update the `auth0:ClientId`, `auth0:ClientSecret` and `auth0:Domain` settings in the `web.config` with the values of your Client.

## Issue Reporting

If you have found a bug or if you have a feature request, please report them at this repository issues section. Please do not report security vulnerabilities on the public GitHub issue tracker. The [Responsible Disclosure Program](https://auth0.com/whitehat) details the procedure for disclosing security issues.

## Author

[Auth0](auth0.com)