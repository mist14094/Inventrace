/*carousel details*/
(function ($) {

    var carouselDetails = new function () {
        var instance;

        function carouselDetails(el, storageOffers) {
            if (!instance) {

                this.$obj = $(el);
                this.storageOffers = storageOffers;
                this.Init();
                this.Filter();
                instance = this;

                return instance;
            } else {
                return instance;
            }
        }

        carouselDetails.prototype.Init = function () {
            var carouselDetailsObj = this;

            carouselDetailsObj.carouselData = carouselDetailsObj.$obj.data('flexslider');

            if (carouselDetailsObj.carouselData == null) {
                var opts = Advantshop.Utilities.Eval(carouselDetailsObj.$obj.attr('data-flexslider-options')) || {};
                carouselDetailsObj.carouselData = carouselDetailsObj.$obj.flexslider(opts).data('flexslider');
            }

            if (carouselDetailsObj.carouselData != null && carouselDetailsObj.carouselData.repository == null) {

                carouselDetailsObj.carouselData.repository = [];

                var itemTemp, attrtemp, objTemp = {};
                for (var i = 0, l = carouselDetailsObj.carouselData.slides.length; i < l; i += 1) {

                    itemTemp = carouselDetailsObj.carouselData.slides.eq(i);
                    attrtemp = itemTemp.attr('data-color-id');

                    if (attrtemp == null || attrtemp.length === 0) {
                        objTemp = { type: 'any', obj: itemTemp };
                    } else {
                        objTemp = { type: 'color', obj: itemTemp };
                    }

                    carouselDetailsObj.carouselData.repository.push(objTemp);
                }

                carouselDetailsObj.$obj.data('flexslider', carouselDetailsObj.carouselData);
            }
        };

        carouselDetails.prototype.Filter = function () {
            var carouselDetailsObj = this, itemTemp;

            if (carouselDetailsObj.storageOffers.Offers.length === 0) {
                return;
            }


            carouselDetailsObj.carouselData.clear();

            /*add items carousel*/

            for (var a = 0, arrAddLength = carouselDetailsObj.carouselData.repository.length; a < arrAddLength; a += 1) {
                itemTemp = carouselDetailsObj.carouselData.repository[a];
                if (itemTemp.type === 'any' || !carouselDetailsObj.storageOffers.ColorIdSelected) {
                    carouselDetailsObj.carouselData.addSlide(itemTemp.obj);
                } else if (itemTemp.type === 'color' && carouselDetailsObj.storageOffers.ColorIdSelected === parseInt(itemTemp.obj.attr('data-color-id'))) {
                    carouselDetailsObj.carouselData.addSlide(itemTemp.obj);
                }
            }

            carouselDetailsObj.$obj.trigger('carouselDetailsFilter', [carouselDetailsObj]);
        };

        return carouselDetails;
    };

    Advantshop.NamespaceRequire('Advantshop.Details.Part');
    Advantshop.Details.Part.CarouselDetails = carouselDetails;

})(jQuery);

