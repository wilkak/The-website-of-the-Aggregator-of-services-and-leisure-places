using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebStoreMap.Models.Data
{
    [Table("TableRoles")]
    public class RoleDataTransferObject
    {
        [Key]
        public int RoleId { get; set; }

        public string RoleName { get; set; }
    }
}