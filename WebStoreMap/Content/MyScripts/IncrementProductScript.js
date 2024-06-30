/* Increment product */
$(function () {

    $("a.incproduct").click(function (e) {
        e.preventDefault();
        var productId = $(this).data("id");
        var url = "/desired/IncrementProduct";

        $.getJSON(url,
            { productId: productId },
            function (data) {
                $(".qty" + productId).html(data.qty);

                var price = data.qty * data.price;
                var priceHtml = price.toFixed(2) + "₽";

                $(".total" + productId).html(priceHtml);

                var gt = parseFloat($(".grandtotal").text());
                var grandtotal = (gt + data.price).toFixed(2) + "₽";

                $(".grandtotal").text(grandtotal);


            }).done(function (data) {
                var url2 = "/desired/PaypalPartial";

                $.get(url2,
                    {},
                    function (data) {
                        $("div.paypaldiv").html(data);
                    });
            });

    });
    /*-----------------------------------------------------------*/

    /* Decriment product */
    $(function () {

        $("a.decproduct").click(function (e) {
            e.preventDefault();

            var $this = $(this);
            var productId = $(this).data("id");
            var url = "/desired/DecrementProduct";

            $.getJSON(url,
                { productId: productId },
                function (data) {

                    if (data.qty == 0) {
                        $this.parent().fadeOut("fast",
                            function () {
                                location.reload();
                            });
                    } else {
                        $(".qty" + productId).html(data.qty);

                        var price = data.qty * data.price;
                        var priceHtml = price.toFixed(2) + " ₽";

                        $(".total" + productId).html(priceHtml);

                        var gt = parseFloat($(".grandtotal").text());
                        var grandtotal = (gt - data.price).toFixed(2);

                        $(".grandtotal").text(grandtotal);
                    }

                }).done(function (data) {

                    var url2 = "/desired/PaypalPartial";

                    $.get(url2,
                        {},
                        function (data) {
                            $("div.paypaldiv").html(data);
                        });
                });

        });
    });
    /*-----------------------------------------------------------*/

    /* Remove product */
    $(function () {

        $("a.removeproduct").click(function (e) {
            e.preventDefault();

            var $this = $(this)
            var productId = $(this).data("id");
            var url = "/desired/RemoveProduct";

            $.get(url,
                { productId: productId },
                function (data) {
                    location.reload();
                });
        });
    });
});