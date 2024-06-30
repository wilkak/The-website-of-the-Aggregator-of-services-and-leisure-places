using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebStoreMap.Models.Data
{
    [Table("TableFavorites")]
    public class FavoriteDataTransferObject
    {
      
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int PlaceId { get; set; }
        public int Quantity { get; set; }
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual UserDataTransferObject User { get; set; }
    }
}