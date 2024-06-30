using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebStoreMap.Models.Data
{
    [Table("TableRatings")]
    public class RatingDataTransferObject
    {
      
        public int Id { get; set; }

        [Required]
        public string Text { get; set; }

        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public int PlaceId { get; set; }
        public int Rating { get; set; }
        public string ImageName { get; set; }

        [ForeignKey("PlaceId")]
        public virtual PlaceDataTransferObject Place { get; set; }

        [ForeignKey("UserId")]
        public virtual UserDataTransferObject User { get; set; }
    }
}