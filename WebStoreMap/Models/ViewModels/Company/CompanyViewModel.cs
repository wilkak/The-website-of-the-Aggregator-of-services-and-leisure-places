using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebStoreMap.Models.Data;

namespace WebStoreMap.Models.ViewModels.Company
{
    public class CompanyViewModel
    {
        public CompanyViewModel()
        {
        }

        public CompanyViewModel(CompanyDataTransferObject row)
        {
            Id = row.Id;
            FullCompanyName = row.FullCompanyName;
            CompanyName = row.CompanyName;
            LegalAddress = row.LegalAddress;
            PostalAddress = row.PostalAddress;
            INN = row.INN;
            OGRN = row.OGRN;
            KPP = row.KPP;
            EmailAdress = row.EmailAdress;
            PhoneNumber = row.PhoneNumber;
            Description = row.Description;
            SlugTelegram = row.SlugTelegram;
            SlugVK = row.SlugVK;
            SlugWhatsapp = row.SlugWhatsapp;
            ImageName = row.ImageName;
            View = row.View;
            UserId = row.UserId;
        }
        
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Поле полное название должно быть заполнено")]
        [DisplayName("Полное название")]
        [StringLength(150, ErrorMessage = "Полное название должен содержать как минимум 3 символа, и как максимум 150 символов", MinimumLength = 3)]
        public string FullCompanyName { get; set; }

        [StringLength(100, ErrorMessage = "Название должен содержать как минимум 3 символа, и как максимум 100 символов", MinimumLength = 3)]
        [Required(ErrorMessage = "Поле название должно быть заполнено")]
        [DisplayName("Название")]
        public string CompanyName { get; set; }

        public string LegalAddress { get; set; }
        public string PostalAddress { get; set; }
        public string INN { get; set; }
        public string OGRN { get; set; }
        public string KPP { get; set; }

        [DataType(DataType.EmailAddress)]
        public string EmailAdress { get; set; }

        [StringLength(50, ErrorMessage = "Телефон должен содержать как минимум 3 символа, и как максимум 50 символов", MinimumLength = 3)]
        [Required(ErrorMessage = "Поле телефон должно быть заполнено")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Поле описание должно быть заполнено")]
        [DisplayName("Описание")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        public string SlugTelegram { get; set; }
        
        public string SlugVK { get; set; }
        public string SlugWhatsapp { get; set; }

        [DisplayName("Изображение")]
        public string ImageName { get; set; }

        public bool View { get; set; }
        public int UserId { get; set; }
        public IEnumerable<string> GalleryImages { get; set; }
    }
}