

$(document).ready(function () {
    $("#btnConfirm").prop('disabled', false);
    $("#btnReject").prop('disabled', true);

    $("#RejectRemarks").on('keyup', function () {

        var RejectRemarks = $.trim($('#RejectRemarks').val());

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

    var POSReferenceNo = $("#POSReferenceNo").val();
    var POSupplierNumber = $("#POSupplierNumber").val();

    var OldStatus = $("#FormStatus").val();
    var EndorserID = $("#EndorserID").val();
    var ApproverID = $("#ApproverID").val();
    var StatusNotification = "";
    var Action = "";

    if (OldStatus == "FOR ENDORSEMENT") {
        StatusNotification = "Endorsed";
    }
    else if (OldStatus == "FOR REVIEW") {
        StatusNotification = "Reviewed";
    }
    else {
        StatusNotification = "Approved";
    }


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

            var action = '/Main/UpdateSupplierForRoutingApproval';
            $.ajax({
                url: action,
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify({ POSReferenceNO: POSReferenceNo, FormStatus: OldStatus, EndorserID: EndorserID, ApproverID: ApproverID }),
                dataType: "json",
                success: function (data) {

                    if (data == 0) {
                        sweetAlert("Please contact our administrator for assistance.", "", "error");
                        return false;
                    }
                    else if (data == "REJECTED") {
                        sweetAlert("This " + POSupplierNumber + " is already Rejected.", "", "error");
                        return false;
                    }
                    else if (data == "FOR APPROVAL") {
                        sweetAlert("This " + POSupplierNumber + " is already for approval.", "", "error");
                        return false;
                    }
                    else if (data == "APPROVED") {
                        sweetAlert("This " + POSupplierNumber + " is already approved.", "", "error");
                        return false;
                    }
                    else {
                        swal
                            ({
                                 title: "Success",
                                text: "" + POSupplierNumber + " successfully " + StatusNotification + ".",
                                type: "success"
                            }).then(function () {
                                // Redirect the user

                                App.loader('show');
                                setTimeout(function () {
                                    App.loader('hide');
                                    if (OldStatus == "FOR ENDORSEMENT") {

                                        var $this = $("#btnConfirm");
                                        $this.button('loading');
                                        setTimeout(function () {
                                            $this.button('reset');
                                        }, 10000);

                                        window.location.href = "/Report/ReportSupplier/";
                                    }

                                    else if (OldStatus == "FOR REVIEW") {

                                        var $this = $("#btnConfirm");
                                        $this.button('loading');
                                        setTimeout(function () {
                                            $this.button('reset');
                                        }, 10000);

                                        window.location.href = "/Report/ReportSupplier/";
                                       

                                    }
                                    else {
                                        window.location.href = '/Home/FORAPPROVAL/';
                                    }
                                }, 1000);

                            });
                    }
                }
            });
        }
    })
}

function Rejected(button) {

    var RejectRemarks = $.trim($('#RejectRemarks').val());

    if (RejectRemarks == null || RejectRemarks == "") {

        return false;
    }

    else {

        var POSReferenceNo = $("#POSReferenceNo").val();
        var POSupplierNumber = $("#POSupplierNumber").val();


        var OldStatus = $("#FormStatus").val();
        //var EndorserID = $("#EndorserID").val();
        //var ApproverID = $("#ApproverID").val();
        //var StatusNotification = "";
        //var Action = "";
        //if (OldStatus == "FOR ENDORSEMENT") {
        //    StatusNotification = "Endorsed";
        //}
        //else {
        //    StatusNotification = "Approved";
        //}

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

                var action = '/Main/UpdateSupplierRejected';
                $.ajax({
                    url: action,
                    type: "POST",
                    contentType: "application/json",
                    data: JSON.stringify({ POSReferenceNO: POSReferenceNo, FormStatus: OldStatus, RejectRemarks: RejectRemarks }),
                    dataType: "json",
                    success: function (data) {

                        if (data == 0) {
                            sweetAlert("Please contact our administrator for assistance.", "", "error");
                            return false;
                        }
                        else if (data == "REJECTED") {
                            sweetAlert("This " + POSupplierNumber + " is already rejected.", "", "error");
                            return false;
                        }
                        else if (data == "FOR APPOVAL") {
                            sweetAlert("This " + POSupplierNumber + " is already for approval.", "", "error");
                            return false;
                        }
                        else if (data == "APPROVED") {
                            sweetAlert("This " + POSupplierNumber + " is already approved.", "", "error");
                            return false;

                        }
                        else {
                            swal
                                ({
                                     title: "Success",
                                    text: "" + POSupplierNumber + " successfully Rejected",
                                    type: "success"
                                }).then(function () {
                                    // Redirect the user
                                    App.loader('show');
                                    setTimeout(function () {
                                        App.loader('hide');
                                        if (OldStatus == "FOR ENDORSEMENT") {
                                            window.location.href = '/Home/FORENDORSEMENT/';
                                        }
                                        else {
                                            window.location.href = '/Home/FORAPPROVAL/';
                                        }
                                    }, 1000);
                                });
                        }
                    }
                });
            }
        })
    }


}