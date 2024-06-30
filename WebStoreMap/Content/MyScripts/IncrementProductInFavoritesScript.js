/* Increment product */
$(function () {

    $("a.incproduct").click(function (e) {
        e.preventDefault();
        var PlaceId = $(this).data("id");
        var url = "/favorites/IncrementPlace";

        $.getJSON(url,
            { PlaceId: PlaceId },
            function (data) {
                $(".qty" + productId).html(data.qty);

                var price = data.qty * data.price;
                var priceHtml = price.toFixed(2) + "₽";

                $(".total" + productId).html(priceHtml);

                var gt = parseFloat($(".grandtotal").text());
                var grandtotal = (gt + data.price).toFixed(2) + "₽";

                $(".grandtotal").text(grandtotal);


            }).done(function (data) {
                var url2 = "/favorites/PaypalPartial";

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
            var PlaceId = $(this).data("id");
            var url = "/favorites/DecrementProduct";

            $.getJSON(url,
                { PlaceId: PlaceId },
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

                    var url2 = "/favorites/PaypalPartial";

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
            var PlaceId = $(this).data("id");
            var url = "/favorites/RemovePlace";

            $.get(url,
                { PlaceId: PlaceId },
                function (data) {
                    location.reload();
                });
        });
    });
});