using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebStoreMap.Models.Data
{
    [Table("TableRegions")]
    public class RegionDataTransferObject
    {
        
        public int Id { get; set; }

        public string RegionName { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int CountryId { get; set; }
        public string CountryName { get; set; }

        [ForeignKey("CountryId")]
        public virtual CountryDataTransferObject Country { get; set; }
    }
}