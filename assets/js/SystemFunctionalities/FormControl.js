$('#TableSupplier').on('change', '#SupplierTrasanction_SupplierStatus', function () {
    var count = $(this).closest("tr");
    var PONumber = $("#SupplierTrasanction_PONumberSupplier", count).val();
    var SupplierStatus = $("#SupplierTrasanction_SupplierStatus", count).val();
    var POSReferenceNo = $("#SupplierTrasanction_POSReferenceNo", count).val();
    var SupplierName = $("#SupplierTrasanction_SupplierName", count).val();
    var ClientStatus = $("#SupplierTrasanction_ClientStatus", count).val();
    var FormStatus = $("#SupplierTrasanction_FormStatus", count).val();

    if (FormStatus == "REJECTED") {
        sweetAlert("This Supplier has been Rejected.", "", "error");
        return false;
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
    }).then(function () {
        $.ajax({
            url: "/Main/UpdateTransactionStatus",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({
                StringSupplierStatus: SupplierStatus,
                StringPOSReferenceNo: POSReferenceNo,
                PONumber: PONumber,
            }),
            dataType: "json",
            success: function (data) {
                if (data == 0) {
                    sweetAlert("contact our administrator for assistance", "Data error", "error");
                    return false;
                }
                else {
                    swal
                        ({
                            title: "Success",
                            text: SupplierName + " has been successfully updated.",
                            type: "success", allowOutsideClick: false
                        }).then(function () {
                            window.location.reload();
                        });

                }
            }
        })

    }, function (dismiss) {
        if (dismiss == 'cancel') {
            window.location.reload();

        }
    });

});


$('#TablePurchaseRequisition').on('change', '#item_PRStatus', function () {
    var count = $(this).closest("tr");
    var PRReferenceNo = $("#item_PRReferenceNo", count).val();
    var PRStatus = $("#item_PRStatus", count).val();
    var PRNumber = $("#item_PRNumber", count).val();
    var FormStatus = $("#item_FormStatus", count).val();

    if (FormStatus == "REJECTED") {
        sweetAlert("This Purchase Requisition has been Rejected.", "", "error");
        return false;
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
    }).then(function () {
        $.ajax({
            url: "/Main/UpdateTransactionStatus",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({
                PRStatus: PRStatus,
                PRReferenceNo: PRReferenceNo,
            }),
            dataType: "json",
            success: function (data) {
                if (data == 0) {
                    sweetAlert("contact our administrator for assistance", "Data error", "error");
                    return false;
                }
                else {
                    swal
                        ({
                            title: "Success",
                            text: PRNumber + " has been successfully updated.",
                            type: "success", allowOutsideClick: false
                        }).then(function () {
                            window.location.reload();
                        });

                }
            }
        })
    }, function (dismiss) {
        if (dismiss == 'cancel') {
                            window.location.reload();

        }
    });



});


$('#TableClient').on('change', '#item_ClientStatus', function () {
    var count = $(this).closest("tr");
    var ClientStatus = $("#item_ClientStatus", count).val();
    var POCReferenceNo = $("#item_POCReferenceNo", count).val();
    var ClientName = $("#item_ClientName", count).val();

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
    }).then(function () {
        $.ajax({
            url: "/Main/UpdateTransactionStatus",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({
                StringClientStatus: ClientStatus,
                StringPOCReferenceNo: POCReferenceNo
            }),
            dataType: "json",
            success: function (data) {

                if (data == 0) {
                    sweetAlert("contact our administrator for assistance", "Data error", "error");
                    return false;
                }
                else {
                    swal
                        ({
                            title: "Success",
                            text: ClientName + " successfully updated.",
                            type: "success", allowOutsideClick: false
                        }).then(function () {
                            window.location.reload();
                        });
                }
            }
        })

    }, function (dismiss) {
        if (dismiss == 'cancel') {
            window.location.reload();

        }
    });

})