using Newtonsoft.Json;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using WebStoreMap.Models.Data;
using WebStoreMap.Models.ViewModels.Place;

namespace WebStoreMap.Areas.Manager.Controllers
{
    [Authorize(Roles = "Manager")]
    public class PlaceController : Controller
    {
        // GET: Manager/Place
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult DeleteImagesError()
        {
            return View();
        }
  
        [HttpGet]
        public ActionResult AddPlace()
        {
            string UserEmail = User.Identity.Name;

            // Объявляем модель
            PlaceViewModel Model = new PlaceViewModel();

            // Добавляем список выбора категорий в модель
            using (DataBase DataBase = new DataBase())
            {
                UserDataTransferObject UserDataTransferObject = DataBase.Users.FirstOrDefault(x => x.EmailAddress == UserEmail);

                Model.Categories = new SelectList(DataBase.Categories.ToList(), "Id", "CategoryName");

                Model.Country = new SelectList(DataBase.Countries.ToList(), "Id", "CountryName");

                if (Model.Country != null)
                {
                    Model.Region = new SelectList(DataBase.Regions.Where(x => x.CountryId == Model.CountryId).ToList(), "Id", "RegionName");
                    if (Model.Region != null)
                    {
                        Model.City = new SelectList(DataBase.Cities.Where(x => x.RegionId == Model.RegionId).ToList(), "Id", "CityName");
                    }
                }

                Model.Company = new SelectList(DataBase.Companies.ToList(), "Id", "CompanyName");

                CompanyDataTransferObject CompanyDataTransferObject = DataBase.Companies.FirstOrDefault(x => x.UserId == UserDataTransferObject.Id);

                ViewBag.IsCompanyExist = CompanyDataTransferObject != null;
            }

            // Возвращаем модель в представление
            return View(Model);
        }

