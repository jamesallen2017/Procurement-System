

var ClientList = []
var ItemCategories = [];
var SelectedItemCategories = [];
var LocationList = []


$("body").on("click", "#RefreshClientContent", function (event) {


    ClientList = [];


    $("#ClientAddress").val("");
    $('#AddressDiv').removeClass('form-material form-material-primary')
    $('#AddressDiv').addClass('form-material form-material-primary')

    LoadClientCategory($('#ClientNameList'));
    $('#ClientLocation').empty();
    $('#ClientLocation').append($('<option/>').val('').text('No results found'));

    event.preventDefault();
    event.stopImmediatePropagation();
})

$("body").on("click", "#RefreshItemContent", function () {

    $("#tableItemList tbody tr").each(function () {
        var row = $(this);

        var ItemName = $("#ItemID", row).val();

        if (ItemName != "") {

            //ajax function for fetch data

            $.ajax({
                type: "GET",
                url: '/Home/PopulateitemList',
                success: function (data) {
                    ItemCategories = data;

                    var $ele = $("#ItemID", row);
                    $ele.empty();
                    $.each(ItemCategories, function (i, val) {
                        $ele.append($('<option/>').val(this['Value']).html(this['Text']));
                    })
                    $("#ItemID", row).val(ItemName);


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

                            $("#ItemMeasure", row).val(data.ItemMeasure);
                            $("#Qty", row).attr('placeholder', data.Qty);
                            $("#itemDescription", row).val(data.itemDescription); // Render data into textbox 
                        }
                    });

                }
            })
        }
        else {

            $.ajax({
                type: "GET",
                url: '/Home/PopulateitemList',
                success: function (data) {
                    ItemCategories = data;

                    var $ele = $("#ItemID", row);
                    $ele.empty();
                    $.each(ItemCategories, function (i, val) {
                        $ele.append($('<option/>').val(this['Value']).html(this['Text']));
                    })
                }
            })

        }

    })
})

$("body").on("click", "#RefreshSelectedItemContent", function () {

    $("#tableItemLists tbody tr").each(function () {
        var row = $(this);

        var item_ItemID = $("#item_ItemID", row).val();

        if (item_ItemID != "") {

            //ajax function for fetch data

            $.ajax({
                type: "GET",
                url: '/Home/PopulateitemList',
                success: function (data) {
                    SelectedItemCategories = data;

                    var $ele = $("#item_ItemID", row);
                    $ele.empty();
                    $.each(SelectedItemCategories, function (i, val) {
                        $ele.append($('<option/>').val(this['Value']).html(this['Text']));
                    })
                    $("#item_ItemID", row).val(item_ItemID);


                    $.ajax({
                        url: '/Home/GetItemDetails',
                        type: "POST",
                        contentType: "application/json",
                        data: JSON.stringify({
                            value: item_ItemID,
                            POCReferenceNo: "",
                        }),

                        dataType: "json",
                        success: function (data) {

                            $("#item_ItemMeasure", row).val(data.ItemMeasure);
                            $("#item_Qty", row).attr('placeholder', data.Qty);
                            $("#item_itemDescription", row).val(data.itemDescription); // Render data into textbox 
                        }
                    });


                }
            })
        }
        else {

            $.ajax({
                type: "GET",
                url: '/Home/PopulateitemList',
                success: function (data) {
                    ItemCategories = data;

                    var $ele = $("#item_ItemID", row);
                    $ele.empty();
                    $.each(SelectedItemCategories, function (i, val) {
                        $ele.append($('<option/>').val(this['Value']).html(this['Text']));
                    })
                }
            })

        }
    })
})

