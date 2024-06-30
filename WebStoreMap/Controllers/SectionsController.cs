﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Drawing.Imaging;
using WebStoreMap.Models.Data;
using WebStoreMap.Models.Email;
using WebStoreMap.Models.Encoding;
using WebStoreMap.Models.ViewModels.Account;
using WebStoreMap.Models.ViewModels.Email;
using WebStoreMap.Models.ViewModels.geodan;
using WebStoreMap.Models.ViewModels.Place;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Dynamic;
using System.Data.Entity;
using System.Web.Helpers;
using PagedList;
using System.Web.UI;
using WebStoreMap.Models.ViewModels.Rating;

namespace WebStoreMap.Controllers
{
    public class SectionsController : Controller
    {
        public ActionResult CitiesMenuPartial()
        {
            List<CityInformationViewModel> CityInformationModelList;
            using (DataBase DataBase = new DataBase())
            {
                CityInformationModelList = DataBase.CitiesInformation.ToArray().Select(x => new CityInformationViewModel(x)).ToList();
            }
            return PartialView("_CitiesMenuPartial", CityInformationModelList);
        }

        private readonly List<PlaceViewModel> PlacesList = new List<PlaceViewModel>();
        private const int PageSize = 3;

        public ActionResult Index(int CatId = 0, int CityId = 0, string SortType = null, string SearchString = null)
        {
            using (DataBase DataBase = new DataBase())
            {
                var Categories = from d in DataBase.Categories
                                                   orderby d.CategoryName
                                                   select d.CategoryName;

                var Places = from m in DataBase.Places
                    where m.View && m.CityId == CityId
                    select m;
                ViewBag.Categories = DataBase.Categories.ToArray().Select(x => new CategoryViewModel(x)).ToList();
                PlacesList.Clear();

                List<PlaceViewModel> PlaceViewModelList = null;

                ViewBag.SelectedCat = CatId;
                ViewBag.SearchString = SearchString;

                if (!string.IsNullOrEmpty(SearchString))
                {
                    Places = Places.Where(s => s.CityName.Contains(SearchString) || s.Description.Contains(SearchString));
                }
                else
                {
                    if (CatId != 0)
                    {
                        Places = Places.Where(s => s.CategoryId == CatId);
                    }
                }

                PlaceViewModelList = Places.ToArray().Select(x => new PlaceViewModel(x)).ToList();

                string UserEmail = User.Identity.Name;

                bool IsThisInFavorites = false;

                if (PlaceViewModelList.Count == 0)
                {
                    ViewBag.IsThisExist = "Нет совпадений";
                }

                if (Request.IsAuthenticated)
                {
                    UserDataTransferObject userdto = DataBase.Users.FirstOrDefault(x => x.EmailAddress == UserEmail);

                    foreach (PlaceViewModel i in PlaceViewModelList)
                    {
                        FavoriteDataTransferObject favorites = DataBase.Favorites.FirstOrDefault(x => x.PlaceId == i.Id && x.UserId == userdto.Id);

                        IsThisInFavorites = favorites != null;

                        List<RatingViewModel> ListRatingViewModel = DataBase.Ratings.ToArray()
                 .Where(x => x.PlaceId == i.Id)
                    .Select(x => new RatingViewModel(x))
                    .ToList();

                        double RatingResult = 0;
                        int Count = 0;
                        double RaitingOfPlace = 0;
                        if (ListRatingViewModel != null)
                        {
                            foreach (RatingViewModel Rating in ListRatingViewModel)
                            {
                                Count++;
                                RatingResult += Rating.Rating;
                            }

                            RaitingOfPlace = RatingResult / Count;
                        }

                        PlaceViewModel Place = new PlaceViewModel(i.Id, i.PlaceName, i.Slug, i.Description, i.Price, i.CategoryName, i.CategoryId,
                            i.ImageName, i.Latitude, i.Longitude, i.UserId, i.CityName, i.CityId, i.RegionName, i.RegionId, i.CountryName, i.CountryId,
                            i.OldPrice, i.View, i.Address, i.PhoneNumber, i.CompanyName, i.CompanyId, i.CompanySlug, IsThisInFavorites, RaitingOfPlace, i.FirstDay, i.LastDay, i.Schedule);
                        PlacesList.Add(Place);
                    }
                }
                else
                {
                    foreach (PlaceViewModel i in PlaceViewModelList)
                    {

                        List<RatingViewModel> ListRatingViewModel = DataBase.Ratings.ToArray()
                .Where(x => x.PlaceId == i.Id)
                   .Select(x => new RatingViewModel(x))
                   .ToList();

                        double RatingResult = 0;
                        int Count = 0;
                        double RaitingOfPlace = 0;
                        if (ListRatingViewModel != null)
                        {
                            foreach (RatingViewModel Rating in ListRatingViewModel)
                            {
                                Count++;
                                RatingResult += Rating.Rating;
                            }

                            RaitingOfPlace = RatingResult / Count;
                        }

                        PlaceViewModel Place = new PlaceViewModel(i.Id, i.PlaceName, i.Slug, i.Description, i.Price, i.CategoryName, i.CategoryId,
                            i.ImageName, i.Latitude, i.Longitude, i.UserId, i.CityName, i.CityId, i.RegionName, i.RegionId, i.CountryName, i.CountryId,
                            i.OldPrice, i.View, i.Address, i.PhoneNumber, i.CompanyName, i.CompanyId, i.CompanySlug, IsThisInFavorites, RaitingOfPlace, i.FirstDay, i.LastDay, i.Schedule);
                        PlacesList.Add(Place);
                    }
                }

                CityInformationDataTransferObject CityInfoDto = DataBase.CitiesInformation.Where(x => x.CityId == CityId).FirstOrDefault();
                CityInformationViewModel CityModel = new CityInformationViewModel(CityInfoDto);
                try
                {
                    CityModel.GalleryImages = Directory
                        .EnumerateFiles(Server.MapPath("~/Images/Uploads/Cities/" + CityModel.CityId + "/Gallery/Thumbs"))
                        .Select(fn => Path.GetFileName(fn));

                    ViewBag.GalleryCount = CityModel.GalleryImages.Count();
                }
                catch
                {

                }

                ViewBag.CityInfo = CityModel;
                

                CityDataTransferObject City = DataBase.Cities.Where(x => x.Id == CityId).FirstOrDefault();
                ViewBag.CityLatitude = City.Latitude;
                ViewBag.CityLongitude = City.Longitude;
                return View(PlacesList);
            }
        }

