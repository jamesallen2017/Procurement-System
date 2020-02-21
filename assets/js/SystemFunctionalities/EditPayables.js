$("#PayablesValidation").submit(function (event) {

    var ACC_ID = $("#ACC_ID").val();
    var ACC_ATICVNO = $("#ACC_ATICVNO").val();
    var ACC_InvoiceNo = $("#ACC_InvoiceNo").val();
    var ACC_CheckDate = $("#ACC_CheckDate").val();
    var ACC_Particular = $("#ACC_Particular").val();
    var REL_Tax = $("#ACC_LesswithTax").val();
    var REL_Amount = $("#ACC_AmountPaid").val();
    var ACC_Responsible = $("#ACC_Responsible").val();
    var ACC_Remarks = $("#ACC_Remarks").val();
    var ACC_ReferenceNo = $("#ACC_ReferenceNo").val();
    var ACC_DateCollected = $("#ACC_DateCollected").val();
    var ACC_DRNO = $("#ACC_DRNO").val();
    var ACC_MRRNO = $("#ACC_MRRNO").val();
    var ACC_COCNO = $("#ACC_COCNO").val();

    if (ACC_ATICVNO == "" || ACC_InvoiceNo == "" ||
        ACC_CheckDate == "" || ACC_Particular == "" ||
        ACC_LesswithTax == "" || ACC_AmountPaid == "" || ACC_Remarks == "" || ACC_DateCollected == "" || ACC_DRNO == "" || ACC_COCNO == "") {
        return false;
    }
    var action = $("#PayablesValidation").attr("action");
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
                    ACC_ATICVNO: ACC_ATICVNO,
                    ACC_InvoiceNo: ACC_InvoiceNo,
                    ACC_CheckDate: ACC_CheckDate,
                    ACC_Particular: ACC_Particular,
                    REL_Tax: REL_Tax,
                    REL_Amount: REL_Amount,
                    ACC_Responsible: ACC_Responsible,
                    ACC_Remarks: ACC_Remarks,
                    ACC_ReferenceNo: ACC_ReferenceNo,
                    ACC_DateCollected: ACC_DateCollected,
                    ACC_DRNO : ACC_DRNO,
                    ACC_COCNO: ACC_COCNO,
                    ACC_MRRNO: ACC_MRRNO,
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
                                window.location.href = '/Home/EDITPAYABLES/';
                            });
                    }

                }
            });

        }
    })

})