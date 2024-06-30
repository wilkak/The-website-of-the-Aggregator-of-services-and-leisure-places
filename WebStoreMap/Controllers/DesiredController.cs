using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebStoreMap.Models.Data;
using WebStoreMap.Models.ViewModels.Desired;

namespace WebStoreMap.Controllers
{
    public class DesiredController : Controller
    {
        // GET: Desires
        [Authorize]
        public ActionResult Index()
        {
            List<DesiredViewModel> DesiredViewModelList;
            List<DesiredViewModel> DesiredViewModelList2 = new List<DesiredViewModel>();
            string UserEmail = User.Identity.Name;
            using (DataBase DataBase = new DataBase())
            {
                UserDataTransferObject userdto = DataBase.Users.FirstOrDefault(x => x.EmailAddress == UserEmail);
                DesiredViewModelList = DataBase.Desires.ToArray().Where(x => x.UserId == userdto.Id).Select(x => new DesiredViewModel(x)).ToList();

                foreach (DesiredViewModel i in DesiredViewModelList)
                {
                    PlaceDataTransferObject Place = DataBase.Places.FirstOrDefault(x => x.Id == i.PlaceId);
                    DesiredViewModel DES = new DesiredViewModel(i.Id, Place.Id, Place.PlaceName, i.Quantity, Place.Price, Place.ImageName, Place.Slug, userdto.Id);
                    DesiredViewModelList2.Add(DES);
                }
            }
            return View(DesiredViewModelList2);
        }

        [Authorize]
        public ActionResult CartPartial()
        {
            // Объявляем DesiredViewModel
            DesiredViewModel Model = new DesiredViewModel();

            // Объявляем количество
            int Qty = 0;

            // Объявляем цену
            decimal Price = 0m;
            string UserEmail = User.Identity.Name;
            using (DataBase DataBase = new DataBase())
            {
                UserDataTransferObject userdto = DataBase.Users.FirstOrDefault(x => x.EmailAddress == UserEmail);

                DesiredDataTransferObject PlaceInCart = DataBase.Desires.FirstOrDefault(x => x.UserId == userdto.Id);

                // Проверяем сессию корзины
                if (PlaceInCart != null)
                {
                    List<DesiredViewModel> DesiredViewModelList;
                    DesiredViewModelList = DataBase.Desires.ToArray().Where(x => x.UserId == userdto.Id).Select(x => new DesiredViewModel(x)).ToList();

                    foreach (DesiredViewModel item in DesiredViewModelList)
                    {
                        Qty += item.Quantity;
                        Price += item.Quantity * item.Price;
                    }

                    Model.Quantity = Qty;
                    Model.Price = Price;
                    
                }
                else
                {
                    // Или устанавливаем количество и цену на 0
                    Model.Quantity = 0;
                    Model.Price = 0m;
                }

                // Возвращаем частичное представление с моделью
                return PartialView("_CartPartial", Model);
            }
        }

        [Authorize]
        public ActionResult AddToCartPartial(int Id)
        {
            string UserEmail = User.Identity.Name;
            DesiredViewModel Model = new DesiredViewModel();
            using (DataBase DataBase = new DataBase())
            {
                UserDataTransferObject userdto = DataBase.Users.FirstOrDefault(x => x.EmailAddress == UserEmail);
                DesiredDataTransferObject PlaceInCart = DataBase.Desires.FirstOrDefault(x => x.UserId == userdto.Id && x.PlaceId == Id);

                if (PlaceInCart == null)
                {
                    DesiredDataTransferObject newPlaceInCard = new DesiredDataTransferObject
                    {
                        PlaceId = Id,
                        UserId = userdto.Id,
                        Quantity = 1
                    };
                    _ = DataBase.Desires.Add(newPlaceInCard);
                    _ = DataBase.SaveChanges();
                }
                else
                {
                    DesiredDataTransferObject oldPlaceInCard = DataBase.Desires.FirstOrDefault(x => x.UserId == userdto.Id && x.PlaceId == Id);
                    oldPlaceInCard.Quantity++;
                    _ = DataBase.SaveChanges();
                }

                List<DesiredViewModel> DesiredViewModelList;
                List<DesiredViewModel> DesiredViewModelList2 = new List<DesiredViewModel>();
                DesiredViewModelList = DataBase.Desires.ToArray().Where(x => x.UserId == userdto.Id).Select(x => new DesiredViewModel(x)).ToList();

                foreach (DesiredViewModel i in DesiredViewModelList)
                {
                    PlaceDataTransferObject Place = DataBase.Places.FirstOrDefault(x => x.Id == i.PlaceId);
                    DesiredViewModel DES = new DesiredViewModel(i.Id, Place.Id, Place.PlaceName, i.Quantity, Place.Price, Place.ImageName, Place.Slug, userdto.Id);
                    DesiredViewModelList2.Add(DES);
                }

                int Qty = 0;
                decimal Price = 0m;

                foreach (DesiredViewModel item in DesiredViewModelList2)
                {
                    Qty += item.Quantity;
                    Price += item.Quantity * item.Price;
                }

                Model.Quantity = Qty;
                Model.Price = Price;

                return PartialView("_AddToCartPartial", Model);
            }
        }

