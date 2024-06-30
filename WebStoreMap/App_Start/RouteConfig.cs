using System.Web.Mvc;
using System.Web.Routing;

namespace WebStoreMap
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            _ = routes.MapRoute("Dashboard", "Dashboard/{action}/{id}", new { controller = "Dashboard", action = "Index", id = UrlParameter.Optional }, new[] { "WebStoreMap.Controllers" });
            _ = routes.MapRoute("Test", "Test/{action}/{id}", new { controller = "Test", action = "Index", id = UrlParameter.Optional }, new[] { "WebStoreMap.Controllers" });
            _ = routes.MapRoute("Account", "Account/{action}/{id}", new { controller = "Account", action = "Index", id = UrlParameter.Optional }, new[] { "WebStoreMap.Controllers" });
            _ = routes.MapRoute("Information", "Information/{action}/{id}", new { controller = "Information", action = "Index", id = UrlParameter.Optional }, new[] { "WebStoreMap.Controllers" });
            _ = routes.MapRoute("Desires", "Desires/{action}/{id}", new { controller = "Desires", action = "Index", id = UrlParameter.Optional }, new[] { "WebStoreMap.Controllers" });
            _ = routes.MapRoute("Favorites", "Favorites/{action}/{id}", new { controller = "Favorites", action = "Index", id = UrlParameter.Optional }, new[] { "WebStoreMap.Controllers" });
            _ = routes.MapRoute("Search", "Search/{action}/{id}", new { controller = "Search", action = "Index", id = UrlParameter.Optional }, new[] { "WebStoreMap.Controllers" });
            _ = routes.MapRoute("Email", "Email/{action}/{name}", new { controller = "Email", action = "Index", name = UrlParameter.Optional }, new[] { "WebStoreMap.Controllers" });
            _ = routes.MapRoute("Place", "Place/{action}/{name}", new { controller = "Place", action = "Index", name = UrlParameter.Optional }, new[] { "WebStoreMap.Controllers" });
            _ = routes.MapRoute("Sections", "Sections/{action}/{name}", new { controller = "Sections", action = "Index", name = UrlParameter.Optional }, new[] { "WebStoreMap.Controllers" });
            _ = routes.MapRoute("Rating", "Rating/{action}/{name}", new { controller = "Rating", action = "Index", name = UrlParameter.Optional }, new[] { "WebStoreMap.Controllers" });
            _ = routes.MapRoute("Companies", "Companies/{action}/{name}", new { controller = "Companies", action = "Index", name = UrlParameter.Optional }, new[] { "WebStoreMap.Controllers" });
            _ = routes.MapRoute("User", "User/{action}/{name}", new { controller = "User", action = "Index", name = UrlParameter.Optional }, new[] { "WebStoreMap.Controllers" });
            _ = routes.MapRoute("Comments", "Comments/{action}/{name}", new { controller = "Comments", action = "Index", name = UrlParameter.Optional }, new[] { "WebStoreMap.Controllers" });
            _ = routes.MapRoute("SidebarPartial", "Pages/SidebarPartial", new { controller = "Pages", action = "SidebarPartial" }, new[] { "WebStoreMap.Controllers" });
            _ = routes.MapRoute("ChatRoom", "ChatRoom/{action}/{name}", new { controller = "ChatRoom", action = "Index", name = UrlParameter.Optional }, new[] { "WebStoreMap.Controllers" });
            _ = routes.MapRoute("PagesMenuPartial", "Pages/PagesMenuPartial", new { controller = "Pages", action = "PagesMenuPartial" }, new[] { "WebStoreMap.Controllers" });
            _ = routes.MapRoute("Pages", "{page}", new { controller = "Pages", action = "Index" }, new[] { "WebStoreMap.Controllers" });
            _ = routes.MapRoute("Default", "", new { controller = "Pages", action = "Index" }, new[] { "WebStoreMap.Controllers" });
            _ = routes.MapRoute("Map", "Map/Index/{Categories}", new { controller = "Map", action = "Index", Categories = UrlParameter.Optional }, new[] { "WebStoreMap.Controllers" });
        }
    }
}