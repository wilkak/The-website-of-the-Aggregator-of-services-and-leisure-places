using System;
using System.ComponentModel.DataAnnotations;
using WebStoreMap.Models.Data;

namespace WebStoreMap.Models.ViewModels.Rating
{
    public class RatingViewModel
    {
        public RatingViewModel()
        {
        }

        public RatingViewModel(RatingDataTransferObject row)
        {
            Id = row.Id;
            Text = row.Text;
            UserId = row.UserId;
            Date = row.Date;
            PlaceId = row.PlaceId;
            Rating = row.Rating;
            ImageName = row.ImageName;
            FirstName = row.User.FirstName;
            LastName = row.User.LastName;
            ImageUser = row.User.ImageName;
        }
        
        public int Id { get; set; }

        [Required(ErrorMessage = "Поле текста должно быть заполнено правильно")]
        public string Text { get; set; }

        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public int PlaceId { get; set; }
        public int Rating { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ImageUser { get; set; }
        public string ImageName { get; set; }
    }
}