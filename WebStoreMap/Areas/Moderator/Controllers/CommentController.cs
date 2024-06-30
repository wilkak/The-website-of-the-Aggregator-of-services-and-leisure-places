using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebStoreMap.Models.Data;
using WebStoreMap.Models.ViewModels.Chat;

namespace WebStoreMap.Areas.Moderator.Controllers
{
    [Authorize(Roles = "Moderator")]
    public class CommentController : Controller
    {
        // GET: Moderator/Comment

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult Comments(int? Page)
        {
            // Объявляем PlaceViewModel типа лист
            List<CommentViewModel> ListCommentViewModel;
            // Устанавливаем номер страницы
            int PageNumber = Page ?? 1;
            using (DataBase DataBase = new DataBase())
            {
                // Инициализируем лист
                ListCommentViewModel = DataBase.Comments.ToArray()
                       .Select(x => new CommentViewModel(x))
                       .ToList();
            }
            // Устанавливаем постраничную навигацию
            IPagedList<CommentViewModel> onePageOfComment = ListCommentViewModel.ToPagedList(PageNumber, 7);
            ViewBag.OnePageOfComment = onePageOfComment;

            // Возвращаем представление и лист
            return View(ListCommentViewModel);
        }

        // Создаём метод для удаления комментариев
        // POST: Admin
        [Authorize]
        public ActionResult DeleteComment(int Id)
        {
            // Удаляем из базы данных
            using (DataBase DataBase = new DataBase())
            {
                CommentDataTransferObject CommentDataTransferObject = DataBase.Comments.Find(Id);

                List<ReplyDataTransferObject> ListReplyDataTransferObject = DataBase.Replies.Where(x => x.CommentId == Id).ToList();
                foreach (ReplyDataTransferObject Reply in ListReplyDataTransferObject)
                {
                    _ = DataBase.Replies.Remove(Reply);
                }

                _ = DataBase.Comments.Remove(CommentDataTransferObject);
                _ = DataBase.SaveChanges();
            }
            // Переадресовываем пользователя
            return RedirectToAction("Comments");
        }

        [Authorize]
        public ActionResult Replies(int? Page)
        {

            List<ReplyPlaceViewModel> ListReplyViewModel;
            // Устанавливаем номер страницы
            int PageNumber = Page ?? 1;
            using (DataBase DataBase = new DataBase())
            {
                // Инициализируем лист
                ListReplyViewModel = DataBase.Replies.ToArray()
                       .Select(x => new ReplyPlaceViewModel(x))
                       .ToList();
            }
            // Устанавливаем постраничную навигацию
            IPagedList<ReplyPlaceViewModel> onePageOfReply = ListReplyViewModel.ToPagedList(PageNumber, 7);
            ViewBag.OnePageOfReply = onePageOfReply;

            // Возвращаем представление и лист
            return View(ListReplyViewModel);
        }

        // Создаём метод для удаления ответов к комментариям
        // POST: 
        [Authorize]
        public ActionResult DeleteReply(int Id)
        {
            // Удаляем из базы данных
            using (DataBase DataBase = new DataBase())
            {
                ReplyDataTransferObject ReplyDataTransferObject = DataBase.Replies.Find(Id);

                _ = DataBase.Replies.Remove(ReplyDataTransferObject);
                _ = DataBase.SaveChanges();
            }
            // Переадресовываем пользователя
            return RedirectToAction("Replies");
        }
    }
}