using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebStoreMap.Models.Data;
using WebStoreMap.Models.ViewModels.Pages;

namespace WebStoreMap.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PageController : Controller
    {
        // GET: Admin/Page
        [Authorize]
        public ActionResult Index()
        {
            //Объявляем список для представления PageVN
            List<PageViewModel> pageList;

            //Инициализировать список DataBase
            using (DataBase db = new DataBase())
            {
                pageList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageViewModel(x)).ToList();
            }

            //Возвращаем список в представление
            return View(pageList);
        }

        // GET: Admin/Page/AddPage
        [HttpGet]
        [Authorize]
        public ActionResult AddPage()
        {
            return View();
        }

        // POST: Admin/Page/AddPage
        [HttpPost]
        [Authorize]
        public ActionResult AddPage(PageViewModel model)
        {
            //Проверка модели на валидность
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            using (DataBase db = new DataBase())
            {
                //Объявляем переменую для краткого описания (Slug)
                string slug;

                //Инициализируем класс PageDTC
                PageDataTransferObject dtc = new PageDataTransferObject
                {
                    //Присваеваем заголовок модели
                    Title = model.Title
                };

                //Проверяем, есть ли краткое описание, если нет, присваеваем его
                slug = string.IsNullOrWhiteSpace(model.Slug) ? model.Title.Replace(" ", "-").ToLower() : model.Slug.Replace(" ", "-").ToLower();
                //Убеждаеися, что заголовок и краткое описание - уникальное
                if (db.Pages.Any(x => x.Title == model.Title))
                {
                    ModelState.AddModelError("", "Это название уже существует");
                    return View(model);
                }
                else if (db.Pages.Any(x => x.Slug == model.Slug))
                {
                    ModelState.AddModelError("", "Этот краткое описание уже существует");
                    return View(model);
                }
                //Присваеваем оставшиеся значения модели
                dtc.Slug = slug;
                dtc.Body = model.Body;
                dtc.Sidebar = model.Sidebar;
                dtc.Sorting = 100;

                //Сохраняем модель в базу данных
                _ = db.Pages.Add(dtc);
                _ = db.SaveChanges();
            }
            //Передаем сообщение через TempData
            TempData["SM"] = "Вы добавили новую страницу";
            //Переадресовываем пользователя на метод INDEX
            return RedirectToAction("Index");
        }

        // GET: Admin/Page/EditPage/Id
        [HttpGet]
        [Authorize]
        public ActionResult EditPage(int id)
        {
            //Объявляем модель pageVM
            PageViewModel model;

            using (DataBase db = new DataBase())
            {
                //Получаем страницу
                PageDataTransferObject dto = db.Pages.Find(id);

                //Подтверждаем, что страница доступна
                if (dto == null)
                {
                    return Content("Страница не существует.");
                }

                //Инициализируем модель данных
                model = new PageViewModel(dto);
            }
            //Возвращаем представление с моделью
            return View(model);
        }

        //Добавлаем метод редактирования страниц
        // POST: Admin/Page/EditPage/Id
        [HttpPost]
        [Authorize]
        public ActionResult EditPage(PageViewModel model)
        {
            //Проверяем модель на валидность
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (DataBase db = new DataBase())
            {
                //Получаем ID страницы
                int id = model.Id;

                //Объявляем краткий заголовок
                string slug = "home";

                //Получаем страницу
                PageDataTransferObject dto = db.Pages.Find(id);

                //присваиваем название в DTC
                dto.Title = model.Title;

                //Проверяем краткий заголовок и присваеваем его, если это необходимо
                if (model.Slug != "home")
                {
                    slug = string.IsNullOrWhiteSpace(model.Slug) ? model.Title.Replace(" ", "-").ToLower() : model.Slug.Replace(" ", "-").ToLower();
                }

                //Проверяем заголовок и краткий заголовок на уникальность
                if (db.Pages.Where(x => x.Id != id).Any(x => x.Title == model.Title))
                {
                    ModelState.AddModelError("", "Это название уже существует.");
                    return View(model);
                }
                else if (db.Pages.Where(x => x.Id != id).Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "Это краткое описание уже существует.");
                    return View(model);
                }

                //Записываем остальные значения в DTC
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.Sidebar = model.Sidebar;

                //Сохраняем в базу
                _ = db.SaveChanges();
            }
            //Устанавливаем сообщение в TemData
            TempData["SM"] = "Вы отредактировали страницу!";

            //Переадресовываем пользователя
            return RedirectToAction("EditPage");
        }

        //Создаём метод страницы деталей
        // GET: Admin/Page/PageDetails/Id
        [Authorize]
        public ActionResult PageDetails(int id)
        {
            //Объявляем модель PageViewModel
            PageViewModel model;

            using (DataBase db = new DataBase())
            {
                //Получаем страницу
                PageDataTransferObject dto = db.Pages.Find(id);

                //Подтверждаем, что страница доступна
                if (dto == null)
                {
                    return Content("Страница не существует.");
                }

                //Присваиваем модели данные из базы
                model = new PageViewModel(dto);
            }
            //Возвращаем модель в представление
            return View(model);
        }

        //Создаём метод удаления
        // GET: Admin/Page/DeletePage/Id
        [Authorize]
        public ActionResult DeletePage(int id)
        {
            using (DataBase db = new DataBase())
            {
                //Получаем страницу
                PageDataTransferObject dto = db.Pages.Find(id);

                //Удаляем страницу
                _ = db.Pages.Remove(dto);

                //Сохраняем изменения в базе
                _ = db.SaveChanges();
            }

            //Добавляем сообщение о удачном удалении страницы
            TempData["SM"] = "Вы удалили страницу!";

            //Переадресовываем пользователя
            return RedirectToAction("Index");
        }

        //Создаём метод сортировки
        // GET: Admin/Page/ReorderPages
        [HttpPost]
        [Authorize]
        public void ReorderPages(int[] id)
        {
            using (DataBase db = new DataBase())
            {
                //Реализуем начальный счётчик
                int count = 1;

                //Инициализируем модель данных
                PageDataTransferObject dto;

                //Устанавливаем сортировку для каждой страницы
                foreach (int pageId in id)
                {
                    dto = db.Pages.Find(pageId);
                    dto.Sorting = count;

                    _ = db.SaveChanges();

                    count++;
                }
            }
        }

        //Создаём метод редактирования боковой панели
        // GET: Admin/Page/EditSidebar
        [HttpGet]
        [Authorize]
        public ActionResult EditSidebar()
        {
            //Объявить модель
            SidebarViewModel model;

            using (DataBase db = new DataBase())
            {
                //Получить данные из DTO
                SidebarDataTransferObject dto = db.Sidebars.Find(1);

                //Заполнить модель данными
                model = new SidebarViewModel(dto);
            }

            //Вернуть представление с моделью
            return View(model);
        }

        //Создаём POST метод редактирования боковой панели
        // POST: Admin/Page/EditSidebar
        [HttpPost]
        [Authorize]
        public ActionResult EditSidebar(SidebarViewModel model)
        {
            using (DataBase db = new DataBase())
            {
                //Получаем данные (DTO)
                SidebarDataTransferObject dto = db.Sidebars.Find(1);
                //Присваиваем данные в тело
                dto.Body = model.Body;

                //Сохраняем
                _ = db.SaveChanges();
            }

            //Присваиваем сообщение в TempData
            TempData["SM"] = "Вы отредактировали боковую панель!";

            //Переадресовываем
            return RedirectToAction("EditSidebar");
        }
    }
}