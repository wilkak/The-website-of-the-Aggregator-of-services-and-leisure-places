$(function () {
    /* Confirm page deletion */

    $("btn-delete").click(function () {
        if (!confirm("Подтвердите удаление компании")) return false;
    });

    /*-----------------------------------------------------------*/
});