using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace WebStoreMap.Models.ViewModels.Account
{
    public class UserRoleEditorViewModel
    {
        public UserRoleEditorViewModel()
        {
        }

        public UserRoleEditorViewModel(int Id, string FirstName, string LastName, string EmailAdress, string PhoneNumber, DateTime BirthDate, string Password, string RoleName, int RoleId, bool View, bool EmailConfirmed)
        {
            this.Id = Id;
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.EmailAddress = EmailAdress;
            this.PhoneNumber = PhoneNumber;
            ConfirmPassword = Password;
            this.BirthDate = BirthDate;
            this.Password = Password;
            this.RoleName = RoleName;
            this.RoleId = RoleId;
            this.View = View;

            this.EmailConfirmed = EmailConfirmed;
        }
       
        public int Id { get; set; }

        [Required]
        [DisplayName("Имя")]
        public string FirstName { get; set; }

        [Required]
        [DisplayName("Фамилия")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [DisplayName("Почта")]
        public string EmailAddress { get; set; }

        [Required]
        [DisplayName("Номер телефона")]
        public string PhoneNumber { get; set; }

        [Required]
        [DisplayName("Дата Рождения")]
        public DateTime BirthDate { get; set; }

        [Required]
        [DisplayName("Пароль")]
        public string Password { get; set; }

        [Required]
        [DisplayName("Потверждение пароля")]
        public string ConfirmPassword { get; set; }

        public string RoleName { get; set; }

        public int RoleId { get; set; }

        public bool View { get; set; }

        public bool EmailConfirmed { get; set; }

        public IEnumerable<SelectListItem> Role { get; set; }
    }
}