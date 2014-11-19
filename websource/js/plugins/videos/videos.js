(function ($) {

    $(window).load(function () {
        var videos = $('[data-plugin="videos"]');
        var productId;
        for (var i = 0, arrLength = videos.length; i < arrLength; i += 1) {
            productId = videos.eq(i).attr('data-productId');

            if (productId == null) {
                return;
            }

            generate(productId, videos.eq(i));
        }

    });

    function generate(productId, place) {
        $.ajax({
            url: 'httphandlers/details/videos.ashx',
            dataType: 'JSON',
            data: { productId: productId },
            success: function (data) {
                if (data == null || data.length == 0) {
                    return;
                }

                var html = new EJS({ url: 'js/plugins/videos/templates/default.tpl' }).render(data);

                place.html(html);
            },
            error: function (data) {
                notify(data.responseText + " in get video", notifyType.error, true);
            }
        });
    }
})(jQuery);