using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebStoreMap.Models.Data
{
    [Table("TableCities")]
    public class CityDataTransferObject
    {
        
        public int Id { get; set; }
        public string CityName { get; set; }
        public string CountryName { get; set; }
        
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int RegionId { get; set; }
        public string RegionName { get; set; }

        [ForeignKey("RegionId")]
        public virtual RegionDataTransferObject Region { get; set; }
    }
}