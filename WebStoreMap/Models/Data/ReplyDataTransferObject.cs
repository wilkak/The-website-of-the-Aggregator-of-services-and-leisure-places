using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebStoreMap.Models.Data
{
    [Table("TableReply")]
    public class ReplyDataTransferObject
    {
    
        public int Id { get; set; }

        [Required]
        [StringLength(1000, ErrorMessage = "Комментарий должен содержать как минимум 1 символов", MinimumLength = 1)]
        public string Text { get; set; }

        public int CommentId { get; set; }
        public DateTime Date { get; set; }
        public int UserId { get; set; }

       
        [ForeignKey("UserId")]
        public virtual UserDataTransferObject User { get; set; }

        [ForeignKey("CommentId")]
        public virtual CommentDataTransferObject Comment { get; set; }
    }
}