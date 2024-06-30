using PagedList;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using WebStoreMap.Models.Data;
using WebStoreMap.Models.ViewModels.Company;

namespace WebStoreMap.Areas.Moderator.Controllers
{
    [Authorize(Roles = "Moderator")]
    public class CompanyController : Controller
    {
        // GET: Moderator/Companies
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        //Создаём метод добавления компании
        // GET:  Moderator/Companies/AddCompany
        [HttpGet]
        [Authorize]
        public ActionResult AddCompany()
        {
            // Объявляем модель
            CompanyViewModel model = new CompanyViewModel();
            // Возвращаем модель в представление
            return View(model);
        }

        //Создаём метод добавления компании
        // POST: Moderator/Companies/AddCompany/model/file
        [HttpPost]
        [Authorize]
        public ActionResult AddCompany(CompanyViewModel model, HttpPostedFileBase file)
        {
            // Проверяем модель на валидность
            if (!ModelState.IsValid)
            {
                using (DataBase db = new DataBase())
                {
                    return View(model);
                }
            }

            // Проверяем имя продукта на уникальность
            using (DataBase db = new DataBase())
            {
                if (db.Companies.AsParallel().Any(x => x.CompanyName == model.CompanyName))
                {
                    ModelState.AddModelError("", "Это название компании уже занято!");
                    return View(model);
                }
            }

            int id;
            string userEmail = User.Identity.Name;


            // Инициализируем и сохраняем в базу модель companyDTO
            using (DataBase db = new DataBase())
            {
                // Получаем пользователя
                UserDataTransferObject userdto = db.Users.FirstOrDefault(x => x.EmailAddress == userEmail);
                CompanyDataTransferObject company = new CompanyDataTransferObject
                {
                    Id = model.Id,
                    FullCompanyName = model.FullCompanyName,
                    CompanyName = model.CompanyName,
                    LegalAddress = model.LegalAddress,
                    PostalAddress = model.PostalAddress,
                    INN = model.INN,
                    OGRN = model.OGRN,
                    KPP = model.KPP,
                    EmailAdress = model.EmailAdress,
                    PhoneNumber = model.PhoneNumber,
                    SlugTelegram = model.SlugTelegram,
                 
                    SlugVK = model.SlugVK,
                    SlugWhatsapp = model.SlugWhatsapp,
                    Description = model.Description,
                    Slug = model.CompanyName.Replace(@"""", "").Replace(",", "").Replace("'", "").Replace("?", "").Replace("!", "").Replace("/", "").Replace(".", "").Replace(" ", "-").ToLower(),
                    View = userdto.View,
                    UserId = userdto.Id
                };

                _ = db.Companies.Add(company);
                _ = db.SaveChanges();

                // Получаем введённый ID
                id = company.Id;
            }

            // Добавляем сообщение в TempData
            TempData["SM"] = "Вы добавили компанию!";

            #region Upload Image

            // Создаём необходимые директории
            DirectoryInfo originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));

            string pathString1 = Path.Combine(originalDirectory.ToString(), "Companies");
            string pathString2 = Path.Combine(originalDirectory.ToString(), "Companies\\" + id.ToString());
            string pathString3 = Path.Combine(originalDirectory.ToString(), "Companies\\" + id.ToString() + "\\Thumbs");
            string pathString4 = Path.Combine(originalDirectory.ToString(), "Companies\\" + id.ToString() + "\\Gallery");
            string pathString5 = Path.Combine(originalDirectory.ToString(), "Companies\\" + id.ToString() + "\\Gallery\\Thumbs");

            // Проверяем, есть ли дериктория по пути
            if (!Directory.Exists(pathString1))
            {
                _ = Directory.CreateDirectory(pathString1);
            }

            if (!Directory.Exists(pathString2))
            {
                _ = Directory.CreateDirectory(pathString2);
            }

            if (!Directory.Exists(pathString3))
            {
                _ = Directory.CreateDirectory(pathString3);
            }

            if (!Directory.Exists(pathString4))
            {
                _ = Directory.CreateDirectory(pathString4);
            }

            if (!Directory.Exists(pathString5))
            {
                _ = Directory.CreateDirectory(pathString5);
            }

            // Проверяем, был ли файл загружен
            if (file != null && file.ContentLength > 0)
            {
                // Получаем расширение файла
                string ext = file.ContentType.ToLower();

                // Проверяем расширение
                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png")
                {
                    using (DataBase db = new DataBase())
                    {
                        ModelState.AddModelError("", "Изображение не было загружено - неправильное расширение изображения");
                        return View(model);
                    }
                }

                // Объявляем переменную имени изображения
                string imageName = file.FileName;

                // Сохраняем имя изображения в DTO
                using (DataBase db = new DataBase())
                {
                    CompanyDataTransferObject dto = db.Companies.Find(id);
                    dto.ImageName = imageName;

                    _ = db.SaveChanges();
                }

                // Назначаем пути к оригинальному и уменьшенному изображению
                string path = string.Format($"{pathString2}\\{imageName}");
                string path2 = string.Format($"{pathString3}\\{imageName}");

                // Сохраняем оригинальное изображение
                file.SaveAs(path);

                // Создаём и сохраняем уменьшенную копию
                WebImage img = new WebImage(file.InputStream);
                _ = img.Resize(200, 200).Crop(1, 1);
                _ = img.Save(path2);
            }

            #endregion Upload Image

            // Переадресовываем пользователя
            return RedirectToAction("Companies");
        }

        //Создаём метод отображения товаров
        // GET: Moderator/Companies/Companies
        [HttpGet]
        [Authorize]
        public ActionResult Companys(int? page)
        {

            List<CompanyViewModel> listOfCompanyVM;
            // Устанавливаем номер страницы
            int pageNumber = page ?? 1;

            using (DataBase db = new DataBase())
            {
                // Инициализируем лист
                listOfCompanyVM = db.Companies.ToArray().Select(x => new CompanyViewModel(x)).ToList();
            }

            // Устанавливаем постраничную навигацию
            IPagedList<CompanyViewModel> onePageOfCompany = listOfCompanyVM.ToPagedList(pageNumber, 3);
            ViewBag.OnePageOfCompany = onePageOfCompany;

            // Возвращаем представление и лист
            return View(listOfCompanyVM);
        }

        [HttpGet]
        [Authorize]
        public ActionResult EditCompany(int id)
        {

            CompanyViewModel model;

            using (DataBase db = new DataBase())
            {
                // Получаем компанию
                CompanyDataTransferObject dto = db.Companies.Find(id);

                // Проверяем, доступна ли компания
                if (dto == null)
                {
                    return Content("Этой компании не существует.");
                }

                // Инициализируем модель данными
                model = new CompanyViewModel(dto)
                {
                    GalleryImages = Directory
    .EnumerateFiles(Server.MapPath("~/Images/Uploads/Companies/" + id + "/Gallery/Thumbs"))
    .Select(fn => Path.GetFileName(fn))
                };
            }

            // Возвращаем модель в представление
            return View(model);
        }

        // Создаём метод редактирования товаров
        // POST: Moderator/Companies/EditCompany
        [HttpPost]
        [Authorize]
        public ActionResult EditCompany(CompanyViewModel model, HttpPostedFileBase file)
        {
            // Получаем ID 
            int id = model.Id;
            // Проверяем модель на валидность
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Проверяем имя компании на уникальность
            using (DataBase db = new DataBase())
            {
                if (db.Companies.Where(x => x.Id != id).Any(x => x.CompanyName == model.CompanyName))
                {
                    ModelState.AddModelError("", "Это название компании уже занято!");
                    return View(model);
                }
            }
            string userEmail = User.Identity.Name;
            // Обновляем компанию
            using (DataBase db = new DataBase())
            {
                UserDataTransferObject userdto = db.Users.FirstOrDefault(x => x.EmailAddress == userEmail);
                CompanyDataTransferObject dto = db.Companies.Find(id);

                dto.Id = model.Id;
                dto.FullCompanyName = model.FullCompanyName;
                dto.CompanyName = model.CompanyName;
                dto.LegalAddress = model.LegalAddress;
                dto.PostalAddress = model.PostalAddress;
                dto.INN = model.INN;
                dto.OGRN = model.OGRN;
                dto.KPP = model.KPP;
                dto.EmailAdress = model.EmailAdress;
                dto.PhoneNumber = model.PhoneNumber;
                dto.SlugTelegram = model.SlugTelegram;
               
                dto.SlugVK = model.SlugVK;
                dto.SlugWhatsapp = model.SlugWhatsapp;
                dto.Description = model.Description;
                dto.Slug = model.CompanyName.Replace(@"""", "").Replace(",", "").Replace("'", "").Replace("?", "").Replace("!", "").Replace("/", "").Replace(".", "").Replace(" ", "-").ToLower();
                dto.View = userdto.View;

                _ = db.SaveChanges();
            }

            // Устанавливаем сообщение в TempData
            TempData["SM"] = "Вы отредактировали компанию!";

            // Загрузка изображений в следующим видео!

            #region Image Upload

            // Проверяем загрузку файла
            if (file != null && file.ContentLength > 0)
            {
                // Получаем расширение файла
                string ext = file.ContentType.ToLower();

                // Проверяем расширение
                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png")
                {
                    using (DataBase db = new DataBase())
                    {
                        ModelState.AddModelError("", "Изображение не было загружено - неправильное расширение изображения");
                        return View(model);
                    }
                }

                // Устанавливаем пути загрузки
                DirectoryInfo originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));

                string pathString1 = Path.Combine(originalDirectory.ToString(), "Companies\\" + id.ToString());
                string pathString2 = Path.Combine(originalDirectory.ToString(), "Companies\\" + id.ToString() + "\\Thumbs");

                // Удаляем существующие файлы в директориях
                DirectoryInfo di1 = new DirectoryInfo(pathString1);
                DirectoryInfo di2 = new DirectoryInfo(pathString2);

                foreach (FileInfo file2 in di1.GetFiles())
                {
                    file2.Delete();
                }

                foreach (FileInfo file3 in di2.GetFiles())
                {
                    file3.Delete();
                }

                // Сохраняем имя изображения
                string imageName = file.FileName;

                using (DataBase db = new DataBase())
                {
                    CompanyDataTransferObject dto = db.Companies.Find(id);
                    dto.ImageName = imageName;

                    _ = db.SaveChanges();
                }

                // Сохраняем оригинал и превью версии изображений
                string path = string.Format($"{pathString1}\\{imageName}");
                string path2 = string.Format($"{pathString2}\\{imageName}");

                // Сохраняем оригинальное изображение
                file.SaveAs(path);

                // Создаём и сохраняем уменьшенную копию
                WebImage img = new WebImage(file.InputStream);
                _ = img.Resize(200, 200).Crop(1, 1);
                _ = img.Save(path2);
            }

            #endregion Image Upload

            // Переадресовываем пользователя
            return RedirectToAction("EditCompany");
        }

