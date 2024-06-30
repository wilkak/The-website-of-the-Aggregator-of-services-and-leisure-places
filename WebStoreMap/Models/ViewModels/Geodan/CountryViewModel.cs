using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebStoreMap.Models.Data;

namespace WebStoreMap.Models.ViewModels.geodan
{
    public class CountryViewModel
    {
        public CountryViewModel()
        {
        }

        public CountryViewModel(CountryDataTransferObject row)
        {
            Id = row.Id;
            CountryName = row.CountryName;
            Latitude = row.Latitude;
            Longitude = row.Longitude;
        }
        
        public int Id { get; set; }
        public string CountryName { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        public ICollection<RegionDataTransferObject> Regions { get; set; }
    }
}