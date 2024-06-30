$(function () {

    /* Add new category */

  
    /* Объявляем и инициализируем нужные переменные */

    var newCatA = $("a#newcata"); /*Класс линка добавления*/
    var newCatTextInput = $("#newcatname"); /*Класс текстового поля ввода имени*/
    var ajaxText = $("span.ajax-text"); /*Класс картинки загрузки*/
    var table = $("table#pages tbody"); /*Класс таблицы вывода*/

    /* Пишем функцию на отлов нажатия Enter */
    newCatTextInput.keyup(function (e) {
        if (e.keyCode == 13) {
            newCatA.click();
        }
    });

    /* Пишем функцию Click */
    newCatA.click(function (e) {
        e.preventDefault();

        var catName = newCatTextInput.val();

        if (catName.length < 3) {
            alert("Название категории должно состоять не менее чем из 3 символов.");
            return false;
        }

        ajaxText.show();

        var url = "/admin/Place/AddNewCategory";

        $.post(url, { catName: catName }, function (data) {
            var response = data.trim();

            if (response == "titletaken") {
                ajaxText.html("<span class='alert alert-danger'>Это название уже взято!</span>");
                setTimeout(function () {
                    ajaxText.fadeOut("fast", function () {
                        ajaxText.html("<img src='/Content/img/ajax-loader.gif' height='50' />");
                    });
                }, 4000);
                return false;
            }
            else {
                if (!$("table#pages").length) {
                    location.reload();
                }
                else {
                    ajaxText.html("<span class='alert alert-success'>Категория была добавлена!</span>");
                    setTimeout(function () {
                        ajaxText.fadeOut("fast", function () {
                            ajaxText.html("<img src='/Content/img/ajax-loader.gif' height='50' />");
                        });

                        window.location.reload();
                    }, 2000);

                    newCatTextInput.val("");

                    var toAppend = $("table#pages tbody tr:last").clone();
                    toAppend.attr("id", "id_" + data);
                    toAppend.find("#item_Name").val(catName);
                    toAppend.find("a.delete").attr("href", "/admin/Shop/DeleteCategory/" + data);
                    table.append(toAppend);
                    table.sortable("refresh");
                }
            }
        });
    });

    /*-----------------------------------------------------------*/



    /* Confirm category deletion */

    $("body").on("click", "a.delete", function () {
        if (!confirm("Подтвердите удаление категории")) return false;
    });

    /*-----------------------------------------------------------*/

    
    /* Rename category */

    var originalTextBoxValue;

    $("table#pages input.text-box").dblclick(function () {
        originalTextBoxValue = $(this).val();
        $(this).attr("readonly", false);
    });

    $("table#pages input.text-box").keyup(function (e) {
        if (e.keyCode == 13) {
            $(this).blur();
        }
    });

    $("table#pages input.text-box").blur(function () {
        var $this = $(this);
        var ajaxdiv = $this.parent().parent().parent().find(".ajaxdivtd");
        var newCatName = $this.val();
        var id = $this.parent().parent().parent().parent().parent().attr("id").substring(3);
        var url = "/admin/Place/RenameCategory";

        if (newCatName.length < 3) {
            alert("Название категории должно состоять не менее чем из 3 символов.");
            $this.attr("readonly", true);
            return false;
        }

        $.post(url, { newCatName: newCatName, id: id }, function (data) {
            var response = data.trim();

            if (response == "titletaken") {
                $this.val(originalTextBoxValue);
                ajaxdiv.html("<div class='alert alert-danger'>Это название уже взято!</div>").show();
            }
            else {
                ajaxdiv.html("<div class='alert alert-success'>Название категории было изменено/div>").show();
            }

            setTimeout(function () {
                ajaxdiv.fadeOut("fast", function () {
                    ajaxdiv.html("");
                });
            }, 4000);
        }).done(function () {
            $this.attr("readonly", true);
        });
    });

    /*-----------------------------------------------------------*/



    /* Sorting script */

    $("table#pages tbody").sortable({
        items: "tr:not(.home)",
        placeholder: "ui-state-highlight",
        update: function () {
            var ids = $("table#pages tbody").sortable("serialize");
            var url = "/admin/Place/ReorderCategories";

            $.post(url, ids, function (data) {
            });
        }
    });
});