        //Создаём метод добавления мест
        // POST:  Manager/Place/AddPlace/Model/File
        [HttpPost]
        public ActionResult AddPlace(PlaceViewModel Model, HttpPostedFileBase File)
        {
            int Id;
            string UserEmail = User.Identity.Name;

            // Проверяем модель на валидность
            if (!ModelState.IsValid)
            {
                using (DataBase DataBase = new DataBase())
                {
                    Model.Country = new SelectList(DataBase.Countries.ToList(), "Id", "CountryName");

                    Model.Company = new SelectList(DataBase.Companies.ToList(), "Id", "CompanyName");

                    if (Model.Country != null)
                    {
                        Model.Region = new SelectList(DataBase.Regions.Where(x => x.CountryId == Model.CountryId).ToList(), "Id", "RegionName");
                        if (Model.Region != null)
                        {
                            Model.City = new SelectList(DataBase.Cities.Where(x => x.RegionId == Model.RegionId).ToList(), "Id", "CityName");
                        }
                    }

                    Model.Categories = new SelectList(DataBase.Categories.ToList(), "Id", "CategoryName");

                    return View(Model);
                }
            }

            // Проверяем имя продукта на уникальность
            using (DataBase DataBase = new DataBase())
            {
                if (DataBase.Places.Any(x => x.PlaceName == Model.PlaceName))
                {
                    Model.Categories = new SelectList(DataBase.Categories.ToList(), "Id", "CategoryName");
                    Model.Country = new SelectList(DataBase.Countries.ToList(), "Id", "CountryName");
                    Model.Company = new SelectList(DataBase.Companies.ToList(), "Id", "CompanyName");

                    if (Model.Country != null)
                    {
                        Model.Region = new SelectList(DataBase.Regions.Where(x => x.CountryId == Model.CountryId).ToList(), "Id", "RegionName");
                        if (Model.Region != null)
                        {
                            Model.City = new SelectList(DataBase.Cities.Where(x => x.RegionId == Model.RegionId).ToList(), "Id", "CityName");
                        }
                    }

                    ModelState.AddModelError("", "Это название уже занято!");
                    return View(Model);
                }
            }

            // сохраняем в базу модель
            using (DataBase DataBase = new DataBase())
            {
                UserDataTransferObject UserDataTransferObject = DataBase.Users.FirstOrDefault(x => x.EmailAddress == UserEmail);

                PlaceDataTransferObject PlaceDataTransferObject = new PlaceDataTransferObject
                {
                    PlaceName = Model.PlaceName,
                    Slug = Model.PlaceName.Replace(@"""", "").Replace(",", "").Replace("'", "").Replace("?", "").Replace("!", "").Replace("/", "").Replace(".", "").Replace(" ", "-").ToLower(),
                    Description = Model.Description,
                    Price = Model.Price,
                    CategoryId = Model.CategoryId,
                    CityId = Model.CityId,
                    CityName = Model.CityName,
                    RegionId = Model.RegionId,
                    CountryId = Model.CountryId,
                    Latitude = Model.Latitude,
                    Longitude = Model.Longitude,
                    OldPrice = Model.OldPrice,
                    Address = Model.Address,
                    View = Model.View,
                    PhoneNumber = Model.PhoneNumber,
                    UserId = UserDataTransferObject.Id,

                    CompanyId = Model.CompanyId
                };
                CategoryDataTransferObject CategoryDataTransferObject = DataBase.Categories.FirstOrDefault(x => x.Id == Model.CategoryId);
                PlaceDataTransferObject.CategoryName = CategoryDataTransferObject.CategoryName;
                CountryDataTransferObject CountryDataTransferObject = DataBase.Countries.FirstOrDefault(x => x.Id == Model.CountryId);
                PlaceDataTransferObject.CountryName = CountryDataTransferObject.CountryName;
                RegionDataTransferObject RegionDataTransferObject = DataBase.Regions.FirstOrDefault(x => x.Id == Model.RegionId);

                PlaceDataTransferObject.RegionName = RegionDataTransferObject.RegionName;
                CityDataTransferObject CityDataTransferObject = DataBase.Cities.FirstOrDefault(x => x.Id == Model.CityId);

                PlaceDataTransferObject.CityName = CityDataTransferObject.CityName;
                PlaceDataTransferObject.FirstDay = Model.FirstDay;
                PlaceDataTransferObject.LastDay = Model.LastDay;

                PlaceDataTransferObject.Schedule = Model.Schedule;
                _ = DataBase.Places.Add(PlaceDataTransferObject);
                _ = DataBase.SaveChanges();

                // Получаем введённый ID
                Id = PlaceDataTransferObject.Id;
            }

            // Добавляем сообщение в TempData
            TempData["SM"] = "Вы добавили место!";

            #region Upload Image

            // Создаём необходимые директории
            DirectoryInfo OriginalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));

            string PathString1 = Path.Combine(OriginalDirectory.ToString(), "Places");
            string PathString2 = Path.Combine(OriginalDirectory.ToString(), "Places\\" + Id.ToString());
            string PathString3 = Path.Combine(OriginalDirectory.ToString(), "Places\\" + Id.ToString() + "\\Thumbs");
            string PathString4 = Path.Combine(OriginalDirectory.ToString(), "Places\\" + Id.ToString() + "\\Gallery");
            string PathString5 = Path.Combine(OriginalDirectory.ToString(), "Places\\" + Id.ToString() + "\\Gallery\\Thumbs");

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
                        Model.Country = new SelectList(DataBase.Countries.ToList(), "Id", "CountryName");
                        if (Model.Country != null)
                        {
                            Model.Region = new SelectList(DataBase.Regions.Where(x => x.CountryId == Model.CountryId).ToList(), "Id", "Services");
                            if (Model.Region != null)
                            {
                                Model.City = new SelectList(DataBase.Cities.Where(x => x.RegionId == Model.RegionId).ToList(), "Id", "CityName");
                            }
                        }
                        Model.Company = new SelectList(DataBase.Companies.ToList(), "Id", "ServiceName");

                        Model.Categories = new SelectList(DataBase.Categories.ToList(), "Id", "ServiceName");
                        ModelState.AddModelError("", "Изображение не было загружено - неправильное расширение изображения");
                        return View(Model);
                    }
                }

                // Объявляем переменную имени изображения
                string ImageName = File.FileName;

                // Сохраняем имя изображения в DTO
                using (DataBase DataBase = new DataBase())
                {
                    PlaceDataTransferObject PlaceDataTransferObject = DataBase.Places.Find(Id);
                    PlaceDataTransferObject.ImageName = ImageName;

                    _ = DataBase.SaveChanges();
                }

                // Назначаем пути к оригинальному и уменьшенному изображению
                string Path = string.Format($"{PathString2}\\{ImageName}");
                string Path2 = string.Format($"{PathString3}\\{ImageName}");

                // Сохраняем оригинальное изображение
                File.SaveAs(Path);

                // Создаём и сохраняем уменьшенную копию
                WebImage Image = new WebImage(File.InputStream);
                _ = Image.Resize(400, 250);
                _ = Image.Save(Path2);
            }

            #endregion Upload Image

            // Переадресовываем пользователя
            return RedirectToAction("AddPlace");
        }

        //Создаём метод отображения мест
        // GET:  Manager/Place/Places
        [HttpGet]
        public ActionResult Places(int? Page, int? CategoryId)
        {
            // Объявляем PlaceViewModel типа лист
            List<PlaceViewModel> ListOfPlaceViewModel;

            // Объявляем PlaceViewModel типа лист
            List<PlaceViewModel> ListOfPlaceViewModel2 = new List<PlaceViewModel>();

            // Устанавливаем номер страницы
            int PageNumber = Page ?? 1;

            using (DataBase DataBase = new DataBase())
            {
                // Инициализируем лист
                ListOfPlaceViewModel = DataBase.Places.ToArray()
                           .Where(x => CategoryId == null || CategoryId == 0 || x.CategoryId == CategoryId)
                           .Select(x => new PlaceViewModel(x))
                           .ToList();

                // Заполняем лист категорий
                ViewBag.Categories = new SelectList(DataBase.Categories.ToList(), "Id", "CategoryName");

                // Устанавливаем выбранную категорию
                ViewBag.SelectedCat = CategoryId.ToString();
            }

            // Устанавливаем постраничную навигацию
            IPagedList<PlaceViewModel> onePageOfPlaces = ListOfPlaceViewModel.ToPagedList(PageNumber, 3);
            ViewBag.OnePageOfPlaces = onePageOfPlaces;

            // Возвращаем представление и лист
            return View(ListOfPlaceViewModel);
        }

        //Создаём метод редактирования 
        // GET:  Manager/Place/EditPlace/Id
        [HttpGet]
        public ActionResult EditPlace(int Id)
        {
            // Объявляем модель PlaceViewModel
            PlaceViewModel Model;

            using (DataBase DataBase = new DataBase())
            {
                // Получаем
                PlaceDataTransferObject PlaceDataTransferObject = DataBase.Places.Find(Id);

                // Проверяем, доступно ли место
                if (PlaceDataTransferObject == null)
                {
                    return Content("Этого места не существует.");
                }

                // Инициализируем модель данными
                Model = new PlaceViewModel(PlaceDataTransferObject)
                {
                    // Создаём список категорий
                    Categories = new SelectList(DataBase.Categories.ToList(), "Id", "CategoryName"),

                    Country = new SelectList(DataBase.Countries.ToList(), "Id", "CountryName")
                };

                if (Model.Country != null)
                {
                    Model.Region = new SelectList(DataBase.Regions.Where(x => x.CountryId == Model.CountryId).ToList(), "Id", "RegionName");
                    if (Model.Region != null)
                    {
                        Model.City = new SelectList(DataBase.Cities.Where(x => x.RegionId == Model.RegionId).ToList(), "Id", "CityName");
                    }
                }
                Model.Company = new SelectList(DataBase.Companies.ToList(), "Id", "CompanyName");

                // Получаем все изображения из галереи
                Model.GalleryImages = Directory
                    .EnumerateFiles(Server.MapPath("~/Images/Uploads/Places/" + Id + "/Gallery/Thumbs"))
                    .Select(fn => Path.GetFileName(fn));
            }

            // Возвращаем модель в представление
            return View(Model);
        }

        // Создаём метод редактирования 
        // POST:  Manager/Place/EditPlace
        [HttpPost]
        public ActionResult EditPlace(PlaceViewModel Model, HttpPostedFileBase File)
        {
            // Получаем ID 
            int Id = Model.Id;

            // Заполняем список категориями и изображениями
            using (DataBase DataBase = new DataBase())
            {
                Model.Categories = new SelectList(DataBase.Categories.ToList(), "Id", "CategoryName");

                Model.Country = new SelectList(DataBase.Countries.ToList(), "Id", "CountryName");

                if (Model.Country != null)
                {
                    Model.Region = new SelectList(DataBase.Regions.Where(x => x.CountryId == Model.CountryId).ToList(), "Id", "RegionName");
                    if (Model.Region != null)
                    {
                        Model.City = new SelectList(DataBase.Cities.Where(x => x.RegionId == Model.RegionId).ToList(), "Id", "CityName");
                    }
                }
                Model.Company = new SelectList(DataBase.Companies.ToList(), "Id", "CompanyName");
            }

            Model.GalleryImages = Directory
                .EnumerateFiles(Server.MapPath("~/Images/Uploads/Places/" + Id + "/Gallery/Thumbs"))
                .Select(fn => Path.GetFileName(fn));

            // Проверяем модель на валидность
            if (!ModelState.IsValid)
            {
                return View(Model);
            }

            // Проверяем имя места на уникальность
            using (DataBase DataBase = new DataBase())
            {
                if (DataBase.Places.Where(x => x.Id != Id).Any(x => x.PlaceName == Model.PlaceName))
                {
                    ModelState.AddModelError("", "Это название уже занято!");
                    return View(Model);
                }
            }
            // Обновляем место
            using (DataBase DataBase = new DataBase())
            {
                PlaceDataTransferObject PlaceDataTransferObject = DataBase.Places.Find(Id);

                PlaceDataTransferObject.PlaceName = Model.PlaceName;
                PlaceDataTransferObject.Slug = Model.PlaceName.Replace(@"""", "").Replace(",", "").Replace("'", "").Replace("?", "").Replace("!", "").Replace("/", "").Replace(".", "").Replace(" ", "-").ToLower();
                PlaceDataTransferObject.Description = Model.Description;
                PlaceDataTransferObject.Price = Model.Price;
                PlaceDataTransferObject.CategoryId = Model.CategoryId;
                PlaceDataTransferObject.CityId = Model.CityId;
                PlaceDataTransferObject.CityName = Model.CityName;
                PlaceDataTransferObject.RegionId = Model.RegionId;
                PlaceDataTransferObject.CountryId = Model.CountryId;

                if (Model.ImageName != null)
                {
                    PlaceDataTransferObject.ImageName = Model.ImageName;
                }
                PlaceDataTransferObject.Latitude = Model.Latitude;
                PlaceDataTransferObject.Longitude = Model.Longitude;
                PlaceDataTransferObject.OldPrice = Model.OldPrice;
                PlaceDataTransferObject.PhoneNumber = Model.PhoneNumber;
                PlaceDataTransferObject.Address = Model.Address;
                PlaceDataTransferObject.View = Model.View;

                PlaceDataTransferObject.CompanyId = Model.CompanyId;
                CategoryDataTransferObject CategoryDataTransferObject = DataBase.Categories.FirstOrDefault(x => x.Id == Model.CategoryId);
                PlaceDataTransferObject.CategoryName = CategoryDataTransferObject.CategoryName;
                RegionDataTransferObject RegionDataTransferObject = DataBase.Regions.FirstOrDefault(x => x.Id == Model.RegionId);
                PlaceDataTransferObject.RegionName = RegionDataTransferObject.RegionName;

                PlaceDataTransferObject.FirstDay = Model.FirstDay;
                PlaceDataTransferObject.LastDay = Model.LastDay;
                CountryDataTransferObject CountryDataTransferObject = DataBase.Countries.FirstOrDefault(x => x.Id == Model.CountryId);
                PlaceDataTransferObject.CountryName = CountryDataTransferObject.CountryName;
                CityDataTransferObject CityDataTransferObject = DataBase.Cities.FirstOrDefault(x => x.Id == Model.CityId);

                PlaceDataTransferObject.CityName = CityDataTransferObject.CityName;

                _ = DataBase.SaveChanges();
            }

            // Устанавливаем сообщение в TempData
            TempData["SM"] = "Вы отредактировали место!";

            #region Image Upload

            // Проверяем загрузку файла
            if (File != null && File.ContentLength > 0)
            {
                // Получаем расширение файла
                string ext = File.ContentType.ToLower();

                // Проверяем расширение
                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png")
                {
                    using (DataBase DataBase = new DataBase())
                    {
                        ModelState.AddModelError("", "Изображение не было загружено - неправильное расширение изображения");
                        return View(Model);
                    }
                }

                // Устанавливаем пути загрузки
                DirectoryInfo OriginalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));

                string PathString1 = System.IO.Path.Combine(OriginalDirectory.ToString(), "Places\\" + Id.ToString());
                string PathString2 = System.IO.Path.Combine(OriginalDirectory.ToString(), "Places\\" + Id.ToString() + "\\Thumbs");

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
                    PlaceDataTransferObject PlaceDataTransferObject = DataBase.Places.Find(Id);
                    PlaceDataTransferObject.ImageName = ImageName;

                    _ = DataBase.SaveChanges();
                }

                // Сохраняем оригинал и превью версии изображений
                string Path = string.Format($"{PathString1}\\{ImageName}");
                string Path2 = string.Format($"{PathString2}\\{ImageName}");

                // Сохраняем оригинальное изображение
                File.SaveAs(Path);

                // Создаём и сохраняем уменьшенную копию
                WebImage Image = new WebImage(File.InputStream);
                _ = Image.Resize(400, 250);
                _ = Image.Save(Path2);
            }

