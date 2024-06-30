using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebStoreMap.Models.Data
{
    [Table("TableRegionsInformation")]
    public class RegionInformationDataTransferObject
    {
        [Key, Column(Order = 0)]
        public int RegionId { get; set; }
        
        public int CountryId { get; set; }

        public string Slug { get; set; }

        public string Description { get; set; }
        public string ImageName { get; set; }
        public bool View { get; set; }

        [ForeignKey("RegionId")]
        public virtual RegionDataTransferObject Region { get; set; }

        public ICollection<CityInformationDataTransferObject> CityInformation { get; set; }
    }
}