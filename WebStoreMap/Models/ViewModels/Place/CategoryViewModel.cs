using System.ComponentModel.DataAnnotations;
using WebStoreMap.Models.Data;

namespace WebStoreMap.Models.ViewModels.Place
{
    public class CategoryViewModel
    {
        public CategoryViewModel()
        {
        }

        public CategoryViewModel(CategoryDataTransferObject row)
        {
            Id = row.Id;
            CategoryName = row.CategoryName;
            Slug = row.Slug;
            Sorting = row.Sorting;
        }
       
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public string Slug { get; set; }
        public int Sorting { get; set; }
    }
}