using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using WebStoreMap.Models.Data;

namespace WebStoreMap.Models.ViewModels.geodan
{
    public class RegionViewModel
    {
        public RegionViewModel()
        {
        }

        public RegionViewModel(RegionDataTransferObject row)
        {
            Id = row.Id;
            CountryName = row.CountryName;
            CountryId = row.CountryId;
            Region = row.RegionName;
            Latitude = row.Latitude;
            Longitude = row.Longitude;
        }
        
        public int Id { get; set; }
        public string Region { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int CountryId { get; set; }
        public string CountryName { get; set; }

        public IEnumerable<SelectListItem> Country { get; set; }
    }
}