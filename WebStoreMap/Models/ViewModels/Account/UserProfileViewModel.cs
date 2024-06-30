using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using WebStoreMap.Models.Data;

namespace WebStoreMap.Models.ViewModels.Account
{
    public class UserProfileViewModel
    {
        public UserProfileViewModel()
        {
        }

        public UserProfileViewModel(UserDataTransferObject row)
        {
            Id = row.Id;
            FirstName = row.FirstName;
            LastName = row.LastName;
            EmailAddress = row.EmailAddress;
            PhoneNumber = row.PhoneNumber;
            BirthDate = row.BirthDate;
            Password = row.Password;
            ImageName = row.ImageName;
        }
        
        public int Id { get; set; }

        [Required(ErrorMessage = "Поле имя должно быть заполнено")]
        [DisplayName("Имя")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Поле фамилия должно быть заполнено")]
        [DisplayName("Фамилия")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Поле почта должно быть заполнено")]
        [DataType(DataType.EmailAddress)]
        [DisplayName("Почта")]
        public string EmailAddress { get; set; }

        [StringLength(100, ErrorMessage = "Пароль должен содержать как минимум 6 символов", MinimumLength = 6)]
        [Required(ErrorMessage = "Поле номер телефона должно быть заполнено")]
        [DisplayName("Номер телефона")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Поле дата рождения должно быть заполнено")]
        [DisplayName("Дата Рождения")]
        public DateTime BirthDate { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Поле пароль должно быть заполнено")]
        [DisplayName("Пароль")]
        public string Password { get; set; }

        [StringLength(100, ErrorMessage = "Пароль должен содержать как минимум 6 символов", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Поле потверждение пароля должно быть заполнено")]
        [DisplayName("Потверждение пароля")]
        public string ConfirmPassword { get; set; }

        [ScaffoldColumn(false)]
        [DisplayName("Изображение")]
        public string ImageName { get; set; }

        [Required(ErrorMessage = "Поле капчи должно быть заполнено")]
        [Display(Name = "Введите число с картинки")]
        public string Captcha { get; set; }
    }
}