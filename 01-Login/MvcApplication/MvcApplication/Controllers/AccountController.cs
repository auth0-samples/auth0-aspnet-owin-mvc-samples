using System;
using System.Collections.Generic;
using System.Linq;
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
        public ActionResult Claims()
        {
            return View();
        }
    }
}