        public ActionResult GetPlaces(int CatId = 0, int CityId = 0, string SortType = null, string SearchString = null)
        {
            using (DataBase DataBase = new DataBase())
            {
                var Places = from m in DataBase.Places
                                                                where m.View && m.CityId == CityId
                                                                select m;

                List<PlaceViewModel> PlaceViewModelList = null;
                PlacesList.Clear();

                if (!string.IsNullOrEmpty(SearchString))
                {
                    Places = Places.Where(s => s.CityName.Contains(SearchString) || s.Description.Contains(SearchString));
                }

                if (CatId != 0)
                {
                    Places = Places.Where(s => s.CategoryId == CatId);
                }

                if (!string.IsNullOrEmpty(SortType))
                {
                    if (SortType == "По возрастанию")
                    {
                        Places = Places.OrderBy(s => s.Price);
                    }
                    else if (SortType == "По убыванию")
                    {
                        Places = Places.OrderByDescending(s => s.Price);
                    }
                    else if (SortType == "Новые")
                    {
                        Places = Places.Reverse();
                    }
                }

                PlaceViewModelList = Places.ToArray().Select(x => new PlaceViewModel(x)).ToList();

                string UserEmail = User.Identity.Name;

                bool IsThisInFavorites = false;
                if (PlaceViewModelList.Count == 0)
                {
                    ViewBag.IsThisExist = "Нет совпадений";
                }
                    if (Request.IsAuthenticated)
                    {
                        UserDataTransferObject userdto = DataBase.Users.FirstOrDefault(x => x.EmailAddress == UserEmail);

                        foreach (PlaceViewModel i in PlaceViewModelList)
                        {
                            FavoriteDataTransferObject favorites = DataBase.Favorites.FirstOrDefault(x => x.PlaceId == i.Id && x.UserId == userdto.Id);

                            IsThisInFavorites = favorites != null;

                        List<RatingViewModel> ListRatingViewModel = DataBase.Ratings.ToArray()
           .Where(x => x.PlaceId == i.Id)
              .Select(x => new RatingViewModel(x))
              .ToList();

                        double RatingResult = 0;
                        int Count = 0;
                        double RaitingOfPlace = 0;
                        if (ListRatingViewModel != null)
                        {
                            foreach (RatingViewModel Rating in ListRatingViewModel)
                            {
                                Count++;
                                RatingResult += Rating.Rating;
                            }

                            RaitingOfPlace = RatingResult / Count;
                        }

                        PlaceViewModel Place = new PlaceViewModel(i.Id, i.PlaceName, i.Slug, i.Description, i.Price, i.CategoryName, i.CategoryId,
                            i.ImageName, i.Latitude, i.Longitude, i.UserId, i.CityName, i.CityId, i.RegionName, i.RegionId, i.CountryName, i.CountryId,
                            i.OldPrice, i.View, i.Address, i.PhoneNumber, i.CompanyName, i.CompanyId, i.CompanySlug, IsThisInFavorites, RaitingOfPlace, i.FirstDay, i.LastDay, i.Schedule);
                        PlacesList.Add(Place);
                    }
                    }
                    else
                    {
                        foreach (PlaceViewModel i in PlaceViewModelList)
                        {
                        List<RatingViewModel> ListRatingViewModel = DataBase.Ratings.ToArray()
                  .Where(x => x.PlaceId == i.Id)
                     .Select(x => new RatingViewModel(x))
                     .ToList();

                        double RatingResult = 0;
                        int Count = 0;
                        double RaitingOfPlace = 0;
                        if (ListRatingViewModel != null)
                        {
                            foreach (RatingViewModel Rating in ListRatingViewModel)
                            {
                                Count++;
                                RatingResult += Rating.Rating;
                            }

                            RaitingOfPlace = RatingResult / Count;
                        }

                        PlaceViewModel Place = new PlaceViewModel(i.Id, i.PlaceName, i.Slug, i.Description, i.Price, i.CategoryName, i.CategoryId,
                            i.ImageName, i.Latitude, i.Longitude, i.UserId, i.CityName, i.CityId, i.RegionName, i.RegionId, i.CountryName, i.CountryId,
                            i.OldPrice, i.View, i.Address, i.PhoneNumber, i.CompanyName, i.CompanyId, i.CompanySlug, IsThisInFavorites, RaitingOfPlace, i.FirstDay, i.LastDay, i.Schedule);
                        PlacesList.Add(Place);
                    }
                    }

                ViewBag.PlacesList = PlacesList;
                
                return PartialView("GetPlaces");
            }
        }