/*init page*/
(function ($) {
    $(function () {


        $('#zoom').removeClass('cloud-zoom-progress');

        var hfProductId = document.getElementById('hfProductId');

        if (hfProductId == null || hfProductId.getAttribute('data-page') !== 'details' || isNaN(hfProductId.value) === true) {
            return;
        }

        var offers = new Advantshop.Offers(hfProductId.value),
            sizeColorPicker,
            zoom = $('#zoom'),
            hasPhoto = Advantshop.Utilities.Eval(zoom.attr('data-has-photo')),
            fancyboxControls = $('#icon-zoom, #link-fancybox');

        if (offers.storageOffers != null && offers.storageOffers.Offers.length > 0) {
            sizeColorPicker = Advantshop.Details.Part.SizeColorPickerDetails.prototype.Init(offers.storageOffers, $('[data-part="sizeColorPickerDetails"]:first'));
        }


        if (zoom.hasClass('clood-zoom') !== true && hasPhoto === true) {
            fancyboxControls = fancyboxControls.add(zoom);
        }


        

        //#region sizeColorPicker
        if (sizeColorPicker != null && offers.storageOffers != null && offers.storageOffers.Offers.length > 0) {

            offers.UpdateProduct();

            sizeColorPicker.$obj.on('changeSize', '.size-item', function () {
                offers.UpdateProduct();
            });

            sizeColorPicker.$obj.on('changeColor', '.color-item', function () {
                offers.UpdateProduct();
            });

            sizeColorPicker.$obj.on('sizeColorFilter', function () {
                if (carouselDetails != null) {
                    carouselDetails.Filter();
                }
            });
        }
        //#endregion sizeColorPicker

        //#region Carousel
        if (document.getElementById('flexsliderDetails') != null) {
            var carouselDetails = new Advantshop.Details.Part.CarouselDetails(document.getElementById('flexsliderDetails'), offers.storageOffers);

            if (carouselDetails.carouselData.slides.length === 0) {
                $('#icon-zoom, #link-fancybox').hide();
                $('#preview-img').attr('src', 'images/nophoto.jpg');
                zoom.attr('href', 'javascript:void(0);');
                if (zoom.data('zoom') != null) {
                    zoom.data('zoom').destroy();
                }
            } else {
                $('.cloud-zoom').CloudZoom();
                $('#icon-zoom, #link-fancybox').show();
            }

            carouselDetails.$obj.on('carouselDetailsFilter', function (e, carouselDetailsObj) {

                if (zoom.data('zoom') != null) {
                    zoom.data('zoom').destroy();
                }

                if (carouselDetailsObj.carouselData.slides.length === 0) {
                    $('#icon-zoom, #link-fancybox').hide();
                    $('#preview-img').attr('src', 'images/nophoto.jpg');
                    zoom.attr('href', 'javascript:void(0);');
                    return;
                } else {
                    $('#icon-zoom, #link-fancybox').show();
                }

                carouselDetailsObj.carouselData.slides.eq(0).find('[data-imagePicker-caller]').click();

                if (zoom.hasClass('cloud-zoom') === true) {
                    zoom.CloudZoom();
                }
            });
        } else if (hasPhoto === true) {
            zoom.CloudZoom();
        }

        //#endregion Carousel

        fancyboxControls.on('click', function (e) {

            e.preventDefault();

            if (carouselDetails == null || carouselDetails.carouselData.slides.length === 0) {
                $.fancybox([{ href: zoom.attr('href'), title: zoom.attr('title') ||  $('#preview-img').attr('title') }]);
                return;
            } else {

                var arr = [],
                    item,
                    index = 0,
                    arrImgs;

                for (var i = 0, arrItemsLength = carouselDetails.carouselData.slides.length; i < arrItemsLength; i += 1) {
                    item = carouselDetails.carouselData.slides.eq(i).find('[data-imagePicker-caller]');


                    arrImgs = Advantshop.Utilities.Eval(item.attr('data-imagePicker-img'));

                    arr.push({ href: arrImgs['big'].src, title: arrImgs['big'].title || '' });

                    if (item.hasClass('selected') === true) {
                        index = i;
                    }
                }

                $.fancybox(arr, { index: index });
            }
        });

        // add to wishlist
        if ($("#addToWishlist").length) {
            $("#addToWishlist").on("click.a", function (e) {
                if ($("#addToWishlist").attr("href") == "wishlist.aspx") {
                    return;
                }
                
                var offerid = $("#btnAdd").length && $("#btnAdd").is(":visible") ? $("#btnAdd").attr("data-offerid") : $("#addToWishlist").attr("data-offerid");
                $.ajax({
                        dataType: "json",
                        cache: false,
                        type: "POST",
                        async: false,
                        data: {
                            offerId: offerid,
                            customOptions: htmlEncode($("#customOptionsHidden_" + $("#hfProductId").val()).length > 0 ? $("#customOptionsHidden_" + $("#hfProductId").val()).val() : null)
                        },
                        url: "httphandlers/details/addtowishlist.ashx",
                        success: function (data) {
                            $("#addToWishlist").text(localize("AlreadyInWishlist"));
                            $("#addToWishlist").attr("href", "wishlist.aspx");
                            $("#addToWishlist").off("click");
                            e.preventDefault();
                        },
                        error: function () {
                            notify(localize("WishlistError"), notifyType.error, true);
                        }
                    });

                
            });
        }
    });


    $(window).load(function () {
        //$('.cloud-zoom').CloudZoom();
        //$('#zoom').removeClass('cloud-zoom-progress');
    });


})(jQuery);


