using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using WebStoreMap.Models.Data;

namespace WebStoreMap.Models.ViewModels.Account
{
    public class UserProfileInformationViewModel
    {
        public UserProfileInformationViewModel()
        {
        }

        public UserProfileInformationViewModel(UserDataTransferObject row)
        {
            Id = row.Id;
            FirstName = row.FirstName;
            LastName = row.LastName;
            EmailAddress = row.EmailAddress;
            PhoneNumber = row.PhoneNumber;
            BirthDate = row.BirthDate;
            ImageName = row.ImageName;
        }
       
        public int Id { get; set; }

        [DisplayName("Имя")]
        public string FirstName { get; set; }

        [DisplayName("Фамилия")]
        public string LastName { get; set; }

        [DisplayName("Почта")]
        public string EmailAddress { get; set; }

        [DisplayName("Номер телефона")]
        public string PhoneNumber { get; set; }

        [DisplayName("Дата Рождения")]
        public DateTime BirthDate { get; set; }

        public string ImageName { get; set; }
    }
}