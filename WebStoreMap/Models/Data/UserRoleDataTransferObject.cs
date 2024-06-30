using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebStoreMap.Models.Data
{
    [Table("TableUsersRole")]
    public class UserRoleDataTransferObject
    {
        [Key, Column(Order = 0)]
        public int UserId { get; set; }

        [Key, Column(Order = 1)]
        public int RoleId { get; set; }

        [ForeignKey("UserId")]
        public virtual UserDataTransferObject User { get; set; }

        [ForeignKey("RoleId")]
        public virtual RoleDataTransferObject Role { get; set; }
    }
}