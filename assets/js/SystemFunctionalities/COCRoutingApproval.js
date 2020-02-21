$(document).ready(function () {

    $("#btnConfirm").prop('disabled', false);
    $("#btnReject").prop('disabled', true);

    $("#COC_RejectRemarks").on('keyup', function () {

        var RejectRemarks = $.trim($('#COC_RejectRemarks').val());

        if (RejectRemarks == null || RejectRemarks == "") {
            $("#btnConfirm").prop('disabled', false);
            $("#btnReject").prop('disabled', true);
        }
        else {
            $("#btnConfirm").prop('disabled', true);
            $("#btnReject").prop('disabled', false);

        }
    })
})


function Routing(button) {
    var COC_ControlNo = $("#COC_ControlNo").val();
    var COC_Status = $("#COC_Status").val();
    var COC_Resposible = $("#COC_Resposible").val();


    swal({
        title: "Are you sure?",
        text: "",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: '#5F69E0',
        confirmButtonText: 'Yes',
        cancelButtonText: "No",
        closeOnConfirm: false, width: 420,
        closeOnCancel: false,
        allowOutsideClick: false
    }).then((isConfirm) => {
        if (isConfirm) {

            var action = '/Main/UpdateCOCForRoutingApproval';

            $.ajax({
                url: action,
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify({ COC_ControlNo: COC_ControlNo, COC_Status: COC_Status, COC_Resposible: COC_Resposible }),
                dataType: "json",
                success: function (data) {

                    if (data == 0) {
                        sweetAlert("Please contact our administrator for assistance.", "", "error");
                        return false;
                    }
                    else if (data == "REJECTED") {
                        sweetAlert("This " + COC_ControlNo + " is already Rejected.", "", "error");
                        return false;
                    }
                    else if (data == "APPROVED") {
                        sweetAlert("This " + COC_ControlNo + " is already approved.", "", "error");
                        return false;
                    }
                    else {
                        swal
                            ({
                                 title: "Success",
                                text: "The Certificate of Completion successfully Approved.",
                                type: "success"
                            }).then(function () {
                                // Redirect the user

                                App.loader('show');
                                setTimeout(function () {
                                    App.loader('hide');
                                    window.location.href = '/Home/COCFORAPPROVAL/';

                                }, 1000);

                            });
                    }
                }
            });
        }
    })

}

function Rejected(button) {
    var COC_ControlNo = $("#COC_ControlNo").val();
    var COC_Status = $("#COC_Status").val();
    var COC_Resposible = $("#COC_Resposible").val();
    var COC_RejectRemarks = $("#COC_RejectRemarks").val();


    swal({
        title: "Are you sure?",
        text: "",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: '#5F69E0',
        confirmButtonText: 'Yes',
        cancelButtonText: "No",
        closeOnConfirm: false, width: 420,
        closeOnCancel: false,
        allowOutsideClick: false
    }).then((isConfirm) => {
        if (isConfirm) {

            var action = '/Main/UpdateRejectCOCForRoutingApproval';

            $.ajax({
                url: action,
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify({ COC_ControlNo: COC_ControlNo, COC_Status: COC_Status, COC_Resposible: COC_Resposible, COC_RejectRemarks: COC_RejectRemarks }),
                dataType: "json",
                success: function (data) {

                    if (data == 0) {
                        sweetAlert("Please contact our administrator for assistance.", "", "error");
                        return false;
                    }
                    else if (data == "REJECTED") {
                        sweetAlert("This " + COC_ControlNo + " is already Rejected.", "", "error");
                        return false;
                    }
                    else if (data == "APPROVED") {
                        sweetAlert("This " + COC_ControlNo + " is already approved.", "", "error");
                        return false;
                    }
                    else {
                        swal
                            ({
                                 title: "Success",
                                text: "The Certificate of Completion successfully Rejected.",
                                type: "success"
                            }).then(function () {
                                // Redirect the user

                                App.loader('show');
                                setTimeout(function () {
                                    App.loader('hide');
                                    window.location.href = '/Home/COCFORAPPROVAL/';

                                }, 1000);

                            });
                    }
                }
            });
        }
    })

}