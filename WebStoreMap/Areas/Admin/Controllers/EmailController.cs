using System.Web.Mvc;
using WebStoreMap.Models.Email;
using WebStoreMap.Models.ViewModels.Email;

namespace WebStoreMap.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EmailController : Controller
    {
        // GET: Admin/Email
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        // GET:
        [HttpGet]
        [Authorize]
        public ActionResult SendEmail()
        {
            // Объявляем модель
            EmailViewModel Model = new EmailViewModel();

            // Возвращаем модель в представление
            return View(Model);
        }

        // POST:
        [HttpPost]
        [Authorize]
        public ActionResult SendEmail(EmailViewModel Model)
        {
            EmailService EmailService = new EmailService();
            EmailService.SendEmailCustom(Model);

            // Добавляем сообщение в TempData
            TempData["SM"] = "Вы отпрвавили письмо!";

            // Переадресовываем пользователя
            return RedirectToAction("SendEmail");
        }
    }
}