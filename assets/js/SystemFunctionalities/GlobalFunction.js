

$(function () {
    // signalr js code for start hub and send receive notification
    var notificationHub = $.connection.notificationHub;
    $.connection.hub.start().done(function () {
        console.log('Notification hub started');
    }).fail(function (e) {
        alert("Real time notification is not working. Refresh your page to update your notification");
        })




    notificationHub.client.notify = function (message) {
        if (message && message.toLowerCase() == "added") {
            GetValue();
            Notification();

            if (UserRoleName == "ENCODER") {
                SystemRequestedNotification();
            }
            else {
                SystemNotification();
            }
        }
       
    };

    GetValue();
    setNavigation();

    function GetValue() {
        $.ajax({
            type: 'GET',

            url: '/Home/GetCountSupplierNotification',
            success: function (data) {
                $('span.count').html(data)

            },
            error: function (error) {
                console.log(error);
            }
        })
    }
    Notification();

    function Notification() {
        $.ajax({
            url: "/Home/PopulateNotification",
            type: "GET",
            success: function (data) {
                $("#FORREVIEW").html(data.FORREVIEW)
                $("#FORENDORSEMENT").html(data.FORENDORSEMENT )
                $("#FORAPPROVAL").html(data.FORAPPROVAL)
                $("#APPROVED").html(data.APPROVED )
                $("#REJECTED").html(data.REJECTED )
                $("#SUPPLIER").html(data.CountSupplier )
                $("#CLIENT").html(data.CountClient )
                $("#ONGOING").html(data.CountOngoing )
                $("#CLOSED").html(data.CountClosed )
                $("#SALES").html(data.CountSales)
                $("#SALESTODAY").html(data.CountSalesToday)
                $("#TOTALEARNINGS").html(numberWithCommas(data.CountTotalEarnings))
                $("#AVERAGESALES").html(numberWithCommas(data.CountAverageSales))
                $("#ORDERS").html(data.CountOrders)
                $("#REVIEWED").html(data.CountReviewed)
                $("#ENDORSED").html(data.CountEndorsed)

                if (UserRoleName == "ENCODER") {
                    $("span.PRcount").html(data.CountPR_TotalRequested);
                    $("span.POcount").html(data.CountPO_TotalRequested);
                    $("span.COCcount").html(data.CountCOC_TotalRequested);
                }
                else {

                    $("span.PRcount").html(parseInt(data.CountPRForReview) + parseInt(data.CountPRForEndorsement) + parseInt(data.CountPRForApproval));
                    $("span.POcount").html(parseInt(data.CountPOForReview) + parseInt(data.CountPOForEndorsement) + parseInt(data.CountPOForApproval));
                    $("span.COCcount").html(data.CountCOC);


                    $("#txCountCOCApproval").html(data.CountCOC);
                    $("#txCountPRApproval").html(data.CountPRForApproval);
                    $("#txCountPOApproval").html(data.CountPOForApproval);

                    $("#txCountPREndorsement").html(data.CountPRForEndorsement);
                    $("#txCountPOEndorsement").html(data.CountPOForEndorsement);

                    $("#txCountPRReview").html(data.CountPRForReview);
                    $("#txCountPOReview").html(data.CountPOForReview);



                }

             

            }
        })

    }

    function numberWithCommas(x) {
        return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    }

    function SystemRequestedNotification() {

        $.ajax({
            type: 'GET',
            url: '/Home/PopulateSystemRequestedNotifaction',
            success: function (response) {
                    
                $('#SupplierNotificationContent').empty();

                $.each(response, function (index, value) {
                    if (value.NotificationRequestedType == 'POREVIEWED') {
                        $('#SupplierNotificationContent').append($('<li><i class="fa fa-circle text-primary"></i><div class="font-w600 text-black">' + value.NotificationNumber + ' - <a href="/Report/ApprovedReportSupplier/' + value.NotificationReferenceNo + '"  target="_blank"> ' + value.NotificationProjectTitle + '</a> </div > <div><small class="text-muted">' + value.NotificationDate + '</small></div></li>'));
                    }
                    else if (value.NotificationRequestedType == 'POENDORSED') { 
                        $('#SupplierNotificationContent').append($('<li><i class="fa fa-circle text-warning"></i><div class="font-w600 text-black">' + value.NotificationNumber + ' - <a href="/Report/ApprovedReportSupplier/' + value.NotificationReferenceNo + '"  target="_blank"> ' + value.NotificationProjectTitle + '</a> </div > <div><small class="text-muted">' + value.NotificationDate + '</small></div></li>'));
                    }
                    else if (value.NotificationRequestedType == 'POAPPROVED') {
                        $('#SupplierNotificationContent').append($('<li><i class="fa fa-circle text-success"></i><div class="font-w600 text-black">' + value.NotificationNumber + ' - <a href="/Report/ApprovedReportSupplier/' + value.NotificationReferenceNo + '"  target="_blank"> ' + value.NotificationProjectTitle + '</a> </div > <div><small class="text-muted">' + value.NotificationDate + '</small></div></li>'));
                    }
                    else if (value.NotificationRequestedType == 'POREJECTED') {
                        $('#SupplierNotificationContent').append($('<li><i class="fa fa-circle text-danger"></i><div class="font-w600 text-black">' + value.NotificationNumber + ' - <a href="/Report/ApprovedReportSupplier/' + value.NotificationReferenceNo + '"  target="_blank"> ' + value.NotificationProjectTitle + '</a> </div > <div><small class="text-muted">' + value.NotificationDate + '</small></div></li>'));
                    }

                });

                $('#COCNotificationContent').empty();
                $.each(response, function (index, value) {
                    if (value.NotificationRequestedType == 'COCAPPROVED') {
                        $('#COCNotificationContent').append($('<li><i class="fa fa-circle text-success"></i><div class="font-w600 text-black">' + value.NotificationNumber + ' - <a href="/Report/COC_Report/' + value.NotificationNumber + '"  target="_blank"> ' + value.NotificationProjectTitle + '</a> </div > <div><small class="text-muted">' + value.NotificationDate + '</small></div></li>'));
                    }
                    else if (value.NotificationRequestedType == 'COCREJECTED') {
                        $('#COCNotificationContent').append($('<li><i class="fa fa-circle text-danger"></i><div class="font-w600 text-black">' + value.NotificationNumber + ' - <a href="/Report/COC_Report/' + value.NotificationNumber + '"  target="_blank"> ' + value.NotificationProjectTitle + '</a> </div > <div><small class="text-muted">' + value.NotificationDate + '</small></div></li>'));
                    }
                });

                $('#PRNotificationContent').empty();
                $.each(response, function (index, value) {
                    if (value.NotificationRequestedType == 'PRREVIEWED') {
                       $('#PRNotificationContent').append($('<li><i class="fa fa-circle text-primary"></i><div class="font-w600 text-black">' + value.NotificationNumber + ' - <a href="/Report/PurchaseRequisitionReport/' + value.NotificationReferenceNo + '"  target="_blank"> ' + value.NotificationProjectTitle + '</a> </div > <div><small class="text-muted">' + value.NotificationDate + '</small></div></li>'));
                    }
                    else if (value.NotificationRequestedType == 'PRENDORSED') {
                       $('#PRNotificationContent').append($('<li><i class="fa fa-circle text-warning"></i><div class="font-w600 text-black">' + value.NotificationNumber + ' - <a href="/Report/PurchaseRequisitionReport/' + value.NotificationReferenceNo + '"  target="_blank"> ' + value.NotificationProjectTitle + '</a> </div > <div><small class="text-muted">' + value.NotificationDate + '</small></div></li>'));
                    }
                    else if (value.NotificationRequestedType == 'PRAPPROVED') {
                       $('#PRNotificationContent').append($('<li><i class="fa fa-circle text-success"></i><div class="font-w600 text-black">' + value.NotificationNumber + ' - <a href="/Report/PurchaseRequisitionReport/' + value.NotificationReferenceNo + '"  target="_blank"> ' + value.NotificationProjectTitle + '</a> </div > <div><small class="text-muted">' + value.NotificationDate + '</small></div></li>'));
                    }
                    else if (value.NotificationRequestedType == 'PRREJECTED') {
                       $('#PRNotificationContent').append($('<li><i class="fa fa-circle text-danger"></i><div class="font-w600 text-black">' + value.NotificationNumber + ' - <a href="/Report/PurchaseRequisitionReport/' + value.NotificationReferenceNo + '"  target="_blank"> ' + value.NotificationProjectTitle + '</a> </div > <div><small class="text-muted">' + value.NotificationDate + '</small></div></li>'));
                    }


                });
            },
            error: function (error) {
                console.log(error);
            }
        })
    }

    function SystemNotification() {

        $.ajax({
            type: 'GET',
            url: '/Home/PopulateSystemNotifaction',
            success: function (response) {

                $('#SupplierNotificationContent').empty();

                $.each(response, function (index, value) {
                    if (value.NotificationType == 'SUPPLIER_REVIEWER') {
                        $('#SupplierNotificationContent').append($('<li><i class="fa fa-circle text-default"></i><div class="font-w600 text-black">' + value.NotificationNumber + ' - <a href="/Home/GetForReviewDetails/' + value.NotificationReferenceNo + '"> ' + value.NotificationProjectTitle + '</a> </div > <div><small class="text-muted">' + value.NotificationDate + '</small></div></li>'));
                    }
                    else if (value.NotificationType == 'SUPPLIER_ENDORSER') {
                        $('#SupplierNotificationContent').append($('<li><i class="fa fa-circle text-warning"></i><div class="font-w600 text-black">' + value.NotificationNumber + ' - <a href="/Home/GetForEndorsementDetails/' + value.NotificationReferenceNo + '"> ' + value.NotificationProjectTitle + '</a> </div > <div><small class="text-muted">' + value.NotificationDate + '</small></div></li>'));
                    }
                    else if (value.NotificationType == 'SUPPLIER_APPROVER') {
                        $('#SupplierNotificationContent').append($('<li><i class="fa fa-circle text-amethyst-light"></i><div class="font-w600 text-black">' + value.NotificationNumber + ' - <a href="/Home/GetForApprovalDetails/' + value.NotificationReferenceNo + '"> ' + value.NotificationProjectTitle + '</a> </div > <div><small class="text-muted">' + value.NotificationDate + '</small></div></li>'));
                    }
                });

                $('#COCNotificationContent').empty();
                $.each(response, function (index, value) {
                    if (value.NotificationType == 'COC_APPROVER') {
                        $('#COCNotificationContent').append($('<li><i class="fa fa-circle text-amethyst-light"></i><div class="font-w600 text-black">' + value.NotificationNumber + ' - <a href="/Home/COCFORAPPROVAL/' + value.NotificationNumber + '"> ' + value.NotificationProjectTitle + '</a> </div > <div><small class="text-muted">' + value.NotificationDate + '</small></div></li>'));
                    }
                });

                $('#PRNotificationContent').empty();
                $.each(response, function (index, value) {
                    if (value.NotificationType == 'PR_REVIEWER') {
                        $('#PRNotificationContent').append($('<li><i class="fa fa-circle text-default"></i><div class="font-w600 text-black">' + value.NotificationNumber + ' - <a href="/Home/GetPR_ForReviewDetails/' + value.NotificationReferenceNo + '"> ' + value.NotificationProjectTitle + '</a> </div > <div><small class="text-muted">' + value.NotificationDate + '</small></div></li>'));
                    }
                    else if (value.NotificationType == 'PR_ENDORSER') {
                        $('#PRNotificationContent').append($('<li><i class="fa fa-circle text-warning"></i><div class="font-w600 text-black">' + value.NotificationNumber + ' - <a href="/Home/GetPR_ForEndorsementDetails/' + value.NotificationReferenceNo + '"> ' + value.NotificationProjectTitle + '</a> </div > <div><small class="text-muted">' + value.NotificationDate + '</small></div></li>'));
                    }
                    else if (value.NotificationType == 'PR_APPROVER') {
                        $('#PRNotificationContent').append($('<li><i class="fa fa-circle text-amethyst-light"></i><div class="font-w600 text-black">' + value.NotificationNumber + ' - <a href="/Home/GetPR_ForApprovalDetails/' + value.NotificationReferenceNo + '"> ' + value.NotificationProjectTitle + '</a> </div > <div><small class="text-muted">' + value.NotificationDate + '</small></div></li>'));
                    }


                });
            },
            error: function (error) {
                console.log(error);
            }
        })
    }

    $("#SupplierApproverClick").on("click", function () {

        $('#apps-modal').modal({
            show: 'false',
            backdrop: 'static',
        });

    })

    $("#SupplierEndorserClick").on("click", function () {

        $('#apps-Endorser').modal({
            show: 'false',
            backdrop: 'static',
        });

    })

    $("#SupplierReviewerClick").on("click", function () {

        $('#apps-Reviewer').modal({
            show: 'false',
            backdrop: 'static',
        });

    })

    $("#InquiryClick").on("click", function () {

        $('#inquiry-modal').modal({
            show: 'false',
            backdrop: 'static',
        });

    })

    $("#RefreshPOPendingRequest").click(function (e) {

        e.preventDefault();

        if (UserRoleName == "ENCODER") {
            SystemRequestedNotification();
        }
        else {
            SystemNotification();
        }
    })
    
    $("#BellNotification").click(function (e) {

        e.preventDefault();

        if (UserRoleName == "ENCODER") {
            SystemRequestedNotification();
        }
        else {
            SystemNotification();
        }

    });

    $('.LogOut').click(function (e) {
        e.preventDefault();
        var link = $(this).attr('href');

        swal({
            title: "Are you sure you want to logout?",
            text: "You will be redirected to the Login Page.",
            type: "warning",
            showCancelButton: true,
            confirmButtonColor: '#5F69E0',
            confirmButtonText: 'Logout',
            cancelButtonText: "No",
            closeOnConfirm: false, width: 500,
            closeOnCancel: false,
            allowOutsideClick: false
        }).then((isConfirm) => {
            if (isConfirm) {
                var backlen = history.length;
                history.go(-backlen);
                window.location.href = link;
            }
        })
    });

$.expr[":"].containsNoCase = function (el, i, m) {
    var search = m[3];
    if (!search) return false;
    return eval("/" + search + "/i").test($(el).text());
};

$(document).ready(function () {
    $('#txSearch').keyup(function () {
        if ($('#txSearch').val().length > 1) {
            $('#myTables tr').hide();
            $('#myTables tr:first').show();
            $('#myTables tr td:containsNoCase(\'' + $('#txSearch').val() + '\')').parent().show();

        }
        else if ($('#txSearch').val().length == 0) {
            resetSearchValue();

        }

        if ($('#myTables tr:visible').length == 1) {
            $('.norecords').remove();
            $('#myTables').append('<tr class="norecords"><td colspan="6" class="Normal" style="text-align: center">No records were found</td></tr>');
        }
    });

    $('#txSearch').keyup(function (event) {
        if (event.keyCode == 27) {
            resetSearchValue();
        }
    });
});

function resetSearchValue() {
    $('#txSearch').val('');
    $('#myTables tr').show();
    $('.norecords').remove();
    $('#txSearch').focus();
}

document.onkeydown = function () {
    switch (event.keyCode) {
        case 116: //F5 button
            event.returnValue = false;
            event.keyCode = 0;
            return false;
        case 82: //R button
            if (event.ctrlKey) {
                event.returnValue = false;
                event.keyCode = 0;
                return false;
            }
    }
}

});



function validate(evt) {
    var theEvent = evt || window.event;

    // Handle paste
    if (theEvent.type === 'paste') {
        key = event.clipboardData.getData('text/plain');
    } else {
        // Handle key press
        var key = theEvent.keyCode || theEvent.which;
        key = String.fromCharCode(key);
    }
    var regex = /[0-9]|\./;
    if (!regex.test(key)) {
        theEvent.returnValue = false;
        if (theEvent.preventDefault) theEvent.preventDefault();
    }
}

function validatePhone(evt) {
    var theEvent = evt || window.event;

    // Handle paste
    if (theEvent.type === 'paste') {
        key = event.clipboardData.getData('text/plain');
    } else {
        // Handle key press
        var key = theEvent.keyCode || theEvent.which;
        key = String.fromCharCode(key);
    }
    var regex = /[0-9\s\+\()\/]|\./;
    if (!regex.test(key)) {
        theEvent.returnValue = false;
        if (theEvent.preventDefault) theEvent.preventDefault();
    }
}

