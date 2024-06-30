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

namespace WebStoreMap.Areas.Moderator.Controllers
{
    [Authorize(Roles = "Moderator")]
    public class PlaceController : Controller
    {
        // GET: Moderator/Place
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

        //Список категорий
        // GET: Moderator/Place/Categories
        [Authorize]
        public ActionResult Categories()
        {
            //Объявляем модель типа List
            List<CategoryViewModel> categoryVMList;
            using (DataBase db = new DataBase())
            {
                //Инициализируем его
                categoryVMList = db.Categories
                                .ToArray()
                                .OrderBy(x => x.Sorting)
                                .Select(x => new CategoryViewModel(x))
                                .ToList();
            }

            //Возвращаем лист в представление
            return View(categoryVMList);
        }

        // POST: Moderator/Place/AddNewCategory
        [HttpPost]
        [Authorize]
        public string AddNewCategory(string catName)
        {
            //Объявить переменную ID
            string id;

            using (DataBase db = new DataBase())
            {
                //Проверка имени категории на уникальность
                if (db.Categories.Any(x => x.CategoryName == catName))
                {
                    return "titletaken";
                }

                //Инициализировать модель CategoryDataTransferObject
                CategoryDataTransferObject dto = new CategoryDataTransferObject
                {
                    //Добавить данные в CategoryDataTransferObject
                    CategoryName = catName,
                    Slug = catName.Replace(" ", "-").ToLower(),
                    Sorting = 100
                };

                //Сохранить CategoryDataTransferObject
                _ = db.Categories.Add(dto);
                _ = db.SaveChanges();

                //Получить ID
                id = dto.Id.ToString();
            }

            //Вернуть ID
            return id;
        }

        //Создаём метод сортировки
        // GET: Moderator/Place/ReorderCategories
        [HttpPost]
        [Authorize]
        public void ReorderCategories(int[] id)
        {
            using (DataBase db = new DataBase())
            {
                //Реализуем начальный счётчик
                int count = 1;

                //Инициализируем модель данных
                CategoryDataTransferObject dto;

                //Устанавливаем сортировку для каждой страницы
                foreach (int catId in id)
                {
                    dto = db.Categories.Find(catId);
                    dto.Sorting = count;
                    _ = db.SaveChanges();

                    count++;
                }
            }
        }

        // GET: Moderator/Place/DeleteCategory/Id
        [Authorize]
        public ActionResult DeleteCategory(int id)
        {
            using (DataBase db = new DataBase())
            {
                //Получаем категорию
                CategoryDataTransferObject dto = db.Categories.Find(id);

                //Удаляем категорию
                _ = db.Categories.Remove(dto);

                //Сохраняем изменения в базе
                _ = db.SaveChanges();
            }

            //Добавляем сообщение о удачном удалении категории
            TempData["SM"] = "Вы удалили категорию!";

            //Переадресовываем пользователя
            return RedirectToAction("Categories");
        }

        // POST: Moderator/Shop/RenameCategory/Id
        [HttpPost]
        [Authorize]
        public string RenameCategory(string newCatName, int id)
        {
            using (DataBase db = new DataBase())
            {
                // Проверяем имя на уникальность
                if (db.Categories.Any(x => x.CategoryName == newCatName))
                {
                    return "titletaken";
                }

                // Получаем CategoryDataTransferObject
                CategoryDataTransferObject dto = db.Categories.Find(id);

                // Редактируем CategoryDataTransferObject
                dto.CategoryName = newCatName;
                dto.Slug = newCatName.Replace(" ", "-").ToLower();

                // Сохраняем изменения
                _ = db.SaveChanges();
            }

            // Возвращаем результат
            return "ok";
        }