        // GET: /Desires/IncrementPlace
        [Authorize]
        public JsonResult IncrementPlace(int PlaceId)
        {
            // Объявляем лист Desired

            string UserEmail = User.Identity.Name;
            using (DataBase DataBase = new DataBase())
            {
                UserDataTransferObject userdto = DataBase.Users.FirstOrDefault(x => x.EmailAddress == UserEmail);

                DesiredDataTransferObject oldProductInCard = DataBase.Desires.FirstOrDefault(x => x.UserId == userdto.Id && x.PlaceId == PlaceId);
                oldProductInCard.Quantity++;
                _ = DataBase.SaveChanges();

                List<DesiredViewModel> Desired = DataBase.Desires.ToArray().Where(x => x.UserId == userdto.Id).Select(x => new DesiredViewModel(x)).ToList();
                // Получаем DesiredViewModel из листа
                DesiredViewModel Model = Desired.FirstOrDefault(x => x.PlaceId == PlaceId);

              
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
            // Объявляем лист Desired
            string UserEmail = User.Identity.Name;
            using (DataBase DataBase = new DataBase())
            {
                UserDataTransferObject userdto = DataBase.Users.FirstOrDefault(x => x.EmailAddress == UserEmail);

                DesiredDataTransferObject OldPlaceInCard = DataBase.Desires.FirstOrDefault(x => x.UserId == userdto.Id && x.PlaceId == PlaceId);
                if (OldPlaceInCard.Quantity > 1)
                {
                    OldPlaceInCard.Quantity--;
                    _ = DataBase.SaveChanges();
                    List<DesiredViewModel> Desired = DataBase.Desires.ToArray().Where(x => x.UserId == userdto.Id).Select(x => new DesiredViewModel(x)).ToList();
                    DesiredViewModel Model = Desired.FirstOrDefault(x => x.PlaceId == PlaceId);
                    var Result = new { qty = Model.Quantity, price = Model.Price };
                    return Json(Result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    OldPlaceInCard.Quantity = 0;
                    _ = DataBase.Desires.Remove(OldPlaceInCard);
                    _ = DataBase.SaveChanges();
                   
                    var Result = new { qty = 0, price = 0 };
                    return Json(Result, JsonRequestBehavior.AllowGet);
                }
            }
        }

        // GET: /Desires/RemovePlace
        public void RemovePlace(int PlaceId)
        {
            // Объявляем лист Desired

            string UserEmail = User.Identity.Name;
            using (DataBase DataBase = new DataBase())
            {
                UserDataTransferObject userdto = DataBase.Users.FirstOrDefault(x => x.EmailAddress == UserEmail);

                DesiredDataTransferObject OldPlaceInCard = DataBase.Desires.FirstOrDefault(x => x.UserId == userdto.Id && x.PlaceId == PlaceId);
                _ = DataBase.Desires.Remove(OldPlaceInCard);
                _ = DataBase.SaveChanges();
            }
      
        }
    }
}