$("body").on("click", "#RefreshLocationContent", function (event) {


    LocationList = [];
    $("#ClientAddress").val("");
    $('#AddressDiv').removeClass('form-material form-material-primary')
    $('#AddressDiv').addClass('form-material form-material-primary')
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
        var ClientID = $("#ClientNameList").val();
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

$("#ClientID").on('change', function () {

    $("#ClientID").prop('disabled', true);

    setTimeout(function () {
        $("#ClientID").prop('disabled', false);
    }, 2000);
    $("#ClientLocation").val("");
    $('#LocationDiv').removeClass('form-material form-material-primary')
    $('#LocationDiv').addClass('form-material form-material-primary')
    $('#AddressDiv').removeClass('form-material form-material-primary')
    $('#AddressDiv').addClass('form-material form-material-primary')

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
                $ele.append($('<option/>').val('').text('No results found'));
            }

        }

    })

})

$("#ClientNameList").on('change', function () {

    $("#ClientNameList").prop('disabled', true);

    setTimeout(function () {
        $("#ClientNameList").prop('disabled', false);
    }, 2000);

    $('#LocationDiv').removeClass('form-material form-material-primary input-group')
    $('#LocationDiv').addClass('form-material form-material-primary input-group')
    $("#ClientLocation").val("");

    $('#AddressDiv').removeClass('form-material form-material-primary')
    $('#AddressDiv').addClass('form-material form-material-primary')
    $("#ClientAddress").val("");

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

                $('#AddressDiv').removeClass('form-material form-material-primary')
                $('#AddressDiv').addClass('form-material form-material-primary')
                $("#ClientAddress").val(data);
            }
            else {
                $('#AddressDiv').removeClass('form-material form-material-primary')
                $('#AddressDiv').addClass('form-material form-material-primary')
                $("#ClientAddress").val("");

            }

        }

    });

});

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
//------------------------------- PO FROM CLIENT MODULE ------------------------------------//

$('#tableItemList').on('change', '#ItemID', function () {

    var count = $(this).closest("tr");
    var ItemName = $("#ItemID", count).val();

    $("#ItemID", count).prop('disabled', true);

    setTimeout(function () {
        $("#ItemID", count).prop('disabled', false);
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
            var markup;

            $("#ItemMeasure", count).val(data.ItemMeasure);
            $("#itemDescription", count).val(data.itemDescription);

        }

    });

})

