using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebStoreMap.Models.Data;
using WebStoreMap.Models.ViewModels.Favorites;

namespace WebStoreMap.Controllers
{
    public class FavoritesController : Controller
    {
        // GET: Desires
        [Authorize]
        public ActionResult Index()
        {
            List<FavoritesViewModel> FavoritesViewModelList;
            List<FavoritesViewModel> FavoritesViewModelList2 = new List<FavoritesViewModel>();
            decimal FinalPrice = 0;
            string UserEmail = User.Identity.Name;
            using (DataBase DataBase = new DataBase())
            {
                UserDataTransferObject userdto = DataBase.Users.FirstOrDefault(x => x.EmailAddress == UserEmail);
                FavoritesViewModelList = DataBase.Favorites.ToArray().Where(x => x.UserId == userdto.Id).Select(x => new FavoritesViewModel(x)).ToList();

                foreach (FavoritesViewModel i in FavoritesViewModelList)
                {
                    PlaceDataTransferObject Place = DataBase.Places.FirstOrDefault(x => x.Id == i.PlaceId);
                    FavoritesViewModel DES = new FavoritesViewModel(i.Id, Place.Id, Place.PlaceName, i.Quantity, Place.Price, Place.ImageName, Place.Slug, userdto.Id);
                    FavoritesViewModelList2.Add(DES);
                    FinalPrice += DES.Price;
                }
                ViewBag.FinalPrice = FinalPrice;
                ViewBag.NumberOfPlaces = FavoritesViewModelList2.Count();
            }
            return View(FavoritesViewModelList2);
        }

        [Authorize]
        public ActionResult FavoritesPartial()
        {
            // Объявляем DesiredViewModel
            FavoritesViewModel Model = new FavoritesViewModel();

            // Объявляем количество
            int Qty = 0;

            // Объявляем цену
            decimal Price = 0m;
            string UserEmail = User.Identity.Name;
            using (DataBase DataBase = new DataBase())
            {
                UserDataTransferObject userdto = DataBase.Users.FirstOrDefault(x => x.EmailAddress == UserEmail);

                FavoriteDataTransferObject PlaceInCart = DataBase.Favorites.FirstOrDefault(x => x.UserId == userdto.Id);

                // Проверяем сессию корзины
                if (PlaceInCart != null)
                {
                    List<FavoritesViewModel> FavoritesViewModelList;
                    FavoritesViewModelList = DataBase.Favorites.ToArray().Where(x => x.UserId == userdto.Id).Select(x => new FavoritesViewModel(x)).ToList();

                    foreach (FavoritesViewModel item in FavoritesViewModelList)
                    {
                        Qty += item.Quantity;
                        Price += item.Quantity * item.Price;
                    }

                    Model.Quantity = Qty;
                    Model.Price = Price;
                    //*********************
                }
                else
                {
                    // Или устанавливаем количество и цену на 0
                    Model.Quantity = 0;
                    Model.Price = 0m;
                }

                // Возвращаем частичное представление с моделью
                return PartialView("_FavoritesPartial", Model);
            }
        }

        [Authorize]
        public ActionResult AddToFavoritesPartial(int Id)
        {
            string UserEmail = User.Identity.Name;
            FavoritesViewModel Model = new FavoritesViewModel();
            using (DataBase DataBase = new DataBase())
            {
                UserDataTransferObject userdto = DataBase.Users.FirstOrDefault(x => x.EmailAddress == UserEmail);
                FavoriteDataTransferObject PlaceInCart = DataBase.Favorites.FirstOrDefault(x => x.UserId == userdto.Id && x.PlaceId == Id);

                if (PlaceInCart == null)
                {
                    FavoriteDataTransferObject newPlaceInCard = new FavoriteDataTransferObject
                    {
                        PlaceId = Id,
                        UserId = userdto.Id,
                        Quantity = 1
                    };
                    _ = DataBase.Favorites.Add(newPlaceInCard);
                    _ = DataBase.SaveChanges();
                }
                else
                {
                    FavoriteDataTransferObject OldPlaceInCard = DataBase.Favorites.FirstOrDefault(x => x.UserId == userdto.Id && x.PlaceId == Id);
                    _ = DataBase.Favorites.Remove(OldPlaceInCard);
                    _ = DataBase.SaveChanges();
                }

                List<FavoritesViewModel> FavoritesViewModelList;
                List<FavoritesViewModel> FavoritesViewModelList2 = new List<FavoritesViewModel>();
                FavoritesViewModelList = DataBase.Favorites.ToArray().Where(x => x.UserId == userdto.Id).Select(x => new FavoritesViewModel(x)).ToList();

                foreach (FavoritesViewModel i in FavoritesViewModelList)
                {
                    PlaceDataTransferObject Place = DataBase.Places.FirstOrDefault(x => x.Id == i.PlaceId);
                    FavoritesViewModel DES = new FavoritesViewModel(i.Id, Place.Id, Place.PlaceName, i.Quantity, Place.Price, Place.ImageName, Place.Slug, userdto.Id);
                    FavoritesViewModelList2.Add(DES);
                }

                int Qty = 0;
                decimal Price = 0m;

                foreach (FavoritesViewModel Item in FavoritesViewModelList2)
                {
                    Qty += Item.Quantity;
                    Price += Item.Quantity * Item.Price;
                }

                Model.Quantity = Qty;
                Model.Price = Price;

                return PartialView("_AddToFavoritesPartial", Model);
            }
        }

       

