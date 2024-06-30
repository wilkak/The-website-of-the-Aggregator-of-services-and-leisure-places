using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebStoreMap.Models.Data;
using WebStoreMap.Models.ViewModels.Place;

namespace WebStoreMap.Models.ViewModels.Rating
{
  
    public class PlaceRatingViewModel
    {
        public PlaceRatingViewModel(RatingViewModel Review, PlaceViewModel Place)
        {
            this.Review = Review;
            this.Place = Place;
        }

        public RatingViewModel Review { get; set; }
        public PlaceViewModel Place { get; set; }
    }
}