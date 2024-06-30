using System.ComponentModel.DataAnnotations;

namespace WebStoreMap.Models.Email
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Поле капчи должно быть заполнено")]
        [Display(Name = "Введите число с картинки")]
        public string Captcha { get; set; }
    }
}