

$('#tableItemLists').on('change', '#item_ItemID', function () {

    var count = $(this).closest("tr");
    var ItemName = $("#item_ItemID", count).val();

    $("#item_ItemID",count).prop('disabled', true);

    setTimeout(function () {
        $("#item_ItemID",count).prop('disabled', false);
    }, 2000);


    if (ItemName == "") {
        return;
    }
    $.ajax({
        url: '/Home/GetItemDetails',
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({
            value: ItemName,
            POCReferenceNo: "",
        }),

        dataType: "json",
        success: function (data) {
            $("#item_ItemMeasure", count).val(data.ItemMeasure);
            $("#item_itemDescription", count).val(data.itemDescription);

        }

    });

})

$("#ClientLocation").on('change', function () {

    var ClientLocation = $(this).val();


    $("#ClientLocation").prop('disabled', true);

    setTimeout(function () {
        $("#ClientLocation").prop('disabled', false);
    }, 2000);

    $.ajax({
        url: '/Home/PopulateListClientAddress',
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({
            value: ClientLocation
        }),
        dataType: "json",
        success: function (data) {
            if (data != 0) {

                $('#AddressDiv').removeClass('form-material form-material-primary floating')
                $('#AddressDiv').addClass('form-material form-material-primary')
                $("#ClientAddress").val(data);
            }
            else {
                $('#AddressDiv').removeClass('form-material form-material-primary')
                $('#AddressDiv').addClass('form-material form-material-primary floating')
                $("#ClientAddress").val("");

            }

        }

    });

});

$("#ClientID").on('change', function () {

    $("#ClientID").prop('disabled', true);

    setTimeout(function () {
        $("#ClientID").prop('disabled', false);
    }, 2000);
    $("#ClientLocation").val("");
    $("#ClientAddress").val("");
    $('#LocationDiv').removeClass('form-material form-material-primary')
    $('#LocationDiv').addClass('form-material form-material-primary floating')
    $('#AddressDiv').removeClass('form-material form-material-primary')
    $('#AddressDiv').addClass('form-material form-material-primary floating')

    var ClientID = $(this).val();

    var location = [];


    $.ajax({
        url: '/Home/GetClientLocation',
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({
            ClientID: ClientID,
        }),

        contentType: "application/json",
        dataType: "json",
        success: function (data) {
            location = data;

            var $ele = $("#ClientLocation");
            $ele.empty();

            if (location.length != 0) {
                $ele.append($('<option/>').val('').text('Select Location'));
                $.each(location, function (i, val) {
                    $ele.append($('<option/>').val(this['Value']).html(this['Text']));
                })
            }
            else {
                $ele.append($('<option/>').val('').text('No Record.'));
            }

        }

    })

})

var ItemCategories = []

$("body").on("click", "#RefreshItemContent", function (event) {
    event.preventDefault();
    event.stopImmediatePropagation();
    ItemCategories = [];
    $("#ItemMeasure").val(""); // Render data into textbox 
    $("#itemDescription").val(""); // Render data into textbox 
    LoadItemCategory($('#ItemName'));
})


var ClientList = []

$("body").on("click", "#RefreshClientContent", function (event) {


    ClientList = [];


    $("#ClientAddress").val("");
    $('#AddressDiv').removeClass('form-material form-material-primary')
    $('#AddressDiv').addClass('form-material form-material-primary floating')

    LoadClientCategory($('#ClientID'));
    $('#ClientLocation').empty();
    $('#ClientLocation').append($('<option/>').val('').text('No results found'));

    event.preventDefault();
    event.stopImmediatePropagation();
})


var LocationList = []

$("body").on("click", "#RefreshLocationContent", function (event) {


    LocationList = [];
    $("#ClientAddress").val("");
    $('#AddressDiv').removeClass('form-material form-material-primary')
    $('#AddressDiv').addClass('form-material form-material-primary floating')
    LoadLocationCategory($('#ClientLocation'));

    event.preventDefault();
    event.stopImmediatePropagation();
})


