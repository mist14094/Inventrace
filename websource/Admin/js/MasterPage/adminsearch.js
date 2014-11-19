;(function ($) {
    $(function () {

        var baseurl = document.getElementsByTagName('base')[0].href,
            searchHeader = $('#searchHeader'),
            anchorsSearch = $('#liSearchItems a'),
            searchSubmenuContainer = $('#searchSubmenuContainer'),
            searchSubmenu = $('#searchSubmenu');

        if (localStorage.getItem("searchUrl") && localStorage.getItem("searchArea")) {
            searchHeader.text(localStorage.getItem("searchArea"));
            searchHeader.attr("data-href", localStorage.getItem("searchUrl"));
        } else {
            searchHeader.text(anchorsSearch.eq(0).text());
            localStorage.setItem("searchArea", anchorsSearch.eq(0).text());

            searchHeader.attr("data-href", anchorsSearch.eq(0).attr('href'));
            localStorage.setItem("searchUrl", anchorsSearch.eq(0).attr('href'));
        }


        searchSubmenuContainer.on('click', function () {
            searchSubmenu.show();
        });

        searchSubmenuContainer.on('mouseleave', function () {
            searchSubmenu.hide();
        })

        anchorsSearch.on('click', function (event) {

            event.preventDefault();
            event.stopPropagation();

            var aSearch = $(this),
                aSearchHref = aSearch.attr('href'),
                aSearchText = aSearch.text();

            searchHeader.html(aSearchText);
            localStorage.setItem("searchArea", aSearchText);

            searchHeader.attr('data-href', aSearchHref);
            localStorage.setItem("searchUrl", aSearchHref);

            searchSubmenu.hide();
        });

        $('#btnAdminSearch').on('click', function (event) {

            if (document.getElementById('txtAdminSearch').value.length === 0) {
                return;
            }

            event.preventDefault();

            var urlSearch = new Advantshop.Utilities.Uri(baseurl + searchHeader.attr('data-href'));

            urlSearch.replaceQueryParam('search', document.getElementById('txtAdminSearch').value);

            window.location = urlSearch.toString();
        });
    });
})(jQuery);
