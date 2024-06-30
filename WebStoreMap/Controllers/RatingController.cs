using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using WebStoreMap.Models.Data;
using WebStoreMap.Models.ViewModels.Rating;

namespace WebStoreMap.Controllers
{
    public class RatingController : Controller
    {
        // GET: Ratings
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult PlaceReviews(int? Page, int? PlaceId)
        {
            int PageNumber = Page ?? 1;
            // Объявляем PlaceViewModel типа лист
            List<RatingViewModel> ListRatingViewModel;

            RatingViewModel Model = new RatingViewModel();

            using (DataBase DataBase = new DataBase())
            {
                // Инициализируем лист
                ListRatingViewModel = DataBase.Ratings.ToArray()
                    .Where(x => x.PlaceId == PlaceId)
                       .Select(x => new RatingViewModel(x))
                       .ToList();
                PlaceDataTransferObject Place = DataBase.Places.FirstOrDefault(x => x.Id == PlaceId);
               
                ViewBag.PlaceSlug = Place.Slug;

                double PlaceRating = 0;
                int NumberOfReviews = 0;

                if (ListRatingViewModel != null)
                {
                    foreach (var Review in ListRatingViewModel)
                    {
                        NumberOfReviews++;
                        PlaceRating = PlaceRating + Review.Rating;
                    }

                    double RatingOfPlace = PlaceRating / NumberOfReviews;
                    ViewBag.NumberOfReviews = NumberOfReviews;

                    if (RatingOfPlace == 5)
                    {
                        ViewBag.Rating = "ОТЛИЧНО";
                    }
                    else if (RatingOfPlace == 4)
                    {
                        ViewBag.Rating = "ХОРОШО";
                    }
                    else if (RatingOfPlace == 3)
                    {
                        ViewBag.Rating = "НОРМАЛЬНО";
                    }
                    else if (RatingOfPlace == 2)
                    {
                        ViewBag.Rating = "ПЛОХО";
                    }
                    else if (RatingOfPlace == 1)
                    {
                        ViewBag.Rating = "УЖАСНО";
                    }
                    ViewBag.RatingOfPlace = RatingOfPlace;
                    ViewBag.NumberOfReviews = NumberOfReviews;
                }
                else
                {
                    ViewBag.RatingOfPlace = 0;
                    ViewBag.Rating = "НЕТ ОТЗЫВОВ";
                }
            }

            ViewBag.PlaceId = PlaceId;

            // Возвращаем представление и лист
            IPagedList<RatingViewModel> OnePageOfRatinglist = ListRatingViewModel.ToPagedList(PageNumber, 7);
            ViewBag.OnePageOfRatinglist = OnePageOfRatinglist;

            return View("PlaceReviews", Model);
        }

        //[HttpGet]
        //[Authorize]
        //public ActionResult AddReview(int PlaceId)
        //{
        //    string UserEmail = User.Identity.Name;

        //    // Объявляем модель
        //    RatingViewModel Model = new RatingViewModel
        //    {
        //        PlaceId = PlaceId
        //    };
        //    ViewBag.PlaceId = PlaceId;

        //    Model.Date = DateTime.Now;

        //    // Добавляем список выбора категорий в модель
        //    using (DataBase db = new DataBase())
        //    {
        //        UserDataTransferObject userdto = db.Users.AsParallel().FirstOrDefault(x => x.EmailAddress == UserEmail);

        //        Model.UserId = userdto.Id;
        //    }

        //    // Возвращаем модель в представление
        //    return View("AddReview", Model);
        //}

        [HttpPost]
        [Authorize]
        public ActionResult AddReview(RatingViewModel Model, HttpPostedFileBase File)
        {
            int Id;
            int? PlaceId = Model.PlaceId;
            string UserEmail = User.Identity.Name;
            if (string.IsNullOrEmpty(UserEmail))
            {
                return RedirectToAction("Login", "Account");
            }
            // Инициализируем и сохраняем в базу модель DTO
            using (DataBase DataBase = new DataBase())
            {
                UserDataTransferObject userdto = DataBase.Users.FirstOrDefault(x => x.EmailAddress == UserEmail);
                RatingDataTransferObject Ratingsdto = DataBase.Ratings.FirstOrDefault(x => x.UserId == userdto.Id && x.PlaceId == Model.PlaceId);
                if (Ratingsdto != null)
                {
                    return RedirectToAction("PlaceReviews", "Rating", new { PlaceId });

                }
                RatingDataTransferObject Review = new RatingDataTransferObject();
                if (Model.Text == null)
                {
                    return RedirectToAction("PlaceReviews", "Rating", new { PlaceId });

                }
                Review.Text = Model.Text;
                Review.Rating = Model.Rating;
                Review.Date = DateTime.Now;
                Review.UserId = userdto.Id;
                Review.PlaceId = Model.PlaceId;

                _ = DataBase.Ratings.Add(Review);
                _ = DataBase.SaveChanges();

                // Получаем введённый ID
                Id = Review.Id;
                RatingDataTransferObject dto = DataBase.Ratings.Find(Id);

                int IdPol;

                IdPol = dto.Id;


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

                        ModelState.AddModelError("", "Изображение не было загружено - неправильное расширение изображения");

                        return RedirectToAction("PlaceReviews", "Ratings", new { PlaceId });

                    }

                    // Устанавливаем пути загрузки
                    DirectoryInfo OriginalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));


                    // Создаём необходимые директории

                    string pathString1 = Path.Combine(OriginalDirectory.ToString(), "Feedback");
                    string pathString2 = Path.Combine(OriginalDirectory.ToString(), "Feedback\\" + IdPol.ToString());
                    string pathString3 = Path.Combine(OriginalDirectory.ToString(), "Feedback\\" + IdPol.ToString() + "\\Thumbs");

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


                    // Сохраняем имя изображения
                    string ImageName = File.FileName;
                    RatingDataTransferObject dtom = DataBase.Ratings.Find(IdPol);
                    dtom.ImageName = ImageName;

                    _ = DataBase.SaveChanges();


                    // Сохраняем оригинал и превью версии изображений
                    string path = string.Format($"{pathString2}\\{ImageName}");
                    string path2 = string.Format($"{pathString3}\\{ImageName}");

                    // Сохраняем оригинальное изображение
                    File.SaveAs(path);

                    // Создаём и сохраняем уменьшенную копию
                    WebImage img = new WebImage(File.InputStream);
                    _ = img.Resize(200, 200).Crop(1, 1);
                    _ = img.Save(path2);
                }

                #endregion Image Upload
            }
            return RedirectToAction("PlaceReviews", "Rating", new { PlaceId });
        }
    }
}