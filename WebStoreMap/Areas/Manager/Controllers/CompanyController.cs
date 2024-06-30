using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using WebStoreMap.Models.Data;
using WebStoreMap.Models.ViewModels.Company;

namespace WebStoreMap.Areas.Manager.Controllers
{
    [Authorize(Roles = "Manager")]
    public class CompanyController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        //Создаём метод для добавления компании
        // GET:  Manager/Companies/AddCompany
        [HttpGet]
        [Authorize]
        public ActionResult AddCompany()
        {
            // Объявляем модель
            CompanyViewModel Model = new CompanyViewModel();
            // Возвращаем модель в представление
            return View(Model);
        }

        //Создаём метод добавления компании
        // POST: Manager/Companies/AddCompany/Model/File
        [HttpPost]
        [Authorize]
        public ActionResult AddCompany(CompanyViewModel Model, HttpPostedFileBase File)
        {
            // Проверяем модель на валидность
            if (!ModelState.IsValid)
            {
                using (DataBase DataBase = new DataBase())
                {
                    return View(Model);
                }
            }

            // Проверяем имя компании на уникальность
            using (DataBase DataBase = new DataBase())
            {
                if (DataBase.Companies.Any(x => x.CompanyName == Model.CompanyName))
                {
                    ModelState.AddModelError("", "Это название компании уже занято!");
                    return View(Model);
                }
            }

            // Объявляем переменную CompanyID
            int Id;
            string UserEmail = User.Identity.Name;

            // Инициализируем и сохраняем в базу модель CompanyDataTransferObject
            using (DataBase DataBase = new DataBase())
            {
                // Получаем пользователя
                UserDataTransferObject UserDataTransferObject = DataBase.Users.FirstOrDefault(x => x.EmailAddress == UserEmail);
                CompanyDataTransferObject Company = new CompanyDataTransferObject
                {
                    Id = Model.Id,
                    FullCompanyName = Model.FullCompanyName,
                    CompanyName = Model.CompanyName,
                    LegalAddress = Model.LegalAddress,
                    PostalAddress = Model.PostalAddress,
                    INN = Model.INN,
                    OGRN = Model.OGRN,
                    KPP = Model.KPP,
                    EmailAdress = Model.EmailAdress,
                    PhoneNumber = Model.PhoneNumber,
                    Description = Model.Description,
                    SlugTelegram = Model.SlugTelegram,

                    SlugVK = Model.SlugVK,
                    SlugWhatsapp = Model.SlugWhatsapp,
                    Slug = Model.CompanyName.Replace(@"""", "").Replace(",", "").Replace("'", "").Replace("?", "").Replace("!", "").Replace("/", "").Replace(".", "").Replace(" ", "-").ToLower(),
                    View = UserDataTransferObject.View,
                    UserId = UserDataTransferObject.Id
                };

                _ = DataBase.Companies.Add(Company);
                _ = DataBase.SaveChanges();

                // Получаем введённый ID
                Id = Company.Id;
            }

            // Добавляем сообщение в TempData
            TempData["SM"] = "Вы добавили компанию!";

            #region Upload Image

            // Создаём необходимые директории
            DirectoryInfo OriginalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));

            string PathString1 = Path.Combine(OriginalDirectory.ToString(), "Companies");
            string PathString2 = Path.Combine(OriginalDirectory.ToString(), "Companies\\" + Id.ToString());
            string PathString3 = Path.Combine(OriginalDirectory.ToString(), "Companies\\" + Id.ToString() + "\\Thumbs");
            string PathString4 = Path.Combine(OriginalDirectory.ToString(), "Companies\\" + Id.ToString() + "\\Gallery");
            string PathString5 = Path.Combine(OriginalDirectory.ToString(), "Companies\\" + Id.ToString() + "\\Gallery\\Thumbs");

            // Проверяем, есть ли дериктория по пути
            if (!Directory.Exists(PathString1))
            {
                _ = Directory.CreateDirectory(PathString1);
            }

            if (!Directory.Exists(PathString2))
            {
                _ = Directory.CreateDirectory(PathString2);
            }

            if (!Directory.Exists(PathString3))
            {
                _ = Directory.CreateDirectory(PathString3);
            }

            if (!Directory.Exists(PathString4))
            {
                _ = Directory.CreateDirectory(PathString4);
            }

            if (!Directory.Exists(PathString5))
            {
                _ = Directory.CreateDirectory(PathString5);
            }

            // Проверяем, был ли файл загружен
            if (File != null && File.ContentLength > 0)
            {
                // Получаем расширение файла
                string Ext = File.ContentType.ToLower();

                // Проверяем расширение
                if (Ext != "image/jpg" &&
                    Ext != "image/jpeg" &&
                    Ext != "image/pjpeg" &&
                    Ext != "image/gif" &&
                    Ext != "image/x-png" &&
                    Ext != "image/png")
                {
                    using (DataBase DataBase = new DataBase())
                    {
                        ModelState.AddModelError("", "Изображение не было загружено - неправильное расширение изображения");
                        return View(Model);
                    }
                }

                // Объявляем переменную имени изображения
                string ImageName = File.FileName;

                // Сохраняем имя изображения в DTO
                using (DataBase DataBase = new DataBase())
                {
                    CompanyDataTransferObject CompanyDataTransferObject = DataBase.Companies.Find(Id);
                    CompanyDataTransferObject.ImageName = ImageName;

                    _ = DataBase.SaveChanges();
                }

                // Назначаем пути к оригинальному и уменьшенному изображению
                string Path = string.Format($"{PathString2}\\{ImageName}");
                string Path2 = string.Format($"{PathString3}\\{ImageName}");

                // Сохраняем оригинальное изображение
                File.SaveAs(Path);

                // Создаём и сохраняем уменьшенную копию
                WebImage Image = new WebImage(File.InputStream);
                _ = Image.Resize(200, 200).Crop(1, 1);
                _ = Image.Save(Path2);
            }

            #endregion Upload Image

            // Переадресовываем пользователя
            return RedirectToAction("Companies");
        }

        //Создаём метод отображения товаров
        // GET: Manager/Companies/Companies
        [HttpGet]
        [Authorize]
        public ActionResult Companies(int? Page)
        {
            // Объявляем CompanyViewModel типа лист
            List<CompanyViewModel> ListOfCompanyViewModel;
            // Устанавливаем номер страницы
            int PageNumber = Page ?? 1;

            using (DataBase DataBase = new DataBase())
            {
                // Инициализируем лист
                ListOfCompanyViewModel = DataBase.Companies.ToArray().Select(x => new CompanyViewModel(x)).ToList();
            }

            // Устанавливаем постраничную навигацию
            IPagedList<CompanyViewModel> OnePageOfCompany = ListOfCompanyViewModel.ToPagedList(PageNumber, 3);
            ViewBag.OnePageOfCompany = OnePageOfCompany;

            // Возвращаем представление и лист
            return View(ListOfCompanyViewModel);
        }

        [HttpGet]
        [Authorize]
        public ActionResult EditCompany(int Id)
        {
            // Объявляем модель CompanyViewModel
            CompanyViewModel Model;

            using (DataBase DataBase = new DataBase())
            {
                // Получаем компанию
                CompanyDataTransferObject CompanyDataTransferObject = DataBase.Companies.Find(Id);

                // Проверяем, доступна ли эта компания
                if (CompanyDataTransferObject == null)
                {
                    return Content("Этой компании не существует.");
                }

                // Инициализируем модель данными
                Model = new CompanyViewModel(CompanyDataTransferObject)
                {
                    GalleryImages = Directory
    .EnumerateFiles(Server.MapPath("~/Images/Uploads/Companies/" + Id + "/Gallery/Thumbs"))
    .Select(fn => Path.GetFileName(fn))
                };
            }

            // Возвращаем модель в представление
            return View(Model);
        }

        // Создаём метод редактирования компании
        // POST: Manager/Companies/EditCompany
        [HttpPost]
        [Authorize]
        public ActionResult EditCompany(CompanyViewModel Model, HttpPostedFileBase File)
        {
            // Получаем ID компании
            int Id = Model.Id;
            // Проверяем модель на валидность
            if (!ModelState.IsValid)
            {
                return View(Model);
            }

            // Проверяем имя компании на уникальность
            using (DataBase DataBase = new DataBase())
            {
                if (DataBase.Companies.Where(x => x.Id != Id).Any(x => x.CompanyName == Model.CompanyName))
                {
                    ModelState.AddModelError("", "Это название компании уже занято!");
                    return View(Model);
                }
            }
            string UserEmail = User.Identity.Name;
            // Обновляем компанию
            using (DataBase DataBase = new DataBase())
            {
                UserDataTransferObject UserDataTransferObject = DataBase.Users.FirstOrDefault(x => x.EmailAddress == UserEmail);
                CompanyDataTransferObject CompanyDataTransferObject = DataBase.Companies.Find(Id);

                CompanyDataTransferObject.Id = Model.Id;
                CompanyDataTransferObject.FullCompanyName = Model.FullCompanyName;
                CompanyDataTransferObject.CompanyName = Model.CompanyName;
                CompanyDataTransferObject.LegalAddress = Model.LegalAddress;
                CompanyDataTransferObject.PostalAddress = Model.PostalAddress;
                CompanyDataTransferObject.INN = Model.INN;
                CompanyDataTransferObject.OGRN = Model.OGRN;
                CompanyDataTransferObject.KPP = Model.KPP;
                CompanyDataTransferObject.EmailAdress = Model.EmailAdress;
                CompanyDataTransferObject.PhoneNumber = Model.PhoneNumber;
                CompanyDataTransferObject.Description = Model.Description;
                CompanyDataTransferObject.SlugTelegram = Model.SlugTelegram;

                CompanyDataTransferObject.SlugVK = Model.SlugVK;
                CompanyDataTransferObject.SlugWhatsapp = Model.SlugWhatsapp;
                CompanyDataTransferObject.Slug = Model.CompanyName.Replace(@"""", "").Replace(",", "").Replace("'", "").Replace("?", "").Replace("!", "").Replace("/", "").Replace(".", "").Replace(" ", "-").ToLower();
                CompanyDataTransferObject.View = UserDataTransferObject.View;

                _ = DataBase.SaveChanges();
            }

            // Устанавливаем сообщение в TempData
            TempData["SM"] = "Вы отредактировали компанию!";

            #region Image Upload

            // Проверяем загрузку файла
            if (File != null && File.ContentLength > 0)
            {
                // Получаем расширение файла
                string Ext = File.ContentType.ToLower();

                // Проверяем расширение
                if (Ext != "image/jpg" &&
                    Ext != "image/jpeg" &&
                    Ext != "image/pjpeg" &&
                    Ext != "image/gif" &&
                    Ext != "image/x-png" &&
                    Ext != "image/png")
                {
                    using (DataBase DataBase = new DataBase())
                    {
                        ModelState.AddModelError("", "Изображение не было загружено - неправильное расширение изображения");
                        return View(Model);
                    }
                }

                // Устанавливаем пути загрузки
                DirectoryInfo OriginalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));

                string PathString1 = System.IO.Path.Combine(OriginalDirectory.ToString(), "Companies\\" + Id.ToString());
                string PathString2 = System.IO.Path.Combine(OriginalDirectory.ToString(), "Companies\\" + Id.ToString() + "\\Thumbs");

                // Удаляем существующие файлы в директориях
                DirectoryInfo Directory1 = new DirectoryInfo(PathString1);
                DirectoryInfo Directory2 = new DirectoryInfo(PathString2);

                foreach (FileInfo File2 in Directory1.GetFiles())
                {
                    File2.Delete();
                }

                foreach (FileInfo File3 in Directory2.GetFiles())
                {
                    File3.Delete();
                }

                // Сохраняем имя изображения
                string ImageName = File.FileName;

                using (DataBase DataBase = new DataBase())
                {
                    CompanyDataTransferObject CompanyDataTransferObject = DataBase.Companies.Find(Id);
                    CompanyDataTransferObject.ImageName = ImageName;

                    _ = DataBase.SaveChanges();
                }

                // Сохраняем оригинал и превью версии изображений
                string Path = string.Format($"{PathString1}\\{ImageName}");
                string Path2 = string.Format($"{PathString2}\\{ImageName}");

                // Сохраняем оригинальное изображение
                File.SaveAs(Path);

                // Создаём и сохраняем уменьшенную копию
                WebImage Image = new WebImage(File.InputStream);
                _ = Image.Resize(200, 200).Crop(1, 1);
                _ = Image.Save(Path2);
            }

            #endregion Image Upload

            // Переадресовываем пользователя
            return RedirectToAction("EditCompany");
        }

