using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebStoreMap.Models.Data
{
    [Table("TableRegistrationUsers")]
    public class RegistrationUserDataTransferObject
    {
       
        public int Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime BirthDate { get; set; }
        public string Password { get; set; }
        public bool View { get; set; }
        public bool EmailConfirmed { get; set; }
        public string ImageName { get; set; }
    }
}