        public ActionResult GetPlacesForMap(int CatId = 0, int CityId = 0, decimal NorthWestLat = 0, decimal NorthWestLng = 0, decimal NorthEastLat = 0,
            decimal NorthEastLng = 0, decimal SouthWestLat = 0, decimal SouthWestLng = 0,
            decimal SouthEastLat = 0, decimal SouthEastLng = 0)


        {
            using (DataBase DataBase = new DataBase())
            {
                var Places = from m in DataBase.Places
                                                                where m.View && m.CityId == CityId
                                                                select m;

                List<PlaceViewModel> PlaceViewModelList = null;
                PlacesList.Clear();
                if (CatId != 0)
                {
                    Places = Places.Where(s => s.CategoryId == CatId);
                }

                PlaceViewModelList = Places.ToArray().Select(x => new PlaceViewModel(x)).ToList();

                foreach (PlaceViewModel i in PlaceViewModelList)
                {
                    if (i.Latitude != null && i.Longitude != null)
                    {
                        decimal lat = Decimal.Parse(i.Latitude);
                        decimal lng = Decimal.Parse(i.Longitude);
                        if (lat < NorthWestLat && lng > NorthWestLng && lat < NorthEastLat && lng < NorthEastLng && lat > SouthWestLat && lng > SouthWestLng && lat > SouthEastLat && lng < SouthEastLng)
                        {
                            PlacesList.Add(i);
                        }
                    }
                }
                ViewBag.PlacesList = PlacesList;

                return PartialView("GetPlacesForMap");
            }
        }
    }
}