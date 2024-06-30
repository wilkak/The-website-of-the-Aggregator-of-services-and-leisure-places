using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebStoreMap.Models.Data
{
    [Table("TableCategories")]
    public class CategoryDataTransferObject
    {
        
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public string Slug { get; set; }
        public int Sorting { get; set; }
    }
}