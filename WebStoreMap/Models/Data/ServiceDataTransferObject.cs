using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebStoreMap.Models.Data
{
    [Table("TableServices")]
    public class ServiceDataTransferObject
    {
        public int Id { get; set; }
        public string ServiceName { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal OldPrice { get; set; }
        public int PlaceId { get; set; }
        public bool View { get; set; }
        public string ImageName { get; set; }
        public int UserId { get; set; }
        [ForeignKey("PlaceId")]
        public virtual PlaceDataTransferObject Place { get; set; }
        [ForeignKey("UserId")]
        public virtual UserDataTransferObject User { get; set; }

    }
}