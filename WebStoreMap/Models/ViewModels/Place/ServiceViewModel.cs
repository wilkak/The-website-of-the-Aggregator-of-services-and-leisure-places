using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;
using WebStoreMap.Models.Data;
using System.Web.Mvc;

namespace WebStoreMap.Models.ViewModels.Place
{
    public class ServiceViewModel
    {
        public ServiceViewModel()
        {
        }

        public ServiceViewModel(ServiceDataTransferObject row)
        {
            Id = row.Id;
            ServiceName = row.ServiceName;
            Slug = row.Slug;
            Description = row.Description;
            Price = row.Price;
            PlaceId = row.PlaceId;
            ImageName = row.ImageName;
            OldPrice = row.OldPrice;
            View = row.View;
            UserId = row.UserId;

        }
        public int Id { get; set; }
        [Required(ErrorMessage = "Поле название должно быть заполнено правильно")]
        [DisplayName("Название")]
        [StringLength(150, ErrorMessage = "Название должен содержать как минимум 3 символа, и как максимум 150 символов", MinimumLength = 3)]
        public string ServiceName { get; set; }
        public string Slug { get; set; }
        [Required(ErrorMessage = "Поле описание должно быть заполнено правильно")]
        [DisplayName("Описание")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        [DisplayName("Цена")]
        [Required(ErrorMessage = "Поле цены должно быть заполнено правильно")]
        public decimal Price { get; set; }

        public decimal OldPrice { get; set; }

        public int UserId { get; set; }

        public int PlaceId { get; set; }

        public bool View { get; set; }
        [DisplayName("Изображение")]
        public string ImageName { get; set; }

        public IEnumerable<string> GalleryImages { get; set; }
        public IEnumerable<SelectListItem> Place { get; set; }
    }

}