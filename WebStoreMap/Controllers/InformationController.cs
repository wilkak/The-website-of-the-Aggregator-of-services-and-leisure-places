using System.Web.Mvc;

namespace WebStoreMap.Controllers
{
    public class InformationController : Controller
    {
        // GET: Information
        public ActionResult AboutUs()
        {
            return View();
        }

        public ActionResult UseConditions()
        {
            return View();
        }

        public ActionResult PrivacyPolicy()
        {
            return View();
        }

        public ActionResult CookieInfo()
        {
            return View();
        }
    }
}