        [HttpGet]
        public ActionResult AddPlace()
        {
            string userEmail = User.Identity.Name;

            // Объявляем модель
            PlaceViewModel model = new PlaceViewModel();

            // Добавляем список выбора категорий в модель
            using (DataBase db = new DataBase())
            {
                UserDataTransferObject userdto = db.Users.FirstOrDefault(x => x.EmailAddress == userEmail);

                model.Categories = new SelectList(db.Categories.ToList(), "Id", "CategoryName");

                model.Country = new SelectList(db.Countries.ToList(), "Id", "CountryName");

                if (model.Country != null)
                {
                    model.Region = new SelectList(db.Regions.Where(x => x.CountryId == model.CountryId).ToList(), "Id", "RegionName");
                    if (model.Region != null)
                    {
                        model.City = new SelectList(db.Cities.Where(x => x.RegionId == model.RegionId).ToList(), "Id", "CityName");
                    }
                }

                model.Company = new SelectList(db.Companies.ToList(), "Id", "CompanyName");

                CompanyDataTransferObject companydto = db.Companies.FirstOrDefault(x => x.UserId == userdto.Id);

                ViewBag.IsCompanyExist = companydto != null;
            }

            // Возвращаем модель в представление
            return View(model);
        }

        //Создаём метод добавления товаров
        // POST:  Moderator/Place/AddPlace/model/file
        [HttpPost]
        public ActionResult AddPlace(PlaceViewModel model, HttpPostedFileBase file)
        {
            int id;
            string userEmail = User.Identity.Name;

            // Проверяем модель на валидность
            if (!ModelState.IsValid)
            {
                using (DataBase db = new DataBase())
                {
                    model.Country = new SelectList(db.Countries.ToList(), "Id", "CountryName");

                    model.Company = new SelectList(db.Companies.ToList(), "Id", "CompanyName");

                    if (model.Country != null)
                    {
                        model.Region = new SelectList(db.Regions.Where(x => x.CountryId == model.CountryId).ToList(), "Id", "RegionName");
                        if (model.Region != null)
                        {
                            model.City = new SelectList(db.Cities.Where(x => x.RegionId == model.RegionId).ToList(), "Id", "CityName");
                        }
                    }

                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "CategoryName");

                    return View(model);
                }
            }

            // Проверяем имя продукта на уникальность
            using (DataBase db = new DataBase())
            {
                if (db.Places.AsParallel().Any(x => x.PlaceName == model.PlaceName))
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "CategoryName");
                    model.Country = new SelectList(db.Countries.ToList(), "Id", "CountryName");
                    model.Company = new SelectList(db.Companies.ToList(), "Id", "CompanyName");

                    if (model.Country != null)
                    {
                        model.Region = new SelectList(db.Regions.Where(x => x.CountryId == model.CountryId).ToList(), "Id", "RegionName");
                        if (model.Region != null)
                        {
                            model.City = new SelectList(db.Cities.Where(x => x.RegionId == model.RegionId).ToList(), "Id", "CityName");
                        }
                    }

