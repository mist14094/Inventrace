function clickButton(e, buttonid) {
    var evt = e ? e : window.event;
    var bt = document.getElementById(buttonid);

    if (bt) {
        if (evt.keyCode == 13) {
            bt.click();
            return false;
        }
    }
}

function keyboard_navigation(D) {
    D = D || window.event;
    var A = D.keyCode;
    if (D.ctrlKey) {
        if (A == 13) {
            //Enter
        }
        var E = (D.target || D.srcElement).tagName;
        if (E != "INPUT" && E != "TEXTAREA") {
            var C;
            if (A == 37) {
                C = document.getElementById("prevpage");
            }
            if (A == 39) {
                C = document.getElementById("nextpage");
            }
            if (C && C != undefined && C.href != undefined) {
                //C.onclick();
                location.href = C.href;
            }
            if ((A == 38) && B.text) {
                B.text.focus();
                B.text.select();
            }
        }
    }
}

function open_window(link, w, h) {
    var xOffset = screen.availWidth / 2 - w / 2;
    var yOffset = screen.availHeight / 2 - h / 2; //(opener.outerHeight *2)/10;

    xOffset = xOffset > 0 ? xOffset : 0;
    yOffset = yOffset > 0 ? yOffset : 0;

    var win = "width=" + w + ",height=" + h + ",menubar=no,location=no,resizable=yes,scrollbars=yes,left=" + xOffset + ",top=" + yOffset;
    wishWin = window.open(link, 'wishWin', win);
    wishWin.focus();
}

function open_printable_version(link) { var win = "menubar=no,location=no,resizable=yes,scrollbars=yes"; newWin = window.open(link, 'printableWin', win); newWin.focus(); }
//function TextBoxKeyPress() {
//    if (document.all) {
//        if (event.keyCode == 13) {
//            event.returnValue = false;
//            event.cancel = true;
//            var btn;
//            var btns = $("input.btn");
//            for (var i = 0; i < btns.length; i++)
//                if (btns[i].value == "Filter") {
//                btn = btns[i];
//                break;
//            }
//            btn.click();
//        }
//    }
//}


function translite(src) {
    var lvol = new Array();
    var uvol = new Array();
    var lrus = new String("абвгдеёжзийклмнопрстуфхцчшщъыьэюя");
    var urus = new String("АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ");
    var res = new String('');
    lvol[0] = 'a'; uvol[0] = 'A';
    lvol[1] = 'b'; uvol[1] = 'B';
    lvol[2] = 'v'; uvol[2] = 'V';
    lvol[3] = 'g'; uvol[3] = 'G';
    lvol[4] = 'd'; uvol[4] = 'D';
    lvol[5] = 'e'; uvol[5] = 'E';
    lvol[6] = 'e'; uvol[6] = 'E';
    lvol[7] = 'zh'; uvol[7] = 'Zh';
    lvol[8] = 'z'; uvol[8] = 'Z';
    lvol[9] = 'i'; uvol[9] = 'I';
    lvol[10] = 'i'; uvol[10] = 'I';
    lvol[11] = 'k'; uvol[11] = 'K';
    lvol[12] = 'l'; uvol[12] = 'L';
    lvol[13] = 'm'; uvol[13] = 'M';
    lvol[14] = 'n'; uvol[14] = 'N';
    lvol[15] = 'o'; uvol[15] = 'O';
    lvol[16] = 'p'; uvol[16] = 'P';
    lvol[17] = 'r'; uvol[17] = 'R';
    lvol[18] = 's'; uvol[18] = 'S';
    lvol[19] = 't'; uvol[19] = 'T';
    lvol[20] = 'u'; uvol[20] = 'U';
    lvol[21] = 'f'; uvol[21] = 'F';
    lvol[22] = 'kh'; uvol[22] = 'Kh';
    lvol[23] = 'ts'; uvol[23] = 'Ts';
    lvol[24] = 'ch'; uvol[24] = 'Ch';
    lvol[25] = 'sh'; uvol[25] = 'Sh';
    lvol[26] = 'sch'; uvol[26] = 'Sch';
    lvol[27] = ''; uvol[27] = '';
    lvol[28] = 'y'; uvol[28] = 'Y';
    lvol[29] = ""; uvol[29] = "";
    lvol[30] = 'e'; uvol[30] = 'E';
    lvol[31] = 'iu'; uvol[31] = 'Iu';
    lvol[32] = 'ia'; uvol[32] = 'Ia';

    src = src.replace(/[^a-zA-Zа-яА-Я 0-9]+/g, ''); // убираем все не буквы, цирры и пробелы
    src = src.replace(/ /gi, '-'); // заменяем пробелы на тире

    // заменяем буквы траснлитом
    for (var i = 0; i < src.length; i++) {
        if (lrus.indexOf(src.charAt(i)) >= 0) {
            res += lvol[lrus.indexOf(src.charAt(i))];
        } else if (urus.indexOf(src.charAt(i)) >= 0) {
            res += uvol[urus.indexOf(src.charAt(i))];
        } else {
            res += src.charAt(i);
        }
    }
    return res;
}

