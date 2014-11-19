(function ($) {
    var storage = {};

    var offers = function (productId) {
        if (productId == null || productId.length === 0) {
            throw Error('Undefined productId');
        }

        if (productId in storage) {
            return storage[productId];
        }

        this.productId = productId;

        this.GetOffers();
    };

    offers.prototype.GetOffers = function () {
        var offersObj = this;
        $.ajax({
            type: 'GET',
            dataType: 'json',
            async: false,
            url: 'httphandlers/details/offers.ashx',
            data: { productId: offersObj.productId },
            success: function (data) {

                offersObj.storageOffers = data;

                storage[offersObj.productId] = offersObj;
            },
            error: function (data) {
                throw Error(data.responseText);
            }
        });

        return storage[offersObj.productId];
    };

    offers.prototype.GetPrice = function (params) {
        var offersObj = this;
        var result;

        if (params == null) {

            if (offersObj.storageOffers.offerSelected == null) {
                return '';
            }

            var cOptions = document.getElementById('customOptionsHidden_' + offersObj.productId);

            params = {
                price: offersObj.storageOffers.offerSelected.Price,
                discount: offersObj.storageOffers.offerSelected.Discount,
                attributesXml: cOptions != null ? cOptions.value : null,
                productId: offersObj.productId
            };
        }

        $.ajax({
            type: 'POST',
            dataType: 'json',
            url: 'httphandlers/details/offerprice.ashx',
            data: params,
            async: false,
            success: function (data) {
                result = data.Price;
            },
            error: function (data) {
                throw Error(data.responseText);
            }
        });

        return result;
    };


    offers.prototype.GetFirstPaymentPrice = function (params) {
        var offersObj = this;
        var result;

        if (params == null) {

            if (offersObj.storageOffers.offerSelected == null) {
                return '';
            }

            var cOptions = document.getElementById('customOptionsHidden_' + offersObj.productId);

            var fPPercent = 0;
            if (document.getElementById('hfFirstPaymentPercent') != null) {
                fPPercent = document.getElementById('hfFirstPaymentPercent').attributes['value'].nodeValue;
            }

            params = {
                price: offersObj.storageOffers.offerSelected.Price,
                discount: offersObj.storageOffers.offerSelected.Discount,
                attributesXml: cOptions != null ? cOptions.value : null,
                productId: offersObj.productId,
                firstPaymentPercent: fPPercent
            };
        }

        $.ajax({
            type: 'POST',
            dataType: 'json',
            url: 'httphandlers/details/offerfirstpaymentPrice.ashx',
            data: params,
            async: false,
            success: function (data) {
                result = data.Price;
            },
            error: function (data) {
                throw Error(data.responseText);
            }
        });

        return result;
    };

    offers.prototype.UpdateProduct = function () {
        var offersObj = this,
            storageOffersCurrent = offersObj.storageOffers,
            priceContainer = document.getElementById('priceWrap'),
            firstPaymentPercentContainer = document.getElementById('lblFirstPayment'),
            lblFirstPaymentNoteContainer = document.getElementById('lblFirstPaymentNote'),
            sku = document.getElementById('skuValue'),
            btn = document.getElementById('btnAdd'),
            btnBuyInOneClick = document.getElementById('lBtnBuyInOneClick'),
            btnOrderByRequest = document.getElementById('btnOrderByRequest'),
            btnAddCredit = document.getElementById('btnAddCredit'),
            spanAvailability = document.getElementById('availability'),
            wishlist = document.getElementById('addToWishlist');

        if (storageOffersCurrent.offerSelected == null) {
            if (btn != null) {
                btn.style.display = 'inline-block';
                btn.setAttribute('data-offerid', storageOffersCurrent.Offers[0].OfferId); // for codebehaind
            }

            if (btnBuyInOneClick != null) {
                btnBuyInOneClick.style.display = 'inline-block';
                btn.setAttribute('data-offerid', storageOffersCurrent.Offers[0].OfferId); // for codebehaind
            }

            if (btnAddCredit != null) {
                btnAddCredit.style.display = 'inline-block';
                btn.setAttribute('data-offerid', storageOffersCurrent.Offers[0].OfferId); // for codebehaind
            }

            if (btn == null && btnOrderByRequest != null) {
                btnOrderByRequest.style.display = 'inline-block';
                btnOrderByRequest.setAttribute('data-offerid', storageOffersCurrent.Offers[0].OfferId); // for codebehaind
            }

            return;
        }


        var isAvalable = storageOffersCurrent.offerSelected.Amount > 0,
            isCanBuy = storageOffersCurrent.offerSelected.Price > 0;
            isCanPreorder = offersObj.storageOffers.AllowPreOrder;

        if (priceContainer != null) {
            priceContainer.innerHTML = this.GetPrice();
        }

        if (firstPaymentPercentContainer != null) {
            firstPaymentPercentContainer.innerHTML = this.GetFirstPaymentPrice();
        }

        if (sku != null && storageOffersCurrent.offerSelected != null) {
            sku.innerHTML = storageOffersCurrent.offerSelected.ArtNo;
        }

        if (spanAvailability != null) {
            if (storageOffersCurrent.offerSelected.Amount > 0) {
                if (storageOffersCurrent.ShowStockAvailability) {
                    spanAvailability.innerHTML = localize("detailsAvailable") +
                        " <span>(" + storageOffersCurrent.offerSelected.Amount + (storageOffersCurrent.Unit != "" ? " " + storageOffersCurrent.Unit : "") + ")</span>";
                } else {
                    spanAvailability.innerHTML = localize("detailsAvailable");
                }
                spanAvailability.setAttribute("class", "available");
            } else {
                spanAvailability.innerHTML = localize("detailsNotAvailable");
                spanAvailability.setAttribute("class", "not-available");
            }


            //if (storageOffersCurrent.ShowStockAvailability) {
            //    var formatStr = storageOffersCurrent.Unit == "" ? "{0} <span>{1}{2}</span>" : "{0} <span>({1} {2})</span>";
            //    spanAvalable.innerHTML = String.Format(formatStr, localize("productAvailable"), storageOffersCurrent.offerSelected.Amount, storageOffersCurrent.Unit);
            //}
        }

        ///if price  === 0
        if (isCanBuy === false) {

            if (btn != null) {
                btn.style.display = 'none';
            }

            if (btnBuyInOneClick != null) {
                btnBuyInOneClick.style.display = 'none';
            }
            if (btnAddCredit != null) {
                btnBuyInOneClick.style.display = 'none';
            }
            if (btnOrderByRequest != null) {
                btnOrderByRequest.style.display = 'none';
            }
            
            if (wishlist != null) {
                wishlist.style.display = 'none';
            }

        } else {
            if (btn != null) {
                btn.setAttribute('data-cart-add-offerid', storageOffersCurrent.offerSelected.OfferId);
                btn.setAttribute('data-offerid', storageOffersCurrent.offerSelected.OfferId); // for codebehaind
                btn.style.display = isAvalable === true ? 'inline-block' : 'none';
            }

            if (btnBuyInOneClick != null) {
                btnBuyInOneClick.setAttribute('data-buyoneclick-offerid', storageOffersCurrent.offerSelected.OfferId);
                btnBuyInOneClick.setAttribute('data-offerid', storageOffersCurrent.offerSelected.OfferId); // for codebehaind
                btnBuyInOneClick.style.display = isAvalable === true ? 'inline-block' : 'none';
            }

            if (btnAddCredit != null) {
                btnAddCredit.setAttribute('data-cart-add-offerid', storageOffersCurrent.offerSelected.OfferId);
                btnAddCredit.setAttribute('data-offerid', storageOffersCurrent.offerSelected.OfferId); // for codebehaind
                btnAddCredit.style.display = isAvalable === true ? 'inline-block' : 'none';
                firstPaymentPercentContainer.style.display = isAvalable === true ? 'inline-block' : 'none';
                lblFirstPaymentNoteContainer.style.display = isAvalable === true ? 'inline-block' : 'none';
            }

            if (btnOrderByRequest != null) {
                btnOrderByRequest.setAttribute('data-offerid', storageOffersCurrent.offerSelected.OfferId); // for codebehaind
                btnOrderByRequest.style.display = isAvalable === true ? 'none' : 'inline-block';
            }
            
            if (wishlist != null) {
                wishlist.setAttribute('data-offerid', storageOffersCurrent.offerSelected.OfferId);
            }

        }
    };

    Advantshop.Offers = offers;

})(jQuery);