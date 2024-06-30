using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using WebStoreMap.Models.Data;
using WebStoreMap.Models.ViewModels.Chat;

namespace WebStoreMap.Controllers
{
    public class ChatRoomController : Controller
    {
        // GET: ChatRoom
        public ActionResult Index()
        {
            
            using (DataBase DataBase = new DataBase())
            {
                System.Collections.Generic.List<CommentDataTransferObject> comments = DataBase.Comments.Include(x => x.Replies).OrderByDescending(x => x.Date).ToList();

                return View(comments);
            }
        }

        [HttpPost]
        public ActionResult PostReply(ReplyViewModel Model)
        {
            string UserEmail = User.Identity.Name;
            if (string.IsNullOrEmpty(UserEmail))
            {
                return RedirectToAction("Login", "Account");
            }
            ReplyDataTransferObject r = new ReplyDataTransferObject
            {
                Text = Model.Reply,
                CommentId = Model.CID,
                Date = DateTime.Now
            };
            

            using (DataBase DataBase = new DataBase())
            {
                int UserId = DataBase.Users.Single(x => x.EmailAddress == UserEmail).Id;
                r.UserId = UserId;

                _ = DataBase.Replies.Add(r);
                _ = DataBase.SaveChanges();
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult PostComment(string CommentText)
        {
            string UserEmail = User.Identity.Name;
            if (string.IsNullOrEmpty(UserEmail))
            {
                return RedirectToAction("Login", "Account");
            }
            CommentDataTransferObject Comment = new CommentDataTransferObject
            {
                Text = CommentText,
                Date = DateTime.Now
            };

            using (DataBase DataBase = new DataBase())
            {
                int userId = DataBase.Users.Single(x => x.EmailAddress == UserEmail).Id;
                Comment.UserId = userId;
                _ = DataBase.Comments.Add(Comment);
                _ = DataBase.SaveChanges();
                return RedirectToAction("Index");
            }
        }
    }
}