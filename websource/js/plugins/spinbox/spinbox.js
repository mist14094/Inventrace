(function ($) {

    var advantshop = Advantshop, scriptManager = advantshop.ScriptsManager, utilities = advantshop.Utilities;

    var spinbox = function (selector, options) {
        this.$obj = advantshop.GetJQueryObject(selector);
        this.options = $.extend({}, this.defaultOptions, options);

        return this;
    };

    advantshop.NamespaceRequire('Advantshop.ScriptsManager');
    scriptManager.Spinbox = spinbox;

    spinbox.prototype.InitTotal = function () {
        var objects = $('[data-plugin="spinbox"]');

        for (var i = 0, arrLength = objects.length; i < arrLength; i += 1) {
            spinbox.prototype.Init(objects.eq(i), utilities.Eval(objects.eq(i).attr('data-spinbox-options')) || {});
        }
    };

    $(spinbox.prototype.InitTotal); // call document.ready

    spinbox.prototype.Init = function (selector, options) {
        var spinboxObj = new spinbox(selector, options);

        spinboxObj.GenerateHtml();
        spinboxObj.BindEvent();

        return spinboxObj;
    };

    spinbox.prototype.GenerateHtml = function () {
        var spinboxObj = this;
        var numVal;
        numVal = spinboxObj.$obj.val().replace(',', '.');
        var value$Obj = Number(numVal);
        spinboxObj.$obj.addClass('spinbox');
        var $plus = $('<span />', { 'class': 'spinbox-control spinbox-plus' + (value$Obj === Number(spinboxObj.options.max) ? ' spinbox-disabled-plus' : '') });
        var $minus = $('<span />', { 'class': 'spinbox-control spinbox-minus' + (value$Obj === Number(spinboxObj.options.min) ? ' spinbox-disabled-minus' : '') });

        spinboxObj.Parts = {};
        spinboxObj.Parts.$plus = $plus;
        spinboxObj.Parts.$minus = $minus;

        var tempCollection = $plus.add($minus);

        spinboxObj.$obj.after(tempCollection);
        spinboxObj.$obj.val();
    };

    spinbox.prototype.BindEvent = function () {
        var spinboxObj = this;

        spinboxObj.$obj.on('keydown.spinbox', function (e) {

            if (e.altKey || e.ctrlKey || e.shiftKey) {
                e.preventDefault();
                return false;
            }

            var code = e.keyCode,
                result = true,
                valOld = spinboxObj.$obj.val();

            //numpad
            if (code > 95 && code < 106) {
                code = code - 48;
            }

            
            //8 - backspace, 46 - delete
            if (code == 8 || code == 46) {
                result = true;
            } else if (code == 110 || code == 188 || code == 190 || code == 191) { // '.' - NumPad, En, Ru,
                result = true;
            } else {
                var symbol = Number(String.fromCharCode(code));
                if (isNaN(symbol) === true) {
                    result = false;
                }
            }

            if (result === true) {
                spinboxObj.valOld = valOld;
            }

            return result;
        });

        spinboxObj.$obj.on('keyup.spinbox', function (e) {
            var caretPos;
            switch (e.keyCode) {
                case 40:
                    // down arrow
                    spinboxObj.minus(e);
                    break;
                case 38:
                    // up arrow
                    spinboxObj.plus(e);
                    break;
                case 37:
                    // left arrow
                    if (spinboxObj.$obj[0]) {
                        caretPos = spinboxObj.$obj[0].selectionStart - 1;
                        if (caretPos == 0)
                            caretPos = "start";
                        spinboxObj.$obj.advMoveCaret(caretPos);
                    }
                    break;
                case 39:
                    // right arrow
                    if (spinboxObj.$obj[0]) {
                        caretPos = spinboxObj.$obj[0].selectionStart + 1;
                        if (caretPos == spinboxObj.$obj.val().length)
                            caretPos = "end";
                        spinboxObj.$obj.advMoveCaret(caretPos);
                    }
                    break;
                default:
                    spinboxObj.manual(e);
                    break;
            }
        });

        spinboxObj.$obj.on('mousewheel.spinbox', function (e, delta) {
            var dir = delta > 0 ? 'plus' : 'minus';

            if (dir === 'plus') {
                spinboxObj.plus(e);
            } else {
                spinboxObj.minus(e);
            }

        });
        
        spinboxObj.$obj.on('blur', function (e) {
            var num = Number(spinboxObj.$obj.val());
            var newVal = num;
            
            if (isNaN(num) || num < spinboxObj.options.min) {
                newVal = spinboxObj.options.min;
            }
			
			
            var overstep = (newVal % spinboxObj.options.step).toFixed(3);
            if (overstep != 0 && overstep != spinboxObj.options.step) {
                newVal = (newVal + spinboxObj.options.step - newVal % spinboxObj.options.step).toFixed(3);
            }
			
			if (newVal > spinboxObj.options.max) {
                newVal = spinboxObj.options.max;
            }
			

            spinboxObj.valNew = parseFloat(newVal);
            spinboxObj.$obj.val(parseFloat(newVal));
            spinboxObj.set(e);
        });


        spinboxObj.Parts.$plus.on('click.spinbox', function (e) {
            spinboxObj.plus(e);
        });

        spinboxObj.Parts.$minus.on('click.spinbox', function (e) {
            spinboxObj.minus(e);
        });
    };

    spinbox.prototype.plus = function (e) {
        var spinboxObj = this;
        var caretPos = spinboxObj.caretPosition();

        spinboxObj.valOld = Number(spinboxObj.$obj.val());
        spinboxObj.valNew = Number((spinboxObj.valOld + Number(spinboxObj.options.step)).toFixed(3));
        spinboxObj.set(e);

        spinboxObj.$obj.advMoveCaret(caretPos);
    };

  spinbox.prototype.minus = function (e) {
        var spinboxObj = this;
        var caretPos = spinboxObj.caretPosition();

        spinboxObj.valOld = Number(spinboxObj.$obj.val());
        spinboxObj.valNew = Number((spinboxObj.valOld - Number(spinboxObj.options.step)).toFixed(3));
        spinboxObj.set(e);

        spinboxObj.$obj.advMoveCaret(caretPos);
    };

    spinbox.prototype.manual = function (e) {
        var spinboxObj = this;
        var caretPos = spinboxObj.caretPosition();

        var newVal = spinboxObj.$obj.val().replace(',', '.');
        if (newVal[newVal.length - 1] == '.')
            spinboxObj.set(e);
        else if (newVal == '0') {
            spinboxObj.valNew = 0;
            spinboxObj.set(e);
        } else {
            spinboxObj.valNew = Number(Number(spinboxObj.$obj.val()).toFixed(3));
            spinboxObj.set(e);
        }
        spinboxObj.$obj.advMoveCaret(caretPos);
        return true;
    };

    spinbox.prototype.set = function (e) {
        var spinboxObj = this,
            input = spinboxObj.$obj,
            minus = spinboxObj.Parts.$minus,
            plus = spinboxObj.Parts.$plus,
            valNew = input.val().replace(',', '.');

        if (valNew[valNew.length - 1] != '.')
            valNew = spinboxObj.valNew;
        
        if (input.val().length === 0) {

            if (e.type === 'click') {
                valNew = spinboxObj.options.min;
            } else {
                return;
            }
        }


        plus.removeClass('spinbox-disabled-plus');
        minus.removeClass('spinbox-disabled-minus');

        if (valNew >= Number(spinboxObj.options.max)) {
            plus.addClass('spinbox-disabled-plus');
            minus.removeClass('spinbox-disabled-minus');
        }

        if (valNew <= Number(spinboxObj.options.min)) {
            plus.removeClass('spinbox-disabled-plus');
            minus.addClass('spinbox-disabled-minus');
        }

        input.val(valNew);
        input.trigger('set');
    };

    spinbox.prototype.caretPosition = function() {
        var pos = -1;
        if (this.$obj[0]) {
            pos = this.$obj[0].selectionStart;
            if (pos == this.$obj.val().length)
                pos = "end";
            if (pos == 0)
                pos = "start";
        }
        return pos;
    };

    spinbox.prototype.defaultOptions = {
        min: 0,
        max: Number.POSITIVE_INFINITY,
        step: 1
    };
})(jQuery);
