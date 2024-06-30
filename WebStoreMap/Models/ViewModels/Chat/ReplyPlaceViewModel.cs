using System;
using System.ComponentModel.DataAnnotations;
using WebStoreMap.Models.Data;

namespace WebStoreMap.Models.ViewModels.Chat
{
    public class ReplyPlaceViewModel
    {
        public ReplyPlaceViewModel()
        {
        }

        public ReplyPlaceViewModel(ReplyDataTransferObject row)
        {
            Id = row.Id;
            Text = row.Text;
            UserId = row.UserId;
            Date = row.Date;
            CommentId = row.CommentId;
            FirstName = row.User.FirstName;
            LastName = row.User.LastName;
        }
        
        public int Id { get; set; }

        [Required(ErrorMessage = "Поле текст должно быть заполнено правильно")]
        [StringLength(1000, ErrorMessage = "Комментарий должен содержать как минимум 1 символов", MinimumLength = 1)]
        [DataType(DataType.MultilineText)]
        public string Text { get; set; }

        public int UserId { get; set; }
        public int CommentId { get; set; }
        public DateTime Date { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}