$("input[id*='Qty']").on('keyup', function () {

    var qty = $(this).val().replace(/,/g, ''); // GET VALUE OF QTY
    var row = $(this).closest("tr"); // GET ROW
    var price = $("input[id*='Amount']", row).val(); // GET VALUE OF AMOUNT DEPEND WHAT ROW.

    if (qty != "" && price != "") { // IF BOTH NOT NULL
        var subTotal = parseInt(qty) * parseFloat(price); // QTY * PRICE = SUBTOTAL
        if (!isNaN(subTotal)) { // IF NOT NULL

            var i = (subTotal).toFixed(2).toString(); // SET DECIMAL 
            var Comma2 = i.toString().split(".");// PUT PERIOD BETWEEN DECIMAL AND TOTAL
            Comma2[0] = Comma2[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");// SET COMMA 
            $("input[id*='DisplayTotalAmount']", row).val(Comma2.join(".")); // RENDER TOTAL IN TEXTBOX ID
        }

        else // IF NULL
        {
            $("input[id*='DisplayGrandTotal']").val((0).toFixed(2));  // SET 0 WITH 2 DECIMALS
            $("input[id*='DisplayTotalAmount']", row).val((0).toFixed(2)); // SET 0 WITH 2 DECIMALS
        }
    }
    else { //IF ONE OF THEM IS NULL

        $("input[id*='DisplayGrandTotal']").val((0).toFixed(2)); // SET 0 WITH 2 DECIMALS
        $("input[id*='DisplayTotalAmount']", row).val((0).toFixed(2)); // SET 0 WITH 2 DECIMALS
    }

    var TotalAmounts = 0;
    // $("#VAT").each(function () {
    $("input[id*='DisplayTotalAmount']").each(function () {
        TotalAmounts = TotalAmounts + parseFloat($(this).val().replace(/,/g, ''));
    });

    if (TotalAmounts == "NaN") {
        TotalAmounts = 0;
    } 

    if (Discount == "") {
        Discount = 0;
    }
    var TotalWithDiscount = parseFloat(TotalAmounts) - parseFloat(Discount);


    var GrandTOtalAmount = parseFloat(TotalWithDiscount);
    var GrandTOtalAmount = (GrandTOtalAmount).toFixed(2).toString();
    var Comma1 = GrandTOtalAmount.toString().split(".");
    Comma1[0] = Comma1[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    $("#DisplayGrandTotal").val(Comma1.join("."));
});


$("input[id*='Amount']").on('keyup', function () {
    var price = $(this).val().replace(/,/g, '');
    var row = $(this).closest("tr");
    var qty = $("input[id*='Qty']", row).val();


    if (qty != "" && price != "") {
        var subTotal = parseInt(qty) * parseFloat(price);
        if (!isNaN(subTotal)) {

            var i = (subTotal).toFixed(2).toString();
            var Comma2 = i.toString().split(".");
            Comma2[0] = Comma2[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            $("input[id*='DisplayTotalAmount']", row).val(Comma2.join("."));
        }

        else {
            $("input[id*='DisplayGrandTotal']").val((0).toFixed(2));
            $("input[id*='DisplayTotalAmount']", row).val((0).toFixed(2));
        }


    }
    else {
        $("input[id*='DisplayGrandTotal']").val((0).toFixed(2));
        $("input[id*='DisplayTotalAmount']", row).val((0).toFixed(2));
    }

    var TotalAmounts = 0;
    // $("#VAT").each(function () {
    $("input[id*='DisplayTotalAmount']").each(function () {
        TotalAmounts = TotalAmounts + parseFloat($(this).val().replace(/,/g, ''));
    });

    if (TotalAmounts == "NaN") {
        TotalAmounts = 0;
    }

    if (Discount == "") {
        Discount = 0;
    }
    var TotalWithDiscount = parseFloat(TotalAmounts) - parseFloat(Discount);


    var GrandTOtalAmount = parseFloat(TotalWithDiscount);
    var GrandTOtalAmount = (GrandTOtalAmount).toFixed(2).toString();
    var Comma1 = GrandTOtalAmount.toString().split(".");
    Comma1[0] = Comma1[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    $("#DisplayGrandTotal").val(Comma1.join("."));

});

//add button click event

$("#tableItemList").on("click", "#add", function () {

    //clear select data
    var Particular = $('#ItemID').val();
    var ItemMeasure = $('#ItemMeasure').val();
    var Amount = $('#Amount').val();
    var TotalAmount = $('#DisplayTotalAmount').val();
    var Qty = $('#Qty').val();

    if (Particular == "" || ItemMeasure == "" || Qty == "" || Amount == "") {

        sweetAlert("Item row can't be empty.", "", "error");
        return false;
    }

    $('select#ItemID').select2('destroy'); 

    var $newRow = $('#mainrow').clone(true).insertBefore('#mainrow'); 

    $('.pc', $newRow).val($('#ItemID').val());
    //$('#add', $newRow).remove().clone();

    

    $('#ItemID,#Amount,#Qty,#ItemMeasure,#itemDescription,#DisplayTotalAmount', $newRow);

    //clear select data
    $('#Amount').val('');
    $('#Qty').val('');
    $('#ItemMeasure').val('');
    $('#itemDescription').val('');
    $('#DisplayTotalAmount').val('0.00');
    $('#ItemID').val('0');

    $('select#ItemID').select2();
    $("#ItemID").prop('disabled', false);

})

//remove button click event
$('#tableItemList').on('click', '#Remove', function () {

    var trIndex = $(this).closest("tr").index();
    var count = $('#tableItemList tbody tr').length; // COUNT ROW
    var Discount = $("#Discount").val();
    var LISTVAT = $("#AmountVAT").val();

    if (count > 1) { // IF ROW GREATER THAN 1 ABLE TO REMOVE
        //if (trIndex == "0") {

        //    $("#tableItemList tbody:first #Remove").before('<a href="#" name="add" id="add" class="btn btn-success" style="margin-right:5px;"><i class="fa fa-plus"></i></a>')
        //}

        $(this).closest("tr").remove(); // REMOVE ROW DEPEND OF WHAT YOU CLICK
        var newTotal = 0; // SET NEW TOTAL AMOUNT = 0;

        var TotalAmounts = 0;
        // $("#VAT").each(function () {
        $("input[id*='TotalAmount']").each(function () {
            TotalAmounts = TotalAmounts + parseFloat($(this).val().replace(/,/g, ''));
        });
        if (TotalAmounts == "NaN") {
            TotalAmounts = 0;
        }
        var TotalWithDiscount = parseFloat(TotalAmounts) - parseFloat(Discount)

        var GrandTOtalAmount = parseFloat(TotalWithDiscount);
        var GrandTOtalAmount = (GrandTOtalAmount).toFixed(2).toString();
        var Comma1 = GrandTOtalAmount.toString().split(".");
        Comma1[0] = Comma1[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        $("#GrandTotal").val(Comma1.join("."));

    } else {
        // IF ROW IS LESS THAN OR EQUAL TO 1
        sweetAlert("Item row can't be empty.", "", "error");
        return false;
    }
});

$("#ClientValidation").submit(function (event) { // WHEN CLICK SUBMIT INSIDE FORM
    var validate = false;
    var ClientDetails = new Array();   //build a JSON array.

    $("#tableItemLists TBODY TR").each(function () { //Loop through the Table rows 
        var row = $(this);

        var customer = {}; // SET DATA ARRAY TO STORED ALL DATA INSIDE OF TABLE 

        var vItemName = $("#item_ItemID", row).val();
        var vQty = $("#item_Qty", row).val();
        var vAmount = $("#item_Amount", row).val();

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

        ClientDetails.push(customer); // RENDER DATA ARRAY TO JSON ARRAY
    });


    var ClientID = $("#ClientID").val();
    var PODate = $("input[id*='PODate']").val();
    var PONumber = $("input[id*='PONumber']").val();
    var TermsofPayment = $("#TermsofPayment").val();
    var ProjectName = $("input[id*='ProjectName']").val();
    var GrandTotal = $("#DisplayGrandTotal").val();
    var SignProposal = $("input[id*='SignProposal']").val();
    var SalesPersonnel = $("#SalesPersonnel").val();
    var Discount = $("#Discount").val();
    var SupplierPONumberList = $("#SupplierPONumberList").val();
    var ClientLocation = $("#ClientLocation").val();
    var ClientAddress = $("#ClientAddress").val();

    if (validate == true || ClientID == "" || TermsofPayment == "" || ProjectName == "" || ClientLocation == "") {
        return false;
    }

    var $this = $("#btnSaveClient");
    $this.button('loading');
    setTimeout(function () {
        $this.button('reset');
    }, 10000);

    var action = $("#ClientValidation").attr("action"); // GET ACTION URL FROM FORM

    event.preventDefault();
    event.stopImmediatePropagation();

    $.ajax({
        url: action, //SEND TO CONTROLLER ------- (MAIN CONTROLLER -> InsertUserClientInformation)
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({
            ClientDetails, // JSON ARRAY
            ClientID: ClientID,
            PODate: PODate,
            PONumber: PONumber,
            TermsofPayment: TermsofPayment,
            ProjectName: ProjectName,
            GrandTotal: GrandTotal,
            SignProposal: SignProposal,
            SalesPersonnel: SalesPersonnel,
            Discount: Discount,
            SupplierPONumberList: SupplierPONumberList,
            ClientLocation: ClientLocation,
            ClientAddress: ClientAddress,
        }),
        dataType: "json",
        success: function (data) { //AFTER INSERT RETURN RESULT 

            if (data == 'Information') {
                sweetAlert("Fill out Required field.", "", "error");
                //alert("File Shouldn't Be Empty!!");
                return false;
            }
            else if (data == 'ItemGrid') {
                sweetAlert("fill out empty field.", "", "error");

                //alert("File Shouldn't Be Empty!!");
                return false;
            }
            else if (data == 'SessionEnd') {
                sweetAlert("Session was expired", "Please Re-login", "error");
                //alert("File Shouldn't Be Empty!!");
                location.reload();
            }
            else {
                swal
                    ({
                        title: "Success",
                        text: data + " Item/s Successfully Added; and PO from CLIENT Successfully Saved.",
                        type: "success", allowOutsideClick: false
                    }).then(function () {
                        // Redirect the user
                        window.location.href = "/Home/POBYCLIENT_QUOTATION";

                    });
            }
        }
    });
});

//------------------------------- QUOTATION MODULE ------------------------------------//

$('#tableItemLists').on('change', '#item_ItemID', function () {

    var count = $(this).closest("tr");
    var ItemName = $("#item_ItemID", count).val();

    $("#item_ItemID", count).prop('disabled', true);

    setTimeout(function () {
        $("#item_ItemID", count).prop('disabled', false);
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
            var markup;

            $("#item_ItemMeasure", count).val(data.ItemMeasure);
            $("#item_itemDescription", count).val(data.itemDescription);

        }

    });

})

//add button click event
$("#tableItemLists").on("click", "#add", function () {

    //clear select data
    var Particular = $('#item_ItemID').val();
    var ItemMeasure = $('#item_ItemMeasure').val();
    var Amount = $('#item_Amount').val();
    var TotalAmount = $('#item_DisplayTotalAmount').val();
    var Qty = $('#item_Qty').val();

    if (Particular == "" || ItemMeasure == "" || Qty == "" || Amount == "") {

        sweetAlert("Item row can't be empty.", "", "error");
        //alert("File Shouldn't Be Empty!!");
        return false;
    }
    $('select#item_ItemID').select2('destroy');

    var $newRow = $('#mainrow').clone(true).insertBefore('#mainrow'); 

    $('.pc', $newRow).val($('#item_ItemID').val());
    //$('#add', $newRow).remove().clone();


    //remove id attribute from new clone row
    $('#item_ItemID,#item_ItemMeasure,#Qty,#item_DisplayAmount,#item_DisplayTotalAmount,#item_Markup', $newRow);



    //clear select data
    $('#item_ItemID').val('0');
    $('#item_Markup').val('');
    $('#item_ItemMeasure').val('');
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
    var Discount = $("#Discount").val();
    var count = $('#tableItemLists tbody tr').length;
    var LISTVAT = $("#AmountVAT").val();

    if (count > 1) {
        $(this).closest("tr").remove();

        //if (trIndex == "0") {
        //    $("#tableItemLists tbody:first #Remove").before('<a href="#" name="add" id="add" class="btn btn-success" style="margin-right:5px;"><i class="fa fa-plus"></i></a>')
        //}
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

        var TotalWithDiscount = parseFloat(TotalAmounts) - parseFloat(Discount)

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




    var ClientName = $("#ClientName").val();
    var ClientID = $("#ClientNameList").val();
    var ProjectName = $("#ProjectName").val();
    var GrandTotal = $("#DisplayGrandTotal").val();
    var Discount = $("#Discount").val();
    var ClientLocation = $("#ClientLocation").val();
    var ClientAddress = $("#ClientAddress").val();
    var ddlOthersPayment = $("#ddlOthersPayment").val();
    var Submittedby_ID = $("#Submittedby_ID").val();
    var AmountVAT = $("#AmountVAT").val();
    
    var FullNameResponsible = myUserID;

    if (ClientName == "" || ClientID == "" || ProjectName == "" || ClientLocation == "" || ClientAddress == "" || ddlOthersPayment == "" || Submittedby_ID == "") {

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
                        text: data + " Item/s Successfully Added; and Client Quotation Successfully Saved.",
                        type: "success", allowOutsideClick: false
                    }).then(function () {
                        // Redirect the user
                        window.open("/Report/ClientQuotationReport/")

                        location.href = "/Home/CLIENT_QUOTATIONITEM";

                    });
            }

        }
    })
})


//$('#ItemID').val('');
//$('#ItemMeasure').val('');
//$('#Amount').val('');
//$('#TotalAmount').val('0.00');
//$('#Qty').val('');