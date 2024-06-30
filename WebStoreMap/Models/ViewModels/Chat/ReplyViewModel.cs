using System.ComponentModel.DataAnnotations;

namespace WebStoreMap.Models.ViewModels.Chat
{
    public class ReplyViewModel
    {
        [Required(ErrorMessage = "Поле комментария должно быть заполнено правильно")]
        [StringLength(1000, ErrorMessage = "Комментарий должен содержать как минимум 1 символов", MinimumLength = 1)]
        [DataType(DataType.MultilineText)]
        public string Reply { get; set; }

        public int CID { get; set; }
    }
}