using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebStoreMap.Models.Data;
using WebStoreMap.Models.ViewModels.Place;

namespace WebStoreMap.Controllers
{
    public class MapController : Controller
    {
        public ActionResult Index(string Category = null)
        {
            List<PlaceViewModel> PlaceViewModelList = null;
            List<string> CategoryViewModelList = new List<string>();

            using (DataBase DataBase = new DataBase())
            {
                var Categories = from d in DataBase.Categories
                                                    orderby d.CategoryName
                                                    select d.CategoryName;
                var Place = from m in DataBase.Places
                                                    where m.View
                                                    select m;

                CategoryViewModelList.AddRange(Categories.Distinct());
                ViewBag.Categories = new SelectList(CategoryViewModelList);

                if (!string.IsNullOrEmpty(Category))

                {
                    Place = Place.Where(x => x.CategoryName == Category);
                }

                PlaceViewModelList = Place.ToArray().Select(x => new PlaceViewModel(x)).ToList();
            }

            return View(PlaceViewModelList);
        }
    }
}