using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using WebStoreMap.Models.Data;

namespace WebStoreMap.Models.ViewModels.Account
{
    public class UserRegistrationViewModel
    {
        public UserRegistrationViewModel()
        {
        }

        public UserRegistrationViewModel(RegistrationUserDataTransferObject row)
        {
            Id = row.Id;
            FirstName = row.FirstName;
            LastName = row.LastName;
            EmailAddress = row.EmailAddress;
            PhoneNumber = row.PhoneNumber;
            BirthDate = row.BirthDate;
            Password = row.Password;
            View = row.View;
            EmailConfirmed = row.EmailConfirmed;
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

        [Required(ErrorMessage = "Поле номер телефона должно быть заполнено")]
        [DisplayName("Номер телефона")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Поле дата рождения должно быть заполнено")]
        [DisplayName("Дата Рождения")]
        public DateTime BirthDate { get; set; }

        [StringLength(50, ErrorMessage = "Пароль должен содержать как минимум 6 символов", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [DisplayName("Пароль")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Поле потверждение пароля должно быть заполнено")]
        [DisplayName("Потверждение пароля")]
        [StringLength(50, ErrorMessage = "Пароль должен содержать как минимум 6 символов", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        public bool View { get; set; }

        [ScaffoldColumn(false)]
        public string ImageName { get; set; }

        public bool EmailConfirmed { get; set; }

        [Required(ErrorMessage = "Поле капчи должно быть заполнено")]
        [Display(Name = "Введите число с картинки")]
        public string Captcha { get; set; }
    }
}