        // Создаём метод удаления
        // POST: Admin/Companies/Delete/Id

        [Authorize]
        public void DeleteImage(int Id, string ImageName)
        {
            string FullPath1 = Request.MapPath("~/Images/Uploads/Companies/" + Id.ToString() + "/Gallery/" + ImageName);
            string FullPath2 = Request.MapPath("~/Images/Uploads/Companies/" + Id.ToString() + "/Gallery/Thumbs/" + ImageName);

            if (System.IO.File.Exists(FullPath1))
            {
                System.IO.File.Delete(FullPath1);
            }

            if (System.IO.File.Exists(FullPath2))
            {
                System.IO.File.Delete(FullPath2);
            }
        }

        [Authorize]
        public ActionResult DeleteCompany(int Id)
        {
            // Удаляем компанию из базы данных
            using (DataBase DataBase = new DataBase())
            {
                List<PlaceDataTransferObject> ListPlaceDataTransferObject = DataBase.Places.Where(x => x.CompanyId == Id).ToList();
                foreach (PlaceDataTransferObject Place in ListPlaceDataTransferObject)
                {
                    List<ServiceDataTransferObject> ServiceDataTransferObject = DataBase.Services.Where(x => x.PlaceId == Place.Id).ToList();
                    foreach (ServiceDataTransferObject Service in ServiceDataTransferObject)
                    {
                        _ = DataBase.Services.Remove(Service);
                        // Удаляем директорию места
                        DirectoryInfo OriginalDirectory4 = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));
                        string PathString4 = Path.Combine(OriginalDirectory4.ToString(), "Services\\" + Place.Id.ToString());

                        try
                        {
                            if (Directory.Exists(PathString4))
                            {
                                Directory.Delete(PathString4, true);
                            }
                        }
                        catch
                        {
                            Console.WriteLine("Ошибка при попытки удаления");
                        }


                    }
                    //Удаляем корзину
                    List<DesiredDataTransferObject> DesiredDataTransferObject = DataBase.Desires.Where(x => x.PlaceId == Place.Id).ToList();
                    foreach (DesiredDataTransferObject Desired in DesiredDataTransferObject)
                    {
                        _ = DataBase.Desires.Remove(Desired);
                    }

                    //Удаляем избранное
                    List<FavoriteDataTransferObject> FavoriteDataTransferObject = DataBase.Favorites.Where(x => x.PlaceId == Place.Id).ToList();
                    foreach (FavoriteDataTransferObject Favorite in FavoriteDataTransferObject)
                    {
                        _ = DataBase.Favorites.Remove(Favorite);
                    }

                    //Удаляем комментарии
                    List<CommentDataTransferObject> CommentDataTransferObject = DataBase.Comments.Where(x => x.PlaceId == Place.Id).ToList();
                    foreach (CommentDataTransferObject Comment in CommentDataTransferObject)
                    {
                        int IdComment = Comment.Id;
                        _ = DataBase.Comments.Remove(Comment);

                        //Удаляем ответы
                        List<ReplyDataTransferObject> ReplyDataTransferObject = DataBase.Replies.Where(x => x.CommentId == IdComment).ToList();
                        foreach (ReplyDataTransferObject Reply in ReplyDataTransferObject)
                        {
                            _ = DataBase.Replies.Remove(Reply);
                        }
                    }

                    //Удаляем отзывы
                    List<RatingDataTransferObject> RatingDataTransferObject = DataBase.Ratings.Where(x => x.PlaceId == Place.Id).ToList();
                    foreach (RatingDataTransferObject Rating in RatingDataTransferObject)
                    {
                        _ = DataBase.Ratings.Remove(Rating);

                        // Удаляем директорию товара
                        DirectoryInfo OriginalDirectory1 = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));
                        string PathString1 = Path.Combine(OriginalDirectory1.ToString(), "Feedback\\" + Rating.Id.ToString());

