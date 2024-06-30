using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebStoreMap.Models.Data;

namespace WebStoreMap.Models.ViewModels.Chat
{
    public class CommentPlaceViewModel
    {
        public CommentPlaceViewModel()
        {
        }

        public CommentPlaceViewModel(CommentDataTransferObject row)
        {
            Id = row.Id;
            Text = row.Text;
            UserId = row.UserId;
            Date = row.Date;
            PlaceId = row.PlaceId;
            Replies = row.Replies;
            FirstName = row.User.FirstName;
            LastName = row.User.LastName;
            ImageName = row.User.ImageName;
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Поле комментарий должно быть заполнено правильно")]
        [StringLength(1000, ErrorMessage = "Комментарий должен содержать как минимум 1 символов", MinimumLength = 1)]
        [DataType(DataType.MultilineText)]
        public string Text { get; set; }

        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public int PlaceId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ImageName { get; set; }

        public ICollection<ReplyDataTransferObject> Replies { get; set; }
    }
}