using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebStoreMap.Models.Data
{
    [Table("TableCitiesInformation")]
    public class CityInformationDataTransferObject
    {
        [Key, Column(Order = 0)]
        public int CityId { get; set; }
        
        public string Slug { get; set; }

        public string Description { get; set; }
        public string ImageName { get; set; }
        public bool View { get; set; }
        public int RegionId { get; set; }
        public int? YearOfFoundation { get; set; }
        public string Population { get; set; }
        public string Square { get; set; }
        [ForeignKey("CityId")]
        public virtual CityDataTransferObject City { get; set; }
    }
}