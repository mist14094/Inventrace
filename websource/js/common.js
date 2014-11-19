$(function () {

    if (window.DisplayFilter) {
        DisplayFilter(10, 5);
    }

    $("#brand-filter input, #property-filter input, #color-filter input, #size-filter input").click(function () {
        ApplyFilter(this, false, true, true);
    });

    $("#ddlCategory").change(function () {
        ApplyFilter(this, false, true, true);
    });


    if ($("a.zoom span.label-p").length) {
        $("a.zoom span.label-p").click(function () {
            return false;
        });
    }


    $("a.btn.btn-disabled").live("click", function (e) {
        e = e || window.event;
        e.preventDefault();
        return false;
    });


    if ($("input[placeholder], textarea[placeholder]").length)
        $("input[placeholder], textarea[placeholder]").placeholder();


    $('div[id^=rating_]').each(function () {
        $(this).raty({
            hints: [localize("ratingAwfull"), localize("ratingBad"), localize("ratingNormal"), localize("ratingGood"), localize("ratingExcelent")],
            readOnly: $(this).hasClass("rating-readonly"),
            score: parseFloat($(this).next("input:hidden[id^=rating_hidden_]").val()),
            click: function (score, evt) {
                var newscore = VoteRating($(this).attr('id').replace("rating_", ""), score);
                if (newscore != 0) {
                    $(this).raty('score', newscore);
                } else {
                    $(this).raty('score', $(this).next("input:hidden[id^=rating_hidden_]").val());
                    notify(localize("ratingAlreadyVote"), notifyType.notify);
                }
                $(this).raty('readOnly', true);
            }
        });
    });


    if ($("ul.carousel-product:visible").length)
        $("ul.carousel-product:visible").jcarousel({ scroll: 1 });

    if ($("[data-plugin=fancybox]").length) {
        $("[data-plugin=fancybox]").fancybox({
            onComplete: function () {
                if ($.browser.msie && $.browser.version < 9) {
                    $('body').addClass('fancybox-videos-hide');
                }
            },
            onClosed: function () {
                if ($.browser.msie && $.browser.version < 9) {
                    $('body').removeClass('fancybox-videos-hide');
                }
            }
        });
    }

    if ($("div.slider").length) {

        var min = parseInt($(this).find("span.min").text());
        var max = parseInt($(this).find("span.max").text());
        var curMin = parseInt($("#sliderCurentMin").val());
        var curMax = parseInt($("#sliderCurentMax").val());

        if (isNaN(min)) min = 0;
        if (isNaN(max)) max = 0;
        if (isNaN(curMin)) curMin = 0;
        if (isNaN(curMax)) curMax = 0;

        var slider = $("div.slider");

        slider.data("prices", { from: curMin, to: curMax });

        slider.slider({
            range: true,
            min: min,
            max: max,
            values: [curMin, curMax],
            slide: function (event, ui) { sladeMove.call(this, ui); makeMagic($(this)); },
            change: function (event, ui) { sladeMove.call(this, ui); ApplyFilter(this, false, true, false); }

        });

        function sladeMove(ui) {
            $(this).find("span.min").text(ui.values[0]);
            $(this).find("span.max").text(ui.values[1]);
        }

        $("div.slider").each(function () {
            $(this).find("span.min").text($(this).slider("option", "values")[0]);
            $(this).find("span.max").text($(this).slider("option", "values")[1]);
        });

        makeMagic(slider);

    }

    if ($("table.avangard").length) {
        var rbs = $("table.avangard input:radio");
        var trs = $("table.avangard tr:not(.header)");
        $("table.avangard").click(function (e) {
            var tr = $(e.target).closest("tr:not(.header,.selected)");

            if (!tr.length) return;

            trs.removeClass("selected");
            tr.addClass("selected");

            var rb = tr.find("input:radio");
            if (!rb.length) return;
            rbs.removeAttr("checked");
            rb.attr("checked", "checked");
        });
    }

    if ($("input:checkbox.adress-payment").length) {
        $("input:checkbox.adress-payment").live("click", function () {
            var listP = $(this).closest("li").find("div.adress-payment");

            if (!listP.length) return false;

            listP.slideToggle();
            return true;
        });
    };

    if ($("table.pv-table").length) {
        var el, cell, tooltip, tooltipImg, tooltipPosition, path;
        var wrapperTooltip = "<div class='tooltip' id='pv-table-tooltip'><img src=\".\" /><div class='declare'></div></div>";

        $("table.pv-table tr:not(.head)").mouseenter(function (e) {

            cell = $(e.target).closest("tr").children("td.icon[abbr]");

            if (!cell.length) return;

            el = cell.find("div.photo");
            path = cell.attr("abbr");

            if (!path) return;

            if (!$("#pv-table-tooltip").length)
                $("body").append(wrapperTooltip);

            tooltip = $("#pv-table-tooltip");

            tooltipImg = tooltip.find("img").attr("src", path);

            tooltip.append(tooltipImg);

            tooltipImg.load(function () {

                tooltip.show();

                tooltipPosition = el.offset();

                tooltip.css({
                    top: tooltipPosition.top - 47,
                    left: tooltipPosition.left - (tooltip.outerWidth() + 13)
                });

                PIELoad(tooltip);
            });
        });

        $("table.pv-table tr:not(.head)").mouseleave(function () {
            tooltip = $("#pv-table-tooltip");
            if (tooltip && tooltip.is(":visible")) {
                tooltip.hide();
            }
        });
    }

    $("#check-status a.btn").click(function () {
        var number = $("#check-status input").val();
        if (number.length) {
            var status = CheckOrder(htmlEncode(number.substr(0, 20)));
            if (status) {
                $("#orderStatus").text(localize("checkOrderState") + ": " + status.StatusName);
                if (status.StatusComment) {
                    $("#orderStatus").append("<br />" + localize("checkOrderComent") + ": " + status.StatusComment);
                }
                $("#orderStatus").show();
            } else {
                throw Error(localize("checkOrderError"));
            }
        }
        return false;
    });

    $(".tree-item:not(.tree-item-nosubcat), .tree-item-selected:not(.tree-item-nosubcat) ").hover(function (e) {
        var target = $(this);

        var submenu = target.find(".tree-submenu");
        if (!submenu.length) return true;

        submenu.removeClass("submenu-orientation").css({ left: "" });

        submenu.show();

        var windowWidth = $('body').width(),
            contentOffset = $('#form .container').offset().left,
            contentWidth = $('#form .container').width(),
            contentFull = contentOffset + contentWidth,
            submenuWidth = submenu.outerWidth(),
            submenuOffset = submenu.offset().left,
            submenuFull = submenuWidth + submenuOffset,
            dimContentSubmenu = submenuOffset - contentOffset,
            left = 0;


        if (windowWidth <= submenuWidth) {
            left = -submenuOffset;
        } else if (contentWidth < submenuWidth) {
            left = -(submenuWidth - contentWidth);
        } else if (contentFull < submenuFull) {
            left = -(submenuFull - contentFull);
        }

        submenu.css({ left: left });
    },
        function (e) {
            var target = $(this);
            var submenu = target.find(".tree-submenu");

            if (!submenu.length) return true;

            setTimeout(function () {
                submenu.hide();
            }, 1);

        });

    $("table.categories").click(function (e) {

        e.cancelBubble = true;
        var target = $(e.target);

        if (!target.is("td:not(cat-split)")) return true;

        var link = target.find("a").attr("href");

        if (!link || !link.length) return true;

        window.location.href = $("base").attr("href") + link;
    });

    var isCtrl = false;

    $(document).keyup(function (e) {
        if (e.keyCode == 17)
            isCtrl = false;
    });

    $(document).keydown(function (e) {
        if (e.keyCode == 17)
            isCtrl = true;

        //left arrow
        if (e.keyCode == 37 && isCtrl == true) {
            if ($("#paging-prev").length)
                document.location = $("#paging-prev").attr("href");
        }

        //right arrow
        if (e.keyCode == 39 && isCtrl == true) {
            if ($("#paging-next").length)
                document.location = $("#paging-next").attr("href");
        }
    });

    if ($(".btn-disabled").length) {
        var btn = $(".btn-disabled");
        if (btn.attr("onClick")) {
            btn.attr("onClickOld", btn.attr("onClick")).attr("onClick", "return false;");
        }
    }

    if ($("input.autocompleteRegion").length) {
        $("input.autocompleteRegion").autocomplete("HttpHandlers/GetRegions.ashx", {
            delay: 10,
            minChars: 1,
            matchSubset: 1,
            autoFill: true,
            matchContains: 1,
            cacheLength: 10,
            selectFirst: true,
            //formatItem: liFormat,
            maxItemsToShow: 10
        });
    }

    if ($("input.autocompleteCity").length) {
        $("input.autocompleteCity").autocomplete('HttpHandlers/GetCities.ashx', {
            delay: 10,
            minChars: 1,
            matchSubset: 1,
            autoFill: true,
            matchContains: 1,
            cacheLength: 10,
            selectFirst: true,
            //formatItem: liFormat,
            maxItemsToShow: 10
        });
    }

    if ($("input.autocompleteSearch").length) {
        $("input.autocompleteSearch").autocomplete('HttpHandlers/GetSearch.ashx', {
            delay: 10,
            minChars: 1,
            matchSubset: 1,
            autoFill: false,
            matchContains: 1,
            cacheLength: null,
            selectFirst: false,
            //formatItem: liFormat,
            maxItemsToShow: 10,
            onItemSelect: function (li, $lnk, $input) {

                setTimeout(function () { window.location.assign($('base').attr('href') + $lnk.attr('href')); }, 1);

            }
        });
    }


    if ($("a.trialAdmin").length) {
        $.advModal({
            title: localize("demoMode"),
            control: $("a.trialAdmin"),
            isEnableBackground: true,
            htmlContent: localize("demoCreateTrial"),
            buttons: [
                { textBtn: localize("demoCreateNow"), isBtnClose: true, classBtn: "btn-confirm", func: function () { window.location = localize("trialUrl"); } },
                { textBtn: localize("demoCancel"), isBtnClose: true, classBtn: "btn-action" }
            ]
        });
    }



    var catAlt = $('.catalog-menu-root > li'),
        catAltItems = $('.catalog-menu-items');

    if (catAlt.length > 0 && catAltItems.length > 0) {

        if (catAltItems.closest(catAlt).length > 0) {
            catAlt.on('mouseenter', function () {
                if (catAltItems.is(':visible') !== true) {
                    catAltItems.show();
                }
            });

            catAlt.on('mouseleave', function () {
                if (catAltItems.is(':visible') === true) {
                    catAltItems.hide();
                }
            });

            catAltItems.on('mouseleave', function () {
                if (catAltItems.is(':visible') === true) {
                    catAltItems.hide();
                }
            });
        }



        catAltItems.children('.item.parent').on('mouseenter', function (e) {
            $(this).children('.tree-submenu').show();
        });
        catAltItems.children('.item.parent').on('mouseleave', function (e) {
            $(this).children('.tree-submenu').hide();
        });
    }


    if ($("#discountaction").length) {
        $.advModal({
            modalClass: "disc-actions",
            isEnableBackground: true,
            htmlContent: $("#discountaction")
        }).modalShow();
    }

});

