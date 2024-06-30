using System.ComponentModel.DataAnnotations;
using System.Web;

namespace WebStoreMap.Models.ViewModels.Account
{
    public class PictureViewModel
    {
        [Required]
        public HttpPostedFileWrapper Picture { get; set; }
    }
}