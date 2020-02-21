$("#ReceivablesValidation").submit(function (event) {

    var ACC_ID = $("#ACC_ID").val();
    var ACC_BillNo = $("#ACC_BillNo").val();
    var ACC_Particular = $("#ACC_Particular").val();
    var ACC_BillDate = $("#ACC_BillDate").val();
    var REC_Amount = $("#ACC_BillAmount").val();
    var ACC_Responsible = $("#ACC_Responsible").val();
    var ACC_Remarks = $("#ACC_Remarks").val();
    var ACC_ReferenceNo = $("#ACC_ReferenceNo").val();


    var ACC_BillType = $("#ACC_BillType").val();
    var ORCRNO = $("input[id*='ORCRNO']").val();
    var ACC_DatePaid = $("input[id*='ACC_DatePaid']").val();



    if (ACC_BillNo == "" || ACC_Particular == "" ||
        ACC_BillDate == "" || REC_Amount == "" || ACC_Remarks == "") {
        return false;
    }


    var action = $("#ReceivablesValidation").attr("action");
    event.preventDefault();
    event.stopImmediatePropagation();

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
            $.ajax({
                url: action,
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify({
                    ACC_ID: ACC_ID,
                    ACC_BillNo: ACC_BillNo,
                    ACC_Particular: ACC_Particular,
                    ACC_BillDate: ACC_BillDate,
                    REC_Amount: REC_Amount,
                    ACC_Responsible: ACC_Responsible,
                    ACC_Remarks: ACC_Remarks,
                    ACC_ReferenceNo: ACC_ReferenceNo,
                    ACC_BillType: ACC_BillType,
                    ORCRNO: ORCRNO,
                    ACC_DatePaid: ACC_DatePaid
                }),
                dataType: "json",
                success: function (data) {

                    if (data == "0") {
                        sweetAlert("System Error, Please contact our administrator for assistance.", "", "error");
                        return false;
                    }
                    else {

                        swal
                            ({
                                 title: "Success",
                                text: "The Payment has been successfully updated.",
                                type: "success", allowOutsideClick: false
                            }).then(function () {
                                // Redirect the user
                                window.location.href = '/Home/EDITRECEIVABLES/';
                            });
                    }

                }
            });

        }
    })

});