        // Создаём метод удаления изображения
        // POST: Moderator/Companies/DeleteImageCountry/Id

        [Authorize]
        public void DeleteImage(int id, string imageName)
        {
            string fullPath1 = Request.MapPath("~/Images/Uploads/Companies/" + id.ToString() + "/Gallery/" + imageName);
            string fullPath2 = Request.MapPath("~/Images/Uploads/Companies/" + id.ToString() + "/Gallery/Thumbs/" + imageName);

            if (System.IO.File.Exists(fullPath1))
            {
                System.IO.File.Delete(fullPath1);
            }

            if (System.IO.File.Exists(fullPath2))
            {
                System.IO.File.Delete(fullPath2);
            }
        }

        [Authorize]
        public ActionResult DeleteCompany(int id)
        {
            // Удаляем компанию и все связанные с ней данные из БД
            using (DataBase db = new DataBase())
            {
                List<PlaceDataTransferObject> proddto = db.Places.Where(x => x.CompanyId == id).ToList();
                foreach (PlaceDataTransferObject i in proddto)
                {
                    //Удаляем корзину
                    List<DesiredDataTransferObject> desdto = db.Desires.Where(x => x.PlaceId == i.Id).ToList();
                    foreach (DesiredDataTransferObject j in desdto)
                    {
                        _ = db.Desires.Remove(j);
                    }

                    //Удаляем избранное
                    List<FavoriteDataTransferObject> favdto = db.Favorites.Where(x => x.PlaceId == i.Id).ToList();
                    foreach (FavoriteDataTransferObject j in favdto)
                    {
                        _ = db.Favorites.Remove(j);
                    }

                    //Удаляем комментарии
                    List<CommentDataTransferObject> com = db.Comments.Where(x => x.PlaceId == i.Id).ToList();
                    foreach (CommentDataTransferObject h in com)
                    {
                        int idcom = h.Id;
                        _ = db.Comments.Remove(h);

                        //Удаляем ответы
                        List<ReplyDataTransferObject> repl = db.Replies.Where(x => x.CommentId == idcom).ToList();
                        foreach (ReplyDataTransferObject r in repl)
                        {
                            _ = db.Replies.Remove(r);
                        }
                    }

                    //Удаляем отзывы
                    List<RatingDataTransferObject> rating = db.Ratings.Where(x => x.PlaceId == i.Id).ToList();
                    foreach (RatingDataTransferObject k in rating)
                    {
                        _ = db.Ratings.Remove(k);

                        // Удаляем директорию
                        DirectoryInfo originalDirectory1 = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));
                        string pathString1 = Path.Combine(originalDirectory1.ToString(), "Feedback\\" + k.Id.ToString());

                        try
                        {
                            if (Directory.Exists(pathString1))
                            {
                                Directory.Delete(pathString1, true);
                            }
                        }
                        catch
                        {

                        }
                    }
                }

                //Удаляем товар и его директорию
                foreach (PlaceDataTransferObject k in proddto)
                {
                    _ = db.Places.Remove(k);

                    // Удаляем директорию товара
                    DirectoryInfo originalDirectory3 = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));
                    string pathString3 = Path.Combine(originalDirectory3.ToString(), "Places\\" + k.Id.ToString());

                    try
                    {
                        if (Directory.Exists(pathString3))
                        {
                            Directory.Delete(pathString3, true);
                        }
                    }
                    catch
                    {

                    }
                }

                CompanyDataTransferObject dto = db.Companies.Find(id);
                _ = db.Companies.Remove(dto);

                _ = db.SaveChanges();
            }
            // Удаляем директорию компании
            DirectoryInfo originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));
            string pathString = Path.Combine(originalDirectory.ToString(), "Companies\\" + id.ToString());
            try
            {
                if (Directory.Exists(pathString))
                {
                    Directory.Delete(pathString, true);
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