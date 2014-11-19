
$(function () {

    if ($(".ddlViewOrderStatus").length) {
        var dll = $(".ddlViewOrderStatus");

        $(".ddlViewOrderStatus").change(function () {
            setOrderStatus(dll.val(), dll.attr("data-orderid"));
        });
    }
    if ($(".editableTextBoxInViewOrder").length) {
        var editableTextBox, progress;
        $(".editableTextBoxInViewOrder").focusout(function () {
            editableTextBox = $(this);
            progress = new Advantshop.ScriptsManager.Progress.prototype.Init(editableTextBox);
            progress.Show();
            updateOrderField(editableTextBox.attr("data-orderid"), editableTextBox.val(), editableTextBox.attr("data-field-type"));
            progress.Hide();
        });
    }
});

function setOrderPaid(paid, orderid) {

    $.ajax({
        dataType: "json",
        traditional: true,
        cache: false,
        type: "POST",
        async: false,
        data: { orderid: orderid, paid: paid },
        url: "httphandlers/order/setorderpaid.ashx",
        success: function () {
            if ($(".orders-list-name[data-current-order=1]").length) {
                if (paid == 1) {
                    $(".orders-list-name[data-current-order=1]").removeClass("orders-list").addClass("orders-list-ok");
                } else if (paid == 0) {
                    $(".orders-list-name[data-current-order=1]").removeClass("orders-list-ok").addClass("orders-list");
                }
            }
            notify(localize("Admin_ViewOrder_SaveOrderPayStatus"), "notify");
        },
        error: function () {
            notify(localize("Admin_ViewOrder_ErrorOnSaveOrderPayStatus"), "error");
        }
    });
}


function setOrderStatus(statusid, orderid) {

    $.ajax({
        dataType: "json",
        traditional: true,
        cache: false,
        type: "POST",
        async: false,
        data: { orderid: orderid, statusid: statusid },
        url: "httphandlers/order/setorderstatus.ashx",
        success: function (data) {
            if ($(".order-main").length) {
                $(".order-main").first().css("border-left-color", "#" + data.result);
            }
            if ($(".orders-list-row[data-current-order=1]").length) {
                $(".orders-list-row[data-current-order=1]").css("border-left-color", "#" + data.result);
            }
            notify(localize("Admin_ViewOrder_SaveOrderStatus"), "notify");
            $("#lnkSendMail").show();
        },
        error: function () {
            notify(localize("Admin_ViewOrder_ErrorOnSaveOrderStatus"), "error");
        },
        complete: function () {
        }
    });
}

function updateOrderField(orderid, text, field) {

    $.ajax({
        dataType: "json",
        traditional: true,
        cache: false,
        type: "POST",
        async: false,
        data: { orderid: orderid, text: text, field: field },
        url: "httphandlers/order/updateorderfield.ashx",
        success: function () {
            notify(localize("Admin_ViewOrder_SaveComment"), "notify");
        },
        error: function () {
            notify(localize("Admin_ViewOrder_ErrorOnSaveComment"), "error");
        }
    });
}

function sendMailOrderStatus(orderid) {

    $.ajax({
        dataType: "json",
        traditional: true,
        cache: false,
        type: "POST",
        async: false,
        data: { orderid: orderid },
        url: "httphandlers/order/sendmailorderstatus.ashx",
        success: function (data) {
            if ($(".order-main").length) {
                $(".order-main").first().css("border-left-color", "#" + data.result);
            }
            if ($(".orders-list-row[data-current-order=1]").length) {
                $(".orders-list-row[data-current-order=1]").css("border-left-color", "#" + data.result);
            }
            notify(localize("Admin_ViewOrder_SendMailStatusOrder"), "notify");
            $("#lnkSendMail").hide();
        },
        error: function () {
            notify(localize("Admin_ViewOrder_ErrorOnSendMailStatusOrder"), "error");
        },
        complete: function () {
        }
    });
}