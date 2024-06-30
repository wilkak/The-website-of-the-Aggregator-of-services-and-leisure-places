using System.Web.Mvc;

namespace WebStoreMap.Areas.Moderator
{
    public class ModeratorAreaRegistration : AreaRegistration
    {
        public override string AreaName => "Moderator";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            _ = context.MapRoute(
                "Moderator_default",
                "Moderator/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}