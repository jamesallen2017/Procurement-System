

$("#TableReleasing #btnDisplayItemRelease").on('click', function () {

    $('#PONumber').val("");
    $('#SignProposal').val("");
    $("#DeliveryRemarks").val("");
    $("#DeliveryDate").val("");


    var count = $(this).closest("tr");
    var POCReferenceNo = $("#DR_POC_ReferenceNo", count).val();
    var Location = $("#DR_Location", count).val();
    var DR_PONumber = $("#DR_PONumber", count).val();
    var DR_SignProposal = $("#DR_SignProposal", count).val();
    var ClientAddress = $("#DR_ClientAddress", count).val();
    var ClientID = $("#DR_ClientID", count).val();

  

    $("#POC_ReferenceNo").val(POCReferenceNo);
    $("#ClientAddress").val(ClientAddress);
    $("#FullNameResponsible").val(mySessionVariable);
    $('#PONumber').val(DR_PONumber);
    $('#SignProposal').val(DR_SignProposal);
    $('#ClientID').val(ClientID);



    $.ajax({
        url: '/Home/PopulateItemReleasing/',
        method: 'POST',
        data: JSON.stringify({
            Location: Location,
            StringPOCReferenceNo: POCReferenceNo,
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
                employeeTable.append('<tr class="norecords"><td colspan="6" class="Normal" style="text-align: center">All item has been released</td></tr>');
            }
            else {
                employeeTable.empty();
                $("#UpdateItem").prop('disabled', false);
                $(data).each(function (index, data) {
                    employeeTable.append('<tr><td>' + data.ItemName + '</td><td class="text-center">' + data.Qty + '</td><td  class="text-center">' + data.Delivered
                        + '</td><td><input type="text" class="form-control" id="ItemCount"></td><td style="display:none;"><input type="text" class="form-control" value="' + data.ItemID
                        + '" id="HiddenItemID"></td><td style="display:none;"><input type="text" class="form-control" value="' + data.ItemMeasure
                        + '" id="HiddenItemMeasure"></td><td style="display:none;"><input type="text" class="form-control" value="' + data.ItemName
                        + '" id="HiddenParticularName"></td><td style="display:none;"><input type="text" class="form-control" value="' + data.Qty
                        + '" id="HiddenQty"></td><td style="display:none;"></td><td style="display:none;"><input type="text" class="form-control" value="' + data.Delivered
                        + '" id="HiddenDelivered"></td></tr>');
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



$("#ClientReleasingItem").submit(function (event) {

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
        var itemcounts = $("#ItemCount", row).val();
        var ItemText = $("#ItemCount", row).val().trim();
        var re = /^[-+]?[0-9]+\.[0-9]+$/;
        var Delivered = $("#HiddenDelivered", row).val();
        if (itemcounts == "")
            itemcounts = 0;

        var TotalDelivered = parseInt(itemcounts) + parseInt(Delivered)

        if (itemcounts < 0 || ItemText.match(re)) {
            validate = "InvalidNumber";
        }
        else if (TotalDelivered > Qty) {
            validate = "Exceeds";
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
      



       
        var item = $("#HiddenParticularName", row).val();

        Item.ItemID = $("#HiddenItemID", row).val();
        Item.ItemName = item
        Item.Qty = $("#HiddenQty", row).val();
        Item.ItemCount = $("#ItemCount", row).val();
        Item.Delivered = $("#HiddenDelivered", row).val();
        Item.ItemMeasure = $("#HiddenItemMeasure", row).val();

        ItemDetails.push(Item);

    })

    var POCReferenceNo = $("#POC_ReferenceNo").val();
    var DeliveryRemarks = $("#DeliveryRemarks").val();
    var DeliveryDate = $("#DeliveryDate").val();
    var FullNameResponsible = $("#FullNameResponsible").val();
    var PONumber = $("#PONumber").val();
    var SignProposal = $("#SignProposal").val();
    var ClientAddress = $("#ClientAddress").val();
    var ClientID = $("#ClientID").val();

  
    if (DeliveryDate == "") {
        return false;
    }
    else if (validate == "InvalidNumber") {
        sweetAlert("Invalid Number Format", "", "error");
        return false;

    }
    else if (validate == "Exceeds") {

        sweetAlert("Released Item/s Exceeds the Quantity Limit.", "Please try again.", "error");
        return false;
    }
    else if (zeronotallowed != 0 && zero == true) {
        sweetAlert("0 not allowed.", "Please try again.", "error");
        return false;
    }

    if (textEmpty == 0) {
        sweetAlert("Fill out Releasing item at least one.", "", "error");
        return false;
    }
   

    var action = $("#ClientReleasingItem").attr("action");
    event.preventDefault();
    event.stopImmediatePropagation();
    $.ajax({
        url: action,
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({
            ItemDetails,
            POC_ReferenceNo: POCReferenceNo,
            DeliveryRemarks: DeliveryRemarks,
            DeliveryDate: DeliveryDate,
            FullNameResponsible: FullNameResponsible,
            PONumber: PONumber,
            SignProposal: SignProposal,
            ClientAddress: ClientAddress,
            ClientID: ClientID,
        }),
        dataType: "json",
        success: function (data) {

            var $this = $("#UpdateItem");
            $this.button('loading');
            setTimeout(function () {
                $this.button('reset');
            }, 8000);

            if (data == "failed") {
                sweetAlert("fill out Released item at least one.", "", "error");
                return false;
            }

            else {
                swal
                    ({
                         title: "Success",
                        text: + data + " Item/s successfully updated.",
                        type: "success", allowOutsideClick: false
                    }).then(function () {
                        // Redirect the user
                        window.open("/Report/DR_Report/",'_blank');
                        window.open("/Report/CustomerDR_Report/",'_blank');
                        window.location.href = '/Home/DR_RELEASING';
                    });
            }
        },
        error: function (err) {
            sweetAlert("Invalid Format", "", "error");
            return false;

        }
    })
})



