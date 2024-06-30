using System.Web.Mvc;

namespace WebStoreMap.Models.ViewModels.Search
{
    public class SearchViewModel
    {
        public SelectList CategoryViewModel { get; set; }
        public SelectList PlaceViewModel { get; set; }
    }
}