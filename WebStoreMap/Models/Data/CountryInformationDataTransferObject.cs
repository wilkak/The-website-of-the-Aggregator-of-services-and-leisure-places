using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebStoreMap.Models.Data
{
    [Table("TableCountriesInformation")]
    public class CountryInformationDataTransferObject
    {
        [Key, Column(Order = 0)]
        public int CountryId { get; set; }
        
        public string Slug { get; set; }
        public string Description { get; set; }
        public string ImageName { get; set; }
        public bool View { get; set; }

        [ForeignKey("CountryId")]
        public virtual CountryDataTransferObject Country { get; set; }

        public ICollection<RegionInformationDataTransferObject> RegionInformation { get; set; }
   
    }
}