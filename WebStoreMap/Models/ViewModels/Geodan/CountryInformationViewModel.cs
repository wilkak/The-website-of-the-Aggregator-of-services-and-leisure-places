using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using WebStoreMap.Models.Data;

namespace WebStoreMap.Models.ViewModels.geodan
{
    public class CountryInformationViewModel
    {
        public CountryInformationViewModel()
        {
        }

        public CountryInformationViewModel(CountryInformationDataTransferObject row)
        {
            CountryInformationId = row.CountryId;
                       
            CountryName = row.Country.CountryName;
            Latitude = row.Country.Latitude;
            Longitude = row.Country.Longitude;
            Slug = row.Slug;
            Description = row.Description;
            ImageName = row.ImageName;
            View = row.View;
        }
        [Key]
        public int CountryInformationId { get; set; }
        public string CountryName { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public string ImageName { get; set; }
        public bool View { get; set; }
        public IEnumerable<SelectListItem> Country { get; set; }
        public ICollection<RegionInformationDataTransferObject> RegionsInformation { get; set; }
        public IEnumerable<string> GalleryImages { get; set; }
    }
}