using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebStoreMap.Areas.Moderator.Controllers
{
    [Authorize(Roles = "Moderator")]
    public class TestController : Controller
    {
        // GET: Moderator/Test
        public ActionResult Index()
        {
            return View();
        }
    }
}