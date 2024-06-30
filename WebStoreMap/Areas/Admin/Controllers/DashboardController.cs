using System.Web.Mvc;

namespace WebStoreMap.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        // GET: Admin/Dashboard
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
    }
}