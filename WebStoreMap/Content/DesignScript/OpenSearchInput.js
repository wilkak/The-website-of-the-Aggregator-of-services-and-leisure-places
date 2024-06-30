var inptxt = $('.search-box input');
$(inptxt).on('focusout', function () {
    $(this).parent().removeClass('selected');
});
var inptxt2 = $('.search-box');
$(inptxt2).on('click', function () {
    $(this).addClass('selected');
});