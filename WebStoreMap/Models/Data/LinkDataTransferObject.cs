using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebStoreMap.Models.Data
{
    [Table("TableLinks")]
    public class LinkDataTransferObject
    {
        
        public int Id { get; set; }
        public string Link { get; set; }
        public DateTime Date { get; set; }
        public string Code { get; set; }
    }
}