$(function () {
    tabInit();

    decodeFormData();

    if ($("input.autocompleteRegion").length) {
        $("input.autocompleteRegion").autocomplete("../HttpHandlers/GetRegions.ashx", {
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
        $("input.autocompleteCity").autocomplete('../HttpHandlers/GetCities.ashx', {
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

    $(".main-menu > ul > li.main-menu-item").each(function () {

        var show_parent = false;

        $(this).children("ul").children(".m-hide").each(function () {

            if ($(this).children("ul").children(".m-item").length > 0) {

                $(this).removeClass("m-hide");
                show_parent = true;
            }
        });

        if (!show_parent) {
            show_parent = $(this).children("ul").children(".m-item").length > 0;
        }

        if (show_parent && $(this).hasClass("m-hide")) {
            $(this).removeClass("m-hide");
        }
    });


    
    $(document).on('keydown.pagenumber',function (e) {
        //37 - left arrow
        //39 - right arrow
        if (e.ctrlKey === true && e.keyCode === 37) {
            if ($("#paging-prev").length)
                document.location = $("#paging-prev").attr("href");
        } else if (e.ctrlKey === true && e.keyCode === 39) {
            if ($("#paging-next").length)
                document.location = $("#paging-next").attr("href");
        }
    });


    $(".tpl-save-btn").click(function () {

        var settings = [];

        $(".tpl-settings-control").each(function () {
            var control = $(this).children();

            if (control.is("input[type='checkbox']")) {
                settings.push(control.attr("id") + "~" + (control.is(":checked") ? "True" : "False"));
            } else {
                settings.push(control.attr("id") + "~" + control.val());
            }
        });

        $.ajax({
            dataType: "json",
            traditional: true,
            cache: false,
            type: "POST",
            async: false,
            data: { settings: settings },
            url: "httphandlers/settings/savetemplatesettings.ashx",
            success: function (data) {
                $(".tpl-settings-result").html(data.result);
            },
            error: function (data) {
                $(".tpl-settings-result").html(data.result);
                this.location = this.location;
            }
        });
    });
});

function tabInit() {
    if ($("#tabs").length) {
        $("#tabs").advTabs({
            headers: "#tabs-headers > li",
            contents: "#tabs-contents  div.tab-content"
        });

        $("select, input, textarea", "#tabs-contents").change(function (e) {
            e = e || window.event;
            var tabContent = $(e.target).closest("div.tab-content");
            var tabdata = $("#tabs").data("tabs");
            var idx = $(tabdata.contents).index(tabContent);

            var tabHeader;
            if (idx != -1) {
                tabHeader = $(tabdata.headers[idx]);
                tabHeader.find("img.floppy").fadeIn();
            }
        });
    }
}

function SearchProduct() {
    if ($("input.autocompleteSearch").length) {
        $("input.autocompleteSearch").autocomplete('httphandlers/order/productssearch.ashx', {
            delay: 15,
            minChars: 1,
            matchSubset: 1,
            autoFill: false,
            matchContains: 1,
            cacheLength: null,
            selectFirst: false,
            maxItemsToShow: 30,
            onItemSelect: function (li, $lnk, $input) {
                if ($(".acsearchhf").length > 0) {
                    var indexOf = li.innerHTML.indexOf("<span");
                    var offer = $(li).find("span").attr("data-offerid");

                    if (offer != null) {
                        $(".acsearchhf").val(offer);
                    }
                    $input.val(li.innerHTML.substring(0, indexOf));
                }
            }
        });
    }
}