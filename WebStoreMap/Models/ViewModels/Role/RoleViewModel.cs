using System.ComponentModel.DataAnnotations;
using WebStoreMap.Models.Data;

namespace WebStoreMap.Models.ViewModels.Role
{
    public class RoleViewModel
    {
        public RoleViewModel()
        {
        }

        public RoleViewModel(RoleDataTransferObject row)
        {
            Id = row.RoleId;

            Name = row.RoleName;
        }
        
        public int Id { get; set; }
        public string Name { get; set; }
    }
}