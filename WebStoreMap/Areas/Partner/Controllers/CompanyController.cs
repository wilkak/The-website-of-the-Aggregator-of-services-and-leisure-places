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

namespace WebStoreMap.Areas.Partner.Controllers
{
    [Authorize(Roles = "Partner")]
    public class CompanyController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        //Создаём метод добавления компании
        // GET:  Partner/Companies/AddCompany
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
        // POST: Partner/Companies/AddCompany/Model/File
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

            // Объявляем переменную ID
            int Id;
            string UserEmail = User.Identity.Name;

            // Инициализируем и сохраняем в базу модель DTO
            using (DataBase DataBase = new DataBase())
            {
                // Получаем пользователя
                UserDataTransferObject UserDataTransferObject = DataBase.Users.FirstOrDefault(x => x.EmailAddress == UserEmail);
                CompanyDataTransferObject CompanyDataTransferObject = new CompanyDataTransferObject
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
                    SlugTelegram = Model.SlugTelegram,
                 
                    SlugVK = Model.SlugVK,
                    SlugWhatsapp = Model.SlugWhatsapp,
                    Description = Model.Description,
                    Slug = Model.CompanyName.Replace(@"""", "").Replace(",", "").Replace("'", "").Replace("?", "").Replace("!", "").Replace("/", "").Replace(".", "").Replace(" ", "-").ToLower(),
                    View = UserDataTransferObject.View,
                    UserId = UserDataTransferObject.Id
                };

                _ = DataBase.Companies.Add(CompanyDataTransferObject);
                _ = DataBase.SaveChanges();

                // Получаем введённый ID
                Id = CompanyDataTransferObject.Id;
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

        //Создаём метод отображения компании
        // GET: Partner/Companies/Companies
        [HttpGet]
        [Authorize]
        public ActionResult Companies(int? Page)
        {

            List<CompanyViewModel> ListCompanyViewModel;
            // Устанавливаем номер страницы
            int PageNumber = Page ?? 1;
            string UserEmail = User.Identity.Name;

            using (DataBase DataBase = new DataBase())
            {
                UserDataTransferObject UserDataTransferObject = DataBase.Users.FirstOrDefault(x => x.EmailAddress == UserEmail);
                // Инициализируем лист
                ListCompanyViewModel = DataBase.Companies.ToArray().Where(x => x.UserId == UserDataTransferObject.Id).Select(x => new CompanyViewModel(x)).ToList();
            }

            // Устанавливаем постраничную навигацию
            IPagedList<CompanyViewModel> OnePageOfCompanyViewModel = ListCompanyViewModel.ToPagedList(PageNumber, 3);
            ViewBag.OnePageOfCompany = OnePageOfCompanyViewModel;

            // Возвращаем представление и лист
            return View(ListCompanyViewModel);
        }

        [HttpGet]
        [Authorize]
        public ActionResult EditCompany(int Id)
        {
            // Объявляем модель VM
            CompanyViewModel Model;

            using (DataBase DataBase = new DataBase())
            {
                // Получаем компанию
                CompanyDataTransferObject CompanyDataTransferObject = DataBase.Companies.Find(Id);

                // Проверяем, доступна ли компания 
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
        // POST: DeleteCompany/Companies/EditCompany
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
                CompanyDataTransferObject.SlugTelegram = Model.SlugTelegram;
               
                CompanyDataTransferObject.SlugVK = Model.SlugVK;
                CompanyDataTransferObject.SlugWhatsapp = Model.SlugWhatsapp;
                CompanyDataTransferObject.Description = Model.Description;
                CompanyDataTransferObject.Slug = Model.CompanyName.Replace(@"""", "").Replace(",", "").Replace("'", "").Replace("?", "").Replace("!", "").Replace("/", "").Replace(".", "").Replace(" ", "-").ToLower();
                CompanyDataTransferObject.View = UserDataTransferObject.View;
                CompanyDataTransferObject.UserId = UserDataTransferObject.Id;

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
                DirectoryInfo originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));