// CLIENT DROPDOWNLIST
function LoadClientCategory(element) {
    if (ClientList.length == 0) {
        //ajax function for fetch data
        $.ajax({

            type: "GET",
            url: '/Home/RefereshClient', //REDIRECT TO ACTION RESULT (HOME CONTROLLLER)
            contentType: "application/json",
            data: {
            },
            dataType: "json",
            success: function (data) { //RETURN DATA GALING SA CONTROLLER
                ClientList = data; // RENDER DATA TO STRING ARRAY

                //render item category
                renderClientCategory(element);
            }
        })
    }
    else {
        //render catagory to the element
        renderClientCategory(element);
    }
}

function renderClientCategory(element) {
    var $ele = $(element);
    $ele.empty();
    $ele.append($('<option/>').val('').text('Select Client'));
    $.each(ClientList, function (i, val) {
        $ele.append($('<option/>').val(this['Value']).html(this['Text']));
    })
}

// CLIENT LOCATION DROPDOWNLIST
function LoadLocationCategory(element) {
    if (LocationList.length == 0) {
        //ajax function for fetch data
        var ClientID = $("#ClientID").val();
        $.ajax({

            type: "GET",
            url: '/Home/RefereshLocation', //REDIRECT TO ACTION RESULT (HOME CONTROLLLER)
            contentType: "application/json",
            data: {
                ClientID: ClientID
            },
            dataType: "json",
            success: function (data) { //RETURN DATA GALING SA CONTROLLER
                LocationList = data; // RENDER DATA TO STRING ARRAY

                //render item category
                renderLocationCategory(element);
            }
        })
    }
    else {
        //render catagory to the element
        renderLocationCategory(element);
    }
}

function renderLocationCategory(element) {
    var $ele = $(element);
    $ele.empty();
    $ele.append($('<option/>').val('').text('Select Location'));
    $.each(LocationList, function (i, val) {
        $ele.append($('<option/>').val(this['Value']).html(this['Text']));
    })
}

//ITEM DROPDOWNLIST

function LoadItemCategory(element) {
    if (ItemCategories.length == 0) {
        //ajax function for fetch data

        $.ajax({

            type: "GET",
            url: '/Home/PopulateitemList',
            success: function (data) {
                ItemCategories = data;
                //render item category

                renderCategory(element);
            }
        })
    }
    else {
        //render catagory to the element
        renderCategory(element);
    }
}

function renderCategory(element) {
    var $ele = $(element);
    $ele.empty();
    $ele.append($('<option/>').val('0').text('Select Item'));
    $.each(ItemCategories, function (i, val) {
        $ele.append($('<option/>').val(this['Value']).html(this['Text']));
    })
}


