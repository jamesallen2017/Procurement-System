
$("#ReceivablesValidation").submit(function (event) {

    var ACC_BillNo = $("#ACC_BillNo").val();
    var ACC_Particular = $("#ACC_Particular").val();
    var ACC_BillDate = $("#ACC_BillDate").val();
    var ACC_BillAmount = $("input[id*='ACC_BillAmount']").val();
    var ACC_Responsible = $("input[id*='ACC_Responsible']").val();
    var POC_ReferenceNo = $("input[id*='POC_ReferenceNo']").val();
    var PONumber = $("input[id*='PONumber']").val();
    var SignProposal = $("input[id*='SignProposal']").val();
    var TypeOfNumber = $("#TypeOfNumber").val();
    var ORCRNO = $("input[id*='ORCRNO']").val();
    var ACC_DatePaid = $("input[id*='ACC_DatePaid']").val();
    var ACC_TypeOfTerms = $("#ACC_TypeOfTerms").val();
    var Discount = $("#Discount").val();

    



    if (ACC_BillNo == "" || ACC_Particular == "" ||
        ACC_BillDate == "" || ACC_BillAmount == "" || ACC_TypeOfTerms == "") {
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
                    ACC_BillNo: ACC_BillNo,
                    ACC_Particular: ACC_Particular,
                    ACC_BillDate: ACC_BillDate,
                    ACC_BillAmount: ACC_BillAmount,
                    ACC_Responsible: ACC_Responsible,
                    POC_ReferenceNo: POC_ReferenceNo,
                    SignProposal: SignProposal,
                    PONumber: PONumber,
                    ACC_BillType: TypeOfNumber,
                    ORCRNO: ORCRNO,
                    ACC_DatePaid: ACC_DatePaid,
                    ACC_TypeOfTerms: ACC_TypeOfTerms,
                    Discount: Discount,
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
                                window.location.href = '/Home/RECEIVABLES/';
                            });
                    }
                }
            });
        }
    })
})
