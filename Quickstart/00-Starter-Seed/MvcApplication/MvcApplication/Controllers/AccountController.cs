using System.Web.Mvc;

namespace MvcApplication.Controllers
{
    public class AccountController : Controller
    {
        [Authorize]
        public ActionResult Claims()
        {
            return View();
        }
    }
}