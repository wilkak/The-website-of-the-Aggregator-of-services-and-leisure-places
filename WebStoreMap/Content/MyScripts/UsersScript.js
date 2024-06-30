$(function () {


    $("#SelectedRol").on("change", function () {
        var url = $(this).val();

        if (url) {
            window.location = "/admin/User/Users?rolId=" + url;
        }
        return false;
    });
});