$(document).ready(function () {
    //add button click event

    GETTOTAL();


    $("#ClientValidation").submit('#btnSaveClient',function (event) {
        //Loop through the Table rows and build a JSON array.
        var validate = false;
        var ClientDetails = new Array();
        $("#tableItemLists TBODY TR").each(function () {
            var row = $(this);
            var customer = {};
            var vItemName = $("#item_ItemID", row).val();
            var vQty = $("#item_Qty", row).val();
            var vAmount = $("#item_DisplayAmount", row).val();

            if (vItemName == "" || vQty == "" || vAmount == "") {
                validate = true;

            }
            customer.ItemID = $("#item_ItemID", row).val();
            customer.itemDescription = $('#item_itemDescription', row).val();
            customer.ItemMeasure = $("#item_ItemMeasure", row).val();
            customer.Qty = $("input[id*='item_Qty']", row).val();
            customer.Amount = $("input[id*='item_DisplayAmount']", row).val();
            customer.Markup = $("input[id*='item_Markup']", row).val();
            customer.TotalAmount = $("input[id*='item_DisplayTotalAmount']", row).val();
            customer.Released = $("input[id*='item_Released']", row).val();
            
            ClientDetails.push(customer);
        });

        var Old_ReferenceNo = $("#POC_ReferenceNo").val();
        var ClientID = $("#ClientID").val();
        var PODate = $("input[id*='PODate']").val();
        var PONumber = $("input[id*='PONumber']").val();
        var TermsofPayment = $("#TermsofPayment").val();
        var ProjectName = $("input[id*='ProjectName']").val();
        var GrandTotal = $("#DisplayGrandTotal").val();
        var SignProposal = $("input[id*='SignProposal']").val();
        var SupplierPONumberList = $("#SupplierPONumberList").val();
        var ClientLocation = $("#ClientLocation").val();
        var Discount = $("#Discount").val();
        var SalesPersonnel = $("#SalesPersonnel").val();
        var ClientAddress = $("#ClientAddress").val();
        

        if (validate == true || ClientID == "" || TermsofPayment == "" || ProjectName == "" || ClientLocation == "") {
            return false;
        }
        var $this = $("#btnSaveClient");
        $this.button('loading');
        setTimeout(function () {
            $this.button('reset');
        }, 10000);

        var action = $("#ClientValidation").attr("action");
        event.preventDefault();
        event.stopImmediatePropagation();
        $.ajax({
            url: action,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({
                ClientDetails,
                ClientID: ClientID,
                PODate: PODate,
                PONumber: PONumber,
                TermsofPayment: TermsofPayment,
                ProjectName: ProjectName,
                GrandTotal: GrandTotal,
                SignProposal: SignProposal,
                Old_ReferenceNo: Old_ReferenceNo,
                SalesPersonnel: SalesPersonnel,
                Discount: Discount,
                SupplierPONumberList: SupplierPONumberList,
                ClientLocation: ClientLocation,
                ClientAddress: ClientAddress,
            }),
            dataType: "json",
            success: function (data) {

                if (data == 'Information') {
                    sweetAlert("Please fill out Required field.", "", "error");
                    //alert("File Shouldn't Be Empty!!");
                    return false;
                }
                else if (data == 'ItemGrid') {
                    sweetAlert("Please fill out empty field.", "", "error");

                    //alert("File Shouldn't Be Empty!!");
                    return false;
                }
                else {
                    swal
                        ({
                             title: "Success",
                            text: data + " Item/s Successfully Updated; and PO from CLIENT Successfully Updated.",
                            type: "success", allowOutsideClick: false
                        }).then(function () {
                            // Redirect the user
                            window.location.href = '/Home/EditClient/';
                        });

                }
            }
        });
    }); //end .submit()

    $("#ClientQuotationValidation").submit(function (event) {

        var validate = false;

        var ClientQuotationArray = new Array();
            $("#tableItemLists tbody tr").each(function () {

                var row = $(this);

                var ItemList = {};

                var vItemName = $("#item_ItemID", row).val();
                var vQty = $("#item_Qty", row).val();
                var vAmount = $("#item_DisplayAmount", row).val();

                if (vItemName == "" || vQty == "" || vAmount == "") {
                    validate = true
                }

                ItemList.ItemID = $("#item_ItemID", row).val();
                ItemList.itemDescription = $('#item_itemDescription', row).val();
                ItemList.ItemMeasure = $("#item_ItemMeasure", row).val();
                ItemList.Qty = $("input[id*='item_Qty']", row).val();
                ItemList.Amount = $("input[id*='item_DisplayAmount']", row).val();
                ItemList.Markup = $("input[id*='item_Markup']", row).val();
                ItemList.TotalAmount = $("input[id*='item_DisplayTotalAmount']", row).val();

                ClientQuotationArray.push(ItemList);

            })




        var Quotation_ControlNo = $("#Quotation_ControlNo").val();
        var ClientName = $("#ClientName").val();
        var ClientID = $("#ClientID").val();
        var ProjectName = $("#ProjectName").val();
        var GrandTotal = $("#DisplayGrandTotal").val();
        var Discount = $("#Discount").val();
        var ClientLocation = $("#ClientLocation").val();
        var ClientAddress = $("#ClientAddress").val();
        var Quotation_ReferenceNo = $("#Quotation_ReferenceNo").val();
        var ddlOthersPayment = $("#ddlOthersPayment").val();
    var Submittedby_ID = $("#Submittedby_ID").val();
        var AmountVAT = $("#AmountVAT").val();
        
        var FullNameResponsible = myUserID;

        if (ClientName == "" || ClientID == "" || ProjectName == "" || ClientLocation == "" || ClientAddress == "") {

            return false;
        }
        else if (validate == true) {
            sweetAlert("Fill out empty field.", "", "error");
            return false;

        }



        var $this = $("#btnSaveClient");
        $this.button('loading');
        setTimeout(function () {
            $this.button('reset');
        }, 10000);



        var action = $("#ClientQuotationValidation").attr("action")

        event.preventDefault();
        event.stopImmediatePropagation();

        $.ajax({

            url: action,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({
                Quotation_ReferenceNo: Quotation_ReferenceNo,
                Quotation_ControlNo: Quotation_ControlNo,
                ClientQuotationArray: ClientQuotationArray,
                ClientName: ClientName,
                ClientID: ClientID,
                ProjectName: ProjectName,
                GrandTotal: GrandTotal,
                Discount: Discount,
                ClientLocation: ClientLocation,
                ClientAddress: ClientAddress,
                FullNameResponsible: FullNameResponsible,
                ddlOthersPayment: ddlOthersPayment,
                Submittedby_ID: Submittedby_ID,
                AmountVAT: AmountVAT,

            }),
            dataType: "json",
            success: function (data) {

                if (data == 0) {
                    sweetAlert("Contact our administrator for assistance.", "", "error");
                    return false;

                } else {

                    swal
                        ({
                            title: "Success",
                            text: data + " Item/s Successfully Added; and Client Quotation Successfully Updated.",
                            type: "success", allowOutsideClick: false
                        }).then(function () {
                            // Redirect the user
                            window.open("/Report/ClientQuotationReport/" +Quotation_ControlNo)
                            window.location.href = "/Home/CLIENT_EDITQUOTATION";

                        });
                }

            }
        })
    })


})

