
$("#TableReceiving #btnDisplayItemReceive").on('click', function () {


    $("#DeliveryNo").val("");
    $("#DeliveryRemarks").val("");
    $("#DeliveryDate").val("");



    var count = $(this).closest("tr");
    var POCReferenceNo = $("#DR_POCReferenceNo", count).val();
    var POSReferenceNo = $("#DR_POSReferenceNo", count).val();
    var POSupplierNumber = $("#DR_POSupplierNumber", count).val();
    var Supplier = $("#DR_Supplier", count).val();
    var Client = $("#DR_Client", count).val();



    $("#FullNameResponsible").val(mySessionVariable);
    $('#POCReferenceNo').val(POCReferenceNo);
    $('#POSReferenceNo').val(POSReferenceNo);
    $('#POSupplierNumber').val(POSupplierNumber);
    $('#Supplier').val(Supplier);
    $('#Client').val(Client);


    $.ajax({
        url: '/Home/PopulateItemReceiving/',
        method: 'POST',
        data: JSON.stringify({

            StringPOSReferenceNo: POSReferenceNo
        }),
        contentType: "application/json",
        dataType: "json",
        success: function (data) {
            var employeeTable = $('.ItemDisplay .tbdisplay');
            if (data == 0) {
                employeeTable.empty();
                $("#UpdateItem").prop('disabled', true);
                employeeTable.append('<tr class="norecords"><td colspan="6" class="Normal" style="text-align: center">No records were found</td></tr>');
            }
            else if (data == "CLOSED") {
                employeeTable.empty();
                $("#UpdateItem").prop('disabled', true);
                employeeTable.append('<tr class="norecords"><td colspan="6" class="Normal" style="text-align: center">All item has been Received</td></tr>');
            }
            else {
                employeeTable.empty();
                $("#UpdateItem").prop('disabled', false);
                $(data).each(function (index, data) {
                    employeeTable.append('<tr><td>' + data.ParticularName + '</td><td class="text-center">' + data.Qty + '</td><td  class="text-center">' + data.Collected
                        + '</td><td><input type="text" class="form-control" onkeypress = "validate(event)" id="ItemCount"></td><td style="display:none;"><input type="text" class="form-control" value="' + data.ItemID
                        + '" id="HiddenItemID"></td><td style="display:none;"><input type="text" class="form-control" value="' + data.ParticularName
                        + '" id="HiddenParticularName"></td><td style="display:none;"><input type="text" class="form-control" value="' + data.Qty
                        + '" id="HiddenQty"></td><td style="display:none;"><input type="text" class="form-control" value="' + data.Collected
                        + '" id="HiddenCollected"></td></tr>');
                });

            }

        },
        error: function (err) {
            alert(err);
        }
    });

    $('#modal-slideright').modal({
        show: 'false',
        backdrop: 'static',
    });
})




$("#SupplierItemListForm").submit(function (event) {

    var ItemDetails = new Array();
    var Item;
    var validate;
    var zero = false;
    var zeronotallowed = 0;
    var valid = 0;
    var textEmpty = 0;

    $(".ItemDisplay .tbdisplay tr").each(function () {
        Item = {};
        var row = $(this);
    
        var Qty = $("#HiddenQty", row).val();
        var itemcounts = $("#ItemCount", row).val().trim();
        var ItemText =  $("#ItemCount", row).val().trim();
        var Delivered = $("#HiddenCollected", row).val();
        var re = /^[-+]?[0-9]+\.[0-9]+$/;

        if (itemcounts == "")
            itemcounts = 0;

        var TotalDelivered = parseFloat(itemcounts) + parseFloat(Delivered)

        if (itemcounts < 0 || ItemText.match(re)) {

            validate = "InvalidNumber";
            return false;
        }
        else if (TotalDelivered > Qty) {

            validate = "Exceeds";
            return false;
        }
        else if (itemcounts > 0) {
            valid++;
        }
        if (ItemText != "" && valid != 0) {
            textEmpty++;
        }

        if (ItemText == "0") {
            zeronotallowed++;
            zero = true;
        }


        Item.ItemID = $("#HiddenItemID", row).val();
        Item.ParticularName = $("#HiddenParticularName", row).val();
        Item.Qty = $("#HiddenQty", row).val();
        Item.Collected = $("#HiddenCollected", row).val();
        Item.ItemCount = $("#ItemCount", row).val();
        ItemDetails.push(Item);
    })

    var FullNameResponsible = $("#FullNameResponsible").val();
    var POSReferenceNo = $('#POSReferenceNo').val();
    var POSupplierNumber = $('#POSupplierNumber').val();
    var Supplier = $('#Supplier').val();
    var DeliveryNo = $("#DeliveryNo").val();
    var DeliveryRemarks = $("#DeliveryRemarks").val();
    var DeliveryDate = $("#DeliveryDate").val();

    if (DeliveryNo == "" || DeliveryDate == "") {
        return false;
    }
    else if (validate == "InvalidNumber") {
        sweetAlert("Invalid Number Format", "", "error");
        return false;

    }
    else if (validate == "Exceeds") {

        sweetAlert("Received Item/s Exceeds the Quantity Limit.", "Please try again.", "error");
        return false;
    }
    else if (zeronotallowed != 0 && zero == true) {
        sweetAlert("0 not allowed.", "Please try again.", "error");
        return false;
    }

    if (textEmpty == 0) {
        sweetAlert("Fill out Receiving item at least one.", "", "error");
        return false;
    }
   

    var action = $("#SupplierItemListForm").attr("action");
    event.preventDefault();
    event.stopImmediatePropagation();

    $.ajax({
        url: action,
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({
            ItemDetails,
            FullNameResponsible: FullNameResponsible,
            POSReferenceNo: POSReferenceNo,
            POSupplierNumber: POSupplierNumber,
            Supplier: Supplier,
            DeliveryNo: DeliveryNo,
            DeliveryRemarks: DeliveryRemarks,
            DeliveryDate: DeliveryDate,
        }),
        dataType: "json",
        success: function (data) {

            var $this = $("#UpdateItem");
            $this.button('loading');
            setTimeout(function () {
                $this.button('reset');
            }, 8000);

            if (data == "failed") {
                sweetAlert("Fill out Received item at least one.", "", "error");
                return false;
            }
            else {
                swal
                    ({
                         title: "Success",
                        text: + data + " Item/s successfully updated.",
                        type: "success", allowOutsideClick: false
                    }).then(function () {

                        window.location.href = '/Home/DR_RECEIVING';
                    });
            }
        },
        error: function (err) {
            sweetAlert("System Error","","error");
            return false;
        }
    })

})


