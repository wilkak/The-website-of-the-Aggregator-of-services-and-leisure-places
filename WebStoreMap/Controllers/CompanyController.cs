using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using WebStoreMap.Models.Data;
using WebStoreMap.Models.ViewModels.Company;
using WebStoreMap.Models.ViewModels.Place;

namespace WebStoreMap.Controllers
{
    public class CompanyController : Controller
    {
        // GET: CompanyDetails
        [HttpGet]      
        [ActionName("Company-details")]
        public ActionResult CompanyDetails(string Name)
        {
            if (Name == null)
            {
                return HttpNotFound();
            }
            
            CompanyViewModel Model;
            CompanyDataTransferObject dto;

            // Инициализируем ID продукта
            int Id = 0;

            using (DataBase DataBase = new DataBase())
            {
                // Проверяем, доступен ли продукт
                if (!DataBase.Companies.Any(x => x.Slug.Equals(Name)))
                {
                    return RedirectToAction("Index", "Place");
                }

                // Инициализируем модель productDTO
                dto = DataBase.Companies.Where(x => x.Slug == Name).FirstOrDefault();

                // Получаем ID
                Id = dto.Id;

                // Инициализируем модель данными
                Model = new CompanyViewModel(dto);
            }
            // Получаем изображения из галереи
            try
            {
                Model.GalleryImages = Directory
                    .EnumerateFiles(Server.MapPath("~/Images/Uploads/Companies/" + Id + "/Gallery/Thumbs"))
                    .Select(fn => Path.GetFileName(fn));
            }
            catch
            {

            }
            // Возвращаем модель и представление
            return View("CompanyDetails", Model);
        }



        public ActionResult CompanyPlaces(int? Name)
        {

            if (Name == null)
            {
                return HttpNotFound();
            }
           
            // Объявляем модель типа лист CategoryViewModel
            List<PlaceViewModel> CategoryViewModelList;

            // Инициализируем модель данными
            using (DataBase DataBase = new DataBase())
            {
                CategoryViewModelList = DataBase.Places.ToArray().Where(x => x.CompanyId == Name).Select(x => new PlaceViewModel(x)).ToList();
            }

            // Возвращаем частичное представление с моделью
            return PartialView("CompanyPlaces", CategoryViewModelList);
        }
    }
}