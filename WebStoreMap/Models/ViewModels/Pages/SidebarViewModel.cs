using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using WebStoreMap.Models.Data;

namespace WebStoreMap.Models.ViewModels.Pages
{
    public class SidebarViewModel
    {
        public SidebarViewModel()
        {
        }

        public SidebarViewModel(SidebarDataTransferObject row)
        {
            Id = row.Id;
            Body = row.Body;
        }
       
        public int Id { get; set; }

        [AllowHtml]
        public string Body { get; set; }
    }
}