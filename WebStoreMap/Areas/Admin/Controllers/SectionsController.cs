using PagedList;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using WebStoreMap.Models.Data;
using WebStoreMap.Models.ViewModels.geodan;

namespace WebStoreMap.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SectionsController : Controller
    {
        // GET: Admin/Sections
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

        //Создаём метод добавления страны
        // Get:  Admin/Sections/AddСountry
        [HttpGet]
        public ActionResult AddСountry()
        {
            // Объявляем модель
            CountryInformationViewModel Model = new CountryInformationViewModel();
            // Добавляем список выбора стран в модель
            using (DataBase DataBase = new DataBase())
            {
                Model.Country = new SelectList(DataBase.Countries.ToList(), "Id", "CountryName");
                ViewBag.Country = new SelectList(DataBase.Countries.ToList(), "Id", "CountryName");
            }

            // Возвращаем модель в представление
            return View(Model);
        }

        //Создаём метод добавления страны
        // Post:  Admin/Sections/AddСountry/Model/File
        [HttpPost]
        public ActionResult AddСountry(CountryInformationViewModel Model, HttpPostedFileBase File)
        {
            int Id;
            // Проверяем модель на валидность
            if (!ModelState.IsValid)
            {
                using (DataBase DataBase = new DataBase())
                {
                    Model.Country = new SelectList(DataBase.Countries.ToList(), "Id", "CountryName");
                    ViewBag.Country = new SelectList(DataBase.Countries.ToList(), "Id", "CountryName");
                    return View(Model);
                }
            }

            // Инициализируем и сохраняем в базу модель CountryInformationDataTransferObject
            using (DataBase DataBase = new DataBase())
            {
                Model.Country = new SelectList(DataBase.Countries.ToList(), "Id", "CountryName");

                CountryInformationDataTransferObject CountryInformationDataTransferObject = new CountryInformationDataTransferObject();

                CountryDataTransferObject CountryDataTransferObject = DataBase.Countries.Find(Model.CountryInformationId);

                CountryInformationDataTransferObject.Country = CountryDataTransferObject;

                CountryInformationDataTransferObject.Slug = CountryDataTransferObject.CountryName.Replace(@"""", "").Replace(",", "").Replace("'", "").Replace("?", "").Replace("!", "").Replace("/", "").Replace(".", "").Replace(" ", "-").ToLower();
                CountryInformationDataTransferObject.Description = Model.Description;
                CountryInformationDataTransferObject.CountryId = Model.CountryInformationId;

                CountryInformationDataTransferObject.View = Model.View;

                _ = DataBase.CountriesInformation.Add(CountryInformationDataTransferObject);
                _ = DataBase.SaveChanges();

                // Получаем введённый ID
                Id = CountryInformationDataTransferObject.CountryId;
            }

            // Добавляем сообщение в TempData
            TempData["SM"] = "Вы добавили страну!";

            #region Upload Image

            // Создаём необходимые директории
            DirectoryInfo OriginalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));

            string PathString1 = Path.Combine(OriginalDirectory.ToString(), "Countries");
            string PathString2 = Path.Combine(OriginalDirectory.ToString(), "Countries\\" + Id.ToString());
            string PathString3 = Path.Combine(OriginalDirectory.ToString(), "Countries\\" + Id.ToString() + "\\Thumbs");
            string PathString4 = Path.Combine(OriginalDirectory.ToString(), "Countries\\" + Id.ToString() + "\\Gallery");
            string PathString5 = Path.Combine(OriginalDirectory.ToString(), "Countries\\" + Id.ToString() + "\\Gallery\\Thumbs");

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

                        ModelState.AddModelError("", "Изображение не было загружено - неправильное расширение изображения");
                        return View(Model);
                    }
                }

                // Объявляем переменную имени изображения
                string ImageName = File.FileName;

                // Сохраняем имя изображения в CountryInformationDataTransferObject
                using (DataBase DataBase = new DataBase())
                {
                    CountryInformationDataTransferObject CountryInformationDataTransferObject = DataBase.CountriesInformation.Find(Id);
                    CountryInformationDataTransferObject.ImageName = ImageName;

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
            return RedirectToAction("AddСountry");
        }

        //Создаём метод редактирования страны
        // GET:  Admin/Sections/EditСountry/Id
        [HttpGet]
        public ActionResult EditСountry(int Id)
        {
            // Объявляем модель CountriesinfoViewModel
            CountryInformationViewModel Model;

            using (DataBase DataBase = new DataBase())
            {
                // Получаем страну
                CountryInformationDataTransferObject CountryInformationDataTransferObject = DataBase.CountriesInformation.Find(Id);

                // Проверяем, доступна ли страна
                if (CountryInformationDataTransferObject == null)
                {
                    return Content("Этой страны не существует.");
                }

                // Инициализируем модель данными
                Model = new CountryInformationViewModel(CountryInformationDataTransferObject)
                {
                    Country = new SelectList(DataBase.Countries.ToList(), "Id", "CountryName")
                };

                ViewBag.Country = new SelectList(DataBase.Countries.ToList(), "Id", "CountryName");

                // Получаем все изображения из галереи
                Model.GalleryImages = Directory
                    .EnumerateFiles(Server.MapPath("~/Images/Uploads/Countries/" + Id + "/Gallery/Thumbs"))
                    .Select(fn => Path.GetFileName(fn));
            }

            // Возвращаем модель в представление
            return View(Model);
        }

        // Создаём метод редактирования страны
        // POST:  Admin/Sections/EditСountry
        [HttpPost]
        public ActionResult EditСountry(CountryInformationViewModel Model, HttpPostedFileBase File)
        {
            // Получаем ID страны
            int Id = Model.CountryInformationId;

            // Заполняем список стран
            using (DataBase DataBase = new DataBase())
            {
                Model.Country = new SelectList(DataBase.Countries.ToList(), "Id", "CountryName");
                ViewBag.Country = new SelectList(DataBase.Countries.ToList(), "Id", "CountryName");
            }

            Model.GalleryImages = Directory
                .EnumerateFiles(Server.MapPath("~/Images/Uploads/Countries/" + Id + "/Gallery/Thumbs"))
                .Select(fn => Path.GetFileName(fn));

            // Проверяем модель на валидность
            if (!ModelState.IsValid)
            {
                return View(Model);
            }

            // Проверяем имя страны на уникальность
            using (DataBase DataBase = new DataBase())
            {
                if (DataBase.CountriesInformation.Where(x => x.CountryId != Id).Any(x => x.Country.CountryName == Model.CountryName))
                {
                    ModelState.AddModelError("", "Это название уже занято!");
                    return View(Model);
                }
            }
            // Обновляем страну
            using (DataBase DataBase = new DataBase())
            {
                Model.Country = new SelectList(DataBase.Countries.ToList(), "Id", "CountryName");

                CountryDataTransferObject CountryDataTransferObject = DataBase.Countries.Find(Id);
                CountryInformationDataTransferObject CountryInformationDataTransferObject = DataBase.CountriesInformation.Find(Id);

                CountryInformationDataTransferObject.Slug = CountryDataTransferObject.CountryName.Replace(@"""", "").Replace(",", "").Replace("'", "").Replace("?", "").Replace("!", "").Replace("/", "").Replace(".", "").Replace(" ", "-").ToLower();
                CountryInformationDataTransferObject.Description = Model.Description;
                CountryInformationDataTransferObject.CountryId = Model.CountryInformationId;

                if (Model.ImageName != null)
                {
                    CountryInformationDataTransferObject.ImageName = Model.ImageName;
                }

                CountryInformationDataTransferObject.View = Model.View;

                _ = DataBase.SaveChanges();
            }

            // Устанавливаем сообщение в TempData
            TempData["SM"] = "Вы отредактировали страну!";

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

                string PathString1 = System.IO.Path.Combine(OriginalDirectory.ToString(), "Countries\\" + Id.ToString());
                string PathString2 = System.IO.Path.Combine(OriginalDirectory.ToString(), "Countries\\" + Id.ToString() + "\\Thumbs");

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
                    CountryInformationDataTransferObject CountryInformationDataTransferObject = DataBase.CountriesInformation.Find(Id);
                    CountryInformationDataTransferObject.ImageName = ImageName;

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
            return RedirectToAction("EditСountry");
        }

        //Создаём метод отображения стран
        // GET:  Admin/Sections/Сountries
        [HttpGet]
        public ActionResult Сountries(int? Page)
        {
            // Объявляем лист CountryInformationViewModel
            List<CountryInformationViewModel> ListCountryInformationViewModel;

            // Устанавливаем номер страницы
            int PageNumber = Page ?? 1;

            using (DataBase DataBase = new DataBase())
            {
                // Инициализируем лист
                ListCountryInformationViewModel = DataBase.CountriesInformation.ToArray()
                           .Select(x => new CountryInformationViewModel(x))
                           .ToList();
            }

            // Устанавливаем постраничную навигацию
            IPagedList<CountryInformationViewModel> onePageOfCountryInformationViewModel = ListCountryInformationViewModel.ToPagedList(PageNumber, 3);
            ViewBag.OnePageOfCountryInfo = onePageOfCountryInformationViewModel;

            // Возвращаем представление и лист
            return View(ListCountryInformationViewModel);
        }

        //Создаём метод добавления региона
        // POST:  Admin/Sections/AddRegionInformation/
        [HttpGet]
        public ActionResult AddRegion(int? CountryId)
        {
            // Объявляем модель
            RegionInformationViewModel Model = new RegionInformationViewModel();
            // Добавляем список выбора регионов в модель
            using (DataBase DataBase = new DataBase())
            {
                Model.Region = new SelectList(DataBase.Regions.Where(x => x.CountryId == CountryId).ToList(), "Id", "RegionName");
                ViewBag.Region = new SelectList(DataBase.Regions.ToList(), "Id", "RegionName");
                ViewBag.CountryId = CountryId;
            }

            // Возвращаем модель в представление
            return View(Model);
        }

        //Создаём метод добавления региона
        // POST:  Admin/Sections/AddRegionInformation/Model/File
        [HttpPost]
        public ActionResult AddRegion(RegionInformationViewModel Model, HttpPostedFileBase File)
        {
            int Id;
            int CountryId;
            // Проверяем модель на валидность
            if (!ModelState.IsValid)
            {
                using (DataBase DataBase = new DataBase())
                {
                    Model.Region = new SelectList(DataBase.Regions.Where(x => x.CountryId == Model.CountryId).ToList(), "Id", "RegionName");
                    ViewBag.Region = new SelectList(DataBase.Regions.ToList(), "Id", "RegionName");
                    return View(Model);
                }
            }

            // Инициализируем и сохраняем в базу модель
            using (DataBase DataBase = new DataBase())
            {
                Model.Region = new SelectList(DataBase.Regions.Where(x => x.CountryId == Model.CountryId).ToList(), "Id", "RegionName");

                RegionInformationDataTransferObject RegionInformationDataTransferObject = new RegionInformationDataTransferObject();

                RegionDataTransferObject RegionDataTransferObject = DataBase.Regions.Find(Model.RegionInformationId);

                RegionInformationDataTransferObject.Region = RegionDataTransferObject;

                RegionInformationDataTransferObject.Slug = RegionDataTransferObject.RegionName.Replace(@"""", "").Replace(",", "").Replace("'", "").Replace("?", "").Replace("!", "").Replace("/", "").Replace(".", "").Replace(" ", "-").ToLower();
                RegionInformationDataTransferObject.Description = Model.Description;
                RegionInformationDataTransferObject.RegionId = Model.RegionInformationId;
                RegionInformationDataTransferObject.CountryId = RegionDataTransferObject.CountryId;
                RegionInformationDataTransferObject.View = Model.View;

                _ = DataBase.RegionsInformation.Add(RegionInformationDataTransferObject);
                _ = DataBase.SaveChanges();

                // Получаем введённый ID
                Id = RegionInformationDataTransferObject.RegionId;

                CountryId = RegionDataTransferObject.CountryId;

            }

            // Добавляем сообщение в TempData
            TempData["SM"] = "Вы добавили регион!";

            #region Upload Image

            // Создаём необходимые директории
            DirectoryInfo OriginalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));

            string PathString1 = Path.Combine(OriginalDirectory.ToString(), "Regions");
            string PathString2 = Path.Combine(OriginalDirectory.ToString(), "Regions\\" + Id.ToString());
            string PathString3 = Path.Combine(OriginalDirectory.ToString(), "Regions\\" + Id.ToString() + "\\Thumbs");
            string PathString4 = Path.Combine(OriginalDirectory.ToString(), "Regions\\" + Id.ToString() + "\\Gallery");
            string PathString5 = Path.Combine(OriginalDirectory.ToString(), "Regions\\" + Id.ToString() + "\\Gallery\\Thumbs");

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
                        Model.Region = new SelectList(DataBase.Regions.Where(x => x.CountryId == Model.CountryId).ToList(), "Id", "RegionName");
                        ModelState.AddModelError("", "Изображение не было загружено - неправильное расширение изображения");
                        return View(Model);
                    }
                }

                // Объявляем переменную имени изображения
                string ImageName = File.FileName;

                // Сохраняем имя изображения в RegionInformationDataTransferObject
                using (DataBase DataBase = new DataBase())
                {
                    RegionInformationDataTransferObject RegionInformationDataTransferObject = DataBase.RegionsInformation.Find(Id);
                    RegionInformationDataTransferObject.ImageName = ImageName;

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
            return RedirectToAction("AddRegion", new { CountryId });
        }

        //Создаём метод редактирования региона
        // GET:  Admin/Sections/EditRegion/Id
        [HttpGet]
        public ActionResult EditRegion(int Id)
        {
            // Объявляем модель
            RegionInformationViewModel Model;

            using (DataBase DataBase = new DataBase())
            {
                // Получаем регион
                RegionInformationDataTransferObject RegionInformationDataTransferObject = DataBase.RegionsInformation.Find(Id);

                // Проверяем, доступен ли регион
                if (RegionInformationDataTransferObject == null)
                {
                    return Content("Этого региона не существует.");
                }

                // Инициализируем модель данными
                Model = new RegionInformationViewModel(RegionInformationDataTransferObject);

                Model.Region = new SelectList(DataBase.Regions.Where(x => x.CountryId == Model.CountryId).ToList(), "Id", "RegionName");

                ViewBag.Region = new SelectList(DataBase.Regions.ToList(), "Id", "RegionName");

                // Получаем все изображения из галереи
                Model.GalleryImages = Directory
                    .EnumerateFiles(Server.MapPath("~/Images/Uploads/Services/" + Id + "/Gallery/Thumbs"))
                    .Select(fn => Path.GetFileName(fn));

                ViewBag.CountryId = Model.CountryId;
            }

            // Возвращаем модель в представление
            return View(Model);
        }

        // Создаём метод редактирования региона
        // POST:  Admin/Sections/EditRegion
        [HttpPost]
        public ActionResult EditRegion(RegionInformationViewModel Model, HttpPostedFileBase File)
        {
            // Получаем ID региона
            int Id = Model.RegionInformationId;

            // Заполняем список региона
            using (DataBase DataBase = new DataBase())
            {
                Model.Region = new SelectList(DataBase.Regions.Where(x => x.CountryId == Model.CountryId).ToList(), "Id", "RegionName");

                ViewBag.Region = new SelectList(DataBase.Regions.ToList(), "Id", "RegionName");
            }

            Model.GalleryImages = Directory
                .EnumerateFiles(Server.MapPath("~/Images/Uploads/Services/" + Id + "/Gallery/Thumbs"))
                .Select(fn => Path.GetFileName(fn));

            // Проверяем модель на валидность
            if (!ModelState.IsValid)
            {
                return View(Model);
            }

            // Обновляем регион
            using (DataBase DataBase = new DataBase())
            {
                Model.Region = new SelectList(DataBase.Regions.Where(x => x.CountryId == Model.CountryId).ToList(), "Id", "RegionName");

                RegionInformationDataTransferObject RegionInformationDataTransferObject = DataBase.RegionsInformation.Find(Id);

                RegionDataTransferObject RegionDataTransferObject = DataBase.Regions.Find(Model.RegionInformationId);
                RegionInformationDataTransferObject.Region = RegionDataTransferObject;
                RegionInformationDataTransferObject.Slug = RegionDataTransferObject.RegionName.Replace(@"""", "").Replace(",", "").Replace("'", "").Replace("?", "").Replace("!", "").Replace("/", "").Replace(".", "").Replace(" ", "-").ToLower();
                RegionInformationDataTransferObject.Description = Model.Description;
                RegionInformationDataTransferObject.RegionId = Model.RegionInformationId;
                RegionInformationDataTransferObject.CountryId = RegionDataTransferObject.CountryId;
                if (Model.ImageName != null)
                {
                    RegionInformationDataTransferObject.ImageName = Model.ImageName;
                }

                RegionInformationDataTransferObject.View = Model.View;

                _ = DataBase.SaveChanges();
            }

            // Устанавливаем сообщение в TempData
            TempData["SM"] = "Вы отредактировали регион!";

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

                string PathString1 = System.IO.Path.Combine(OriginalDirectory.ToString(), "Regions\\" + Id.ToString());
                string PathString2 = System.IO.Path.Combine(OriginalDirectory.ToString(), "Regions\\" + Id.ToString() + "\\Thumbs");

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
                    RegionInformationDataTransferObject RegionInformationDataTransferObject = DataBase.RegionsInformation.Find(Id);
                    RegionInformationDataTransferObject.ImageName = ImageName;

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
            return RedirectToAction("EditRegion", new { Id });
        }

        [HttpGet]
        public ActionResult Regions(int? Page, int? CountryId)
        {
            // Объявляем типа лист
            List<RegionInformationViewModel> ListRegionInformationViewModel;

            // Устанавливаем номер страницы
            int PageNumber = Page ?? 1;

            using (DataBase DataBase = new DataBase())
            {
                // Инициализируем лист
                ListRegionInformationViewModel = DataBase.RegionsInformation.ToArray()
                    .Where(x => CountryId == null || CountryId == 0 || x.CountryId == CountryId)
                           .Select(x => new RegionInformationViewModel(x))
                           .ToList();
            }

            // Устанавливаем постраничную навигацию
            IPagedList<RegionInformationViewModel> OnePageOfRegionInformationViewModel = ListRegionInformationViewModel.ToPagedList(PageNumber, 3);
            ViewBag.OnePageOfRegionInfo = OnePageOfRegionInformationViewModel;
            ViewBag.CountryId = CountryId;
            // Возвращаем представление и лист
            return View(ListRegionInformationViewModel);
        }

        public ActionResult DeleteCountry(int Id)
        {
            // Удаляем страну из базы данных
            using (DataBase DataBase = new DataBase())
            {
                CountryInformationDataTransferObject CountryInformationDataTransferObject = DataBase.CountriesInformation.Find(Id);
                _ = DataBase.CountriesInformation.Remove(CountryInformationDataTransferObject);
                _ = DataBase.SaveChanges();
            }
            // Удаляем директорию страны
            DirectoryInfo OriginalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));
            string PathString = Path.Combine(OriginalDirectory.ToString(), "Countries\\" + Id.ToString());

            try
            {
                if (Directory.Exists(PathString))
                {
                    Directory.Delete(PathString, true);
                }

                // Переадресовываем пользователя
                return RedirectToAction("Сountries");
            }
            catch
            {
                // Переадресовываем пользователя
                return RedirectToAction("Сountries");
            }
        }

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

                    string PathString1 = System.IO.Path.Combine(OriginalDirectory.ToString(), "Countries\\" + Id.ToString() + "\\Gallery");
                    string PathString2 = System.IO.Path.Combine(OriginalDirectory.ToString(), "Countries\\" + Id.ToString() + "\\Gallery\\Thumbs");

                    // Назначаем пути изображений
                    string Path = string.Format($"{PathString1}\\{File.FileName}");
                    string Path2 = string.Format($"{PathString2}\\{File.FileName}");

                    // Сохраняем оригинальные изображения и уменьшеные копии
                    File.SaveAs(Path);

                    WebImage img = new WebImage(File.InputStream);
                    _ = img.Resize(200, 200).Crop(1, 1);
                    _ = img.Save(Path2);
                }
            }
        }

        // Создаём метод удаления изображений страны из галереи
        // POST:  Admin/Sections/DeleteImage/Id
        public void DeleteImage(int Id, string ImageName)
        {
            string FullPath1 = Request.MapPath("~/Images/Uploads/Countries/" + Id.ToString() + "/Gallery/" + ImageName);
            string FullPath2 = Request.MapPath("~/Images/Uploads/Countries/" + Id.ToString() + "/Gallery/Thumbs/" + ImageName);

            if (System.IO.File.Exists(FullPath1))
            {
                System.IO.File.Delete(FullPath1);
            }

            if (System.IO.File.Exists(FullPath2))
            {
                System.IO.File.Delete(FullPath2);
            }
        }

        // Создаём метод сохранения изображений региона
        // POST:  Admin/Sections/SaveGalleryImagesRegion/Id
        [HttpPost]
        public void SaveGalleryImagesRegion(int Id)
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

                    string PathString1 = System.IO.Path.Combine(OriginalDirectory.ToString(), "Regions\\" + Id.ToString() + "\\Gallery");
                    string PathString2 = System.IO.Path.Combine(OriginalDirectory.ToString(), "Regions\\" + Id.ToString() + "\\Gallery\\Thumbs");

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

        // Создаём метод удаления изображений региона из галереи
        // POST:  Admin/Sections/DeleteImageRegion/Id
        public void DeleteImageRegion(int Id, string ImageName)
        {
            string FullPath1 = Request.MapPath("~/Images/Uploads/Regions/" + Id.ToString() + "/Gallery/" + ImageName);
            string FullPath2 = Request.MapPath("~/Images/Uploads/Regions/" + Id.ToString() + "/Gallery/Thumbs/" + ImageName);

            if (System.IO.File.Exists(FullPath1))
            {
                System.IO.File.Delete(FullPath1);
            }

            if (System.IO.File.Exists(FullPath2))
            {
                System.IO.File.Delete(FullPath2);
            }
        }

        public ActionResult DeleteRegion(int Id, int? CountryId)
        {
            // Удаляем регион из базы данных
            using (DataBase DataBase = new DataBase())
            {
                RegionInformationDataTransferObject RegionInformationDataTransferObject = DataBase.RegionsInformation.Find(Id);
                _ = DataBase.RegionsInformation.Remove(RegionInformationDataTransferObject);
                _ = DataBase.SaveChanges();
            }
            // Удаляем директорию региона
            DirectoryInfo OriginalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));
            string PathString = Path.Combine(OriginalDirectory.ToString(), "Regions\\" + Id.ToString());

            try
            {
                if (Directory.Exists(PathString))
                {
                    Directory.Delete(PathString, true);
                }

                // Переадресовываем пользователя
                return RedirectToAction("Regions", "Sections", new { CountryId });
            }
            catch
            {
                // Переадресовываем пользователя
               
                return RedirectToAction("Regions", "Sections", new { CountryId });
            }
        }
       

        [HttpGet]
        public ActionResult AddCity(int? RegionId)
        {
            // Объявляем модель
            CityInformationViewModel Model = new CityInformationViewModel();
            // Добавляем список выбора города в модель
            using (DataBase DataBase = new DataBase())
            {
                Model.City = new SelectList(DataBase.Cities.Where(x => x.RegionId == RegionId).ToList(), "Id", "CityName");
                ViewBag.City = new SelectList(DataBase.Cities.ToList(), "Id", "CityName");
                ViewBag.RegionId = RegionId;
            }

            // Возвращаем модель в представление
            return View(Model);
        }

        //Создаём метод добавления города
        // POST:  Admin/Sections/AddCity/Model/File
        [HttpPost]
        public ActionResult AddCity(CityInformationViewModel Model, HttpPostedFileBase File)
        {
            int Id;
            int RegionId;
            // Проверяем модель на валидность
            if (!ModelState.IsValid)
            {
                using (DataBase DataBase = new DataBase())
                {
                    Model.City = new SelectList(DataBase.Cities.Where(x => x.RegionId == Model.RegionId).ToList(), "Id", "CityName");
                    ViewBag.City = new SelectList(DataBase.Cities.ToList(), "Id", "CityName");

                    return View(Model);
                }
            }

            // Инициализируем и сохраняем в базу модель
            using (DataBase DataBase = new DataBase())
            {
                Model.City = new SelectList(DataBase.Cities.Where(x => x.RegionId == Model.RegionId).ToList(), "Id", "CityName");

                CityInformationDataTransferObject CityInformationDataTransferObject = new CityInformationDataTransferObject();

                CityDataTransferObject CityDataTransferObject = DataBase.Cities.Find(Model.CityId);

                CityInformationDataTransferObject.City = CityDataTransferObject;

                CityInformationDataTransferObject.Slug = CityDataTransferObject.CityName.Replace(@"""", "").Replace(",", "").Replace("'", "").Replace("?", "").Replace("!", "").Replace("/", "").Replace(".", "").Replace(" ", "-").ToLower();
                CityInformationDataTransferObject.Description = Model.Description;

                CityInformationDataTransferObject.RegionId = CityDataTransferObject.RegionId;
                CityInformationDataTransferObject.CityId = CityDataTransferObject.Id;
                CityInformationDataTransferObject.View = Model.View;
                CityInformationDataTransferObject.YearOfFoundation = Model.YearOfFoundation;
                CityInformationDataTransferObject.Population = Model.Population;
                CityInformationDataTransferObject.Square = Model.Square;
                _ = DataBase.CitiesInformation.Add(CityInformationDataTransferObject);
                _ = DataBase.SaveChanges();

                // Получаем введённый ID
                Id = CityInformationDataTransferObject.CityId;
                RegionId = CityInformationDataTransferObject.RegionId;
            }

            // Добавляем сообщение в TempData
            TempData["SM"] = "Вы добавили город!";

            #region Upload Image

            // Создаём необходимые директории
            DirectoryInfo OriginalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));

            string PathString1 = Path.Combine(OriginalDirectory.ToString(), "Cities");
            string PathString2 = Path.Combine(OriginalDirectory.ToString(), "Cities\\" + Id.ToString());
            string PathString3 = Path.Combine(OriginalDirectory.ToString(), "Cities\\" + Id.ToString() + "\\Thumbs");
            string PathString4 = Path.Combine(OriginalDirectory.ToString(), "Cities\\" + Id.ToString() + "\\Gallery");
            string PathString5 = Path.Combine(OriginalDirectory.ToString(), "Cities\\" + Id.ToString() + "\\Gallery\\Thumbs");

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
                        Model.City = new SelectList(DataBase.Cities.AsParallel().Where(x => x.RegionId == Model.RegionId).ToList(), "Id", "CityName");
                        ModelState.AddModelError("", "Изображение не было загружено - неправильное расширение изображения");
                        return View(Model);
                    }
                }

                // Объявляем переменную имени изображения
                string ImageName = File.FileName;

                // Сохраняем имя изображения в СityInformationDataTransferObject
                using (DataBase DataBase = new DataBase())
                {
                    CityInformationDataTransferObject CityInformationDataTransferObject = DataBase.CitiesInformation.Find(Id);
                    CityInformationDataTransferObject.ImageName = ImageName;

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
            return RedirectToAction("AddCity", new { RegionId });
        }

        //Создаём метод редактирования города
        // GET:  Admin/Sections/EditCity/Id
        [HttpGet]
        public ActionResult EditCity(int Id)
        {

            // Объявляем модель
            CityInformationViewModel Model;

            using (DataBase DataBase = new DataBase())
            {
                // Получаем город
                CityInformationDataTransferObject CityInformationDataTransferObject = DataBase.CitiesInformation.Find(Id);

                // Проверяем, доступен ли город
                if (CityInformationDataTransferObject == null)
                {
                    return Content("Этого города не существует.");
                }

                // Инициализируем модель данными
                Model = new CityInformationViewModel(CityInformationDataTransferObject);

                Model.City = new SelectList(DataBase.Cities.Where(x => x.RegionId == Model.RegionId).ToList(), "Id", "CityName");

                ViewBag.City = new SelectList(DataBase.Cities.ToList(), "Id", "CityName");

                // Получаем все изображения из галереи
                Model.GalleryImages = Directory
                    .EnumerateFiles(Server.MapPath("~/Images/Uploads/Cities/" + Id + "/Gallery/Thumbs"))
                    .Select(fn => Path.GetFileName(fn));

                ViewBag.RegionId = Model.RegionId;
            }

            // Возвращаем модель в представление
            return View(Model);
        }

        // Создаём метод редактирования города
        // POST:  Admin/Sections/EditCity
        [HttpPost]
        public ActionResult EditCity(CityInformationViewModel Model, HttpPostedFileBase File)
        {
            // Получаем ID города
            int Id = Model.CityId;

            // Заполняем список городов
            using (DataBase DataBase = new DataBase())
            {
                Model.City = new SelectList(DataBase.Cities.Where(x => x.RegionId == Model.RegionId).ToList(), "Id", "CityName");
                ViewBag.City = new SelectList(DataBase.Cities.ToList(), "Id", "CityName");
            }

            Model.GalleryImages = Directory
                .EnumerateFiles(Server.MapPath("~/Images/Uploads/Cities/" + Id + "/Gallery/Thumbs"))
                .Select(fn => Path.GetFileName(fn));

            // Проверяем модель на валидность
            if (!ModelState.IsValid)
            {
                return View(Model);
            }

            // Обновляем город
            using (DataBase DataBase = new DataBase())
            {
                Model.City = new SelectList(DataBase.Cities.Where(x => x.RegionId == Model.RegionId).ToList(), "Id", "CityName");

                CityInformationDataTransferObject CityInformationDataTransferObject = DataBase.CitiesInformation.Find(Id);

                CityDataTransferObject CityDataTransferObject = DataBase.Cities.Find(Model.CityId);
                CityInformationDataTransferObject.City = CityDataTransferObject;
                CityInformationDataTransferObject.Slug = CityDataTransferObject.CityName.Replace(@"""", "").Replace(",", "").Replace("'", "").Replace("?", "").Replace("!", "").Replace("/", "").Replace(".", "").Replace(" ", "-").ToLower();
                CityInformationDataTransferObject.Description = Model.Description;
                CityInformationDataTransferObject.CityId = Model.CityId;
                CityInformationDataTransferObject.RegionId = CityDataTransferObject.RegionId;
                if (Model.ImageName != null)
                {
                    CityInformationDataTransferObject.ImageName = Model.ImageName;
                }

                CityInformationDataTransferObject.View = Model.View;
                CityInformationDataTransferObject.YearOfFoundation = Model.YearOfFoundation;
                CityInformationDataTransferObject.Population = Model.Population;
                CityInformationDataTransferObject.Square = Model.Square;
                _ = DataBase.SaveChanges();

            }

            // Устанавливаем сообщение в TempData
            TempData["SM"] = "Вы отредактировали город!";

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

                string PathString1 = System.IO.Path.Combine(OriginalDirectory.ToString(), "Cities\\" + Id.ToString());
                string PathString2 = System.IO.Path.Combine(OriginalDirectory.ToString(), "Cities\\" + Id.ToString() + "\\Thumbs");

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
                    RegionInformationDataTransferObject RegionInformationDataTransferObject = DataBase.RegionsInformation.Find(Id);
                    RegionInformationDataTransferObject.ImageName = ImageName;

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
            return RedirectToAction("EditCity", new { Id });
        }

        [HttpGet]
        public ActionResult Cities(int? Page, int? RegionId)
        {
            // Объявляем типа лист
            List<CityInformationViewModel> ListCityInformationViewModel;

            // Устанавливаем номер страницы
            int PageNumber = Page ?? 1;

            using (DataBase DataBase = new DataBase())
            {
                // Инициализируем лист
                ListCityInformationViewModel = DataBase.CitiesInformation.ToArray()
                    .Where(x => RegionId == null || RegionId == 0 || x.RegionId == RegionId)
                           .Select(x => new CityInformationViewModel(x))
                           .ToList();
            }
            // Устанавливаем постраничную навигацию
            IPagedList<CityInformationViewModel> OnePageOfCityInformationViewModel = ListCityInformationViewModel.ToPagedList(PageNumber, 3);
            ViewBag.OnePageOfCityInfo = OnePageOfCityInformationViewModel;
            ViewBag.RegionId = RegionId;
            // Возвращаем представление и лист
            return View(ListCityInformationViewModel);
        }

        [HttpPost]
        public void SaveGalleryImagesCity(int Id)
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

                    string PathString1 = System.IO.Path.Combine(OriginalDirectory.ToString(), "Cities\\" + Id.ToString() + "\\Gallery");
                    string PathString2 = System.IO.Path.Combine(OriginalDirectory.ToString(), "Cities\\" + Id.ToString() + "\\Gallery\\Thumbs");

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
        // POST:  Admin/Sections/DeleteImageCity/Id
        public void DeleteImageCity(int Id, string ImageName)
        {
            string FullPath1 = Request.MapPath("~/Images/Uploads/Cities/" + Id.ToString() + "/Gallery/" + ImageName);
            string fullPath2 = Request.MapPath("~/Images/Uploads/Cities/" + Id.ToString() + "/Gallery/Thumbs/" + ImageName);

            if (System.IO.File.Exists(FullPath1))
            {
                System.IO.File.Delete(FullPath1);
            }

            if (System.IO.File.Exists(fullPath2))
            {
                System.IO.File.Delete(fullPath2);
            }
        }

        public ActionResult DeleteCity(int Id, int? RegionId)
        {
            // Удаляем город из базы данных
            using (DataBase DataBase = new DataBase())
            {
                CityInformationDataTransferObject CityInformationDataTransferObject = DataBase.CitiesInformation.Find(Id);
                _ = DataBase.CitiesInformation.Remove(CityInformationDataTransferObject);
                _ = DataBase.SaveChanges();
            }
            // Удаляем директорию города
            DirectoryInfo OriginalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));
            string PathString = Path.Combine(OriginalDirectory.ToString(), "Cities\\" + Id.ToString());

            try
            {
                if (Directory.Exists(PathString))
                {
                    Directory.Delete(PathString, true);
                }
                // Переадресовываем пользователя
                
                return RedirectToAction("Cities", "Sections", new { RegionId });
            }
            catch
            {
                // Переадресовываем пользователя
                
                return RedirectToAction("Cities", "Sections", new { RegionId });
            }
        }
        
               
           
        
    }
}