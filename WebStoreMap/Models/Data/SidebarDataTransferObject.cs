using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebStoreMap.Models.Data
{
    [Table("TableSidebars")]
    public class SidebarDataTransferObject
    {
        
        public int Id { get; set; }

        public string Body { get; set; }
    }
}