        // GET: /Desires/IncrementPlace
        [Authorize]
        public JsonResult IncrementPlace(int PlaceId)
        {
            // Объявляем лист desired

            string UserEmail = User.Identity.Name;
            using (DataBase DataBase = new DataBase())
            {
                UserDataTransferObject userdto = DataBase.Users.FirstOrDefault(x => x.EmailAddress == UserEmail);

                FavoriteDataTransferObject OldPlaceInCard = DataBase.Favorites.FirstOrDefault(x => x.UserId == userdto.Id && x.PlaceId == PlaceId);
                OldPlaceInCard.Quantity++;
                _ = DataBase.SaveChanges();

                List<FavoritesViewModel> Favorites = DataBase.Favorites.ToArray().Where(x => x.UserId == userdto.Id).Select(x => new FavoritesViewModel(x)).ToList();
                // Получаем DesiredViewModel из листа
                FavoritesViewModel Model = Favorites.FirstOrDefault(x => x.PlaceId == PlaceId);

             
                // Сохраняем необходимые данные
                var Result = new { qty = Model.Quantity, price = Model.Price };

                // Возвращаем json ответ с данными
                return Json(Result, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: /Desires/DecrementPlace
        [Authorize]
        public ActionResult DecrementPlace(int PlaceId)
        {
            // Объявляем лист desired
            string UserEmail = User.Identity.Name;
            using (DataBase DataBase = new DataBase())
            {
                UserDataTransferObject userdto = DataBase.Users.FirstOrDefault(x => x.EmailAddress == UserEmail);

                FavoriteDataTransferObject OldPlaceInCard = DataBase.Favorites.FirstOrDefault(x => x.UserId == userdto.Id && x.PlaceId == PlaceId);
                if (OldPlaceInCard.Quantity > 1)
                {
                    OldPlaceInCard.Quantity--;
                    _ = DataBase.SaveChanges();
                    List<FavoritesViewModel> Favorites = DataBase.Favorites.ToArray().Where(x => x.UserId == userdto.Id).Select(x => new FavoritesViewModel(x)).ToList();
                    FavoritesViewModel Model = Favorites.FirstOrDefault(x => x.PlaceId == PlaceId);
                    var Result = new { qty = Model.Quantity, price = Model.Price };
                    return Json(Result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    OldPlaceInCard.Quantity = 0;
                    _ = DataBase.Favorites.Remove(OldPlaceInCard);
                    _ = DataBase.SaveChanges();
                   
                    var Result = new { qty = 0, price = 0 };
                    return Json(Result, JsonRequestBehavior.AllowGet);
                }
            }
        }

        // GET: /Desires/RemovePlace
        public ActionResult RemovePlace(int PlaceId)
        {
            string UserEmail = User.Identity.Name;
            List<FavoritesViewModel> FavoritesViewModelList;
            List<FavoritesViewModel> FavoritesViewModelList2 = new List<FavoritesViewModel>();
            using (DataBase DataBase = new DataBase())
            {
                UserDataTransferObject userdto = DataBase.Users.FirstOrDefault(x => x.EmailAddress == UserEmail);

                FavoriteDataTransferObject OldPlaceInCard = DataBase.Favorites.FirstOrDefault(x => x.UserId == userdto.Id && x.PlaceId == PlaceId);
                _ = DataBase.Favorites.Remove(OldPlaceInCard);
                _ = DataBase.SaveChanges();

                FavoritesViewModelList = DataBase.Favorites.ToArray().Where(x => x.UserId == userdto.Id).Select(x => new FavoritesViewModel(x)).ToList();
                foreach (FavoritesViewModel i in FavoritesViewModelList)
                {
                    PlaceDataTransferObject Place = DataBase.Places.FirstOrDefault(x => x.Id == i.PlaceId);
                    FavoritesViewModel DES = new FavoritesViewModel(i.Id, Place.Id, Place.PlaceName, i.Quantity, Place.Price, Place.ImageName, Place.Slug, userdto.Id);
                    FavoritesViewModelList2.Add(DES);
                }

                ViewBag.FavoritesViewModelList = FavoritesViewModelList2;

                return PartialView("RemovePlace");
            }
          
        }
    }
}