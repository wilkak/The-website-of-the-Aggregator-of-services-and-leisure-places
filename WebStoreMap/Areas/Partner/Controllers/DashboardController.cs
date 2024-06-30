using System.Web.Mvc;

namespace WebStoreMap.Areas.Partner.Controllers
{
    [Authorize(Roles = "Partner")]
    public class DashboardController : Controller
    {
        // GET: Partner/Dashboard
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
    }
}