using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebStoreMap.Models.Data;
using WebStoreMap.Models.Work;

namespace WebStoreMap
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            DeleteLinkSheduler.Start();
        }

        //����� ��� ��������������
        protected void Application_AuthenticateRequest()
        {
            
            if (User == null)
            {
                return;
            }

            // ����� ������������
            string UserEmail = Context.User.Identity.Name;

            // ������ ����� � �������
            string[] Roles = null;

            using (DataBase DataBase = new DataBase())
            {
                // ��������� ����
                UserDataTransferObject UserDataTransferObject = DataBase.Users.FirstOrDefault(x => x.EmailAddress == UserEmail);

                if (UserDataTransferObject == null)
                {
                    return;
                }

                Roles = DataBase.UserRoles.Where(x => x.UserId == UserDataTransferObject.Id).Select(x => x.Role.RoleName).ToArray();
            }
            // ������ ���������� IPrincipal
            IIdentity UserIdentity = new GenericIdentity(UserEmail);
            IPrincipal NewUserObject = new GenericPrincipal(UserIdentity, Roles);

            // ��������� Context.User
            Context.User = NewUserObject;
        }
    }
}