LoadItemCategory($('#ItemID'));


//$("#Discount").on('keyup', function () {
//    var Amount = $('#DisplayAmount').val();
//    var Qty = $('#Qty').val();
//    var Discount = $(this).val();

//    if (Amount == "" && Qty == "") {
//        return false;
//    }
//    else {

//        var TotalAmounts = 0;
//        // $("#VAT").each(function () {
//        $("input[id*='DisplayTotalAmount']").each(function () {
//            TotalAmounts = TotalAmounts + parseFloat($(this).val().replace(/,/g, ''));
//        });

//        if (TotalAmounts == "NaN") {
//            TotalAmounts = 0;
//        }

//        if (Discount == "") {
//            Discount = 0;
//        }
//        var TotalWithDiscount = parseFloat(TotalAmounts) - parseFloat(Discount);

//        var GrandTOtalAmount = parseFloat(TotalWithDiscount);
//        var GrandTOtalAmount = (GrandTOtalAmount).toFixed(2).toString();
//        var Comma1 = GrandTOtalAmount.toString().split(".");
//        Comma1[0] = Comma1[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
//        $("#DisplayGrandTotal").val(Comma1.join("."));
//    }

//})

$("#Discount").on('keyup', function (e) {
    e.preventDefault();
    GETTOTAL();
})

$("#AmountVAT").on("change", function (e) {
    var LISTVAT = $("#AmountVAT").val();
    e.preventDefault();
    GETTOTAL();
})

