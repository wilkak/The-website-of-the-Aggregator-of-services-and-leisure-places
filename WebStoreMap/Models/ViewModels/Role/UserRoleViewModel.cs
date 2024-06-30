using WebStoreMap.Models.Data;

namespace WebStoreMap.Models.ViewModels.Role
{
    public class UserRoleViewModel
    {
        public UserRoleViewModel()
        {
        }

        public UserRoleViewModel(UserRoleDataTransferObject row)
        {
            UserId = row.UserId;

            RolesId = row.RoleId;
        }

        public int UserId { get; set; }

        public int RolesId { get; set; }
    }
}