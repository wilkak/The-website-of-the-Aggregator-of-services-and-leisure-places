using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using WebStoreMap.Models.Data;

namespace WebStoreMap.Models.ViewModels.geodan
{
    public class RegionInformationViewModel
    {
        public RegionInformationViewModel()
        {
        }

        public RegionInformationViewModel(RegionInformationDataTransferObject row)
        {
            RegionInformationId = row.RegionId;
            RegionName = row.Region.RegionName;
            CountryName = row.Region.CountryName;
            CountryId = row.Region.CountryId;
            Latitude = row.Region.Latitude;
            Longitude = row.Region.Longitude;
            Slug = row.Slug;
            Description = row.Description;
            ImageName = row.ImageName;
            View = row.View;
        }
        [Key]
        public int RegionInformationId { get; set; }
        public string RegionName { get; set; }
        public string CountryName { get; set; }
        public int CountryId { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public string ImageName { get; set; }
        public bool View { get; set; }
        public IEnumerable<SelectListItem> Region { get; set; }
        public ICollection<CityInformationDataTransferObject> СityInformation { get; set; }
        public IEnumerable<string> GalleryImages { get; set; }
    }
}