using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using MvcApplication.ViewModels;

namespace MvcApplication.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Login(string returnUrl)
        {
            HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties
                {
                    RedirectUri = returnUrl ?? Url.Action("Index", "Home")
                },
                "Auth0");
            return new HttpUnauthorizedResult();
        }

        [Authorize]
        public void Logout()
        {
            HttpContext.GetOwinContext().Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            HttpContext.GetOwinContext().Authentication.SignOut("Auth0");
        }

        [Authorize]
        public ActionResult UserProfile()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;

            return View(new UserProfileViewModel()
            {
                Name = claimsIdentity?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value,
                EmailAddress = claimsIdentity?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
                ProfileImage = claimsIdentity?.Claims.FirstOrDefault(c => c.Type == "picture")?.Value,
                Country = claimsIdentity?.Claims.FirstOrDefault(c => c.Type == "country")?.Value
            });
        }

        [Authorize(Roles = "admin")]
        public ActionResult Admin()
        {
            return View();
        }

        [Authorize]
        public ActionResult Tokens()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;

            // Extract tokens
            string accessToken = claimsIdentity?.Claims.FirstOrDefault(c => c.Type == "access_token")?.Value;
            string idToken = claimsIdentity?.Claims.FirstOrDefault(c => c.Type == "id_token")?.Value;
            string refreshToken = claimsIdentity?.Claims.FirstOrDefault(c => c.Type == "refresh_token")?.Value;

            // Save tokens in ViewBag
            ViewBag.AccessToken = accessToken;
            ViewBag.IdToken = idToken;
            ViewBag.RefreshToken = refreshToken;

            return View();
        }

        [Authorize]
        public ActionResult Claims()
        {
            return View();
        }
    }
}