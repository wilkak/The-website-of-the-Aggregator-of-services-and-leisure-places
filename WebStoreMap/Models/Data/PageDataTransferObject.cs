using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebStoreMap.Models.Data
{
    [Table("TablePages")]
    public class PageDataTransferObject
    {
        
        public int Id { get; set; }

        public string Title { get; set; }
        public string Slug { get; set; }
        public string Body { get; set; }
        public int Sorting { get; set; }
        public bool Sidebar { get; set; }
    }
}