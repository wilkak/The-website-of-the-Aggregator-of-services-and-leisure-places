using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebStoreMap.Models.Data
{
    [Table("TablePlaces")]
    public class PlaceDataTransferObject
    {
       
        public int Id { get; set; }
        public string PlaceName { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string CategoryName { get; set; }
        public int CategoryId { get; set; }
        public string ImageName { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public int UserId { get; set; }
        public string CityName { get; set; }
        public int CityId { get; set; }
        public string RegionName { get; set; }
        public int RegionId { get; set; }
        public string CountryName { get; set; }
        public int CountryId { get; set; }
        
        public bool View { get; set; }
        public decimal OldPrice { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public int CompanyId { get; set; }
        public DateTime FirstDay { get; set; }
        public DateTime LastDay { get; set; }
        public string Schedule { get; set; }

        // Назначаем внешний ключ

        [ForeignKey("RegionId")]
        public virtual RegionDataTransferObject Region { get; set; }

        [ForeignKey("CountryId")]
        public virtual CountryDataTransferObject Country { get; set; }

        [ForeignKey("CategoryId")]
        public virtual CategoryDataTransferObject Category { get; set; }

        [ForeignKey("UserId")]
        public virtual UserDataTransferObject User { get; set; }

        [ForeignKey("CompanyId")]
        public virtual CompanyDataTransferObject Company { get; set; }

        public ICollection<RatingDataTransferObject> Rating { get; set; }
    }
}