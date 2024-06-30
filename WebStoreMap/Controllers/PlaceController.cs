using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using WebStoreMap.Models.Data;
using WebStoreMap.Models.ViewModels.Chat;
using WebStoreMap.Models.ViewModels.Rating;
using WebStoreMap.Models.ViewModels.Place;

namespace WebStoreMap.Controllers
{
    public class PlaceController : Controller
    {
        // GET: Place
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Pages");
        }

        public ActionResult CategoryMenuPartial()
        {
            // Объявляем модель типа лист CategoryViewModel
            List<CategoryViewModel> CategoryViewModelList;

            // Инициализируем модель данными
            using (DataBase DataBase = new DataBase())
            {
                CategoryViewModelList = DataBase.Categories.ToArray().OrderBy(x => x.Sorting).Select(x => new CategoryViewModel(x)).ToList();
            }

            // Возвращаем частичное представление с моделью
            return PartialView("_CategoryMenuPartial", CategoryViewModelList);
        }

        // GET: Place/category/Name
        [HttpGet]
        public ActionResult Category(string Name)
        {
            // Объявляем список типа List<PlaceViewModel>
            List<PlaceViewModel> PlaceViewModelList;

            using (DataBase DataBase = new DataBase())
            {
                // Получаем ID категории
                CategoryDataTransferObject CategoryDTO = DataBase.Categories.Where(x => x.Slug == Name).FirstOrDefault();
                int CategoryId = CategoryDTO.Id;
                // инициализируем список данными
                PlaceViewModelList = DataBase.Places.ToArray().Where(x => x.CategoryId == CategoryId && x.View).Select(x => new PlaceViewModel(x)).ToList();

                // Получаем имя категории
                PlaceDataTransferObject PlaceCategory = DataBase.Places.Where(x => x.CategoryId == CategoryId && x.View).FirstOrDefault();

                // Делаем проверку категории на NULL
                if (PlaceCategory == null)
                {
                    string CategoryName = DataBase.Categories.Where(x => x.Slug == Name).Select(x => x.CategoryName).FirstOrDefault().ToString();
                    ViewBag.CategoryName = CategoryName;
                }
                else
                {
                    ViewBag.CategoryName = PlaceCategory.CategoryName;
                }
            }
            // Возвращаем представление с моделью
            return View(PlaceViewModelList);
        }

