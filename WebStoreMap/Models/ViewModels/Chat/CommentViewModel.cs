using System;
using System.ComponentModel.DataAnnotations;
using WebStoreMap.Models.Data;

namespace WebStoreMap.Models.ViewModels.Chat
{
    public class CommentViewModel
    {
        public CommentViewModel()
        {
        }

        public CommentViewModel(CommentDataTransferObject row)
        {
            Id = row.Id;
            Text = row.Text;
            UserId = row.UserId;
            Date = row.Date;
            PlaceId = row.PlaceId;
        }
        
        public int Id { get; set; }

        [Required(ErrorMessage = "Поле комментарий должно быть заполнено правильно")]
        [StringLength(1000, ErrorMessage = "Комментарий должен содержать как минимум 1 символов", MinimumLength = 1)]
        [DataType(DataType.MultilineText)]
        public string Text { get; set; }

        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public int PlaceId { get; set; }
    }
}