$(document).ready(function () {

    $("#ReleasedModal #DRItemReturn").on('keyup', function () {

        var DRReceived = $(this).val();

        if (DRReceived == "" || DRReceived == null) {

            $("#ReleasedModal #SaveEdit").prop('disabled', true);

        }
        else {
            $("#ReleasedModal #SaveEdit").prop('disabled', false);
        }
    })
})

$(".myTabless #btnEditReleasedLog").on('click', function () {

    $("#SaveEdit").prop('disabled', true);
    $("#DRReleased").val("");

    var count = $(this).closest("tr");

    var item_DRReferenceNo = $("#item_DRReferenceNo", count).val();
    var item_DRNumber = $("#item_DRNumber", count).val();
    var item_DRQty = $("#item_DRQty", count).val();
    var item_DRNumber = $("#item_DRNumber", count).val();
    var item_DRReleased = $("#item_DRReleased", count).val();
    var item_DRPONumber = $("#item_DRPONumber", count).val();
    var item_DRParticular = $("#item_DRParticular", count).val();
    var item_DRResponsible = $("#item_DRResponsible", count).val();
    var item_DRDateReleased = $("#item_DRDateReleased", count).val();
    var item_DRItemID = $("#item_DRItemID", count).val();
    var item_DRRemarks = $("#item_DRRemarks", count).val();
    var item_DRClient = $("#item_DRClient", count).val();

    item_DRDateReleased = item_DRDateReleased.split(' ')[0];

    $("#DRNumber").val(item_DRNumber);
    $("#DRQty").val(item_DRQty);
    $("#DRPONumber").val(item_DRPONumber);
    $("#DRParticular").val(item_DRParticular);
    $("#DRResponsible").val(item_DRResponsible);
    $("#DRDateReleased").val(item_DRDateReleased);
    $("#DRRemarks").val(item_DRRemarks);
    $("#DRItemID").val(item_DRItemID);
    $("#DRReleased").val(item_DRReleased);
    $("#DRReferenceNo").val(item_DRReferenceNo);
    $("#DRClient").val(item_DRClient);
    $("#DRItemReturn").val("");
    $("#DRReasonRemarks").val("");



    $("#EditReleasedForm").submit(function (event) {

        var re = /^[-+]?[0-9]+\.[0-9]+$/;
        var DRNumber = $("#DRNumber").val();
        var DRPONumber = $("#DRPONumber").val();
        var DRItemID = $("#DRItemID").val();
        var DRItemReturn = $("#DRItemReturn").val();
        var DRReferenceNo = $("#DRReferenceNo").val();
        var DRReleased = $("#item_DRReleased").val();
        var DRParticular = $("#DRParticular").val();
        var DRReasonRemarks = $("#DRReasonRemarks").val();
        var DRDateReleased = $("#DRDateReleased").val();
        var DRClient = $("#DRClient").val();

        if (DRItemReturn == "0") {
            sweetAlert("0 not allowed.", "", "error");
            return false;
        }
        if (DRItemReturn.match(re)) {
            sweetAlert("Invalid number format", "", "error");
            return false;
        }

        if (parseFloat(DRItemReturn) > parseFloat(DRReleased)) {

            sweetAlert("Returned Item is less than the Delivered Item.", "", "error");
            return false;

        }
        else if (DRReasonRemarks == null || DRReasonRemarks == "") {
            return false;

        }

        var action = $("#EditReleasedForm").attr("action");
        event.preventDefault();
        event.stopImmediatePropagation();
        $.ajax({
            url: action,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({
                DRNumber: DRNumber,
                DRReferenceNo: DRReferenceNo,
                DRItemID: DRItemID,
                DRItemReturn: DRItemReturn,
                DRReasonRemarks: DRReasonRemarks,
                DRParticular: DRParticular,
                DRClient: DRClient,
                DRDateReleased: DRDateReleased,
                DRPONumber: DRPONumber,
            }),
            dataType: "json",
            success: function (data) {

                if (data == "0") {
                    sweetAlert("Returned Item is less than the Delivered Item.", "", "error");
                    return false;
                }

                else {
                    swal
                        ({
                            title: "Success",
                            text: DRParticular + " successfully updated.",
                            type: "success", allowOutsideClick: false
                        }).then(function () {

                            window.location.href = '/Home/ReleasedLogs';
                        });
                }
            },
            error: function (err) {
                sweetAlert("Invalid Format.", "", "error");

            }
        })
    })



    $('#ReleasedModal').modal({
        show: 'false',
        backdrop: 'static',
    });

})


$(".myTablesss #PrintDRReport").on('click', function () {
    var count = $(this).closest("tr");

    var item_DRNumber = $("#item_DRNumber", count).val();



    window.open("/Report/DR_Report/" + item_DRNumber, '_blank');
    window.open("/Report/CustomerDR_Report/" + item_DRNumber, '_blank');
})