        // GET: Place/Place-details/Name
        [HttpGet]
        // Добавляем другое имя контроллера
        [ActionName("Place-details")]
        public ActionResult PlaceDetails(string Name)
        {
            if (Name == null)
            {
                return HttpNotFound();
            }
            string UserEmail = User.Identity.Name;

           
            List<CommentPlaceViewModel> CommentarViewModelList;
            PlaceViewModel Model;
            PlaceDataTransferObject Dto;
            CompanyDataTransferObject Comdto;
            List<RatingViewModel> ListRatingViewModel;
            List<ServiceViewModel> ListServiceViewModel;

            // Инициализируем ID продукта
            int Id = 0;

            using (DataBase DataBase = new DataBase())
            {
                // Проверяем, доступен ли продукт
                if (!DataBase.Places.Any(x => x.Slug.Equals(Name)))
                {
                    return RedirectToAction("Index", "Place");
                }

                // Инициализируем модель productDTO
                Dto = DataBase.Places.Where(x => x.Slug == Name).FirstOrDefault();
                Comdto = DataBase.Companies.Where(x => x.Id == Dto.CompanyId).FirstOrDefault();

                ViewBag.SlugVK = Comdto.SlugVK;
                ViewBag.SlugTelegram = Comdto.SlugTelegram;
                ViewBag.SlugWhatsapp = Comdto.SlugWhatsapp;
               

                // Получаем ID
                Id = Dto.Id;

                if(Dto.Schedule != null)
                {
                    ViewBag.TourPlan = Dto.Schedule.Split('☹').ToList<string>();
                }

                CommentarViewModelList = DataBase.Comments.Where(x => x.PlaceId == Id).Include(x => x.Replies).ToArray().OrderByDescending(x => x.Date).AsEnumerable().Select(x => new CommentPlaceViewModel(x)).ToList();

                // Инициализируем модель данными
                
                Model = new PlaceViewModel(Dto.Id, Dto.PlaceName, Dto.Slug, Dto.Description, Dto.Price, Dto.CategoryName, Dto.CategoryId,
            Dto.ImageName, Dto.Latitude, Dto.Longitude, Dto.UserId, Dto.CityName, Dto.CityId, Dto.RegionName, Dto.RegionId, Dto.CountryName, Dto.CountryId,
            Dto.OldPrice, Dto.View, Dto.Address, Dto.PhoneNumber, Comdto.CompanyName, Dto.CompanyId, Comdto.Slug, Dto.FirstDay, Dto.LastDay, Dto.Schedule);

                ListRatingViewModel = DataBase.Ratings.ToArray()
                    .Where(x => x.PlaceId == Id)
                    .Select(x => new RatingViewModel(x))
                    .ToList();

                double PlaceRating = 0;
                int NumberOfReviews = 0;

                if (ListRatingViewModel != null)
                {
                    foreach (RatingViewModel Rating in ListRatingViewModel)
                    {
                        NumberOfReviews++;
                        PlaceRating += Rating.Rating;
                    }

                    ViewBag.RatingOfPlace = PlaceRating / NumberOfReviews;
                    ViewBag.NumberOfReviews = NumberOfReviews;
                }
                else
                {
                    ViewBag.RatingOfPlace = 0;
                    ViewBag.NumberOfReviews = 0;
                }

                ListServiceViewModel = DataBase.Services.ToArray()
                    .Where(x => x.PlaceId == Id)
                    .Select(x => new ServiceViewModel(x))
                    .ToList();

                ViewBag.ListService = ListServiceViewModel;

                PlaceCommentViewModel PlaceCommentViewModel = new PlaceCommentViewModel(CommentarViewModelList, Model);
                if (string.IsNullOrEmpty(UserEmail))
                {
                    ViewBag.Image = "~/Content/img/avatar.png";
                }
          
                else
                {
                    string UserImage = DataBase.Users.Single(x => x.EmailAddress == UserEmail).ImageName;
                    int UserId = DataBase.Users.Single(x => x.EmailAddress == UserEmail).Id;
                    ViewBag.Image = !string.IsNullOrEmpty(UserImage) ? "~/Images/Uploads/Profile/" + UserId + "/Thumbs/" + UserImage : "~/Content/img/avatar.png";
                }
                // Получаем изображения из галереи
                try
                {
                    Model.GalleryImages = Directory
                        .EnumerateFiles(Server.MapPath("~/Images/Uploads/Places/" + Id + "/Gallery/Thumbs"))
                        .Select(fn => Path.GetFileName(fn));

                }
                catch
                {

                }
                // Возвращаем модель и представление
                return View("PlaceDetails", PlaceCommentViewModel);
            }
        }

        [HttpGet]
        [Authorize]
        public ActionResult PostReply(ReplyViewModel Model, string Name)
        {
            using (DataBase DataBase = new DataBase())
            {
                string UserEmail = User.Identity.Name;
                if (string.IsNullOrEmpty(UserEmail))
                {
                    return RedirectToAction("Login", "Account");
                }

                if (!ModelState.IsValid)
                {
                    return View("Login", "Account");
                }
             
                ReplyDataTransferObject r = new ReplyDataTransferObject
                {
                    Text = Model.Reply,
                    CommentId = Model.CID,
                    Date = DateTime.Now
                };

                int UserId = DataBase.Users.Single(x => x.EmailAddress == UserEmail).Id;
                r.UserId = UserId;
                if (Model.Reply != null)
                {
                    _ = DataBase.Replies.Add(r);
                    _ = DataBase.SaveChanges();
                }
               
                return RedirectToAction("Place-details", "Place", new { Name });

            }
        }

        [HttpGet]
        [Authorize]
        public ActionResult PostComment(string CommentText, int Id, string Name)
        {
            string UserEmail = User.Identity.Name;
            if (string.IsNullOrEmpty(UserEmail))
            {
                return RedirectToAction("Login", "Account");
            }
            if (!ModelState.IsValid)
            {
                
                return View("Login", "Account");
            }
            CommentDataTransferObject Comment = new CommentDataTransferObject
            {
                Text = CommentText,
                Date = DateTime.Now,
                PlaceId = Id
            };
            
            using (DataBase DataBase = new DataBase())
            {
                int UserId = DataBase.Users.Single(x => x.EmailAddress == UserEmail).Id;
                Comment.UserId = UserId;
                if (CommentText != "")
                {
                    _ = DataBase.Comments.Add(Comment);
                    _ = DataBase.SaveChanges();
                }
               
                return RedirectToAction("Place-details", "Place", new { Name });

            }
        }

    }
}