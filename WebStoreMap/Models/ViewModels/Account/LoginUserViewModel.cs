using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebStoreMap.Models.ViewModels.Account
{
    public class LoginUserViewModel
    {
        [Required(ErrorMessage = "Поле почты должно быть заполнено")]
        [DisplayName("Логин пользователя")]
        public string EmailAddress { get; set; }

        [StringLength(100, ErrorMessage = "Пароль должен содержать как минимум 6 символов", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Поле пароля должно быть заполнено")]
        [DisplayName("Пароль")]
        public string Password { get; set; }

        [DisplayName("Запомнить меня")]
        public bool RememberMe { get; set; }
    }
}