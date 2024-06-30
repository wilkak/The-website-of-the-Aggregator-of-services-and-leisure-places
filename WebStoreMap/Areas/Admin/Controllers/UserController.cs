using PagedList;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using WebStoreMap.Models.Data;
using WebStoreMap.Models.Encoding;
using WebStoreMap.Models.ViewModels.Account;
using WebStoreMap.Models.ViewModels.Company;
using WebStoreMap.Models.ViewModels.Role;
using WebStoreMap.Models.ViewModels.Place;

namespace WebStoreMap.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult Users(int? Page, int? RoleId)
        {
            // Объявляем UserViewModel типа лист
            List<UserViewModel> ListUserViewModel;
            List<UserRoleInformationViewModel> ListUserRoleInformationViewModel = new List<UserRoleInformationViewModel>();
            List<UserRoleViewModel> ListUserRoleViewModel;
            List<RoleViewModel> ListRoleViewModel;

            // Устанавливаем номер страницы
            int PageNumber = Page ?? 1;

            using (DataBase DataBase = new DataBase())
            {
                // Инициализируем лист
                ListUserViewModel = DataBase.Users.ToArray()
                       .Select(x => new UserViewModel(x))
                       .ToList();
                ListUserRoleViewModel = DataBase.UserRoles.ToArray()
                        .Where(x => RoleId == null || RoleId == 0 || x.RoleId == RoleId)
                        .Select(x => new UserRoleViewModel(x))
                        .ToList();
                ListRoleViewModel = DataBase.Roles.ToArray()
                        .Select(x => new RoleViewModel(x))
                        .ToList();

                foreach (RoleViewModel Role in ListRoleViewModel)
                {
                    foreach (UserRoleViewModel UserRole in ListUserRoleViewModel)
                    {
                        foreach (UserViewModel User in ListUserViewModel)
                        {
                            if (User.Id == UserRole.UserId && UserRole.RolesId == Role.Id)
                            {
                                ListUserRoleInformationViewModel.Add(new UserRoleInformationViewModel(User.Id, User.FirstName, User.LastName, User.EmailAddress, User.PhoneNumber, User.BirthDate, User.Password, Role.Name, UserRole.RolesId, User.View, User.EmailConfirmed));
                            }
                        }
                    }
                }

                ViewBag.Roles = new SelectList(DataBase.Roles.ToList(), "RoleId", "RoleName");

                // Устанавливаем выбранную
                ViewBag.SelectedRol = RoleId.ToString();
            }

            // Устанавливаем постраничную навигацию
            IPagedList<UserRoleInformationViewModel> onePageOfUsers = ListUserRoleInformationViewModel.ToPagedList(PageNumber, 7);
            ViewBag.OnePageOfUsers = onePageOfUsers;

            // Возвращаем представление и лист
            return View(ListUserRoleInformationViewModel);
        }

        // GET: Admin/User
        [HttpGet]
        [Authorize]
        public ActionResult AddUser()
        {
            // Объявляем модель
            UserRoleInformationViewModel Model = new UserRoleInformationViewModel();
            using (DataBase DataBase = new DataBase())
            {
                Model.Role = new SelectList(DataBase.Roles.ToList(), "Id", "ServiceName");
            }
            // Возвращаем модель в представление
            return View(Model);
        }

        // POST: Admin/User
        [HttpPost]
        [Authorize]
        public ActionResult AddUser(UserRoleInformationViewModel Model)
        {
            // Проверяем модель на валидность
            if (!ModelState.IsValid)
            {
                return View("AddUser", Model);
            }

            // Проверяем соответствие пароля
            if (!Model.Password.Equals(Model.ConfirmPassword))
            {
                ModelState.AddModelError("", "Пароль не совпадает!");
                return View("AddUser", Model);
            }

            using (DataBase DataBase = new DataBase())
            {
                if (DataBase.Users.Any(x => x.EmailAddress.Equals(Model.EmailAddress)))
                {
                    ModelState.AddModelError("", $"На почту {Model.EmailAddress} уже создан аккаунт.");
                    Model.EmailAddress = "";
                    return View("AddUser", Model);
                }

                if (DataBase.Users.Any(x => x.PhoneNumber.Equals(Model.PhoneNumber)))
                {
                    ModelState.AddModelError("", $"На номер {Model.PhoneNumber} уже создан аккаунт.");
                    Model.PhoneNumber = "";
                    return View("AddUser", Model);
                }

                // Создаём экземплар класса UserDataTransferObject
                UserDataTransferObject UserDataTransferObject = new UserDataTransferObject()
                {
                    FirstName = Model.FirstName,
                    LastName = Model.LastName,
                    EmailAddress = Model.EmailAddress,
                    PhoneNumber = Model.PhoneNumber,
                    BirthDate = Model.BirthDate,
                    Password = Encrypt.GetMD5Hash(Model.Password)
                };

                // Добавлаем данные в DTO
                _ = DataBase.Users.Add(UserDataTransferObject);

                // Сохраняем данные
                _ = DataBase.SaveChanges();

                // Добавлаем роль в userRolesDTO
                int Id = UserDataTransferObject.Id;

                UserRoleDataTransferObject UserRoleDataTransferObject = new UserRoleDataTransferObject()
                {
                    UserId = Id,
                    RoleId = Model.RoleId
                };

                _ = DataBase.UserRoles.Add(UserRoleDataTransferObject);
                _ = DataBase.SaveChanges();
            }

            // Добавляем сообщение в TempData
            TempData["SM"] = "Вы добавили пользователя!";

            // Переадресовываем пользователя
            return RedirectToAction("AddUser");
        }

        //Создаём метод редактирования пользователя
        // GET: Admin/User/EditUser/Id
        [HttpGet]
        [Authorize]
        public ActionResult EditUser(int Id)
        {
            // Объявляем модель UserViewModel
            UserRoleInformationViewModel Model;

            using (DataBase DataBase = new DataBase())
            {
                // Получаем пользователя
                UserDataTransferObject UserDataTransferObject = DataBase.Users.FirstOrDefault(x => x.Id == Id);
                UserRoleDataTransferObject UserRoleDataTransferObject = DataBase.UserRoles.FirstOrDefault(x => x.UserId == Id);
                RoleDataTransferObject RoleDataTransferObject = DataBase.Roles.FirstOrDefault(x => x.RoleId == UserRoleDataTransferObject.RoleId);
                // Инициализируем модель данными

                Model = new UserRoleInformationViewModel(UserDataTransferObject.Id, UserDataTransferObject.FirstName, UserDataTransferObject.LastName, UserDataTransferObject.EmailAddress, UserDataTransferObject.PhoneNumber, UserDataTransferObject.BirthDate, UserDataTransferObject.Password, RoleDataTransferObject.RoleName, UserRoleDataTransferObject.RoleId, UserDataTransferObject.View, UserDataTransferObject.EmailConfirmed)
                {
                    Role = new SelectList(DataBase.Roles.ToList(), "RoleId", "RoleName")
            
                };
            }
            // Возвращаем модель в представление
            return View("EditUser", Model);
        }

        // Создаём метод редактирования пользователя
        // POST: Admin
        [HttpPost]
        [Authorize]
        public ActionResult EditUser(UserRoleInformationViewModel Model)
        {
            // Получаем ID
            int Id = Model.Id;
            using (DataBase DataBase = new DataBase())
            {
                Model.Role = new SelectList(DataBase.Roles.ToList(), "RoleId", "RoleName");
            }
            // Проверяем модель на валидность
            if (!ModelState.IsValid)
            {
                return View("EditUser", Model);
            }

            using (DataBase DataBase = new DataBase())
            {
                // Получаем имя пользователя
                string UserEmail = User.Identity.Name;

                UserEmail = Model.EmailAddress;

                // Проверяем, что имя пользователя - уникально
                if (DataBase.Users.Where(x => x.Id != Model.Id).Any(x => x.EmailAddress == UserEmail))
                {
                    ModelState.AddModelError("", $"На почту {Model.EmailAddress} уже создан аккаунт.");
                    Model.EmailAddress = "";
                    return View("EditUser", Model);
                }

                if (DataBase.Users.Where(x => x.Id != Model.Id).Any(x => x.PhoneNumber == Model.PhoneNumber))
                {
                    ModelState.AddModelError("", $"На номер {Model.PhoneNumber} уже создан аккаунт.");
                    Model.PhoneNumber = "";
                    return View("EditUser", Model);
                }

                // Изменяем контекст данных (DTO)
                UserDataTransferObject UserDataTransferObject = DataBase.Users.Find(Model.Id);

                UserDataTransferObject.FirstName = Model.FirstName;
                UserDataTransferObject.LastName = Model.LastName;
                UserDataTransferObject.EmailAddress = Model.EmailAddress;
                UserDataTransferObject.PhoneNumber = Model.PhoneNumber;
                UserDataTransferObject.BirthDate = Model.BirthDate;
                UserDataTransferObject.View = Model.View;
                _ = DataBase.SaveChanges();

                UserRoleDataTransferObject UserRoleDataTransferObject = DataBase.UserRoles.Single(c => c.UserId == Model.Id);

                _ = DataBase.UserRoles.Remove(UserRoleDataTransferObject);
                _ = DataBase.SaveChanges();

                UserRoleDataTransferObject UserRoleDataTransferObject2 = new UserRoleDataTransferObject()
                {
                    UserId = Id,
                    RoleId = Model.RoleId
                };

                _ = DataBase.UserRoles.Add(UserRoleDataTransferObject2);
                _ = DataBase.SaveChanges();

                if (!string.IsNullOrWhiteSpace(Model.Password))
                {
                    UserDataTransferObject.Password = Encrypt.GetMD5Hash(Model.Password);
                }
                List<PlaceViewModel> ListPlaceViewModel;
                ListPlaceViewModel = DataBase.Places.Where(x => x.UserId == Id).Select(x => new PlaceViewModel(x)).ToList();
                // Обновляем 
                foreach (PlaceViewModel Place in ListPlaceViewModel)
                {
                    PlaceDataTransferObject PlaceDataTransferObject = DataBase.Places.Find(Place.Id);
                    PlaceDataTransferObject.View = Model.View;
                    _ = DataBase.SaveChanges();
                }

                List<CompanyViewModel> ListCompanyViewModel;
                ListCompanyViewModel = DataBase.Companies.ToArray().Where(x => x.UserId == Id).Select(x => new CompanyViewModel(x)).ToList();
                // Обновляем компанию
                foreach (CompanyViewModel Company in ListCompanyViewModel)
                {
                    CompanyDataTransferObject CompanyDataTransferObject = DataBase.Companies.Find(Company.Id);
                    CompanyDataTransferObject.View = Model.View;
                    _ = DataBase.SaveChanges();
                }
                // Сохраняем изменения
                _ = DataBase.SaveChanges();
            }

            // Устанавливаем сообщение в TempData
            TempData["SM"] = "Вы отредактировали пользователя!";
            // Переадресовываем пользователя
            return RedirectToAction("EditUser");
        }

        [Authorize]
        public ActionResult DeleteUser(int Id)
        {
            // Удаляем из базы данных
            using (DataBase DataBase = new DataBase())
            {
                UserDataTransferObject UserDataTransferObject = DataBase.Users.Find(Id);
                UserRoleDataTransferObject UserRoleDataTransferObject = DataBase.UserRoles.AsParallel().Single(c => c.UserId == Id);
                _ = DataBase.UserRoles.Remove(UserRoleDataTransferObject);
                _ = DataBase.SaveChanges();
                _ = DataBase.Users.Remove(UserDataTransferObject);
                _ = DataBase.SaveChanges();
            }
            // Удаляем директорию
            DirectoryInfo OriginalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));
            string PathString = Path.Combine(OriginalDirectory.ToString(), "Profile\\" + Id.ToString());

            if (Directory.Exists(PathString))
            {
                Directory.Delete(PathString, true);
            }
            // Переадресовываем пользователя
            return RedirectToAction("Users");
        }

        //Создаём метод редактирования роли пользователя
        // GET: Admin/User/EditRoleUser/Id
        [HttpGet]
        [Authorize]
        public ActionResult EditRoleUser(int Id)
        {
            // Объявляем модель  UserRoleEditorViewModel
            UserRoleEditorViewModel Model;

            using (DataBase DataBase = new DataBase())
            {
                // Получаем пользователя
                UserDataTransferObject UserDataTransferObject = DataBase.Users.FirstOrDefault(x => x.Id == Id);
                UserRoleDataTransferObject UserRoleDataTransferObject = DataBase.UserRoles.FirstOrDefault(x => x.UserId == Id);
                RoleDataTransferObject RoleDataTransferObject = DataBase.Roles.FirstOrDefault(x => x.RoleId == UserRoleDataTransferObject.RoleId);
                // Инициализируем модель данными

                Model = new UserRoleEditorViewModel(UserDataTransferObject.Id, UserDataTransferObject.FirstName, UserDataTransferObject.LastName, UserDataTransferObject.EmailAddress, UserDataTransferObject.PhoneNumber, UserDataTransferObject.BirthDate, UserDataTransferObject.Password, RoleDataTransferObject.RoleName, UserRoleDataTransferObject.RoleId, UserDataTransferObject.View, UserDataTransferObject.EmailConfirmed)
                {
                    Role = new SelectList(DataBase.Roles.ToList(), "RoleId", "RoleName")
            
                };
            }
            // Возвращаем модель в представление
            return View("EditRoleUser", Model);
        }

        // Создаём метод редактирования пользователя
        // POST: Admin
        [HttpPost]
        [Authorize]
        public ActionResult EditRoleUser(UserRoleEditorViewModel Model)
        {
            // Получаем ID
            int Id = Model.Id;
            using (DataBase DataBase = new DataBase())
            {
                Model.Role = new SelectList(DataBase.Roles.ToList(), "RoleId", "RoleName");
            }
            // Проверяем модель на валидность
            if (!ModelState.IsValid)
            {
                return View("EditRoleUser", Model);
            }

            using (DataBase DataBase = new DataBase())
            {
                // Изменяем контекст данных (DTO)
                UserDataTransferObject UserDataTransferObject = DataBase.Users.Find(Model.Id);

                UserDataTransferObject.FirstName = Model.FirstName;
                UserDataTransferObject.LastName = Model.LastName;
                UserDataTransferObject.EmailAddress = Model.EmailAddress;
                UserDataTransferObject.PhoneNumber = Model.PhoneNumber;
                UserDataTransferObject.BirthDate = Model.BirthDate;
                UserDataTransferObject.View = Model.View;
                UserDataTransferObject.Password = Model.Password;

                _ = DataBase.SaveChanges();

                UserRoleDataTransferObject UserRoleDataTransferObject = DataBase.UserRoles.Single(c => c.UserId == Model.Id);

                _ = DataBase.UserRoles.Remove(UserRoleDataTransferObject);
                _ = DataBase.SaveChanges();

                UserRoleDataTransferObject UserRoleDataTransferObject2 = new UserRoleDataTransferObject()
                {
                    UserId = Id,
                    RoleId = Model.RoleId
                };

                _ = DataBase.UserRoles.Add(UserRoleDataTransferObject2);
                _ = DataBase.SaveChanges();

                List<PlaceViewModel> ListPlaceViewModel;
                ListPlaceViewModel = DataBase.Places.ToArray().Where(x => x.UserId == Id).Select(x => new PlaceViewModel(x)).ToList();
                // Обновляем 
                foreach (PlaceViewModel Place in ListPlaceViewModel)
                {
                    PlaceDataTransferObject PlaceDataTransferObject = DataBase.Places.Find(Place.Id);
                    PlaceDataTransferObject.View = Model.View;
                    _ = DataBase.SaveChanges();
                }

                List<CompanyViewModel> ListCompanyViewModel;
                ListCompanyViewModel = DataBase.Companies.ToArray().Where(x => x.UserId == Id).Select(x => new CompanyViewModel(x)).ToList();
                // Обновляем компанию
                foreach (CompanyViewModel Company in ListCompanyViewModel)
                {
                    CompanyDataTransferObject CompanyDataTransferObject = DataBase.Companies.Find(Company.Id);
                    CompanyDataTransferObject.View = Model.View;
                    _ = DataBase.SaveChanges();
                }
                // Сохраняем изменения
                _ = DataBase.SaveChanges();
            }

            // Устанавливаем сообщение в TempData
            TempData["SM"] = "Вы отредактировали пользователя!";
            // Переадресовываем пользователя
            return RedirectToAction("EditRoleUser");
        }
    }
}