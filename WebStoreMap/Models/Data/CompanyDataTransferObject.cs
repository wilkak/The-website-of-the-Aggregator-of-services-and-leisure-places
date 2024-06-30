using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebStoreMap.Models.Data
{
    [Table("TableCompanies")]
    public class CompanyDataTransferObject
    {
       
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string FullCompanyName { get; set; }
        public string CompanyName { get; set; }
        public string LegalAddress { get; set; }
        public string PostalAddress { get; set; }
        public string INN { get; set; }
        public string OGRN { get; set; }
        public string KPP { get; set; }
        public string EmailAdress { get; set; }
        public string PhoneNumber { get; set; }
        public string Description { get; set; }
        public string SlugTelegram { get; set; }
        public string SlugVK { get; set; }
        public string SlugWhatsapp { get; set; }
        public string ImageName { get; set; }
        public string Slug { get; set; }
        public bool View { get; set; }

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual UserDataTransferObject User { get; set; }
    }
}