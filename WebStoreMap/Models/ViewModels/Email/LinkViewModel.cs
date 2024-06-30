using System;
using System.ComponentModel.DataAnnotations;
using WebStoreMap.Models.Data;

namespace WebStoreMap.Models.ViewModels.Email
{
    public class LinkViewModel
    {
        public LinkViewModel()
        {
        }

        public LinkViewModel(LinkDataTransferObject row)
        {
            Id = row.Id;
            Link = row.Link;
            Date = row.Date;
            Code = row.Code;
        }
        
        public int Id { get; set; }
        public string Link { get; set; }
        public DateTime Date { get; set; }
        public string Code { get; set; }
    }
}