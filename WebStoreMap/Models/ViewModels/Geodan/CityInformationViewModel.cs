using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using WebStoreMap.Models.Data;

namespace WebStoreMap.Models.ViewModels.geodan
{
    public class CityInformationViewModel
    {
        public CityInformationViewModel()
        {
        }

        public CityInformationViewModel(CityInformationDataTransferObject row)
        {
            CityId = row.CityId;
            CityName = row.City.CityName;
            RegionId = row.RegionId;
            CountryName = row.City.CountryName;
            Latitude = row.City.Latitude;
            Longitude = row.City.Longitude;
            Slug = row.Slug;
            Description = row.Description;
            ImageName = row.ImageName;
            View = row.View;
            YearOfFoundation = row.YearOfFoundation;
            Population = row.Population;
            Square = row.Square;
        }
        [Key]
        public int CityId { get; set; }
        public string CountryName { get; set; }
        public string CityName { get; set; }
        public int RegionId { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public string ImageName { get; set; }
        public bool View { get; set; }
        public int? YearOfFoundation { get; set; }
        public string Population { get; set; }
        public string Square { get; set; }
        public IEnumerable<string> GalleryImages { get; set; }
   
        public IEnumerable<SelectListItem> City { get; set; }
    }
}