function GETTOTAL() {

    var Amount = $('#item_DisplayAmount').val();
    var Qty = $('#item_Qty').val();
    var Discount = $("#Discount").val();
    var LISTVAT = $("#AmountVAT").val();
    if (LISTVAT == null || LISTVAT == "") {
        LISTVAT = "1.00"
    }
    if (Amount == "" && Qty == "") {
        return false;
    }
    else {

        var TotalAmounts = 0;
        // $("#VAT").each(function () {
        $("input[id*='item_DisplayTotalAmount']").each(function () {
            TotalAmounts = TotalAmounts + parseFloat($(this).val().replace(/,/g, ''));
        });

        if (TotalAmounts == "NaN") {
            TotalAmounts = 0;
        }

        if (Discount == "") {
            Discount = 0;
        }
        var TotalWithDiscount = parseFloat(TotalAmounts) - parseFloat(Discount);

        var GrandTOtalAmount = parseFloat(TotalWithDiscount) * parseFloat(LISTVAT);
        var GrandTOtalAmount = (GrandTOtalAmount).toFixed(2).toString();
        var Comma1 = GrandTOtalAmount.toString().split(".");
        Comma1[0] = Comma1[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        $("#DisplayGrandTotalWithVat").val(Comma1.join("."));

        var GrandTOtalAmount = parseFloat(TotalWithDiscount);
        var GrandTOtalAmount = (GrandTOtalAmount).toFixed(2).toString();
        var Comma1 = GrandTOtalAmount.toString().split(".");
        Comma1[0] = Comma1[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        $("#DisplayGrandTotal").val(Comma1.join("."));
    }
}

$("#tableItemLists").on("click", "#add", function () {

    //clear select data
    var Particular = $('#item_ItemID').val();
    var ItemMeasure = $('#item_ItemMeasure').val();
    var Amount = $('#item_Amount').val();
    var TotalAmount = $('#item_TotalAmount').val();
    var Qty = $('#item_Qty').val();

    if (Particular == "" || ItemMeasure == "" || Qty == "" || Amount == "") {

        sweetAlert("Item row can't be empty.", "", "error");
        //alert("File Shouldn't Be Empty!!");
        return false;
    }
    $('select#item_ItemID').select2('destroy');

    var $newRow = $('#mainrow').clone(true).insertBefore('#mainrow'); 

    $('.pc', $newRow).val($('#item_ItemID').val());


    //remove id attribute from new clone row
    $('#item_ItemID,#item_ItemMeasure,#Qty,#item_DisplayAmount,#item_DisplayTotalAmount,#item_Markup', $newRow);



    //clear select data
    $('#item_ItemID').val('0');
    $('#item_ItemMeasure').val('');
    $('#item_Markup').val('');
    $('#item_DisplayAmount').val('');
    $('#item_DisplayTotalAmount').val('0.00');
    $('#item_Qty').val('');
    $('#item_itemDescription').val('');
    $('select#item_ItemID').select2();
    $("#item_ItemID").prop('disabled', false);


})

//remove button click event
$('#tableItemLists').on('click', '#Remove', function () {

    var trIndex = $(this).closest("tr").index();
    var count = $('#tableItemLists tbody tr').length;

    if (count > 1) {
        $(this).closest("tr").remove();

        var TotalAmounts = 0;
        // $("#VAT").each(function () {
        $("input[id*='item_DisplayTotalAmount']").each(function () {
            TotalAmounts = TotalAmounts + parseFloat($(this).val().replace(/,/g, ''));
        });

        if (TotalAmounts == "NaN") {
            TotalAmounts = 0;
        }

        var TotalWithDiscount = parseFloat(TotalAmounts);

        var GrandTOtalAmount = parseFloat(TotalWithDiscount);
        var GrandTOtalAmount = (GrandTOtalAmount).toFixed(2).toString();
        var Comma1 = GrandTOtalAmount.toString().split(".");
        Comma1[0] = Comma1[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        $("#DisplayGrandTotal").val(Comma1.join("."));

    } else {
        sweetAlert("Item row can't be empty.", "", "error");
        //alert("File Shouldn't Be Empty!!");
        return false;

    }
});

$("input[id*='item_Qty']").on('keyup', function () {
    var qty = $(this).val().replace(/,/g, '');
    var Discount = $("#Discount").val();
    var row = $(this).closest("tr");
    var price = $("input[id*='item_DisplayAmount']", row).val().replace(/,/g, '');
    var LISTVAT = $("#AmountVAT").val();
    var markup = $("input[id*='item_Markup']", row).val().replace(/,/g, '');

    if (markup == "") {
        markup = 0;
    }

    if (qty != "" && price != "") {
        var subTotal = parseInt(qty) * parseFloat(price);
        if (!isNaN(subTotal)) {

            //var i = (subTotal).toFixed(2).toString();
            //var Comma2 = i.toString().split(".");

            var total = parseInt(subTotal) * (1 + (parseFloat(markup) / 100))
            var i = (total).toFixed(2).toString();
            var Comma2 = i.toString().split(".");
            Comma2[0] = Comma2[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            $("input[id*='item_DisplayTotalAmount']", row).val(Comma2.join("."));

        }

        else {
            $("input[id*='DisplayGrandTotal']").val((0).toFixed(2));
            $("input[id*='item_DisplayTotalAmount']", row).val((0).toFixed(2));
        }
    }
    else {
        $("input[id*='DisplayGrandTotal']").val((0).toFixed(2));
        $("input[id*='item_DisplayTotalAmount']", row).val((0).toFixed(2));
    }

    var TotalAmounts = 0;
    // $("#VAT").each(function () {
    $("input[id*='item_DisplayTotalAmount']").each(function () {
        TotalAmounts = TotalAmounts + parseFloat($(this).val().replace(/,/g, ''));
    });

    if (TotalAmounts == "NaN") {
        TotalAmounts = 0;
    }

    if (Discount == "") {
        Discount = 0;
    }
    var TotalWithDiscount = parseFloat(TotalAmounts) - parseFloat(Discount);

    var GrandTOtalAmount = parseFloat(TotalWithDiscount) * parseFloat(LISTVAT);
    var GrandTOtalAmount = (GrandTOtalAmount).toFixed(2).toString();
    var Comma1 = GrandTOtalAmount.toString().split(".");
    Comma1[0] = Comma1[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    $("#DisplayGrandTotalWithVat").val(Comma1.join("."));


    var GrandTOtalAmount = parseFloat(TotalWithDiscount);
    var GrandTOtalAmount = (GrandTOtalAmount).toFixed(2).toString();
    var Comma1 = GrandTOtalAmount.toString().split(".");
    Comma1[0] = Comma1[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    $("#DisplayGrandTotal").val(Comma1.join("."));
});

$("input[id*='item_DisplayAmount']").on('keyup', function () {
    var price = $(this).val().replace(/,/g, '');
    var Discount = $("#Discount").val();
    var row = $(this).closest("tr");
    var qty = $("input[id*='item_Qty']", row).val().replace(/,/g, '');
    var LISTVAT = $("#AmountVAT").val();
    var markup = $("input[id*='item_Markup']", row).val().replace(/,/g, '');

    if (markup == "") {
        markup = 0;
    }

    if (qty != "" && price != "") {
        var subTotal = parseInt(qty) * parseFloat(price);
        if (!isNaN(subTotal)) {

            //var i = (subTotal).toFixed(2).toString();
            //var Comma2 = i.toString().split(".");

            var total = parseInt(subTotal) * (1 + (parseFloat(markup) / 100))
            var i = (total).toFixed(2).toString();
            var Comma2 = i.toString().split(".");
            Comma2[0] = Comma2[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            $("input[id*='item_DisplayTotalAmount']", row).val(Comma2.join("."));

        }

        else {
            $("input[id*='DisplayGrandTotal']").val((0).toFixed(2));
            $("input[id*='item_DisplayTotalAmount']", row).val((0).toFixed(2));
        }


    }
    else {
        $("input[id*='DisplayGrandTotal']").val((0).toFixed(2));
        $("input[id*='item_DisplayTotalAmount']", row).val((0).toFixed(2));
    }

    var TotalAmounts = 0;
    // $("#VAT").each(function () {
    $("input[id*='item_DisplayTotalAmount']").each(function () {
        TotalAmounts = TotalAmounts + parseFloat($(this).val().replace(/,/g, ''));
    });

    if (TotalAmounts == "NaN") {
        TotalAmounts = 0;
    }

    if (Discount == "") {
        Discount = 0;
    }
    var TotalWithDiscount = parseFloat(TotalAmounts) - parseFloat(Discount);

    var GrandTOtalAmount = parseFloat(TotalWithDiscount) * parseFloat(LISTVAT);
    var GrandTOtalAmount = (GrandTOtalAmount).toFixed(2).toString();
    var Comma1 = GrandTOtalAmount.toString().split(".");
    Comma1[0] = Comma1[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    $("#DisplayGrandTotalWithVat").val(Comma1.join("."));


    var GrandTOtalAmount = parseFloat(TotalWithDiscount);
    var GrandTOtalAmount = (GrandTOtalAmount).toFixed(2).toString();
    var Comma1 = GrandTOtalAmount.toString().split(".");
    Comma1[0] = Comma1[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    $("#DisplayGrandTotal").val(Comma1.join("."));

});

$("input[id*='item_Markup']").on('keyup', function () {
    var row = $(this).closest("tr");
    var markup = $(this).val().replace(/,/g, '');
    var price = $("input[id*='item_DisplayAmount']", row).val().replace(/,/g, '');
    var Discount = $("#Discount").val();
    var qty = $("input[id*='item_Qty']", row).val().replace(/,/g, '');
    var LISTVAT = $("#AmountVAT").val();

    if (markup == "") {
        markup = 0;
    }

    if (qty != "" && price != "") {
        var subTotal = parseInt(qty) * parseFloat(price);
        if (!isNaN(subTotal)) {

            var total = parseInt(subTotal) * (1 + (parseFloat(markup) / 100))
            var i = (total).toFixed(2).toString();
            var Comma2 = i.toString().split(".");
            Comma2[0] = Comma2[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            $("input[id*='item_DisplayTotalAmount']", row).val(Comma2.join("."));

        }

        else {
            $("input[id*='DisplayGrandTotal']").val((0).toFixed(2));
            $("input[id*='item_DisplayTotalAmount']", row).val((0).toFixed(2));
        }


    }
    else {
        $("input[id*='DisplayGrandTotal']").val((0).toFixed(2));
        $("input[id*='item_DisplayTotalAmount']", row).val((0).toFixed(2));
    }

    var TotalAmounts = 0;
    // $("#VAT").each(function () {
    $("input[id*='item_DisplayTotalAmount']").each(function () {
        TotalAmounts = TotalAmounts + parseFloat($(this).val().replace(/,/g, ''));
    });

    if (TotalAmounts == "NaN") {
        TotalAmounts = 0;
    }

    if (Discount == "") {
        Discount = 0;
    }
    var TotalWithDiscount = parseFloat(TotalAmounts) - parseFloat(Discount);

    var GrandTOtalAmount = parseFloat(TotalWithDiscount) * parseFloat(LISTVAT);
    var GrandTOtalAmount = (GrandTOtalAmount).toFixed(2).toString();
    var Comma1 = GrandTOtalAmount.toString().split(".");
    Comma1[0] = Comma1[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    $("#DisplayGrandTotalWithVat").val(Comma1.join("."));


    var GrandTOtalAmount = parseFloat(TotalWithDiscount);
    var GrandTOtalAmount = (GrandTOtalAmount).toFixed(2).toString();
    var Comma1 = GrandTOtalAmount.toString().split(".");
    Comma1[0] = Comma1[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    $("#DisplayGrandTotal").val(Comma1.join("."));

})