                    ModelState.AddModelError("", "Это название уже занято!");
                    return View(model);
                }
            }

            // Инициализируем и сохраняем в базу модель productDTO
            using (DataBase db = new DataBase())
            {
                UserDataTransferObject userdto = db.Users.FirstOrDefault(x => x.EmailAddress == userEmail);

                PlaceDataTransferObject product = new PlaceDataTransferObject
                {
                    PlaceName = model.PlaceName,
                    Slug = model.PlaceName.Replace(@"""", "").Replace(",", "").Replace("'", "").Replace("?", "").Replace("!", "").Replace("/", "").Replace(".", "").Replace(" ", "-").ToLower(),
                    Description = model.Description,
                    Price = model.Price,
                    CategoryId = model.CategoryId,
                    CityId = model.CityId,
                    CityName = model.CityName,
                    RegionId = model.RegionId,
                    CountryId = model.CountryId,
                    Latitude = model.Latitude,
                    Longitude = model.Longitude,
                    OldPrice = model.OldPrice,
                    Address = model.Address,
                    View = model.View,
                    PhoneNumber = model.PhoneNumber,
                    UserId = userdto.Id,

                    CompanyId = model.CompanyId
                };
                CategoryDataTransferObject catDTO = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                product.CategoryName = catDTO.CategoryName;
                CountryDataTransferObject countrDTO = db.Countries.FirstOrDefault(x => x.Id == model.CountryId);
                product.CountryName = countrDTO.CountryName;
                RegionDataTransferObject regDTO = db.Regions.FirstOrDefault(x => x.Id == model.RegionId);

                product.RegionName = regDTO.RegionName;
                CityDataTransferObject sityDTO = db.Cities.FirstOrDefault(x => x.Id == model.CityId);

                product.CityName = sityDTO.CityName;
                product.FirstDay = model.FirstDay;
                product.LastDay = model.LastDay;

                product.Schedule = model.Schedule;
                _ = db.Places.Add(product);
                _ = db.SaveChanges();

                // Получаем введённый ID
                id = product.Id;
            }

            // Добавляем сообщение в TempData
            TempData["SM"] = "Вы добавили услугу!";

            #region Upload Image

            // Создаём необходимые директории
            DirectoryInfo originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));

            string pathString1 = Path.Combine(originalDirectory.ToString(), "Places");
            string pathString2 = Path.Combine(originalDirectory.ToString(), "Places\\" + id.ToString());
            string pathString3 = Path.Combine(originalDirectory.ToString(), "Places\\" + id.ToString() + "\\Thumbs");
            string pathString4 = Path.Combine(originalDirectory.ToString(), "Places\\" + id.ToString() + "\\Gallery");
            string pathString5 = Path.Combine(originalDirectory.ToString(), "Places\\" + id.ToString() + "\\Gallery\\Thumbs");

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
                        model.Country = new SelectList(db.Countries.ToList(), "Id", "CountryName");
                        if (model.Country != null)
                        {
                            model.Region = new SelectList(db.Regions.Where(x => x.CountryId == model.CountryId).ToList(), "Id", "Services");
                            if (model.Region != null)
                            {
                                model.City = new SelectList(db.Cities.Where(x => x.RegionId == model.RegionId).ToList(), "Id", "CityName");
                            }
                        }
                        model.Company = new SelectList(db.Companies.ToList(), "Id", "ServiceName");

                        model.Categories = new SelectList(db.Categories.ToList(), "Id", "ServiceName");
                        ModelState.AddModelError("", "Изображение не было загружено - неправильное расширение изображения");
                        return View(model);
                    }
                }

                // Объявляем переменную имени изображения
                string imageName = file.FileName;

                // Сохраняем имя изображения в DTO
                using (DataBase db = new DataBase())
                {
                    PlaceDataTransferObject dto = db.Places.Find(id);
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
                _ = img.Resize(400, 250);
                _ = img.Save(path2);
            }

            #endregion Upload Image

            // Переадресовываем пользователя
            return RedirectToAction("AddPlace");
        }

        //Создаём метод отображения товаров
        // GET:  Moderator/Place/Places
        [HttpGet]
        public ActionResult Places(int? page, int? catId)
        {
            // Объявляем PlaceViewModel типа лист
            List<PlaceViewModel> listOfPlaceVM;

            // Объявляем PlaceViewModel типа лист
            List<PlaceViewModel> listOfPlaceVM2 = new List<PlaceViewModel>();

            // Устанавливаем номер страницы
            int pageNumber = page ?? 1;

            using (DataBase db = new DataBase())
            {
                // Инициализируем лист
                listOfPlaceVM = db.Places.ToArray()
                           .Where(x => catId == null || catId == 0 || x.CategoryId == catId)
                           .Select(x => new PlaceViewModel(x))
                           .ToList();

                // Заполняем лист категорий
                ViewBag.Categories = new SelectList(db.Categories.ToList(), "Id", "CategoryName");

                // Устанавливаем выбранную категорию
                ViewBag.SelectedCat = catId.ToString();
            }

            // Устанавливаем постраничную навигацию
            IPagedList<PlaceViewModel> onePageOfPlaces = listOfPlaceVM.ToPagedList(pageNumber, 3);
            ViewBag.OnePageOfPlaces = onePageOfPlaces;

            // Возвращаем представление и лист
            return View(listOfPlaceVM);
        }

        //Создаём метод редактирования товаров
        // GET:  Moderator/Place/EditPlace/Id
        [HttpGet]
        public ActionResult EditPlace(int id)
        {
            // Объявляем модель PlaceViewModel
            PlaceViewModel model;

            using (DataBase db = new DataBase())
            {
                // Получаем продукт
                PlaceDataTransferObject dto = db.Places.Find(id);

                // Проверяем, доступен ли продукт
                if (dto == null)
                {
                    return Content("Этой услуги не существует.");
                }

                // Инициализируем модель данными
                model = new PlaceViewModel(dto)
                {
                    // Создаём список категорий
                    Categories = new SelectList(db.Categories.ToList(), "Id", "CategoryName"),

                    Country = new SelectList(db.Countries.ToList(), "Id", "CountryName")
                };

                if (model.Country != null)
                {
                    model.Region = new SelectList(db.Regions.Where(x => x.CountryId == model.CountryId).ToList(), "Id", "RegionName");
                    if (model.Region != null)
                    {
                        model.City = new SelectList(db.Cities.Where(x => x.RegionId == model.RegionId).ToList(), "Id", "CityName");
                    }
                }
                model.Company = new SelectList(db.Companies.ToList(), "Id", "CompanyName");

                // Получаем все изображения из галереи
                model.GalleryImages = Directory
                    .EnumerateFiles(Server.MapPath("~/Images/Uploads/Places/" + id + "/Gallery/Thumbs"))
                    .Select(fn => Path.GetFileName(fn));
            }

            // Возвращаем модель в представление
            return View(model);
        }

        // Создаём метод редактирования товаров
        // POST:  Moderator/Place/EditPlace
        [HttpPost]
        public ActionResult EditPlace(PlaceViewModel model, HttpPostedFileBase file)
        {
            // Получаем ID продукта
            int id = model.Id;

            // Заполняем список категориями и изображениями
            using (DataBase db = new DataBase())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "CategoryName");

                model.Country = new SelectList(db.Countries.ToList(), "Id", "CountryName");

                if (model.Country != null)
                {
                    model.Region = new SelectList(db.Regions.Where(x => x.CountryId == model.CountryId).ToList(), "Id", "RegionName");
                    if (model.Region != null)
                    {
                        model.City = new SelectList(db.Cities.Where(x => x.RegionId == model.RegionId).ToList(), "Id", "CityName");
                    }
                }
                model.Company = new SelectList(db.Companies.ToList(), "Id", "CompanyName");
            }

            model.GalleryImages = Directory
                .EnumerateFiles(Server.MapPath("~/Images/Uploads/Places/" + id + "/Gallery/Thumbs"))
                .Select(fn => Path.GetFileName(fn));

            // Проверяем модель на валидность
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Проверяем имя продукта на уникальность
            using (DataBase db = new DataBase())
            {
                if (db.Places.Where(x => x.Id != id).Any(x => x.PlaceName == model.PlaceName))
                {
                    ModelState.AddModelError("", "Это название уже занято!");
                    return View(model);
                }
            }
            // Обновляем продукт
            using (DataBase db = new DataBase())
            {
                PlaceDataTransferObject dto = db.Places.Find(id);

                dto.PlaceName = model.PlaceName;
                dto.Slug = model.PlaceName.Replace(@"""", "").Replace(",", "").Replace("'", "").Replace("?", "").Replace("!", "").Replace("/", "").Replace(".", "").Replace(" ", "-").ToLower();
                dto.Description = model.Description;
                dto.Price = model.Price;
                dto.CategoryId = model.CategoryId;
                dto.CityId = model.CityId;
                dto.CityName = model.CityName;
                dto.RegionId = model.RegionId;
                dto.CountryId = model.CountryId;

                if (model.ImageName != null)
                {
                    dto.ImageName = model.ImageName;
                }
                dto.Latitude = model.Latitude;
                dto.Longitude = model.Longitude;
                dto.OldPrice = model.OldPrice;
                dto.PhoneNumber = model.PhoneNumber;
                dto.Address = model.Address;
                dto.View = model.View;

                dto.CompanyId = model.CompanyId;
                CategoryDataTransferObject catDTO = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                dto.CategoryName = catDTO.CategoryName;
                RegionDataTransferObject regDTO = db.Regions.FirstOrDefault(x => x.Id == model.RegionId);
                dto.RegionName = regDTO.RegionName;

                dto.FirstDay = model.FirstDay;
                dto.LastDay = model.LastDay;
                CountryDataTransferObject countrDTO = db.Countries.FirstOrDefault(x => x.Id == model.CountryId);
                dto.CountryName = countrDTO.CountryName;
                CityDataTransferObject sityDTO = db.Cities.FirstOrDefault(x => x.Id == model.CityId);

                dto.CityName = sityDTO.CityName;

                _ = db.SaveChanges();
            }

            // Устанавливаем сообщение в TempData
            TempData["SM"] = "Вы отредактировали услугу!";

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

                string pathString1 = Path.Combine(originalDirectory.ToString(), "Places\\" + id.ToString());
                string pathString2 = Path.Combine(originalDirectory.ToString(), "Places\\" + id.ToString() + "\\Thumbs");

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
                    PlaceDataTransferObject dto = db.Places.Find(id);
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
                _ = img.Resize(400, 250);
                _ = img.Save(path2);
            }

            #endregion Image Upload

            // Переадресовываем пользователя
            return RedirectToAction("EditPlace");
        }

        public ActionResult GetRegions(int CountryId)
        {
            using (DataBase db = new DataBase())
            {
                List<RegionDataTransferObject> region = db.Regions.Where(x => x.CountryId == CountryId).ToList();
                ViewBag.Region = new SelectList(region, "Id", "RegionName");
            }
            return PartialView("GetRegions");
        }

        public ActionResult GetCities(int RegionId)
        {
            using (DataBase db = new DataBase())
            {
                List<CityDataTransferObject> sity = db.Cities.Where(x => x.RegionId == RegionId).ToList();
                ViewBag.City = new SelectList(sity, "Id", "CityName");
            }
            return PartialView("GetCities");
        }

        public ActionResult GetCountryCoordinate(int CountryId)
        {
            using (DataBase db = new DataBase())
            {
                CountryDataTransferObject countryCoordinates = db.Countries.FirstOrDefault(x => x.Id == CountryId);
                var json = JsonConvert.SerializeObject(countryCoordinates);
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetRegionCoordinate(int RegionId)
        {
            using (DataBase db = new DataBase())
            {
                RegionDataTransferObject regionCoordinates = db.Regions.FirstOrDefault(x => x.Id == RegionId);
                var array = new decimal[] { regionCoordinates.Latitude, regionCoordinates.Longitude };
                var json = JsonConvert.SerializeObject(array);

                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetCityCoordinate(int CityId)
        {
            using (DataBase db = new DataBase())
            {
                CityDataTransferObject cityCoordinates = db.Cities.FirstOrDefault(x => x.Id == CityId);
                var array = new decimal[] { cityCoordinates.Latitude, cityCoordinates.Longitude };
                var json = JsonConvert.SerializeObject(array);

                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        // Создаём метод удаления товаров
        // POST:  Moderator/Place/DeletePlace/Id
        public ActionResult DeletePlace(int id)
        {
            // Удаляем товар из базы данных
            using (DataBase db = new DataBase())
            {
                List<FavoriteDataTransferObject> favdto = db.Favorites.Where(x => x.PlaceId == id).ToList();
                foreach (FavoriteDataTransferObject i in favdto)
                {
                    _ = db.Favorites.Remove(i);
                }
                List<DesiredDataTransferObject> desdto = db.Desires.Where(x => x.PlaceId == id).ToList();
                foreach (DesiredDataTransferObject i in desdto)
                {
                    _ = db.Desires.Remove(i);
                }

                List<RatingDataTransferObject> rating = db.Ratings.Where(x => x.PlaceId == id).ToList();
                foreach (RatingDataTransferObject i in rating)
                {
                    _ = db.Ratings.Remove(i);

                    // Удаляем директорию товара
                    DirectoryInfo originalDirectory1 = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));
                    string pathString1 = Path.Combine(originalDirectory1.ToString(), "Feedback\\" + i.Id.ToString());

                    try
                    {
                        if (Directory.Exists(pathString1))
                        {
                            Directory.Delete(pathString1, true);
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Ошибка при попытки удаления");
                    }
                }

                List<CommentDataTransferObject> com = db.Comments.Where(x => x.PlaceId == id).ToList();
                foreach (CommentDataTransferObject i in com)
                {
                    int idcom = i.Id;
                    _ = db.Comments.Remove(i);
                    List<ReplyDataTransferObject> repl = db.Replies.Where(x => x.CommentId == idcom).ToList();
                    foreach (ReplyDataTransferObject r in repl)
                    {
                        _ = db.Replies.Remove(r);
                    }
                }
                PlaceDataTransferObject dto = db.Places.Find(id);
                _ = db.Places.Remove(dto);
                _ = db.SaveChanges();
            }
            // Удаляем директорию товара
            DirectoryInfo originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));
            string pathString = Path.Combine(originalDirectory.ToString(), "Places\\" + id.ToString());

            try
            {
                if (Directory.Exists(pathString))
                {
                    Directory.Delete(pathString, true);
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
        // POST:  Moderator/Place/SaveGalleryImages/Id
        [HttpPost]
        public void SaveGalleryImages(int id)
        {
            // Перебираем все файлы
            foreach (string fileName in Request.Files)
            {
                // Инициализируем файлы
                HttpPostedFileBase file = Request.Files[fileName];

                // Проверяем на NULL
                if (file != null && file.ContentLength > 0)
                {
                    // Назначаем пути к директориям
                    DirectoryInfo originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));

                    string pathString1 = Path.Combine(originalDirectory.ToString(), "Places\\" + id.ToString() + "\\Gallery");
                    string pathString2 = Path.Combine(originalDirectory.ToString(), "Places\\" + id.ToString() + "\\Gallery\\Thumbs");

                    // Назначаем пути изображений
                    string path = string.Format($"{pathString1}\\{file.FileName}");
                    string path2 = string.Format($"{pathString2}\\{file.FileName}");

                    // Сохраняем оригинальные изображения и уменьшеные копии
                    file.SaveAs(path);

                    WebImage img = new WebImage(file.InputStream);
                    _ = img.Resize(200, 200).Crop(1, 1);
                    _ = img.Save(path2);
                }
            }
        }

        // Создаём метод удаления изображений из галереи
        // POST:  Moderator/Place/DeleteImage/Id
        public void DeleteImage(int id, string imageName)
        {
            string fullPath1 = Request.MapPath("~/Images/Uploads/Places/" + id.ToString() + "/Gallery/" + imageName);
            string fullPath2 = Request.MapPath("~/Images/Uploads/Places/" + id.ToString() + "/Gallery/Thumbs/" + imageName);

            if (System.IO.File.Exists(fullPath1))
            {
                System.IO.File.Delete(fullPath1);
            }

            if (System.IO.File.Exists(fullPath2))
            {
                System.IO.File.Delete(fullPath2);
            }
        }
    }
}