                string PathString1 = System.IO.Path.Combine(originalDirectory.ToString(), "Companies\\" + Id.ToString());
                string PathString2 = System.IO.Path.Combine(originalDirectory.ToString(), "Companies\\" + Id.ToString() + "\\Thumbs");

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

        [HttpPost]
        [Authorize]
        public void SaveGalleryImages(int Id)
        {
            // Перебираем все файлы
            foreach (string FileName in Request.Files)
            {
                // Инициализируем файлы
                HttpPostedFileBase File = Request.Files[FileName];

                // Проверяем на NULL
                if (File != null && File.ContentLength > 0)
                {
                    // Назначаем пути к директориям
                    DirectoryInfo OriginalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));

                    string PathString1 = System.IO.Path.Combine(OriginalDirectory.ToString(), "Companies\\" + Id.ToString() + "\\Gallery");
                    string PathString2 = System.IO.Path.Combine(OriginalDirectory.ToString(), "Companies\\" + Id.ToString() + "\\Gallery\\Thumbs");

                    // Назначаем пути изображений
                    string Path = string.Format($"{PathString1}\\{File.FileName}");
                    string Path2 = string.Format($"{PathString2}\\{File.FileName}");

                    // Сохраняем оригинальные изображения и уменьшеные копии
                    File.SaveAs(Path);

                    WebImage Image = new WebImage(File.InputStream);
                    _ = Image.Resize(200, 200).Crop(1, 1);
                    _ = Image.Save(Path2);
                }
            }
        }

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

        // Создаём метод удаления компании
        // POST: Partner/DeleteCompany/Delete/Id
        [Authorize]
        public ActionResult DeleteCompany(int Id)
        {
            // Удаляем компании из базы данных
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
                    List<DesiredDataTransferObject> ListDesiredDataTransferObject = DataBase.Desires.Where(x => x.PlaceId == Place.Id).ToList();
                    foreach (DesiredDataTransferObject Desired in ListDesiredDataTransferObject)
                    {
                        _ = DataBase.Desires.Remove(Desired);
                    }

                    //Удаляем избранное
                    List<FavoriteDataTransferObject> ListFavoriteDataTransferObject = DataBase.Favorites.Where(x => x.PlaceId == Place.Id).ToList();
                    foreach (FavoriteDataTransferObject Favorite in ListFavoriteDataTransferObject)
                    {
                        _ = DataBase.Favorites.Remove(Favorite);
                    }

                    //Удаляем комментарии
                    List<CommentDataTransferObject> ListCommentDataTransferObject = DataBase.Comments.Where(x => x.PlaceId == Place.Id).ToList();
                    foreach (CommentDataTransferObject Comment in ListCommentDataTransferObject)
                    {
                        int CommentId = Comment.Id;
                        _ = DataBase.Comments.Remove(Comment);

                        //Удаляем ответы
                        List<ReplyDataTransferObject> ListReplyDataTransferObject = DataBase.Replies.Where(x => x.CommentId == CommentId).ToList();
                        foreach (ReplyDataTransferObject Reply in ListReplyDataTransferObject)
                        {
                            _ = DataBase.Replies.Remove(Reply);
                        }
                    }

                    //Удаляем отзывы
                    List<RatingDataTransferObject> ListRatingDataTransferObject = DataBase.Ratings.Where(x => x.PlaceId == Place.Id).ToList();
                    foreach (RatingDataTransferObject Rating in ListRatingDataTransferObject)
                    {
                        _ = DataBase.Ratings.Remove(Rating);

                        // Удаляем директорию места
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
                        }
                    }
                }

                //Удаляем место и его директорию
                foreach (PlaceDataTransferObject Place in ListPlaceDataTransferObject)
                {
                    _ = DataBase.Places.Remove(Place);

                    // Удаляем директорию место
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
                    }
                }

                CompanyDataTransferObject CompanyDataTransferObject = DataBase.Companies.Find(Id);
                _ = DataBase.Companies.Remove(CompanyDataTransferObject);

                _ = DataBase.SaveChanges();
            }
            // Удаляем директорию компании
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