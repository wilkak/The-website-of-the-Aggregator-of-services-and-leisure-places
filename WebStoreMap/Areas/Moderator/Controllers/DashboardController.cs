using System.Web.Mvc;

namespace WebStoreMap.Areas.Moderator.Controllers
{
    [Authorize(Roles = "Moderator")]
    public class DashboardController : Controller
    {
        // GET: Moderator/Dashboard
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
    }
}