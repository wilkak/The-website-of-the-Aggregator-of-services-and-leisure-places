using System.Web.Optimization;

namespace WebStoreMap
{
    public static class BundleConfig
    {
        // Дополнительные сведения об объединении см. на странице https://go.microsoft.com/fwlink/?Id=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            //Подключаем новую библиотеку JQuery-UI
            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui.js"));

            // Используйте версию Modernizr для разработчиков, чтобы учиться работать. Когда вы будете готовы перейти к работе,
            // готово к выпуску, используйте средство сборки по адресу https://modernizr.com, чтобы выбрать только необходимые тесты.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/Content/jquery-ui.css",
                      "~/Content/css/components.css",
                      "~/Content/css/Admin-Partner-zone.css",
                      "~/Content/css/category-page.css",
                      "~/Content/css/map-page.css",
                      "~/Content/css/productdetails-page.css",
                      "~/Content/css/login-page.css",
                       "~/Content/css/massage-page.css",
                       "~/Content/css/desire-page.css"));

            //Подключаем библиотеку CKEditor.js
            bundles.Add(new ScriptBundle("~/bundles/CKEditor").Include(
                      "~/Scripts/ckeditor/ckeditor.js"));
        }
    }
}