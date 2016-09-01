using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace MvcApplication.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }

        [Authorize]
        public ActionResult Logout()
        {
            HttpContext.GetOwinContext().Authentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult Tokens()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;

            // Extract tokens
            string accessToken = claimsIdentity?.Claims.FirstOrDefault(c => c.Type == "access_token")?.Value;
            string idToken = claimsIdentity?.Claims.FirstOrDefault(c => c.Type == "id_token")?.Value;

            // Save tokens in ViewBag
            ViewBag.AccessToken = accessToken;
            ViewBag.IdToken = idToken;

            return View();
        }

        [Authorize]
        public ActionResult Claims()
        {
            return View();
        }
    }
}