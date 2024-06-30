using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebStoreMap.Models.Data;
using WebStoreMap.Models.ViewModels.Pages;

namespace WebStoreMap.Controllers
{
    public class PagesController : Controller
    {
        // Создаём метод Index
        // GET: Index/{Page}
        public ActionResult Index(string Page = "")
        {
            // Получаем/устанавливаем краткий заголовок (Slug)
            //if (Page == "")
            //{
            //    Page = "/Dashboard/Index";
            //}

            //// Инициализируем модель 
            //PageViewModel Model;
            //PageDataTransferObject dto;

            // Проверяем, доступна ли страница
            //using (DataBase DataBase = new DataBase())
            //{
            //    if (!DataBase.Pages.Any(x => x.Slug.Equals(Page)))
            //    {
            //        return RedirectToAction("Index", new { Page = "" });
            //    }
            //}

            //// Получаем страницы
            //using (DataBase DataBase = new DataBase())
            //{
            //    dto = DataBase.Pages.AsParallel().Where(x => x.Slug == Page).FirstOrDefault();
            //}

            //// Устанавливаем заголовок страницы (Title)
            //ViewBag.PageTitle = dto.Title;

            //// Проверяем боковую панель
            //ViewBag.Sidebar = dto.Sidebar ? "Yes" : "No";
            //// Заполняем модель данными
            //Model = new PageViewModel(dto);

            //// Возвращаем представление с моделью
            //return View(Model);

            return Redirect("~/Dashboard/Index");
        }

        public ActionResult PagesMenuPartial()
        {
            // Инициализируем лист PageViewModel
            List<PageViewModel> PageViewModelList;

            // Получаем все страницы, кроме HOME
            using (DataBase DataBase = new DataBase())
            {
                PageViewModelList = DataBase.Pages.ToArray().OrderBy(x => x.Sorting).Where(x => x.Slug != "home").Select(x => new PageViewModel(x)).ToList();
            }

            // Возвращаем частичное представление и лист с данными
            return PartialView(PageViewModelList);
        }

        public ActionResult SidebarPartial()
        {
            // Объявляем модель
            SidebarViewModel Model;

            // Инициализируем модель
            using (DataBase DataBase = new DataBase())
            {
                SidebarDataTransferObject dto = DataBase.Sidebars.Find(1);

                Model = new SidebarViewModel(dto);
            }
            // Возвращаем модель в частичное представление
            return PartialView("_SidebarPartial", Model);
        }
    }
}