$(window).load(function () {
    if (window.notify) {
        notify(null, null, null, { showContainer: true });
    }

    var pvItems = $('.scp .scp-item');

    if (pvItems.length > 0 && $('html').hasClass('touch') !== true) {
        var pvStoragePhotos = {},
            carouselPattern = $('<div />', {
                'class': 'flexslider flexslider-carousel flexslider-pv pie',
                html: $('<ul />', {
                    'class': 'slides'
                })
            });

        pvItems.on('mouseenter', function (e) {

            e.stopPropagation();
            e.preventDefault();

            var pvItem = $(this),
                productid = pvItem.attr('data-productid'),
                carousel = pvItem.find('.flexslider');

            if (pvItem.hasClass('loading') === true || productid == null || productid.length === 0) {
                return;
            }

            pvItem.addClass('loading');

            if (pvStoragePhotos[productid] != null) {
                generate(pvItem, productid);
                return;
            }

            $.ajax({
                url: 'httphandlers/details/getproductphotos.ashx',
                data: { productid: productid },
                dataType: 'json',
                async: false,
                cache: false,
                success: function (data) {
                    pvStoragePhotos[productid] = data;
                    generate(pvItem, productid);
                },
                error: function (data) {
                    throw Error(data.statusTextfcolor);
                }
            });
        });

        pvItems.on('click', '.color-item', function (e) {

            e.stopPropagation();
            e.preventDefault();

            if ($(e.target).closest('.flexslider').length > 0) {
                return;
            }

            var that = $(this),
                parent = that.closest('.scp-item'),
                scp = that.closest('[data-part="sizeColorPickerCatalog"]');

            generate(parent, scp.attr('data-productid'));

            parent.find('.flexslider .slides li:eq(0)').trigger('click');

            parent.find('.btn-add').attr('data-color-id', scp.attr('data-color-id'));



        })


        function generate(place, productid) {
            var carousel = place.find('.flexslider'),
                data = pvStoragePhotos[productid],
                colorIdSelected = place.find('[data-part="sizeColorPickerCatalog"] .color-item.selected').attr('data-color-id'),
                imgWrapPattern = $('<div />', { 'class': 'flexslider-pv-wrap' });


            if ((data == null || data.length < 2) || (colorIdSelected != null && colorIdSelected == carousel.attr('data-color-id'))) {
                return;
            }

            if (colorIdSelected != null) {
                colorIdSelected = parseInt(colorIdSelected);
            }

            place.find('.flexslider').remove();

            carousel = carouselPattern.clone();

            place.find('[data-part="sizeColorPickerCatalog"]').attr('data-color-id', colorIdSelected);

            place.append(carousel);

            carousel.css({
                width: data[0].XSmallProductImageWidth,
                left: -(data[0].XSmallProductImageWidth + parseInt(carousel.css('paddingLeft')) + parseInt(carousel.css('paddingRight')) + parseInt(carousel.css('borderLeftWidth')) + parseInt(carousel.css('borderRightWidth')))
            });


            var ImagePreview = carousel.closest('.scp-item').find('.scp-img'),
                imgTemp, imgWrapTemp, imgPreviewPath;

            if (ImagePreview.length > 0) {
                imgPreviewPath = ImagePreview.attr('data-img-type') || 'Small';
            }


            for (var key in data) {

                imgTemp = $('<img />', {
                    'src': data[key].PathXSmall,
                    'data-img-path': data[key]['Path' + imgPreviewPath],
                    'alt': data[key].Description,
                    'class': 'pie'
                });

                imgWrapTemp = imgWrapPattern.clone();

                imgWrapTemp.css({
                    'height': data[key].XSmallProductImageHeight,
                    'width': data[key].XSmallProductImageWidth
                });

                imgWrapTemp.html(imgTemp);

                if (colorIdSelected != null) {
                    if (colorIdSelected === data[key].ColorID || data[key].ColorID == null) {
                        carousel.find('.slides').append($('<li />', {
                            html: imgWrapTemp
                        }));
                    }
                }
                else {
                    carousel.find('.slides').append($('<li />', {
                        html: imgWrapTemp
                    }));
                }
            }

            carousel.flexslider({
                controlNav: false,
                animation: 'slide',
                move: 1,
                maxItems: 3,
                animationLoop: false,
                slideshow: false,
                direction: 'vertical',
                useCSS: false
            });

            carousel.find('.slides li:first').addClass('selected');

            carousel.find('.slides li').on('click touchstart', function (e) {

                e.stopPropagation();
                e.preventDefault();

                carousel.find('.slides li').removeClass('selected');

                $(this).addClass('selected');

                var img = $(this).find('img');

                var imgMiddle = $(this).closest('.scp-item').find('.scp-img');

                imgMiddle.attr({
                    'src': img.attr('data-img-path'),
                    'alt': img.attr('alt') || ''
                });
            });

            place.removeClass('loading');
        }
    }


    PIELoad();
});

