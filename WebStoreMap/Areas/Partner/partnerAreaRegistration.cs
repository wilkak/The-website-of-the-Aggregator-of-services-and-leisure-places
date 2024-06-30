using System.Web.Mvc;

namespace WebStoreMap.Areas.Partner
{
    public class PartnerAreaRegistration : AreaRegistration
    {
        public override string AreaName => "Partner";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            _ = context.MapRoute(
                "Partner_default",
                "Partner/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}