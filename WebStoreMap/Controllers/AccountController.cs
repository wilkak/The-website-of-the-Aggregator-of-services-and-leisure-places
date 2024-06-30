using System;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using WebStoreMap.Models.Data;
using WebStoreMap.Models.Email;
using WebStoreMap.Models.Encoding;
using WebStoreMap.Models.ViewModels.Account;
using WebStoreMap.Models.ViewModels.Email;

namespace WebStoreMap.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        [AllowAnonymous]
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }

        public ActionResult Captcha()
        {
            Random Random = new Random();
            string Combination = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefhijkmnprstuvwxyz";
            StringBuilder CaptchaText = new StringBuilder();
            for (int i = 0; i < 6; i++)
            {
                _ = CaptchaText.Append(Combination[Random.Next(Combination.Length)]);
            }
           
            string Code = CaptchaText.ToString();
            Session["Code"] = Code;

            CaptchaImage Captcha = new CaptchaImage(Code, 200, 50);

            Response.Clear();
            Response.ContentType = "image/jpeg";

            Captcha.Image.Save(Response.OutputStream, ImageFormat.Jpeg);

            Captcha.Dispose();
            return null;
        }

        // GET: /Account/Create-account
        [ActionName("Create-account")]
        [HttpGet]
        [AllowAnonymous]
        public ActionResult CreateAccount()
        {
            return View("CreateAccount");
        }

        // POST: /Account/Create-account
        [ActionName("Create-account")]
        [HttpPost]
        [AllowAnonymous]
        public ActionResult CreateAccount(UserViewModel Model)
        {
            if (Model.Captcha != (string)Session["Code"])
            {
                ModelState.AddModelError("Captcha", "Текст с картинки введен неверно");
                return View("CreateAccount", Model);
            }

            // Проверяем модель на валидность
            if (!ModelState.IsValid)
            {
                return View("CreateAccount", Model);
            }
            int UserId;
            string Email;
            // Проверяем соответствие пароля
            if (!Model.Password.Equals(Model.ConfirmPassword))
            {
                ModelState.AddModelError("", "Пароль не совпадает!");
                return View("CreateAccount", Model);
            }

            using (DataBase DataBase = new DataBase())
            {
                // Проверяем имя на уникальность
                if (DataBase.Users.Any(x => x.EmailAddress.Equals(Model.EmailAddress)))
                {
                    ModelState.AddModelError("", $"Почта {Model.EmailAddress} уже занята.");
                    Model.EmailAddress = "";
                    return View("CreateAccount", Model);
                }

                if (DataBase.Users.Any(x => x.PhoneNumber.Equals(Model.PhoneNumber)))
                {
                    ModelState.AddModelError("", $"Номер телефона {Model.PhoneNumber} уже занят.");
                    Model.PhoneNumber = "";
                    return View("CreateAccount", Model);
                }

               
                RegistrationUserDataTransferObject RegistrationUserDataTransferObject = new RegistrationUserDataTransferObject()
                {
                    FirstName = Model.FirstName,
                    LastName = Model.LastName,
                    EmailAddress = Model.EmailAddress,
                    PhoneNumber = Model.PhoneNumber,
                    BirthDate = Model.BirthDate,
                    Password = Encrypt.GetMD5Hash(Model.Password),
                    EmailConfirmed = false,
                    View = false,
                    ImageName = null
                };

                // Добавлаем данные 
                _ = DataBase.RegistrationUsers.Add(RegistrationUserDataTransferObject);

                _ = DataBase.SaveChanges();
                int RegistrationId = RegistrationUserDataTransferObject.Id;
                UserId = RegistrationId;
                Email = Model.EmailAddress;

                string Captcha = Model.Captcha;
                // Записываем сообщение в TempData
                TempData["SM"] = "Теперь вы зарегистрированы и можете войти в систему.";
                EmailService EmailService = new EmailService();
                EmailViewModel ModelEmail = new EmailViewModel
                {
                    Email = Model.EmailAddress,
                    Subject = "Регистрация"
                };
                Captcha = Encrypt.GetMD5Hash(Captcha);
                string Link = Url.Action("ConfirmEmail", "Account", new { Token = UserId, Email = Email, Captcha = Captcha }, Request.Url.Scheme);
                DateTime DateTime = DateTime.UtcNow;
                LinkDataTransferObject LinkDTO = new LinkDataTransferObject()
                {
                    Link = Link,
                    Date = DateTime,
                    Code = Captcha
                };
                ModelEmail.HtmlMessage = string.Format("Для завершения регистрации перейдите по ссылке:" +
                     "<a href=\"{0}\" Title=\"Подтвердить регистрацию\">{0}</a>",
                     Link);
                _ = DataBase.Links.Add(LinkDTO);
              

                EmailService.SendEmailCustom(ModelEmail);
                // Сохраняем данные
                _ = DataBase.SaveChanges();
            }
            // Переадресовываем

            return View("EmailConfirmed");
        }

        [AllowAnonymous]
        public ActionResult ConfirmEmail(int Token, string Email, string Captcha)
        {
            using (DataBase DataBase = new DataBase())
            {
                LinkDataTransferObject LinkDataTransferObject = DataBase.Links.FirstOrDefault(x => x.Code == Captcha);
                DateTime DateTime = DateTime.UtcNow;
                if (LinkDataTransferObject != null && LinkDataTransferObject.Date.Day == DateTime.Day && LinkDataTransferObject.Date.Year == DateTime.Year && LinkDataTransferObject.Date.Month == DateTime.Month)
                {
                    RegistrationUserDataTransferObject RegistrationUserDataTransferObject = DataBase.RegistrationUsers.Find(Token);
                    if (DataBase.Users.Any(x => x.EmailAddress.Equals(Email)))
                    {
                        
                        return View("Noactive");
                        
                    }
                    if (RegistrationUserDataTransferObject != null)
                    {
                        if (RegistrationUserDataTransferObject.EmailAddress == Email)
                        {
                            RegistrationUserDataTransferObject.EmailConfirmed = true;
                            _ = DataBase.SaveChanges();

                            UserDataTransferObject UserDataTransferObject = new UserDataTransferObject()
                            {
                                FirstName = RegistrationUserDataTransferObject.FirstName,
                                LastName = RegistrationUserDataTransferObject.LastName,
                                EmailAddress = RegistrationUserDataTransferObject.EmailAddress,
                                PhoneNumber = RegistrationUserDataTransferObject.PhoneNumber,
                                BirthDate = RegistrationUserDataTransferObject.BirthDate,
                                Password = RegistrationUserDataTransferObject.Password,
                                EmailConfirmed = RegistrationUserDataTransferObject.EmailConfirmed,
                                View = RegistrationUserDataTransferObject.View,
                                ImageName = RegistrationUserDataTransferObject.ImageName
                            };

                            // Добавлаем данные
                            _ = DataBase.Users.Add(UserDataTransferObject);
                            _ = DataBase.SaveChanges();
                            UserDataTransferObject UserDataTransferObject2 = DataBase.Users.FirstOrDefault(x => x.EmailAddress == RegistrationUserDataTransferObject.EmailAddress);
                           
                            int UserId = UserDataTransferObject2.Id;
                            UserRoleDataTransferObject UserRoleDataTransferObject = new UserRoleDataTransferObject()
                            {
                                UserId = UserId,
                                RoleId = 3
                            };

                            _ = DataBase.UserRoles.Add(UserRoleDataTransferObject);

                            _ = DataBase.SaveChanges();

                            #region Upload Image

                            // Создаём необходимые директории
                            DirectoryInfo OriginalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));

                            string PathString1 = Path.Combine(OriginalDirectory.ToString(), "Profile");
                            string PathString2 = Path.Combine(OriginalDirectory.ToString(), "Profile\\" + UserId.ToString());
                            string PathString3 = Path.Combine(OriginalDirectory.ToString(), "Profile\\" + UserId.ToString() + "\\Thumbs");

                            // Проверяем, есть ли дериктория по пути
                            if (!Directory.Exists(PathString1))
                            {
                                _ = Directory.CreateDirectory(PathString1);
                            }

                            if (!Directory.Exists(PathString2))
                            {
                                _ = Directory.CreateDirectory(PathString2);
                            }

                            if (!Directory.Exists(PathString3))
                            {
                                _ = Directory.CreateDirectory(PathString3);
                            }

                            #endregion Upload Image

                            return RedirectToAction("Login", "Account");
                        }
                        else
                        {
                            return View("Noactive");
                            
                        }
                    }
                    else
                    {
                        return View("Noactive");
                        
                    }
                }
                else
                {
                    return View("Noactive");
                   
                }
            }

        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View("ForgotPassword");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ForgotPassword(ForgotPasswordViewModel Model)
        {
            if (Model.Captcha != (string)Session["Code"])
            {
                ModelState.AddModelError("Captcha", "Текст с картинки введен неверно");
            }

            if (ModelState.IsValid)
            {
                using (DataBase DataBase = new DataBase())
                {
                    UserDataTransferObject UserDataTransferObject = DataBase.Users.FirstOrDefault(x => x.EmailAddress == Model.Email);

                    if (UserDataTransferObject == null || UserDataTransferObject.EmailAddress == null)
                    {
                        // пользователь с данным Email может отсутствовать в бд
                        // тем не менее мы выводим стандартное сообщение, чтобы скрыть
                        // наличие или отсутствие пользователя в бд
                        return View("ForgotPasswordConfirmation");
                    }
                    string Email = UserDataTransferObject.EmailAddress;
                    string Captcha = Model.Captcha;
                    int UserId = UserDataTransferObject.Id;

                    Captcha = Encrypt.GetMD5Hash(Captcha);
                    string Link = Url.Action("ResetPassword", "Account", new { Token = UserId, Email, Captcha }, Request.Url.Scheme);
                    DateTime DateTime = DateTime.UtcNow;
                    LinkDataTransferObject LinkDataTransferObject = new LinkDataTransferObject()
                    {
                        Link = Link,
                        Date = DateTime,
                        Code = Captcha
                    };

                    EmailService EmailService = new EmailService();
                    EmailViewModel ModelEmail = new EmailViewModel();
                    ModelEmail.Email = Model.Email;
                    ModelEmail.Subject = "Сброс пароля";
                    ModelEmail.HtmlMessage = string.Format("Для сброса пароля перейдите по ссылке:" +
                                 "<a href=\"{0}\" title=\"Подтвердить сброс пароля\">{0}</a>",
                                 Link);
                   

                    _ = DataBase.Links.Add(LinkDataTransferObject);
                   
                    EmailService.SendEmailCustom(ModelEmail);

                    // Сохраняем данные
                    _ = DataBase.SaveChanges();
                }
                return View("ForgotPasswordConfirmation");
            }
            return View(Model);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ResetPassword(string Token = null, string Email = null, string Captcha = null)
        {
            using (DataBase DataBase = new DataBase())
            {
                LinkDataTransferObject LinkDataTransferObject = DataBase.Links.FirstOrDefault(x => x.Code == Captcha);
                DateTime DateTime = DateTime.UtcNow;
                return LinkDataTransferObject != null && LinkDataTransferObject.Date.Day == DateTime.Day && LinkDataTransferObject.Date.Year == DateTime.Year && LinkDataTransferObject.Date.Month == DateTime.Month
                    ? Token == null ? View("Error") : View()
                    : (ActionResult)View("Noactive");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ResetPassword(ResetPasswordViewModel Model)
        {
           
            using (DataBase DataBase = new DataBase())
            {
                if (!ModelState.IsValid)
                {
                    return View(Model);
                }
                UserDataTransferObject UserDataTransferObject = DataBase.Users.FirstOrDefault(x => x.EmailAddress == Model.Email);

                if (UserDataTransferObject == null)
                {
                    return View("ResetPasswordConfirmation");
                }

                if (Model.Password == Model.ConfirmPassword)
                {
                    UserDataTransferObject.Password = Encrypt.GetMD5Hash(Model.Password);
                    _ = DataBase.SaveChanges();
                    return RedirectToAction("Login");
                }
            }
            return View(Model);
        }

        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            // подтвердить, что пользователь не авторизован
            string UserEmail = User.Identity.Name;

            if (!string.IsNullOrEmpty(UserEmail))
            {
                return RedirectToAction("user-profile");
            }

            // Вернуть представление
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginUserViewModel Model)
        {
            // Проверяем модель на валидность
            if (!ModelState.IsValid)
            {
                return View(Model);
            }
            // Проеряем введенные данные на валидность
            bool Valid = false;

            using (DataBase DataBase = new DataBase())
            {
                string Password = Encrypt.GetMD5Hash(Model.Password);//шифруем введенный пароль для сверки с хранимым 
                if (DataBase.Users.Any(x => x.EmailAddress.Equals(Model.EmailAddress) && x.Password.Equals(Password) && x.EmailConfirmed))
                {
                    Valid = true;
                }
            }

            if (!Valid)
            {
                ModelState.AddModelError("", "Неверное почта пользователя или пароль.");

                return View(Model);
            }
            else
            {
                FormsAuthentication.SetAuthCookie(Model.EmailAddress, Model.RememberMe);
             
                return Redirect(FormsAuthentication.GetRedirectUrl(Model.EmailAddress, Model.RememberMe));
            }
        }

        // GET: /Account/Logout
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        [Authorize]
        public ActionResult UserNavPartial()
        {
            // Получаем Имя пользователя
            string UserEmail = User.Identity.Name;

            // Объявляем модель
            UserNavigationPartialViewModel Model;

            using (DataBase DataBase = new DataBase())
            {
                // Получаем пользователя
                UserDataTransferObject UserDataTransferObject = DataBase.Users.FirstOrDefault(x => x.EmailAddress == UserEmail);

                // Создаём модель
                Model = new UserNavigationPartialViewModel()
                {
                    FirstName = UserDataTransferObject.FirstName,
                    LastName = UserDataTransferObject.LastName
                };
            }
            // Возвращаем частичное представление с моделью
            return PartialView(Model);
        }

        // GET: /Account/User-profile
        [HttpGet]
        [ActionName("User-profile")]
        [Authorize]
        public ActionResult UserProfile()
        {
            // Получаем имя пользователя
            string userEmail = User.Identity.Name;

            // Объявляем модель
            UserProfileViewModel Model;

            using (DataBase DataBase = new DataBase())
            {
                // Получаем пользователя
                UserDataTransferObject UserDataTransferObject = DataBase.Users.FirstOrDefault(x => x.EmailAddress == userEmail);

                // Инициализируем модель данными
                Model = new UserProfileViewModel(UserDataTransferObject);
            }
            // Возвращаем модель в представление
            return View("UserProfile", Model);
        }

        // POST: /Account/User-profile
        [HttpPost]
        [ActionName("User-profile")]
        [Authorize]
        public ActionResult UserProfile(UserProfileViewModel Model)
        {
            if (Model.Captcha != (string)Session["Code"])
            {
                ModelState.AddModelError("Captcha", "Текст с картинки введен неверно");
            }

            bool UserNameChanged = false;

            // Проверяем модель на валидность
            if (!ModelState.IsValid)
            {
                return View("UserProfile", Model);
            }

            // Проверяем пароль (если пользователь его вводит/меняет)
            if (!string.IsNullOrWhiteSpace(Model.Password))
            {
                if (!Model.Password.Equals(Model.ConfirmPassword))
                {
                    ModelState.AddModelError("", "Пароли не совпадают.");
                    return View("UserProfile", Model);
                }
            }

            using (DataBase DataBase = new DataBase())
            {
                // Получаем имя пользователя
                string UserEmail = User.Identity.Name;

                if (UserEmail != Model.EmailAddress)
                {
                    UserEmail = Model.EmailAddress;
                    UserNameChanged = true;
                }

                // Проверяем, что почта пользователя - уникальна
                if (DataBase.Users.Where(x => x.Id != Model.Id).Any(x => x.EmailAddress == UserEmail))
                {
                    ModelState.AddModelError("", $"На почту {Model.EmailAddress} уже зарегистрирован другой аккаунт.");
                    Model.EmailAddress = "";
                    return View("UserProfile", Model);
                }

                // Проверяем, что почта пользователя - уникальна
                if (DataBase.Users.Where(x => x.Id != Model.Id).Any(x => x.PhoneNumber == Model.PhoneNumber))
                {
                    ModelState.AddModelError("", $"На номер телефона {Model.PhoneNumber} уже зарегистрирован другой аккаунт.");
                    Model.PhoneNumber = "";
                    return View("UserProfile", Model);
                }

                // Изменяем контекст данных 
                UserDataTransferObject UserDataTransferObject = DataBase.Users.Find(Model.Id);

                UserDataTransferObject.FirstName = Model.FirstName;
                UserDataTransferObject.LastName = Model.LastName;
                UserDataTransferObject.EmailAddress = Model.EmailAddress;
                UserDataTransferObject.PhoneNumber = Model.PhoneNumber;
               

                if (!string.IsNullOrWhiteSpace(Model.Password))
                {
                    UserDataTransferObject.Password = Encrypt.GetMD5Hash(Model.Password);
                }

                // Сохраняем изменения
                _ = DataBase.SaveChanges();
            }

            // Устанавливаем сообщение в TempData
            TempData["SM"] = "Вы отредактировали свой профиль!";

            if (!UserNameChanged)
            {
                // Переадресовываем пользователя
                return View("UserProfile", Model);
            }
            else
            {
                return RedirectToAction("Logout");
            }
        }

        [HttpGet]
        // [ActionName("user-profile-info")]
        [Authorize]
        public ActionResult UserProfileInfo()
        {
            // Получаем имя пользователя
            string UserEmail = User.Identity.Name;

            // Объявляем модель
            UserProfileInformationViewModel Model;

            using (DataBase DataBase = new DataBase())
            {
                // Получаем пользователя
                UserDataTransferObject UserDataTransferObject = DataBase.Users.FirstOrDefault(x => x.EmailAddress == UserEmail);

                // Инициализируем модель данными
                Model = new UserProfileInformationViewModel(UserDataTransferObject);
            }
            // Возвращаем модель в представление
            return View("UserProfileInfo", Model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdatePicture(PictureViewModel Model)
        {
            // Получаем имя пользователя
            string UserEmail = User.Identity.Name;
            if (string.IsNullOrEmpty(UserEmail))
            {
                return RedirectToAction("Login", "Account");
            }
            UserProfileInformationViewModel Models;
            if (!ModelState.IsValid)
            {
                using (DataBase DataBase = new DataBase())
                {
                    UserDataTransferObject UserDataTransferObject = DataBase.Users.FirstOrDefault(x => x.EmailAddress == UserEmail);
                    Models = new UserProfileInformationViewModel(UserDataTransferObject);

                    return View("UserProfileInfo", Models);
                }
            }
            // Объявляем модель

            int UserId;
            using (DataBase DataBase = new DataBase())
            {
                // Получаем пользователя
                UserDataTransferObject UserDataTransferObject = DataBase.Users.FirstOrDefault(x => x.EmailAddress == UserEmail);
                UserId = UserDataTransferObject.Id;
            }

            System.Web.HttpPostedFileWrapper File = Model.Picture;

            #region Image Upload

            // Проверяем загрузку файла
            if (File != null && File.ContentLength > 0)
            {
                // Получаем расширение файла
                string Ext = File.ContentType.ToLower();

                // Проверяем расширение
                if (Ext != "image/jpg" &&
                    Ext != "image/jpeg" &&
                    Ext != "image/pjpeg" &&
                    Ext != "image/gif" &&
                    Ext != "image/x-png" &&
                    Ext != "image/png")
                {
                    using (DataBase DataBase = new DataBase())
                    {
                        UserDataTransferObject UserDataTransferObject = DataBase.Users.FirstOrDefault(x => x.EmailAddress == UserEmail);
                        Models = new UserProfileInformationViewModel(UserDataTransferObject);
                        ModelState.AddModelError("", "Изображение не было загружено - неправильное расширение изображения");
                       
                        return View("UserProfileInfo", Models);
                    }
                }

                // Устанавливаем пути загрузки
                DirectoryInfo OriginalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));

                string PathString1 = System.IO.Path.Combine(OriginalDirectory.ToString(), "Profile\\" + UserId.ToString());
                string PathString2 = System.IO.Path.Combine(OriginalDirectory.ToString(), "Profile\\" + UserId.ToString() + "\\Thumbs");

                // Удаляем существующие файлы в директориях
                DirectoryInfo Directory1 = new DirectoryInfo(PathString1);
                DirectoryInfo Directory2 = new DirectoryInfo(PathString2);

                foreach (FileInfo File2 in Directory1.GetFiles())
                {
                    File2.Delete();
                }

                foreach (FileInfo File3 in Directory2.GetFiles())
                {
                    File3.Delete();
                }

                // Сохраняем имя изображения
                string ImageName = File.FileName;

                using (DataBase DataBase = new DataBase())
                {
                    UserDataTransferObject UserDataTransferObject = DataBase.Users.Find(UserId);
                    UserDataTransferObject.ImageName = ImageName;

                    _ = DataBase.SaveChanges();
                }

                // Сохраняем оригинал и превью версии изображений
                string Path = string.Format($"{PathString1}\\{ImageName}");
                string Path2 = string.Format($"{PathString2}\\{ImageName}");

                // Сохраняем оригинальное изображение
                File.SaveAs(Path);

                // Создаём и сохраняем уменьшенную копию
                WebImage Image = new WebImage(File.InputStream);
                _ = Image.Resize(200, 200).Crop(1, 1);
                _ = Image.Save(Path2);
            }

            #endregion Image Upload

            using (DataBase DataBase = new DataBase())
            {
                UserDataTransferObject UserDataTransferObject = DataBase.Users.FirstOrDefault(x => x.EmailAddress == UserEmail);
                Models = new UserProfileInformationViewModel(UserDataTransferObject);

                return View("UserProfileInfo", Models);
            }
        }
    }

   
}