            #endregion Image Upload

            // Переадресовываем пользователя
            return RedirectToAction("EditPlace");
        }

        public ActionResult GetRegions(int CountryId)
        {
            using (DataBase DataBase = new DataBase())
            {
                List<RegionDataTransferObject> RegionDataTransferObjectList = DataBase.Regions.Where(x => x.CountryId == CountryId).ToList();
                ViewBag.Region = new SelectList(RegionDataTransferObjectList, "Id", "RegionName");
            }
            return PartialView("GetRegions");
        }

        public ActionResult GetCities(int RegionId)
        {
            using (DataBase DataBase = new DataBase())
            {
                List<CityDataTransferObject> CityDataTransferObjectList = DataBase.Cities.Where(x => x.RegionId == RegionId).ToList();
                ViewBag.City = new SelectList(CityDataTransferObjectList, "Id", "CityName");
            }
            return PartialView("GetCities");
        }

        public ActionResult GetCountryCoordinate(int CountryId)
        {
            using (DataBase DataBase = new DataBase())
            {
                CountryDataTransferObject CountryDataTransferObject = DataBase.Countries.FirstOrDefault(x => x.Id == CountryId);
                var Json = JsonConvert.SerializeObject(CountryDataTransferObject);
                return base.Json(Json, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetRegionCoordinate(int RegionId)
        {
            using (DataBase DataBase = new DataBase())
            {
                RegionDataTransferObject RegionDataTransferObject = DataBase.Regions.FirstOrDefault(x => x.Id == RegionId);
                var Array = new decimal[] { RegionDataTransferObject.Latitude, RegionDataTransferObject.Longitude };
                var Json = JsonConvert.SerializeObject(Array);

                return base.Json(Json, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetCityCoordinate(int CityId)
        {
            using (DataBase DataBase = new DataBase())
            {
                CityDataTransferObject CityDataTransferObject = DataBase.Cities.FirstOrDefault(x => x.Id == CityId);
                var Array = new decimal[] { CityDataTransferObject.Latitude, CityDataTransferObject.Longitude };
                var Json = JsonConvert.SerializeObject(Array);

                return base.Json(Json, JsonRequestBehavior.AllowGet);
            }
        }

        // Создаём метод удаления места
        // POST:  Manager/Place/DeletePlace/Id
        public ActionResult DeletePlace(int Id)
        {
            // Удаляем из базы данных
            using (DataBase DataBase = new DataBase())
            {
                List<ServiceDataTransferObject> ServiceDataTransferObject = DataBase.Services.Where(x => x.PlaceId == Id).ToList();
                foreach (ServiceDataTransferObject Service in ServiceDataTransferObject)
                {
                    _ = DataBase.Services.Remove(Service);
                    // Удаляем директорию места
                    DirectoryInfo OriginalDirectory4 = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));
                    string PathString4 = Path.Combine(OriginalDirectory4.ToString(), "Services\\" + Id.ToString());

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
                List<FavoriteDataTransferObject> FavoriteDataTransferObjectList = DataBase.Favorites.Where(x => x.PlaceId == Id).ToList();
                foreach (FavoriteDataTransferObject Favorite in FavoriteDataTransferObjectList)
                {
                    _ = DataBase.Favorites.Remove(Favorite);
                }
                List<DesiredDataTransferObject> DesiredDataTransferObjectList = DataBase.Desires.Where(x => x.PlaceId == Id).ToList();
                foreach (DesiredDataTransferObject Desired in DesiredDataTransferObjectList)
                {
                    _ = DataBase.Desires.Remove(Desired);
                }

                List<RatingDataTransferObject> RatingDataTransferObjectList = DataBase.Ratings.Where(x => x.PlaceId == Id).ToList();
                foreach (RatingDataTransferObject Rating in RatingDataTransferObjectList)
                {
                    _ = DataBase.Ratings.Remove(Rating);

                    // Удаляем директорию 
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

                List<CommentDataTransferObject> CommentDataTransferObjectLIst = DataBase.Comments.Where(x => x.PlaceId == Id).ToList();
                foreach (CommentDataTransferObject Comment in CommentDataTransferObjectLIst)
                {
                    int CommentId = Comment.Id;
                    _ = DataBase.Comments.Remove(Comment);
                    List<ReplyDataTransferObject> ReplyDataTransferObjectList = DataBase.Replies.Where(x => x.CommentId == CommentId).ToList();
                    foreach (ReplyDataTransferObject Reply in ReplyDataTransferObjectList)
                    {
                        _ = DataBase.Replies.Remove(Reply);
                    }
                }
                PlaceDataTransferObject PlaceDataTransferObject = DataBase.Places.Find(Id);
                _ = DataBase.Places.Remove(PlaceDataTransferObject);
                _ = DataBase.SaveChanges();
            }
            // Удаляем директорию 
            DirectoryInfo OriginalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));
            string PathString = Path.Combine(OriginalDirectory.ToString(), "Places\\" + Id.ToString());

            try
            {
                if (Directory.Exists(PathString))
                {
                    Directory.Delete(PathString, true);
                }

                // Переадресовываем пользователя
                return RedirectToAction("Places");
            }
            catch
            {
                // Переадресовываем пользователя
                return RedirectToAction("Places");
            }
        }

        // Создаём метод добавления изображений в галерею
        // POST:  Manager/Place/SaveGalleryImages/Id
        [HttpPost]
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

                    string PathString1 = System.IO.Path.Combine(OriginalDirectory.ToString(), "Places\\" + Id.ToString() + "\\Gallery");
                    string PathString2 = System.IO.Path.Combine(OriginalDirectory.ToString(), "Places\\" + Id.ToString() + "\\Gallery\\Thumbs");

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

        // Создаём метод удаления изображений из галереи
        // POST:  Manager/Place/DeleteImage/Id
        public void DeleteImage(int Id, string ImageName)
        {
            string FullPath1 = Request.MapPath("~/Images/Uploads/Places/" + Id.ToString() + "/Gallery/" + ImageName);
            string FullPath2 = Request.MapPath("~/Images/Uploads/Places/" + Id.ToString() + "/Gallery/Thumbs/" + ImageName);

            if (System.IO.File.Exists(FullPath1))
            {
                System.IO.File.Delete(FullPath1);
            }

            if (System.IO.File.Exists(FullPath2))
            {
                System.IO.File.Delete(FullPath2);
            }
        }

        //Создаём метод добавления 
        // POST: Manager/Place/AddService/
        [HttpGet]
        public ActionResult AddService(int? PlaceId)
        {
            // Объявляем модель
            ServiceViewModel Model = new ServiceViewModel();
            // Добавляем список выбора в модель
            using (DataBase DataBase = new DataBase())
            {

                Model.Place = new SelectList(DataBase.Places.Where(x => x.Id == PlaceId).ToList(), "Id", "PlaceName");
                ViewBag.Place = new SelectList(DataBase.Places.ToList(), "Id", "PlaceName");
                ViewBag.PlaceId = PlaceId;
            }

            // Возвращаем модель в представление
            return View(Model);
        }

        //Создаём метод добавления 
        // POST:  Manager/Place/AddService/Model/File
        [HttpPost]
        public ActionResult AddService(ServiceViewModel Model, HttpPostedFileBase File)
        {
            int Id;
            int PlaceId = Model.PlaceId;
            // Проверяем модель на валидность
            if (!ModelState.IsValid)
            {
                using (DataBase DataBase = new DataBase())
                {

                    Model.Place = new SelectList(DataBase.Places.Where(x => x.Id == Model.PlaceId).ToList(), "Id", "PlaceName");
                    ViewBag.Place = new SelectList(DataBase.Places.ToList(), "Id", "PlaceName");

                    ViewBag.PlaceId = PlaceId;
                    return View(Model);
                }
            }

            // Инициализируем и сохраняем в базу модель
            using (DataBase DataBase = new DataBase())
            {
                Model.Place = new SelectList(DataBase.Places.Where(x => x.CountryId == Model.PlaceId).ToList(), "Id", "PlaceName");

                ServiceDataTransferObject ServiceDataTransferObject = new ServiceDataTransferObject
                {
                    ServiceName = Model.ServiceName,
                    Slug = Model.ServiceName.Replace(@"""", "").Replace(",", "").Replace("'", "").Replace("?", "").Replace("!", "").Replace("/", "").Replace(".", "").Replace(" ", "-").ToLower(),
                    Description = Model.Description,
                    Price = Model.Price,
                    OldPrice = Model.OldPrice,
                    PlaceId = Model.PlaceId,

                    View = Model.View
                };

                _ = DataBase.Services.Add(ServiceDataTransferObject);
                _ = DataBase.SaveChanges();

                // Получаем введённый ID
                Id = ServiceDataTransferObject.Id;

                PlaceId = ServiceDataTransferObject.PlaceId;

            }

            // Добавляем сообщение в TempData
            TempData["SM"] = "Вы добавили услугу!";

            #region Upload Image

            // Создаём необходимые директории
            DirectoryInfo OriginalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));

            string PathString1 = Path.Combine(OriginalDirectory.ToString(), "Services");
            string PathString2 = Path.Combine(OriginalDirectory.ToString(), "Services\\" + Id.ToString());
            string PathString3 = Path.Combine(OriginalDirectory.ToString(), "Services\\" + Id.ToString() + "\\Thumbs");
            string PathString4 = Path.Combine(OriginalDirectory.ToString(), "Services\\" + Id.ToString() + "\\Gallery");
            string PathString5 = Path.Combine(OriginalDirectory.ToString(), "Services\\" + Id.ToString() + "\\Gallery\\Thumbs");

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
                        Model.Place = new SelectList(DataBase.Places.Where(x => x.Id == Model.PlaceId).ToList(), "Id", "PlaceName");
                        ModelState.AddModelError("", "Изображение не было загружено - неправильное расширение изображения");
                        return View(Model);
                    }
                }

                // Объявляем переменную имени изображения
                string ImageName = File.FileName;

                // Сохраняем имя изображения в ServiceDataTransferObject
                using (DataBase DataBase = new DataBase())
                {
                    ServiceDataTransferObject ServiceDataTransferObject = DataBase.Services.Find(Id);
                    ServiceDataTransferObject.ImageName = ImageName;

                    _ = DataBase.SaveChanges();
                }

                // Назначаем пути к оригинальному и уменьшенному изображению
                string Path = string.Format($"{PathString2}\\{ImageName}");
                string Path2 = string.Format($"{PathString3}\\{ImageName}");

                // Сохраняем оригинальное изображение
                File.SaveAs(Path);

                // Создаём и сохраняем уменьшенную копию
                WebImage Image = new WebImage(File.InputStream);
                _ = Image.Resize(400, 250);
                _ = Image.Save(Path2);
            }

            #endregion Upload Image

            // Переадресовываем пользователя
            return RedirectToAction("AddService", new { PlaceId });
        }

        //Создаём метод редактирования 
        // GET:  Manager/Place/EditService/Id
        [HttpGet]
        public ActionResult EditService(int Id)
        {
            // Объявляем модель
            ServiceViewModel Model;

            using (DataBase DataBase = new DataBase())
            {
                // Получаем
                ServiceDataTransferObject ServiceDataTransferObject = DataBase.Services.Find(Id);

                // Проверяем, доступен ли 
                if (ServiceDataTransferObject == null)
                {
                    return Content("Этой услуги не существует.");
                }

                // Инициализируем модель данными
                Model = new ServiceViewModel(ServiceDataTransferObject);

                Model.Place = new SelectList(DataBase.Places.Where(x => x.Id == Model.PlaceId).ToList(), "Id", "PlaceName");

                ViewBag.Place = new SelectList(DataBase.Places.ToList(), "Id", "PlaceName");

                // Получаем все изображения из галереи
                Model.GalleryImages = Directory
                    .EnumerateFiles(Server.MapPath("~/Images/Uploads/Services/" + Id + "/Gallery/Thumbs"))
                    .Select(fn => Path.GetFileName(fn));

                ViewBag.PlaceId = Model.PlaceId;
            }

            // Возвращаем модель в представление
            return View(Model);
        }

        // Создаём метод редактирования 
        // POST:  Manager/Place/EditService
        [HttpPost]
        public ActionResult EditService(ServiceViewModel Model, HttpPostedFileBase File)
        {
            // Получаем ID 
            int Id = Model.Id;

            // Заполняем список 
            using (DataBase DataBase = new DataBase())
            {
                Model.Place = new SelectList(DataBase.Places.Where(x => x.Id == Model.PlaceId).ToList(), "Id", "PlaceName");

                ViewBag.Place = new SelectList(DataBase.Places.ToList(), "Id", "PlaceName");
            }

            Model.GalleryImages = Directory
                .EnumerateFiles(Server.MapPath("~/Images/Uploads/Services/" + Id + "/Gallery/Thumbs"))
                .Select(fn => Path.GetFileName(fn));

            // Проверяем модель на валидность
            if (!ModelState.IsValid)
            {
                return View(Model);
            }

            // Обновляем 
            // Инициализируем и сохраняем в базу модель
            using (DataBase DataBase = new DataBase())
            {
                Model.Place = new SelectList(DataBase.Places.Where(x => x.CountryId == Model.PlaceId).ToList(), "Id", "PlaceName");
                ServiceDataTransferObject ServiceDataTransferObject = DataBase.Services.Find(Id);

                ServiceDataTransferObject.ServiceName = Model.ServiceName;
                ServiceDataTransferObject.Slug = Model.ServiceName.Replace(@"""", "").Replace(",", "").Replace("'", "").Replace("?", "").Replace("!", "").Replace("/", "").Replace(".", "").Replace(" ", "-").ToLower();
                ServiceDataTransferObject.Description = Model.Description;
                ServiceDataTransferObject.Price = Model.Price;
                ServiceDataTransferObject.OldPrice = Model.OldPrice;
                ServiceDataTransferObject.PlaceId = Model.PlaceId;
                ServiceDataTransferObject.View = Model.View;

                if (Model.ImageName != null)
                {
                    ServiceDataTransferObject.ImageName = Model.ImageName;
                }

                _ = DataBase.SaveChanges();
            }


            // Устанавливаем сообщение в TempData
            TempData["SM"] = "Вы отредактировали услугу!";

            #region Image Upload

            // Проверяем загрузку файла
            if (File != null && File.ContentLength > 0)
            {
                // Получаем расширение файла
                string ext = File.ContentType.ToLower();

                // Проверяем расширение
                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png")
                {
                    using (DataBase DataBase = new DataBase())
                    {
                        ModelState.AddModelError("", "Изображение не было загружено - неправильное расширение изображения");
                        return View(Model);
                    }
                }

                // Устанавливаем пути загрузки
                DirectoryInfo OriginalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));

                string PathString1 = System.IO.Path.Combine(OriginalDirectory.ToString(), "Services\\" + Id.ToString());
                string PathString2 = System.IO.Path.Combine(OriginalDirectory.ToString(), "Services\\" + Id.ToString() + "\\Thumbs");

                // Удаляем существующие файлы в директориях
                DirectoryInfo Directory1 = new DirectoryInfo(PathString1);
                DirectoryInfo Directory2 = new DirectoryInfo(PathString2);

                foreach (FileInfo File2 in Directory1.GetFiles())
                {
                    File2.Delete();
                }

                foreach (FileInfo file3 in Directory2.GetFiles())
                {
                    file3.Delete();
                }

                // Сохраняем имя изображения
                string ImageName = File.FileName;

                using (DataBase DataBase = new DataBase())
                {
                    ServiceDataTransferObject ServiceDataTransferObject = DataBase.Services.Find(Id);
                    ServiceDataTransferObject.ImageName = ImageName;

                    _ = DataBase.SaveChanges();
                }

                // Сохраняем оригинал и превью версии изображений
                string Path = string.Format($"{PathString1}\\{ImageName}");
                string Path2 = string.Format($"{PathString2}\\{ImageName}");

                // Сохраняем оригинальное изображение
                File.SaveAs(Path);

                // Создаём и сохраняем уменьшенную копию
                WebImage Image = new WebImage(File.InputStream);
                _ = Image.Resize(400, 250);
                _ = Image.Save(Path2);
            }

            #endregion Image Upload

            // Переадресовываем пользователя
            return RedirectToAction("EditService", new { Id });
        }

        [HttpGet]
        public ActionResult Services(int? Page, int? PlaceId)
        {
            // Объявляем типа лист
            List<ServiceViewModel> ListServiceViewModel;

            // Устанавливаем номер страницы
            int PageNumber = Page ?? 1;

            using (DataBase DataBase = new DataBase())
            {
                // Инициализируем лист
                ListServiceViewModel = DataBase.Services.ToArray()
                    .Where(x => PlaceId == null || PlaceId == 0 || x.PlaceId == PlaceId)
                           .Select(x => new ServiceViewModel(x))
                           .ToList();
            }

            // Устанавливаем постраничную навигацию
            IPagedList<ServiceViewModel> OnePageOfServiceViewModel = ListServiceViewModel.ToPagedList(PageNumber, 3);
            ViewBag.OnePageOfServices = OnePageOfServiceViewModel;
            ViewBag.PlaceId = PlaceId;
            // Возвращаем представление и лист
            return View(ListServiceViewModel);
        }
        public ActionResult DeleteService(int Id, int? PlaceId)
        {
            // Удаляем из базы данных
            using (DataBase DataBase = new DataBase())
            {
                ServiceDataTransferObject ServiceDataTransferObject = DataBase.Services.Find(Id);
                _ = DataBase.Services.Remove(ServiceDataTransferObject);
                _ = DataBase.SaveChanges();
            }
            // Удаляем директорию 
            DirectoryInfo OriginalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));
            string PathString = Path.Combine(OriginalDirectory.ToString(), "Services\\" + Id.ToString());

            try
            {
                if (Directory.Exists(PathString))
                {
                    Directory.Delete(PathString, true);
                }
                // Переадресовываем пользователя
                // return RedirectToAction("Services");
                return RedirectToAction("Services", "Place", new { PlaceId });
            }
            catch
            {
                // Переадресовываем пользователя
                return RedirectToAction("Services", "Place", new { PlaceId });
            }
        }

        // Создаём метод добавления изображений в галерею
        // POST:  Manager/Place/SaveGalleryImages/Id
        [HttpPost]
        public void SaveGalleryImagesService(int Id)
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

                    string PathString1 = System.IO.Path.Combine(OriginalDirectory.ToString(), "Services\\" + Id.ToString() + "\\Gallery");
                    string PathString2 = System.IO.Path.Combine(OriginalDirectory.ToString(), "Services\\" + Id.ToString() + "\\Gallery\\Thumbs");

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

        // Создаём метод удаления изображений из галереи
        // POST:  Manager/Place/DeleteImage/Id
        public void DeleteImageService(int Id, string ImageName)
        {
            string FullPath1 = Request.MapPath("~/Images/Uploads/Services/" + Id.ToString() + "/Gallery/" + ImageName);
            string FullPath2 = Request.MapPath("~/Images/Uploads/Services/" + Id.ToString() + "/Gallery/Thumbs/" + ImageName);

            if (System.IO.File.Exists(FullPath1))
            {
                System.IO.File.Delete(FullPath1);
            }

            if (System.IO.File.Exists(FullPath2))
            {
                System.IO.File.Delete(FullPath2);
            }
        }

    }
}