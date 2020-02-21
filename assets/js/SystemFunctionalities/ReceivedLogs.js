$(document).ready(function () {

    $("#ReceivedModal #DRItemReturn").on('keyup', function () {

        var DRReceived = $(this).val();

        if (DRReceived == "" || DRReceived == null) {

            $("#ReceivedModal #SaveEdit").prop('disabled', true);

        }
        else {
            $("#ReceivedModal #SaveEdit").prop('disabled', false);
        }
    })
})




$("#myTabless #btnEditReceivedLog").on('click', function () {
    $("#SaveEdit").prop('disabled', true);
    $("#DRReceived").val("");

    var count = $(this).closest("tr");
    var item_DRID = $("#item_DRID", count).val();
    var item_DRQty = $("#item_DRQty", count).val();
    var item_DRNumber = $("#item_DRNumber", count).val();
    var item_DRReceived = $("#item_DRReceived", count).val();
    var item_DRPONumber = $("#item_DRPONumber", count).val();
    var item_DRParticular = $("#item_DRParticular", count).val();
    var item_DRResponsible = $("#item_DRResponsible", count).val();
    var item_DRDateReceived = $("#item_DRDateReceived", count).val();
    var item_DRItemID = $("#item_DRItemID", count).val();
    var item_DRReferenceNo = $("#item_DRReferenceNo", count).val();
    var item_DRRemarks = $("#item_DRRemarks", count).val();
    var item_DRSupplier = $("#item_DRSupplier", count).val();
    var item_DRDateReceived = item_DRDateReceived.split(' ')[0];

    $("#DRID").val(item_DRID);
    $("#DRQty").val(item_DRQty);
    $("#DRNumber").val(item_DRNumber);
    $("#DRParticular").val(item_DRParticular);
    $("#DRResponsible").val(item_DRResponsible);
    $("#DRDateReceived").val(item_DRDateReceived);
    $("#DRRemarks").val(item_DRRemarks);
    $("#DRReferenceNo").val(item_DRReferenceNo);
    $("#DRItemID").val(item_DRItemID);
    $("#DRReceived").val(item_DRReceived);
    $("#DRSupplier").val(item_DRSupplier);
    $("#DRPONumber").val(item_DRPONumber);
    $("#DRItemReturn").val("");
    $("#DRReasonRemarks").val("");
    




    $("#EditReceivedForm").submit(function (event) {

        var re = /^[-+]?[0-9]+\.[0-9]+$/;

        var DRID = $("#DRID").val();
        var DRItemID = $("#DRItemID").val();
        var DRReferenceNo = $("#DRReferenceNo").val();
        var DRQty = $("#DRQty").val();
        var DRNumber = $("#DRNumber").val();
        var DRParticular = $("#DRParticular").val();
        var DRResponsible = $("#DRResponsible").val();
        var DRDateReceived = $("#DRDateReceived").val();
        var DRRemarks = $("#DRRemarks").val();
        var DRReceived = $("#DRReceived").val();
        var DRItemReturn = $("#DRItemReturn").val();
        var DRReasonRemarks = $("#DRReasonRemarks").val();
        var DRSupplier = $("#DRSupplier").val();
        var DRPONumber = $("#DRPONumber").val();


        if (DRItemReturn == "0") {
            sweetAlert("0 not allowed.", "", "error");
            return false;
        }

        if (DRItemReturn.match(re)) {
            sweetAlert("Invalid number format", "", "error");
            return false;
        }

        else if (parseFloat(DRItemReturn) > parseFloat(DRReceived)) {

            sweetAlert("Returned Item is less than the Received Item", "", "error");
            return false;
        }
        else if (DRReasonRemarks == null || DRReasonRemarks == "") { return false }


        var action = $("#EditReceivedForm").attr("action");
        event.preventDefault();
        event.stopImmediatePropagation();
        $.ajax({
            url: action,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({
                DRID: DRID,
                DRItemID: DRItemID,
                DRReferenceNo: DRReferenceNo,
                DRQty: DRQty,
                DRNumber: DRNumber,
                DRParticular: DRParticular,
                DRResponsible: DRResponsible,
                DRDateReceived: DRDateReceived,
                DRRemarks: DRRemarks,
                DRReceived: DRReceived,
                DRItemReturn: DRItemReturn,
                DRReasonRemarks: DRReasonRemarks,
                DRSupplier: DRSupplier,
                DRPONumber: DRPONumber,
            }),
            dataType: "json",
            success: function (data) {

                if (data == "0") {
                    sweetAlert("Returned Item is less than the Received Item", "", "error");
                    return false;
                }

                else {
                    swal
                        ({
                            title: "Success",
                            text: DRParticular + " successfully updated.",
                            type: "success", allowOutsideClick: false
                        }).then(function () {
                            window.location.href = '/Home/ReceivedLogs';
                        });
                }
            },
            error: function (err) {
                sweetAlert("Invalid Format", "", "error");

            }
        })
    })


    $('#ReceivedModal').modal({
        show: 'false',
        backdrop: 'static',
    });
})