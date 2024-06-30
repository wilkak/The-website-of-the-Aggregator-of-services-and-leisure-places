using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebStoreMap.Models.Data;
using WebStoreMap.Models.ViewModels.Place;

namespace WebStoreMap.Models.ViewModels.Chat
{
  
    public class PlaceCommentViewModel
    {
        public PlaceCommentViewModel(List<CommentPlaceViewModel> Comments, PlaceViewModel Place)
        {
            this.Comments = Comments;
            this.Place = Place;
        }

        public List<CommentPlaceViewModel> Comments { get; set; }
        public PlaceViewModel Place { get; set; }
    }
}