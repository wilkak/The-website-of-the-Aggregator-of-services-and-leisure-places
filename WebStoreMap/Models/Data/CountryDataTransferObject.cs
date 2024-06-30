using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebStoreMap.Models.Data
{
    [Table("TableCountries")]
    public class CountryDataTransferObject
    {
        public int Id { get; set; }

        public string CountryName { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public ICollection<RegionDataTransferObject> Regions { get; set; }
    }
}