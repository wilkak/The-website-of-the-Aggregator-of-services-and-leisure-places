using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using WebStoreMap.Models.Data;


namespace WebStoreMap.Models.ViewModels.Place
{
    public class PlaceViewModel
    {
        public PlaceViewModel()
        {
        }

        public PlaceViewModel(PlaceDataTransferObject row)
        {
            Id = row.Id;
            PlaceName = row.PlaceName;
            Slug = row.Slug;
            Description = row.Description;
            Price = row.Price;
            CategoryName = row.CategoryName;
            CategoryId = row.CategoryId;
            ImageName = row.ImageName;
            Latitude = row.Latitude;
            Longitude = row.Longitude;
            UserId = row.UserId;
            CityName = row.CityName;
            CityId = row.CityId;
            RegionName = row.RegionName;
            RegionId = row.RegionId;
            CountryName = row.CountryName;
            CountryId = row.CountryId;
            OldPrice = row.OldPrice;
            View = row.View;
            Address = row.Address;
            PhoneNumber = row.PhoneNumber;
            CompanyId = row.CompanyId;
            FirstDay = row.FirstDay;
            LastDay = row.LastDay;
            Schedule = row.Schedule;
        }

        public PlaceViewModel(int Id, string PlaceName, string Slug, string Description, decimal Price, string CategoryName, int CategoryId,
            string ImageName, string Latitude, string Longitude, int UserId, string CityName, int CityId, string RegionName, int RegionId, string CountryName, int CountryId,
            decimal OldPrice, bool View, string Address, string PhoneNumber,
            string CompanyName, int CompanyId, string CompanySlug, DateTime FirstDay, DateTime LastDay, string Schedule)

        {
            this.Id = Id;
            this.PlaceName = PlaceName;
            this.Slug = Slug;
            this.Description = Description;
            this.Price = Price;
            this.CategoryName = CategoryName;
            this.CategoryId = CategoryId;
            this.ImageName = ImageName;
            this.Latitude = Latitude;
            this.Longitude = Longitude;
            this.UserId = UserId;
            this.CityName = CityName;
            this.CityId = CityId;
            this.RegionName = RegionName;
            this.RegionId = RegionId;
            this.CountryName = CountryName;
            this.CountryId = CountryId;
            this.OldPrice = OldPrice;
            this.View = View;
            this.Address = Address;
            this.PhoneNumber = PhoneNumber;
            this.CompanyName = CompanyName;
            this.CompanyId = CompanyId;
            this.CompanySlug = CompanySlug;
            this.FirstDay = FirstDay;
            this.LastDay = LastDay;
            this.Schedule = Schedule;
        }

        public PlaceViewModel(int Id, string PlaceName, string Slug, string Description, decimal Price, string CategoryName, int CategoryId,
         string ImageName, string Latitude, string Longitude, int UserId, string CityName, int CityId, string RegionName, int RegionId, string CountryName, int CountryId,
         decimal OldPrice, bool View, string Address, string PhoneNumber, string CompanyName, int CompanyId,
         string CompanySlug, bool IsThisInFavorites, double RaitingOfPlace,  DateTime FirstDay, DateTime LastDay, string Schedule)
        {
            this.Id = Id;
            this.PlaceName = PlaceName;
            this.Slug = Slug;
            this.Description = Description;
            this.Price = Price;
            this.CategoryName = CategoryName;
            this.CategoryId = CategoryId;
            this.ImageName = ImageName;
            this.Latitude = Latitude;
            this.Longitude = Longitude;
            this.UserId = UserId;
            this.CityName = CityName;
            this.CityId = CityId;
            this.RegionName = RegionName;
            this.RegionId = RegionId;
            this.CountryName = CountryName;
            this.CountryId = CountryId;
            this.OldPrice = OldPrice;
            this.View = View;
            this.Address = Address;
            this.PhoneNumber = PhoneNumber;
            this.CompanyName = CompanyName;
            this.CompanyId = CompanyId;
            this.CompanySlug = CompanySlug;
            this.IsThisInFavorites = IsThisInFavorites;
            this.RaitingOfPlace = RaitingOfPlace;
            this.FirstDay = FirstDay;
            this.LastDay = LastDay;
            this.Schedule = Schedule;
        }
        
        public int Id { get; set; }

        [Required(ErrorMessage = "Поле название должно быть заполнено правильно")]
        [DisplayName("Название")]
        [StringLength(150, ErrorMessage = "Название должен содержать как минимум 3 символа, и как максимум 150 символов", MinimumLength = 3)]
        public string PlaceName { get; set; }

        public string Slug { get; set; }

        [Required(ErrorMessage = "Поле описание должно быть заполнено правильно")]
        [DisplayName("Описание")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [DisplayName("Цена")]
        [Required(ErrorMessage = "Поле цены должно быть заполнено правильно")]
        public decimal Price { get; set; }

        public string CategoryName { get; set; }

        [Required(ErrorMessage = "Поле категория должно быть заполнено")]
        [DisplayName("Категория")]
        public int CategoryId { get; set; }

        [DisplayName("Изображение")]
        public string ImageName { get; set; }

        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }
        public IEnumerable<SelectListItem> Region { get; set; }
        public IEnumerable<SelectListItem> Country { get; set; }
        public IEnumerable<SelectListItem> City { get; set; }
        public IEnumerable<string> GalleryImages { get; set; }
        public int UserId { get; set; }
        public string CityName { get; set; }
        public int CityId { get; set; }
        public string RegionName { get; set; }
        public int RegionId { get; set; }
        public string CountryName { get; set; }
        public int CountryId { get; set; }
        public bool View { get; set; }

        [Required(ErrorMessage = "Поле старой цены должно быть заполнено правильно")]
        public decimal OldPrice { get; set; }

        [Required(ErrorMessage = "Поле адрес должно быть заполнено правильно")]
        [StringLength(50, ErrorMessage = "Адрес должен содержать как минимум 3 символа, и как максимум 50 символов", MinimumLength = 3)]
        public string Address { get; set; }

        [Required(ErrorMessage = "Поле телефон должно быть заполнено правильно")]
        [StringLength(50, ErrorMessage = "Телефон должен содержать как минимум 3 символа, и как максимум 50 символов", MinimumLength = 3)]
        public string PhoneNumber { get; set; }

        public string CompanyName { get; set; }
        public int CompanyId { get; set; }
        public string CompanySlug { get; set; }
        public bool IsThisInFavorites { get; set; }
        public double RaitingOfPlace { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FirstDay { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime LastDay { get; set; }

        [DataType(DataType.MultilineText)]
        public string Schedule { get; set; }

        public IEnumerable<SelectListItem> Company { get; set; }
        public ICollection<RatingDataTransferObject> Rating { get; set; }
        public ICollection<ServiceDataTransferObject> Services { get; set; }
    }

}