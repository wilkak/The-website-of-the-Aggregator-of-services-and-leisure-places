using System.ComponentModel.DataAnnotations;
using WebStoreMap.Models.Data;

namespace WebStoreMap.Models.ViewModels.Favorites
{
    public class FavoritesViewModel
    {
        public FavoritesViewModel()
        {
        }

        public FavoritesViewModel(int Id, int PlaceId, string PlaceName, int Quantity, decimal Price, string Image, string PlaceSlug, int UserId)
        {
            this.Id = Id;
            this.PlaceId = PlaceId;
            this.PlaceName = PlaceName;
            this.Quantity = Quantity;
            this.Price = Price;
            this.Image = Image;
            this.Price = Price;
            this.Image = Image;
            this.PlaceSlug = PlaceSlug;
            this.UserId = UserId;
        }

        public FavoritesViewModel(FavoriteDataTransferObject row)
        {
            Id = row.Id;
            PlaceId = row.PlaceId;
            UserId = row.UserId;
            Quantity = row.Quantity;
        }
        
        public int Id { get; set; }
        public int PlaceId { get; set; }
        public string PlaceName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total => Quantity * Price;
        public string Image { get; set; }
        public string PlaceSlug { get; set; }
        public int UserId { get; set; }
    }
}