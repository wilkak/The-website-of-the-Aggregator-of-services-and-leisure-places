using PagedList;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using WebStoreMap.Models.Data;
using WebStoreMap.Models.ViewModels.Rating;

namespace WebStoreMap.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RatingController : Controller
    {
        // GET: Admin/Ratings
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult Ratings(int? Page)
        {
            // Объявляем RatingViewModel типа лист
            List<RatingViewModel> ListRatingViewModel;
            // Устанавливаем номер страницы
            int PageNumber = Page ?? 1;
            using (DataBase DataBase = new DataBase())
            {
                // Инициализируем лист
                ListRatingViewModel = DataBase.Ratings.ToArray()
                       .Select(x => new RatingViewModel(x))
                       .ToList();
            }
            // Устанавливаем постраничную навигацию
            IPagedList<RatingViewModel> onePageOfRating = ListRatingViewModel.ToPagedList(PageNumber, 7);
            ViewBag.OnePageOfRating = onePageOfRating;

            // Возвращаем представление и лист
            return View(ListRatingViewModel);
        }

        // Создаём метод удаления
        // POST: Admin
        [Authorize]
        public ActionResult DeleteRating(int Id)
        {
            // Удаляем из базы данных
            using (DataBase DataBase = new DataBase())
            {
                RatingDataTransferObject RatingDataTransferObject = DataBase.Ratings.Find(Id);

                _ = DataBase.Ratings.Remove(RatingDataTransferObject);
                _ = DataBase.SaveChanges();
            }

            // Удаляем директорию отзыва
            DirectoryInfo OriginalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));
            string PathString = Path.Combine(OriginalDirectory.ToString(), "Reviews\\" + Id.ToString());
            try
            {
                if (Directory.Exists(PathString))
                {
                    Directory.Delete(PathString, true);
                }
                // Переадресовываем пользователя
                return RedirectToAction("Ratings");
            }
            catch
            {
                // Переадресовываем пользователя
                return RedirectToAction("Ratings");
            }
        }
    }
}