                        try
                        {
                            if (Directory.Exists(PathString1))
                            {
                                Directory.Delete(PathString1, true);
                            }
                        }
                        catch
                        {
                            Console.WriteLine("Ошибка при попытки удаления");
                        }
                    }
                }

                //Удаляем товар и его директорию
                foreach (PlaceDataTransferObject Place in ListPlaceDataTransferObject)
                {
                    _ = DataBase.Places.Remove(Place);

                    // Удаляем директорию места
                    DirectoryInfo OriginalDirectory3 = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));
                    string PathString3 = Path.Combine(OriginalDirectory3.ToString(), "Places\\" + Place.Id.ToString());

                    try
                    {
                        if (Directory.Exists(PathString3))
                        {
                            Directory.Delete(PathString3, true);
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Ошибка при попытки удаления");
                    }
                }

                CompanyDataTransferObject CompanyDataTransferObject = DataBase.Companies.Find(Id);

                _ = DataBase.Companies.Remove(CompanyDataTransferObject);

                _ = DataBase.SaveChanges();
            }
            // Удаляем директорию
            DirectoryInfo OriginalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));
            string PathString = Path.Combine(OriginalDirectory.ToString(), "Companies\\" + Id.ToString());
            try
            {
                if (Directory.Exists(PathString))
                {
                    Directory.Delete(PathString, true);
                }

                // Переадресовываем пользователя
                return RedirectToAction("Companies");
            }
            catch
            {
                // Переадресовываем пользователя
                return RedirectToAction("Companies");
            }
        }
    }
}