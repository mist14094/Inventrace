﻿var mpBuyInOneClick;

$(function () {
    if ($("#modalBuyInOneClick").length) {
        mpBuyInOneClick = $.advModal({
            title: localize("buyInOneClick_Confirm"),
            htmlContent: $("#modalBuyInOneClick"),
            control: "#lBtnBuyInOneClick",
            afterOpen: function () { initValidation($("form")); $("#errorOneClick").text(""); },
            afterClose: function () { confirmationProductInOneClickFinal(); initValidation($("form")); },
            beforeClose: function () { confirmationProductInOneClickFinal(); },
            clickOut: false
        });
    }

    if ($("#btnBuyInOneClick").length) {
        $("#btnBuyInOneClick").click(function () {
            if ($('form').valid('buyInOneClick') === true) {
                confirmationProductInOneClick(
                    $("#hfProductId").val(),
                    $("#lBtnBuyInOneClick").attr('data-buyoneclick-offerid'),
                    $("#customOptionsHidden_" + $("#hfProductId").val()).length > 0 ? $("#customOptionsHidden_" + $("#hfProductId").val()).val() : null,
                    $("#txtAmount").val(),
                    $("#txtName").val(),
                    $("#txtPhone").val(),
                    $("#txtComment").val(),
                    $("#lBtnBuyInOneClick").attr("data-page"));
            }
        });
    }
    if ($("#btnBuyInOneClickOk").length) {
        $("#btnBuyInOneClickOk").click(function () {
            mpBuyInOneClick.modalClose();
            confirmationProductInOneClickFinal();
        });
    }
});

function confirmationProductInOneClick(productId, offerId, customOptions, amount, name, phone, comment, page) {
    $.ajax({
        dataType: "json",
        cache: false,
        type: "POST",
        async: false,
        data: {
            productId: htmlEncode(productId),
            offerId: htmlEncode(offerId),
            customOptions: htmlEncode(customOptions),
            amount: htmlEncode(amount),
            name: htmlEncode(name),
            phone: htmlEncode(phone),
            comment: htmlEncode(comment),
            page: htmlEncode(page)
        },
        url: "httphandlers/details/buyinoneclick.ashx",
        beforesend: function () {
            $("#modalBuyInOneClickFinal").hide();
            $("#modalBuyInOneClickForm").show();
        },
        success: function (data) {
            if (data != null && data.type === "error") {
                $("#errorOneClick").text(data.result);
            } else {
            $("#modalBuyInOneClickFinal").show();
            $("#modalBuyInOneClickForm").hide();
            if (data != null && data.result == "reload") {
                if ($("#btnBuyInOneClickOk").length) {
                    $("#btnBuyInOneClickOk").click(function () {
                        confirmationProductInOneClickFinalReload();
                    });
                }
            }
            }
        },
        error: function () {
            mpBuyInOneClick.modalClose();
        }
    });
}

function confirmationProductInOneClickFinal() {

    $("#modalBuyInOneClickFinal").hide();
    $("#modalBuyInOneClickForm").show();
    $("#txtName").val("");
    $("#txtComment").val("");
}
function confirmationProductInOneClickFinalReload() {
    mpBuyInOneClick.modalClose();
    window.location = $("base").attr("href");
}