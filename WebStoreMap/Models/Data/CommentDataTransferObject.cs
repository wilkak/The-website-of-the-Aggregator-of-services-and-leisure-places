using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebStoreMap.Models.Data
{
    [Table("TableComments")]
    public class CommentDataTransferObject
    {
         
        public int Id { get; set; }

        [Required]
        [StringLength(1000, ErrorMessage = "Комментарий должен содержать как минимум 1 символов", MinimumLength = 1)]
        public string Text { get; set; }

        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public int PlaceId { get; set; }

        [ForeignKey("PlaceId")]
        public virtual PlaceDataTransferObject Place { get; set; }

        [ForeignKey("UserId")]
        public virtual UserDataTransferObject User { get; set; }

        public ICollection<ReplyDataTransferObject> Replies { get; set; }
    }
}