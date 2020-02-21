
$("#PayablesValidation").submit(function (event) {

    var BillingType = $("#BillingType").val();
    var ACC_ATICVNO = $("#ACC_ATICVNO").val();
    var ACC_InvoiceNo = $("#ACC_InvoiceNo").val();
    var ACC_CheckDate = $("#ACC_CheckDate").val();
    var ACC_Particular = $("input[id*='ACC_Particular']").val();
    var ACC_LesswithTax = $("#ACC_LesswithTax").val();
    var ACC_AmountPaid = $("#ACC_AmountPaid").val();
    var ACC_AmountPaidHidden = $("#ACC_AmountPaidHidden").val();
    var ACC_LesswithTaxHidden = $("#ACC_LesswithTaxHidden").val();
    var ACC_Responsible = $("#ACC_Responsible").val();
    var ACC_Ponumber = $("input[id*='ACC_Ponumber']").val();
    var POSReferenceNo = $("input[id*='POSReferenceNo']").val();
    var ACC_DateCollected = $("input[id*='ACC_DateCollected']").val();
    var ACC_DRNO = $("input[id*='ACC_DRNO']").val();
    var ACC_MRRNO = $("input[id*='ACC_MRRNO']").val();
    var TypeOfTerms = $("#TypeOfTerms").val();
    var Discount = $("#Discount").val();
    




    if (ACC_ATICVNO == "" || ACC_InvoiceNo == "" ||
        ACC_CheckDate == "" || ACC_Particular == "" ||
        ACC_AmountPaid == "" || TypeOfTerms == "") {
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
        ACC_ATICVNO: ACC_ATICVNO,
        ACC_InvoiceNo: ACC_InvoiceNo,
        ACC_CheckDate: ACC_CheckDate,
        ACC_Particular: ACC_Particular,
        ACC_LesswithTax: ACC_LesswithTax,
        ACC_AmountPaidHidden: ACC_AmountPaidHidden,
        ACC_AmountPaid: ACC_AmountPaid,
        ACC_Responsible: ACC_Responsible,
        ACC_Ponumber: ACC_Ponumber,
        POSReferenceNo: POSReferenceNo,
        ACC_DateCollected: ACC_DateCollected,
        ACC_DRNO: ACC_DRNO,
        ACC_MRRNO: ACC_MRRNO,
        TypeOfTerms: TypeOfTerms,
        BillingType: BillingType,
        Discount: Discount,
        ACC_LesswithTaxHidden: ACC_LesswithTaxHidden,
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
                    text: "The Payment successfully updated.",
                    type: "success", allowOutsideClick: false
                }).then(function () {
                    window.location.href = '/Home/PAYABLES/';
                });
        }

    }
});

}
})

})
