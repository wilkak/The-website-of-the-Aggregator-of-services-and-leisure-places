using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebStoreMap.Models.Email;
using WebStoreMap.Models.ViewModels.Email;

namespace WebStoreMap.Areas.Manager.Controllers
{
    [Authorize(Roles = "Manager")]
    public class EmailController : Controller
    {
        // GET: Manager/Email
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
            TempData["SM"] = "Вы отправили письмо!";

            // Переадресовываем пользователя
            return RedirectToAction("SendEmail");
        }
    }
}