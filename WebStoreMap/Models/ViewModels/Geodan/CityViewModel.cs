using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using WebStoreMap.Models.Data;

namespace WebStoreMap.Models.ViewModels.geodan
{
    public class CityViewModel
    {
        public CityViewModel()
        {
        }

        public CityViewModel(CityDataTransferObject row)
        {
            Id = row.Id;
            RegionName = row.RegionName;
            RegionId = row.RegionId;
            CountryName = row.CountryName;
            Latitude = row.Latitude;
            Longitude = row.Longitude;
        }
        
        public int Id { get; set; }
        public string CountryName { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int RegionId { get; set; }
        public string RegionName { get; set; }

        public IEnumerable<SelectListItem> Region { get; set; }
    }
}