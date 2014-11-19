(function ($, $body) {
    $(function () {
        $body.on('click.redirect', '[data-redirect]', function () {
            var loc = this.getAttribute('data-redirect');
            window.location.assign(loc);
        });
    })
})(jQuery, $(document.body));