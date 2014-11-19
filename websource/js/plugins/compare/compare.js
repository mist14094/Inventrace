(function ($) {

    var advantshop = window.Advantshop, scriptManager = advantshop.ScriptsManager, utilities = advantshop.Utilities, $animDiv = $('<div />', {
        'id': 'compareAnimation',
        'class': 'compare-animation'
    });

    var compare = function (selector, options) {
        this.options = $.extend({}, this.defaultOptions, options);
        this.$obj = advantshop.GetJQueryObject(selector);
        this.$lbl = $('label[for="' + this.$obj.attr('id') + '"]');
        this.offerid = this.$obj.val();
        this.$animationObj = $(this.$obj.attr('data-compare-animation-obj'));
        this.$compareCount = $(this.options.compareCount);
        this.isExistCount = this.$compareCount.length;
        this.$compareBasket = $(this.options.compareBasket);
        this.isExistCompareBasket = this.$compareBasket.length;

        return this;
    };

    advantshop.NamespaceRequire('Advantshop.ScriptsManager');
    scriptManager.Compare = compare;

    compare.prototype.InitTotal = function () {
        var objects = $('[data-plugin="compare"]');

        if (objects.length) {
            $(document.body).append($animDiv);
        }

        for (var i = 0, arrLength = objects.length; i < arrLength; i += 1) {
            compare.prototype.Init(objects.eq(i), utilities.Eval(objects.eq(i).attr('data-compare-options')) || {});
        }
    };

    $(compare.prototype.InitTotal); //call document.ready

    compare.prototype.Init = function (selector, options) {
        var compareObj = new compare(selector, options);

        compareObj.GenerateHtml();
        compareObj.BindEvent();

        return compareObj;
    };

    compare.prototype.GenerateHtml = function () {
        var compareObj = this;
        var options = compareObj.options;
        var $obj = compareObj.$obj;
        var $lbl = compareObj.$lbl;

        var $wrap = $('<div />', {
            'class': 'compare-wrap compare-type-' + options.type + ' ' + ($obj.is(':checked') ? 'compare-selected-type-' + options.type : '') + ' ' + options.classContainer
        });

        $obj.before($wrap);

        var tempCollection = $obj.add($lbl);

        $wrap.append(tempCollection);

        compareObj.Parts = {};

        compareObj.Parts.$wrap = $wrap;

        if (compareObj.$compareBasket.length > 0 && compareObj.$compareCount.length > 0 && parseInt(compareObj.$compareCount.text()) > 0) {
            compareObj.$compareBasket.removeClass(options.classDisabledBasket);
        } else {
            compareObj.$compareBasket.addClass(options.classDisabledBasket);
        }
    };

    compare.prototype.BindEvent = function () {
        var compareObj = this;

        compareObj.$lbl.on('click.compareLabel', function (e) {
            //a - link page compare
            if ($(e.target).is('a') === true) {
                e.stopPropagation();
                e.preventDefault();
                var win = window.open(e.target.href);
                //window.location.assign(e.target.href);
            }
        });

        compareObj.$obj.on('change.compare', function () {
            if ($(this).is(':checked') === true) {
                compareObj.Add();
            } else {
                compareObj.Remove();
            }
        });

        if (compareObj.$compareBasket.lenght > 0 && utilities.Events.isExistEvent(compareObj.$compareBasket, 'click.compareRedirect') != true) {
            compareObj.$compareBasket.on('click.compareRedirect', function (e) {
                if (parseInt(compareObj.$compareCount.text()) < 1) {
                    e.preventDefault();
                }
            });
        }
    };
    
    compare.prototype.Add = function () {
        var compareObj = this;

        $.ajax({
            url: "httphandlers/compareproducts/addproduct.ashx",
            dataType: "json",
            cache: false,
            data: { offerid: compareObj.offerid },
            success: function (data) {
                if (data !== true) {
                    notify("Error in compare cart" + " status text:" + data.statusText, notifyType.error, true);
                }

                if (compareObj.isExistCount && compareObj.isExistCompareBasket) {
                    compareObj.Animate();
                }

                compareObj.Parts.$wrap.addClass('compare-selected-type-' + compareObj.options.type);

                var lblText = localize('compareAlready') + ' (<a href="compareproducts.aspx" target="_blank">' + localize('compareView') + '</a>)';
                compareObj.$lbl.html(lblText);
            },
            error: function (data) {
                notify("Error in compare cart" + " status text:" + data.statusText, notifyType.error, true);
            }
        });
    };


    compare.prototype.Remove = function () {
        var compareObj = this;

        $.ajax({
            url: "httphandlers/compareproducts/deleteproduct.ashx",
            dataType: "json",
            cache: false,
            data: { offerid: compareObj.offerid },
            success: function (data) {
                if (data !== true) {
                    notify("Error in compare cart" + " status text:" + data.statusText, notifyType.error, true);
                }

                if (compareObj.isExistCount) {
                    compareObj.Refresh();
                }

                compareObj.Parts.$wrap.removeClass('compare-selected-type-' + compareObj.options.type);

                compareObj.$lbl.html(localize('compareAdd'));
            },
            error: function (data) {
                notify("Error in compare cart" + " status text:" + data.statusText, notifyType.error, true);
            }
        });
    };

    compare.prototype.Refresh = function () {
        var compareObj = this;
        $.ajax({
            url: "httpHandlers/compareProducts/getcomparecount.ashx",
            cache: false,
            dataType: "json",
            success: function (data) {
                compareObj.$compareCount.html(data);

                if (parseInt(data) > 0) {
                    compareObj.$compareBasket.removeClass(compareObj.options.classDisabledBasket);
                } else {
                    compareObj.$compareBasket.addClass(compareObj.options.classDisabledBasket);
                }
            },
            error: function (data) {
                notify("Error in compare" + " status text:" + data.statusText, notifyType.error, true);
            }
        });
    };

    compare.prototype.Animate = function () {
        var compareObj = this;
        var animateObj = compareObj.$animationObj;
        var offset = animateObj.offset();
        var size = { width: animateObj.outerWidth(), height: animateObj.outerHeight() };

        $animDiv.css({ top: offset.top, left: offset.left, width: size.width, height: size.height });

        $animDiv.html(animateObj.clone());

        var options = compareObj.options;
        var offsetFinish = compareObj.$compareBasket.offset();

        $animDiv.show();

        $animDiv.animate({ top: offsetFinish.top, left: offsetFinish.left, opacity: options.animationOpacity }, compareObj.animationSpeed, function () {
            compareObj.Refresh();
            compareObj.ClearContainerAnimate();
        });
    };

    compare.prototype.ClearContainerAnimate = function () {
        $('#compareAnimation').hide().empty().removeAttr('style');
    };

    compare.prototype.Type = { Checkbox: 'checkbox', Icon: 'icon' };

    compare.prototype.defaultOptions = {
        compareCount: '#compareCount',
        compareBasket: '#compareBasket',
        classDisabledBasket: 'compare-disabled',
        animationSpeed: 1200,
        animationOpacity: 0.1,
        type: compare.prototype.Type.Checkbox,
        classContainer: ''
    };

})(jQuery);