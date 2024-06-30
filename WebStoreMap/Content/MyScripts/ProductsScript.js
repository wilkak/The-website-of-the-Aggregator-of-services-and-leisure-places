$(function () {

    /* Select product from specified category */
    
    $("#SelectCategory").on("change", function () {
        var url = $(this).val();

        if (url) {
            window.location = "/admin/place/Places?CategoryId=" + url;
        }
        return false;
    });

    /*-----------------------------------------------------------*/

    /* Confirm page deletion */

    $("a.delete").click(function () {
        if (!confirm("Подтвердите удаление страницы")) return false;
    });

    /*-----------------------------------------------------------*/
});