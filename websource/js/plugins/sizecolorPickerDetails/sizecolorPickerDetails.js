(function ($) {

    var storage = {};

    var sizeColorPickerDetails = function (storageOffers, place) {

        var urlObj = new Advantshop.Utilities.Uri(window.location.href),
            sizeId = parseInt(urlObj.getQueryParamValue('size')),
            colorId = parseInt(urlObj.getQueryParamValue('color'));

        this.$obj = place;
        this.isCanOrderByRequest = Advantshop.Utilities.Eval(this.$obj.attr('data-orderbyrequest').toLowerCase());
        this.storageOffers = storageOffers;
        this.storageOffers.sizeHeader = this.$obj.attr('data-sizeHeader');
        this.storageOffers.colorHeader = this.$obj.attr('data-colorHeader');

        this.PrepareData();

        if (this.storageOffers.isColorsNull === true && this.storageOffers.isSizesNull === true) {
            return null;
        }

        if (this.storageOffers.isColorsNull === false) {
            this.storageOffers.ColorIdSelected = colorId || this.storageOffers.ColorIdSelected || this.storageOffers.Colors[0].ColorId;
        }

        if (this.storageOffers.isSizesNull === false) {
            this.storageOffers.SizeIdSelected = sizeId || this.storageOffers.SizeIdSelected || this.storageOffers.Sizes[0].SizeId;
        }
    };

    Advantshop.NamespaceRequire('Advantshop.Details.Part');

    Advantshop.Details.Part.SizeColorPickerDetails = sizeColorPickerDetails;

    sizeColorPickerDetails.prototype.Init = function (storageOffers, place) {

        if (place.length === 0) {
            return null;
        }

        var sizeColorPickerDetailsObj = new sizeColorPickerDetails(storageOffers, place);

        sizeColorPickerDetailsObj.Filter();

        sizeColorPickerDetailsObj.Generate();

        sizeColorPickerDetailsObj.BindEvent();

        return sizeColorPickerDetailsObj;
    };

    sizeColorPickerDetails.prototype.InitTotal = function () {
        var placeSizeColors = $('[data-part="sizeColorPickerDetails"]'),
            productId, offers, options;

        if (placeSizeColors.length === 0) {
            return null;
        }


        for (var i = 0, l = placeSizeColors.length; i < l; i += 1) {
            productId = placeSizeColors.eq(i).attr('data-productId');
            offers = new Advantshop.Offers(productId);
            options = Advantshop.Utilities.Eval(placeSizeColors.eq(i).attr('data-sizeColorPickerDetails-options')) || {};
            sizeColorPickerDetails.prototype.Init(offers.storageOffers, placeSizeColors.eq(i), options);
        }
    };

    $(sizeColorPickerDetails.prototype.InitTotal);


    sizeColorPickerDetails.prototype.PrepareData = function () {
        var storageOffersCurrent = this.storageOffers,
            arrColorsTemp = [],
            arrSizesTemp = [];

        storageOffersCurrent.isColorsNull = false,
        storageOffersCurrent.isSizesNull = false;

        if (this.isCanOrderByRequest === true) {
            storageOffersCurrent.isColorsNull = storageOffersCurrent.Colors.length === 0;
            storageOffersCurrent.isSizesNull = storageOffersCurrent.Sizes.length === 0;
            arrColorsTemp = storageOffersCurrent.Colors;
            arrSizesTemp = storageOffersCurrent.Sizes;
        } else {
            for (var c = 0, arrColors = storageOffersCurrent.Colors.length; c < arrColors; c += 1) {
                if (storageOffersCurrent.Colors[c].ColorId != null) {
                    arrColorsTemp.push(storageOffersCurrent.Colors[c]);
                }
            }

            for (var s = 0, arrSizes = storageOffersCurrent.Sizes.length; s < arrSizes; s += 1) {
                if (storageOffersCurrent.Sizes[s].SizeId != null) {
                    arrSizesTemp.push(storageOffersCurrent.Sizes[s]);
                }
            }
        }


        var mainOffer = null;
        // вытаскиваем главный оффер
        for (var i = 0, len = storageOffersCurrent.Offers.length; i < len; i++) {
            if ((storageOffersCurrent.Offers[i].Main === true && storageOffersCurrent.Offers[i].Amount > 0 && this.isCanOrderByRequest === false) ||
                (storageOffersCurrent.Offers[i].Main === true && this.isCanOrderByRequest === true)) {
                mainOffer = storageOffersCurrent.Offers[i];
                break;
            }
        }

        // если нет главного оффера, выбираем оффер, такой чтобы его цвет и размер были первыми в списках (все цвета и все размеры)
        if (mainOffer == null && (arrColorsTemp.length > 0 || arrSizesTemp.length > 0)) {
            for (var j = 0, lenj = storageOffersCurrent.Offers.length; j < lenj; j++) {

                if (storageOffersCurrent.Offers[j].Amount > 0) {
                    if (arrColorsTemp.length > 0 && arrSizesTemp.length > 0 &&
                        storageOffersCurrent.Offers[j].Color.ColorId == arrColorsTemp[0].ColorId &&
                        storageOffersCurrent.Offers[j].Size.SizeId == arrSizesTemp[0].SizeId) {

                        mainOffer = storageOffersCurrent.Offers[j];
                        break;

                    } else if (arrColorsTemp.length > 0 && storageOffersCurrent.Offers[j].Color.ColorId == arrColorsTemp[0].ColorId) {
                        mainOffer = storageOffersCurrent.Offers[j];
                        break;
                    } else if (arrSizesTemp.length > 0 && storageOffersCurrent.Offers[j].Size.SizeId == arrSizesTemp[0].SizeId) {
                        mainOffer = storageOffersCurrent.Offers[j];
                        break;
                    }
                }
            }
        }

        // если ни один оффер не подошел, возьмем первый попавшийся с положительным Amount
        if (mainOffer == null) {
            for (var k = 0, lenk = storageOffersCurrent.Offers.length; k < lenk; k++) {
                if (storageOffersCurrent.Offers[k].Amount > 0) {
                    mainOffer = storageOffersCurrent.Offers[k];
                    break;
                }
            }
        }


        if (arrColorsTemp.length > 0) {
            storageOffersCurrent.Colors = arrColorsTemp;
            if (mainOffer != null && mainOffer.Color != null) {
                storageOffersCurrent.ColorIdSelected = mainOffer.Color.ColorId;
            }
        } else {
            storageOffersCurrent.Colors = [];
            storageOffersCurrent.isColorsNull = true;
        }

        if (arrSizesTemp.length > 0) {
            storageOffersCurrent.Sizes = arrSizesTemp;
            if (mainOffer != null && mainOffer.Size != null) {
                storageOffersCurrent.SizeIdSelected = mainOffer.Size.SizeId;
            }
        } else {
            storageOffersCurrent.Sizes = [];
            storageOffersCurrent.isSizesNull = true;
        }
    };

    sizeColorPickerDetails.prototype.Generate = function () {

        var opts = this.options;

        new EJS({ url: 'js/plugins/sizeColorPickerDetails/templates/sizeColorPickerDetails.tpl' }).update(this.$obj[0], this.storageOffers);
    
    };

    sizeColorPickerDetails.prototype.BindEvent = function () {
        var sizeColorPickerDetailsObj = this,
            opts = sizeColorPickerDetailsObj.options,
            storageOffers = sizeColorPickerDetailsObj.storageOffers;

        this.$obj.on('click', '.color-item', function () {

            if (this.className.indexOf('selected') !== -1) {
                return;
            }

            sizeColorPickerDetailsObj.Filter({ Type: sizeColorPickerDetailsObj.FilterType.Color, Value: parseInt(this.getAttribute('data-color-id')) });

            $(this).trigger('changeColor', [sizeColorPickerDetailsObj]);

            sizeColorPickerDetailsObj.Generate();
          
        });

        this.$obj.on('click', '.size-item', function () {

            if (this.className.indexOf('selected') !== -1 || this.getAttribute('data-disabled') != null) {
                return;
            }

            sizeColorPickerDetailsObj.$obj.find('.size-item').removeClass('selected');

            $(this).addClass('selected');

            sizeColorPickerDetailsObj.Filter({ Type: sizeColorPickerDetailsObj.FilterType.Size, Value: parseInt(this.getAttribute('data-size-id')) });

            $(this).trigger('changeSize', [sizeColorPickerDetailsObj]);

        });

    };

    sizeColorPickerDetails.prototype.Filter = function (param) {

        var sizeColorPickerDetailsObj = this, storageOffers = this.storageOffers;

        if (param != null) {
            switch (param.Type) {
                case sizeColorPickerDetails.prototype.FilterType.Color:
                    storageOffers.ColorIdSelected = param.Value;
                    break;
                case sizeColorPickerDetails.prototype.FilterType.Size:
                    storageOffers.SizeIdSelected = param.Value;
                    break;
            }
        }

        var colorId, colorIdx, SizeId, SizeIdx, offerSelectedAlternative;

        //reset state

        if (storageOffers.isSizesNull === false) {
            for (var s = 0, arrSizeLength = storageOffers.Sizes.length; s < arrSizeLength; s += 1) {
                storageOffers.Sizes[s].isDisabled = null;
            }
        }

        storageOffers.offerSelected = null;

        //filter go!

        if (storageOffers.isColorsNull === false && storageOffers.isSizesNull === false) {

            for (var i = 0, arrSizes = storageOffers.Offers.length; i < arrSizes; i += 1) {

                if (storageOffers.Offers[i].Color == null || storageOffers.Offers[i].Size == null) {
                    continue;
                }

                colorId = storageOffers.Offers[i].Color.ColorId;
                colorIdx = sizeColorPickerDetailsObj.inArray(storageOffers.Colors, 'ColorId', colorId);
                SizeId = storageOffers.Offers[i].Size.SizeId;
                SizeIdx = sizeColorPickerDetailsObj.inArray(storageOffers.Sizes, 'SizeId', SizeId);

                if (storageOffers.Sizes[SizeIdx].isDisabled === false) {
                    continue;
                }

                if ((sizeColorPickerDetailsObj.isCanOrderByRequest === true && colorId === storageOffers.ColorIdSelected) || (colorId === storageOffers.ColorIdSelected && storageOffers.Offers[i].Amount > 0)) {

                    offerSelectedAlternative = offerSelectedAlternative || storageOffers.Offers[i];

                    if (SizeId === storageOffers.SizeIdSelected) {
                        storageOffers.offerSelected = storageOffers.offerSelected || storageOffers.Offers[i];
                    }

                    storageOffers.Sizes[SizeIdx].isDisabled = false;

                } else {
                        storageOffers.Sizes[SizeIdx].isDisabled = true; 
                }
            }
        } else if (storageOffers.isColorsNull === false) {
            for (var c = 0, arrColors = storageOffers.Offers.length; c < arrColors; c += 1) {
                colorId = storageOffers.Offers[c].Color.ColorId;
                colorIdx = sizeColorPickerDetailsObj.inArray(storageOffers.Colors, 'ColorId', colorId);

                if ((sizeColorPickerDetailsObj.isCanOrderByRequest === true && colorId === storageOffers.ColorIdSelected) || (colorId === storageOffers.ColorIdSelected && storageOffers.Offers[c].Amount > 0)) {
                    offerSelectedAlternative = offerSelectedAlternative || storageOffers.Offers[c];
                }
            }
        } else if (storageOffers.isSizesNull === false) {
            for (var s = 0, arrSizes = storageOffers.Offers.length; s < arrSizes; s += 1) {
                SizeId = storageOffers.Offers[s].Size.SizeId;
                SizeIdx = sizeColorPickerDetailsObj.inArray(storageOffers.Sizes, 'SizeId', SizeId);

                if ((sizeColorPickerDetailsObj.isCanOrderByRequest === true && SizeId === storageOffers.SizeIdSelected) || (SizeId === storageOffers.SizeIdSelected && storageOffers.Offers[s].Amount > 0)) {
                    offerSelectedAlternative = offerSelectedAlternative || storageOffers.Offers[s];
                }
            }
        }

        storageOffers.offerSelected = storageOffers.offerSelected || offerSelectedAlternative;

        if (storageOffers.isColorsNull === false) {
            storageOffers.ColorIdSelected = storageOffers.offerSelected.Color.ColorId;
        }

        if (storageOffers.isSizesNull === false) {
            storageOffers.SizeIdSelected = storageOffers.offerSelected.Size.SizeId;
        }

        this.$obj.trigger('sizeColorFilter', [sizeColorPickerDetailsObj]);
    };

    sizeColorPickerDetails.prototype.FilterType = { Color: 'color', Size: 'size' };

    sizeColorPickerDetails.prototype.inArray = function (arr, name, val) {

        var index = -1;

        for (var i = 0, arrLength = arr.length; i < arrLength; i += 1) {
            if (arr[i][name] === val) {
                index = i;
                break;
            }
        }

        return index;
    }

})(jQuery);
