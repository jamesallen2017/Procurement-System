 //--------------------------------------------------------------------------------------------- POPULATE ITEM / SUPPLIER -----------------------------------------------------------------------//
var ItemCategories = [];

$("body").on("click", "#RefreshItemContent", function () {

    $("#TableSupplierItemList tbody tr").each(function () {
        var row = $(this);

        var Particular = $("#ParticularName", row).val();

        if (Particular != "") {

            //ajax function for fetch data

            $.ajax({
                type: "GET",
                url: '/Home/PopulateitemList',
                success: function (data) {
                    ItemCategories = data;

                    var $ele = $("#ParticularName", row);
                    $ele.empty();
                    $.each(ItemCategories, function (i, val) {
                        $ele.append($('<option/>').val(this['Value']).html(this['Text']));
                    })
                    $("#ParticularName", row).val(Particular);


                    $.ajax({
                        url: '/Home/GetItemDetails',
                        type: "POST",
                        contentType: "application/json",
                        data: JSON.stringify({
                            value: Particular,
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

                    var $ele = $("#ParticularName", row);
                    $ele.empty();
                    $.each(ItemCategories, function (i, val) {
                        $ele.append($('<option/>').val(this['Value']).html(this['Text']));
                    })
                }
            })

        }
    })
})


$("body").on("click", "#RefreshListSupplier", function (event) {
    event.preventDefault();
    event.stopImmediatePropagation();

    SupplierList = [];
    LoadSupplierCategory($('#SupplierName'));
})

function LoadSupplierCategory(element) {
    if (SupplierList.length == 0) {
        //ajax function for fetch data
        $.ajax({

            type: "GET",
            url: '/Home/RefereshSupplier', //REDIRECT TO ACTION RESULT (HOME CONTROLLLER)
            success: function (data) { //RETURN DATA GALING SA CONTROLLER
                SupplierList = data; // RENDER DATA TO STRING ARRAY

                //render item category
                renderSupplierCategory(element);
            }
        })
    }
    else {
        //render catagory to the element
        renderSupplierCategory(element);
    }
}

function renderSupplierCategory(element) {
    $("#SupplierAddresss").val("");
    var $ele = $(element);
    $ele.empty();
    $ele.append($('<option/>').val('').text('Select Supplier'));
    $.each(SupplierList, function (i, val) {
        $ele.append($('<option/>').val(this['Value']).html(this['Text']));
    })
}

$('#TableSupplierItemList').on('change', '#ParticularName', function () {

    var count = $(this).closest("tr");
    var ItemName = $("#ParticularName", count).val();
    var POCReferenceNo = $("#POCReferenceNo").val();

    $("#ParticularName", count).prop('disabled', true);

    setTimeout(function () {
        $("#ParticularName", count).prop('disabled', false);
    }, 2000);

    $.ajax({
        url: '/Home/GetItemDetails',
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({
            value: ItemName,
            POCReferenceNo: POCReferenceNo,
        }),

        dataType: "json",
        success: function (data) {

            $("#ItemMeasure", count).val(data.ItemMeasure);
            $("#Qty", count).attr('placeholder', data.Qty);
            $("#itemDescription", count).val(data.itemDescription); // Render data into textbox 
        }
    });

})



    //--------------------------------------------------------------------------------------------- GET SUPPLIER ADDRESS -----------------------------------------------------------------------//

    function getSupplierAddress() {
        var SupplierID = $("#SupplierName").val();

        $("#SupplierName").prop('disabled', true);

        setTimeout(function () {
            $("#SupplierName").prop('disabled', false);
        }, 2000);

        $.ajax({
            url: '/Home/PopulateListSupplierAddress',
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({
                value: SupplierID
            }),
            dataType: "json",
            success: function (data) {
                if (data != 0) {
                    $('#DivSupplierAddresss').removeClass('form-material form-material-primary floating')
                    $('#DivSupplierAddresss').addClass('form-material form-material-primary')
                    $("#SupplierAddresss").val(data);
                }
                else {
                    $('#DivSupplierAddresss').removeClass('form-material form-material-primary floating')
                    $('#DivSupplierAddresss').addClass('form-material form-material-primary')
                    $("#SupplierAddresss").val("");
                        
                }

            }

        });

    };


    //--------------------------------------------------------------------------------------------- GET CLIENT ADDRESS -----------------------------------------------------------------------//

    function getClientAddress() {
        var SupplierID = $("#ListLocationAddress").val();


        $("#ListLocationAddress").prop('disabled', true);

        setTimeout(function () {
            $("#ListLocationAddress").prop('disabled', false);
        }, 2000);

        $.ajax({
            url: '/Home/PopulateListClientAddress',
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({
                value: SupplierID
            }),
            dataType: "json",
            success: function (data) {
                if (data != 0) {

                    $('#DeliveryDiv').removeClass('form-material form-material-primary floating')
                    $('#DeliveryDiv').addClass('form-material form-material-primary')
                    $("#DeliveryAddress").val(data);
                }
                else {
                    $('#DeliveryDiv').removeClass('form-material form-material-primary')
                    $('#DeliveryDiv').addClass('form-material form-material-primary floating')
                    $("#DeliveryAddress").val("");

                }

            }

        });

};

    function GetTypeOfVat(){

    $("#TableSupplierItemList TBODY TR").each(function () {

        var Type = $("#TypeOfVat").val();
        var Discount = $("#Discount").val();
        var row = $(this);
        var TotalAmount = $("#TotalAmount", row).val().replace(/,/g, '');

        if (Type == "0") {

            var TotalAmounts = 0;
            // $("#VAT").each(function () {
            $("input[id*='TotalAmount']").each(function () {
                TotalAmounts = TotalAmounts + parseFloat($(this).val().replace(/,/g, ''));
            });
            if (Discount == "") {
                Discount = 0;
            }

            var TotalWithDiscount = parseFloat(TotalAmount) - parseFloat(Discount);

            var TotalWithDiscount = parseFloat(TotalWithDiscount);

            var GrandTOtalAmount = parseFloat(TotalWithDiscount);
            var GrandTOtalAmount = (GrandTOtalAmount).toFixed(2).toString();
            var Comma1 = GrandTOtalAmount.toString().split(".");
            Comma1[0] = Comma1[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            $("#GrandTotal").val(Comma1.join("."));

            $("#AmountWithoutVAT", row).val((0).toFixed(2).toString());
            $("#AmountVat",row).val((0).toFixed(2).toString());
            $("#GrandTotalWithoutVat").val((0).toFixed(2).toString());
            $("#GrandTotalVat").val((0).toFixed(2).toString());

        }
        else {


                    var UnitWithOutVat1 = 0;
                    var UnitWithOutVatTotal1 = 0;
                    UnitWithOutVat1 = parseFloat(TotalAmount) / 1.12;
                    UnitWithOutVatTotal1 = parseFloat(UnitWithOutVat1);

                    var value1 = (UnitWithOutVatTotal1).toFixed(2).toString();
                    var Comma3 = value1.toString().split(".");
                    Comma3[0] = Comma3[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                    //$("input[id*='AmountWithoutVAT']", row).val(Comma3.join("."));
                    $("#AmountWithoutVAT", row).val(Comma3.join("."));


                    var Vat1 = 0;
                    var VatTotal1 = 0;
                    Vat1 = parseFloat(value1) * 0.12;
                    VatTotal1 = parseFloat(Vat1);

                    var Vatvalue1 = (VatTotal1).toFixed(2).toString();
                    var Comma3 = Vatvalue1.toString().split(".");
                    Comma3[0] = Comma3[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                    //$("input[id*='VAT']", row).val(Comma3.join("."));
                    $("#AmountVAT", row).val(Comma3.join("."));

            var TotalAmounts = 0;
            // $("#VAT").each(function () {
            $("input[id*='TotalAmount']").each(function () {
                TotalAmounts = TotalAmounts + parseFloat($(this).val().replace(/,/g, ''));
            });
            if (Discount == "") {
                Discount = 0;
            }



            if (Type == "1") {
                var TotalWithDiscount = parseFloat(TotalAmounts) - parseFloat(Discount);

                var UnitWithOutVat1 = 0;
                var UnitWithOutVatTotal1 = 0;
                UnitWithOutVat1 = parseFloat(TotalWithDiscount) / 1.12;
                UnitWithOutVatTotal1 = parseFloat(UnitWithOutVat1);

                var UnitCostvalue = (UnitWithOutVatTotal1).toFixed(2).toString();
                var Comma = UnitCostvalue.toString().split(".");
                Comma[0] = Comma[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                $("#GrandTotalWithoutVat").val(Comma.join("."));


                var Vat1 = 0;
                var VatTotal1 = 0;
                Vat1 = parseFloat(UnitCostvalue) * 0.12;
                VatTotal1 = parseFloat(Vat1);


                var Vatvalue = (VatTotal1).toFixed(2).toString();
                var Comma = Vatvalue.toString().split(".");
                Comma[0] = Comma[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                $("#GrandTotalVat").val(Comma.join("."));

                var GrandTOtalAmount = parseFloat(TotalWithDiscount);
                var GrandTOtalAmount = (GrandTOtalAmount).toFixed(2).toString();
                var Comma1 = GrandTOtalAmount.toString().split(".");
                Comma1[0] = Comma1[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                $("#GrandTotal").val(Comma1.join("."));

            }
            else if (Type == "2") {
                var TotalWithDiscount = parseFloat(TotalAmounts)
                var UnitWithOutVat1 = 0;
                var UnitWithOutVatTotal1 = 0;
                UnitWithOutVat1 = parseFloat(TotalWithDiscount) - parseFloat(Discount);
                UnitWithOutVatTotal1 = parseFloat(UnitWithOutVat1);

                var UnitCostvalue = (UnitWithOutVatTotal1).toFixed(2).toString();
                var Comma = UnitCostvalue.toString().split(".");
                Comma[0] = Comma[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                $("#GrandTotalWithoutVat").val(Comma.join("."));


                var Vat1 = 0;
                var VatTotal1 = 0;
                Vat1 = parseFloat(UnitCostvalue) * 0.12;
                VatTotal1 = parseFloat(Vat1);


                var Vatvalue = (VatTotal1).toFixed(2).toString();
                var Comma = Vatvalue.toString().split(".");
                Comma[0] = Comma[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                $("#GrandTotalVat").val(Comma.join("."));

                var GrandTOtalAmount = ((parseFloat(TotalWithDiscount) - parseFloat(Discount)) * 1.12);
                var GrandTOtalAmount = (GrandTOtalAmount).toFixed(2).toString();
                var Comma1 = GrandTOtalAmount.toString().split(".");
                Comma1[0] = Comma1[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                $("#GrandTotal").val(Comma1.join("."));

            }
           

        }

    });

}

    $("#Discount").on('keyup', function () {
    var Amount = $('#Amount').val();
        var TypeofVAT = $("#TypeOfVat").val();
    var Qty = $('#Qty').val();
    var Discount = $(this).val();

    if (Amount == "" && Qty == "") {
        return false;
    }
    else {

        var TotalAmounts = 0;
        // $("#VAT").each(function () {
        $("input[id*='TotalAmount']").each(function () {
            TotalAmounts = TotalAmounts + parseFloat($(this).val().replace(/,/g, ''));
        });

        if (TotalAmounts == "NaN") {
            TotalAmounts = 0;
        }

        if (Discount == "") {
            Discount = 0;
        }

        if (TypeofVAT == "1") {
            var TotalWithDiscount = parseFloat(TotalAmounts) - parseFloat(Discount);

            var UnitWithOutVat1 = 0;
            var UnitWithOutVatTotal1 = 0;
            UnitWithOutVat1 = parseFloat(TotalWithDiscount) / 1.12;
            UnitWithOutVatTotal1 = parseFloat(UnitWithOutVat1);

            var UnitCostvalue = (UnitWithOutVatTotal1).toFixed(2).toString();
            var Comma = UnitCostvalue.toString().split(".");
            Comma[0] = Comma[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            $("#GrandTotalWithoutVat").val(Comma.join("."));


            var Vat1 = 0;
            var VatTotal1 = 0;
            Vat1 = parseFloat(UnitCostvalue) * 0.12;
            VatTotal1 = parseFloat(Vat1);


            var Vatvalue = (VatTotal1).toFixed(2).toString();
            var Comma = Vatvalue.toString().split(".");
            Comma[0] = Comma[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            $("#GrandTotalVat").val(Comma.join("."));

                 var GrandTOtalAmount = parseFloat(TotalWithDiscount);
            var GrandTOtalAmount = (GrandTOtalAmount).toFixed(2).toString();
            var Comma1 = GrandTOtalAmount.toString().split(".");
            Comma1[0] = Comma1[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            $("#GrandTotal").val(Comma1.join("."));

        }
        else if (TypeofVAT == "2") {    
            var TotalWithDiscount = parseFloat(TotalAmounts) 
            var UnitWithOutVat1 = 0;
            var UnitWithOutVatTotal1 = 0;
            UnitWithOutVat1 = parseFloat(TotalWithDiscount) - parseFloat(Discount);
            UnitWithOutVatTotal1 = parseFloat(UnitWithOutVat1);

            var UnitCostvalue = (UnitWithOutVatTotal1).toFixed(2).toString();
            var Comma = UnitCostvalue.toString().split(".");
            Comma[0] = Comma[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            $("#GrandTotalWithoutVat").val(Comma.join("."));


            var Vat1 = 0;
            var VatTotal1 = 0;
            Vat1 = parseFloat(UnitCostvalue) * 0.12;
            VatTotal1 = parseFloat(Vat1);


            var Vatvalue = (VatTotal1).toFixed(2).toString();
            var Comma = Vatvalue.toString().split(".");
            Comma[0] = Comma[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            $("#GrandTotalVat").val(Comma.join("."));

            var GrandTOtalAmount = ((parseFloat(TotalWithDiscount) - parseFloat(Discount)) * 1.12);
            var GrandTOtalAmount = (GrandTOtalAmount).toFixed(2).toString();
            var Comma1 = GrandTOtalAmount.toString().split(".");
            Comma1[0] = Comma1[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            $("#GrandTotal").val(Comma1.join("."));

        }
        else {
            var TotalWithDiscount = parseFloat(TotalAmounts) - parseFloat(Discount);

            var GrandTOtalAmount = parseFloat(TotalWithDiscount);
            var GrandTOtalAmount = (GrandTOtalAmount).toFixed(2).toString();
            var Comma1 = GrandTOtalAmount.toString().split(".");
            Comma1[0] = Comma1[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            $("#GrandTotal").val(Comma1.join("."));
        }

    }

})

    //--------------------------------------------------------------------------------------------- QUANTITY AND AMOUNT COMPUTATION  -----------------------------------------------------------------------//

    $("#Qty").on('keyup', function () {
        var qty = $(this).val().replace(/,/g, '');
        var row = $(this).closest("tr");
        var Discount = $("#Discount").val();
        var TypeofVAT = $("#TypeOfVat").val();
        var price = $("#Amount", row).val().replace(/,/g, '');

        if (qty != "" && price != "") {
            var subTotal = parseInt(qty) * parseFloat(price);

            if (!isNaN(subTotal)) {

                var i = (subTotal).toFixed(2).toString();
                var Comma2 = i.toString().split(".");
                Comma2[0] = Comma2[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                //$("input[id*='TotalAmount']", row).val(Comma2.join("."));
                $("#TotalAmount", row).val(Comma2.join("."));



                if (TypeofVAT == "1") {

                    var UnitWithOutVat1 = 0;
                    var UnitWithOutVatTotal1 = 0;
                    UnitWithOutVat1 = parseFloat(i) / 1.12;
                    UnitWithOutVatTotal1 = parseFloat(UnitWithOutVat1);

                    var value1 = (UnitWithOutVatTotal1).toFixed(2).toString();
                    var Comma3 = value1.toString().split(".");
                    Comma3[0] = Comma3[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                    //$("input[id*='AmountWithoutVAT']", row).val(Comma3.join("."));
                    $("#AmountWithoutVAT", row).val(Comma3.join("."));


                    var Vat1 = 0;
                    var VatTotal1 = 0;
                    Vat1 = parseFloat(value1) * 0.12;
                    VatTotal1 = parseFloat(Vat1);

                    var Vatvalue1 = (VatTotal1).toFixed(2).toString();
                    var Comma3 = Vatvalue1.toString().split(".");
                    Comma3[0] = Comma3[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                    //$("input[id*='VAT']", row).val(Comma3.join("."));
                    $("#AmountVAT", row).val(Comma3.join("."));
                }
                else if (TypeofVAT == "2") {

                    var UnitWithOutVat1 = 0;
                    var UnitWithOutVatTotal1 = 0;
                    UnitWithOutVat1 = parseFloat(i);
                    UnitWithOutVatTotal1 = parseFloat(UnitWithOutVat1);

                    var value1 = (UnitWithOutVatTotal1).toFixed(2).toString();
                    var Comma3 = value1.toString().split(".");
                    Comma3[0] = Comma3[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                    //$("input[id*='AmountWithoutVAT']", row).val(Comma3.join("."));
                    $("#AmountWithoutVAT", row).val(Comma3.join("."));


                    var Vat1 = 0;
                    var VatTotal1 = 0;
                    Vat1 = parseFloat(value1) * 0.12;
                    VatTotal1 = parseFloat(Vat1);

                    var Vatvalue1 = (VatTotal1).toFixed(2).toString();
                    var Comma3 = Vatvalue1.toString().split(".");
                    Comma3[0] = Comma3[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                    //$("input[id*='VAT']", row).val(Comma3.join("."));
                    $("#AmountVAT", row).val(Comma3.join("."));
                }



            }
            else {

                $("input[id*='GrandTotal']", row).val((0).toFixed(2).toString());
                $("input[id*='TotalAmount']", row).val((0).toFixed(2).toString());
                $("input[id*='AmountWithoutVAT']", row).val((0).toFixed(2).toString());
                $("input[id*='AmountVat']", row).val((0).toFixed(2).toString());
                $("input[id*='GrandTotalWithoutVat']", row).val((0).toFixed(2).toString());
                $("input[id*='GrandTotalVat']", row).val((0).toFixed(2).toString());
            }
        }

        else {

            $("input[id*='GrandTotal']", row).val((0).toFixed(2).toString());
            $("input[id*='TotalAmount']", row).val((0).toFixed(2).toString());
            $("input[id*='AmountWithoutVAT']", row).val((0).toFixed(2).toString());
            $("input[id*='AmountVat']", row).val((0).toFixed(2).toString());
            $("input[id*='GrandTotalWithoutVat']", row).val((0).toFixed(2).toString());
            $("input[id*='GrandTotalVat']", row).val((0).toFixed(2).toString());
        }


        // get total Amount Without VAT

        var UnitCost = 0;
        //$("#AmountWithoutVAT").each(function () {
        $("input[id*='AmountWithoutVAT']").each(function () {
            UnitCost = UnitCost + parseFloat($(this).val().replace(/,/g, ''));
        });

        // get total of VAT

        var GrandVats = 0;
        // $("#VAT").each(function () {
        $("input[id*='AmountVAT']").each(function () {
            GrandVats = GrandVats + parseFloat($(this).val().replace(/,/g, ''));
        });

        var TotalAmounts = 0;
        // $("#VAT").each(function () {
        $("input[id*='TotalAmount']").each(function () {
            TotalAmounts = TotalAmounts + parseFloat($(this).val().replace(/,/g, ''));
        });
        if (Discount == "") {
            Discount = 0;
        }
        if (TypeofVAT == "1") {
            var TotalWithDiscount = parseFloat(TotalAmounts) - parseFloat(Discount);

            var UnitWithOutVat1 = 0;
            var UnitWithOutVatTotal1 = 0;
            UnitWithOutVat1 = parseFloat(TotalWithDiscount) / 1.12;
            UnitWithOutVatTotal1 = parseFloat(UnitWithOutVat1);

            var UnitCostvalue = (UnitWithOutVatTotal1).toFixed(2).toString();
            var Comma = UnitCostvalue.toString().split(".");
            Comma[0] = Comma[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            $("#GrandTotalWithoutVat").val(Comma.join("."));


            var Vat1 = 0;
            var VatTotal1 = 0;
            Vat1 = parseFloat(UnitCostvalue) * 0.12;
            VatTotal1 = parseFloat(Vat1);


            var Vatvalue = (VatTotal1).toFixed(2).toString();
            var Comma = Vatvalue.toString().split(".");
            Comma[0] = Comma[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            $("#GrandTotalVat").val(Comma.join("."));

            var GrandTOtalAmount = parseFloat(TotalWithDiscount);
            var GrandTOtalAmount = (GrandTOtalAmount).toFixed(2).toString();
            var Comma1 = GrandTOtalAmount.toString().split(".");
            Comma1[0] = Comma1[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            $("#GrandTotal").val(Comma1.join("."));

        }
        else if (TypeofVAT == "2") {
            var TotalWithDiscount = parseFloat(TotalAmounts)
            var UnitWithOutVat1 = 0;
            var UnitWithOutVatTotal1 = 0;
            UnitWithOutVat1 = parseFloat(TotalWithDiscount) - parseFloat(Discount);
            UnitWithOutVatTotal1 = parseFloat(UnitWithOutVat1);

            var UnitCostvalue = (UnitWithOutVatTotal1).toFixed(2).toString();
            var Comma = UnitCostvalue.toString().split(".");
            Comma[0] = Comma[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            $("#GrandTotalWithoutVat").val(Comma.join("."));


            var Vat1 = 0;
            var VatTotal1 = 0;
            Vat1 = parseFloat(UnitCostvalue) * 0.12;
            VatTotal1 = parseFloat(Vat1);


            var Vatvalue = (VatTotal1).toFixed(2).toString();
            var Comma = Vatvalue.toString().split(".");
            Comma[0] = Comma[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            $("#GrandTotalVat").val(Comma.join("."));

            var GrandTOtalAmount = ((parseFloat(TotalWithDiscount) - parseFloat(Discount)) * 1.12);
            var GrandTOtalAmount = (GrandTOtalAmount).toFixed(2).toString();
            var Comma1 = GrandTOtalAmount.toString().split(".");
            Comma1[0] = Comma1[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            $("#GrandTotal").val(Comma1.join("."));

        }
        else if (TypeofVAT == "3") {
            var TotalWithDiscount = parseFloat(TotalAmounts)
            var UnitWithOutVat1 = 0;
            var UnitWithOutVatTotal1 = 0;
            UnitWithOutVat1 = parseFloat(TotalWithDiscount) - parseFloat(Discount);
            UnitWithOutVatTotal1 = parseFloat(UnitWithOutVat1);

            var UnitCostvalue = (UnitWithOutVatTotal1).toFixed(2).toString();
            var Comma = UnitCostvalue.toString().split(".");
            Comma[0] = Comma[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            $("#GrandTotalWithoutVat").val(Comma.join("."));


            var Vat1 = 0;
            var VatTotal1 = 0;
            Vat1 = parseFloat(UnitCostvalue) * 0.12;
            VatTotal1 = parseFloat(Vat1);


            var Vatvalue = (VatTotal1).toFixed(2).toString();
            var Comma = Vatvalue.toString().split(".");
            Comma[0] = Comma[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            $("#GrandTotalVat").val(Comma.join("."));

            var GrandTOtalAmount = ((parseFloat(TotalWithDiscount) * 1.12) - parseFloat(Discount));
            var GrandTOtalAmount = (GrandTOtalAmount).toFixed(2).toString();
            var Comma1 = GrandTOtalAmount.toString().split(".");
            Comma1[0] = Comma1[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            $("#GrandTotal").val(Comma1.join("."));
        }
        else {
            var TotalWithDiscount = parseFloat(TotalAmounts) - parseFloat(Discount);

            var GrandTOtalAmount = parseFloat(TotalWithDiscount);
            var GrandTOtalAmount = (GrandTOtalAmount).toFixed(2).toString();
            var Comma1 = GrandTOtalAmount.toString().split(".");
            Comma1[0] = Comma1[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            $("#GrandTotal").val(Comma1.join("."));
        }




    });

    $("#Amount").on('keyup', function () {
        var price = $(this).val().replace(/,/g, '');
        var row = $(this).closest("tr");
        var Discount = $("#Discount").val();
        var TypeofVAT = $("#TypeOfVat").val();
        var qty = $("input[id*='Qty']", row).val().replace(/,/g, '');

        if (qty != "'" && price != "") {
            var subTotal = parseInt(qty) * parseFloat(price);

            if (!isNaN(subTotal)) {

                var i = (subTotal).toFixed(2).toString();
                var Comma2 = i.toString().split(".");
                Comma2[0] = Comma2[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                //$("input[id*='TotalAmount']", row).val(Comma2.join("."));
                $("#TotalAmount", row).val(Comma2.join("."));
                if (TypeofVAT == "1") {

                    var UnitWithOutVat1 = 0;
                    var UnitWithOutVatTotal1 = 0;
                    UnitWithOutVat1 = parseFloat(i) / 1.12;
                    UnitWithOutVatTotal1 = parseFloat(UnitWithOutVat1);

                    var value1 = (UnitWithOutVatTotal1).toFixed(2).toString();
                    var Comma3 = value1.toString().split(".");
                    Comma3[0] = Comma3[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                    //$("input[id*='AmountWithoutVAT']", row).val(Comma3.join("."));
                    $("#AmountWithoutVAT", row).val(Comma3.join("."));


                    var Vat1 = 0;
                    var VatTotal1 = 0;
                    Vat1 = parseFloat(value1) * 0.12;
                    VatTotal1 = parseFloat(Vat1);

                    var Vatvalue1 = (VatTotal1).toFixed(2).toString();
                    var Comma3 = Vatvalue1.toString().split(".");
                    Comma3[0] = Comma3[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                    //$("input[id*='VAT']", row).val(Comma3.join("."));
                    $("#AmountVAT", row).val(Comma3.join("."));
                }
                else if (TypeofVAT == "2") {

                    var UnitWithOutVat1 = 0;
                    var UnitWithOutVatTotal1 = 0;
                    UnitWithOutVat1 = parseFloat(i);
                    UnitWithOutVatTotal1 = parseFloat(UnitWithOutVat1);

                    var value1 = (UnitWithOutVatTotal1).toFixed(2).toString();
                    var Comma3 = value1.toString().split(".");
                    Comma3[0] = Comma3[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                    //$("input[id*='AmountWithoutVAT']", row).val(Comma3.join("."));
                    $("#AmountWithoutVAT", row).val(Comma3.join("."));


                    var Vat1 = 0;
                    var VatTotal1 = 0;
                    Vat1 = parseFloat(value1) * 0.12;
                    VatTotal1 = parseFloat(Vat1);

                    var Vatvalue1 = (VatTotal1).toFixed(2).toString();
                    var Comma3 = Vatvalue1.toString().split(".");
                    Comma3[0] = Comma3[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                    //$("input[id*='VAT']", row).val(Comma3.join("."));
                    $("#AmountVAT", row).val(Comma3.join("."));
                }

            }
            else {

                $("input[id*='GrandTotal']", row).val((0).toFixed(2).toString());
                $("input[id*='TotalAmount']", row).val((0).toFixed(2).toString());
                $("input[id*='AmountWithoutVAT']", row).val((0).toFixed(2).toString());
                $("input[id*='VAT']", row).val((0).toFixed(2).toString());
                $("input[id*='GrandTotalWithoutVat']", row).val((0).toFixed(2).toString());
                $("input[id*='GrandTotalVat']", row).val((0).toFixed(2).toString());
            }
        }

        else {

            $("input[id*='GrandTotal']", row).val((0).toFixed(2).toString());
            $("input[id*='TotalAmount']", row).val((0).toFixed(2).toString());
            $("input[id*='AmountWithoutVAT']", row).val((0).toFixed(2).toString());
            $("input[id*='VAT']", row).val((0).toFixed(2).toString());
            $("input[id*='GrandTotalWithoutVat']", row).val((0).toFixed(2).toString());
            $("input[id*='GrandTotalVat']", row).val((0).toFixed(2).toString());
        }

        // get total Amount Without VAT

        var UnitCost = 0;
        //$("#AmountWithoutVAT").each(function () {
        $("input[id*='AmountWithoutVAT']").each(function () {
            UnitCost = UnitCost + parseFloat($(this).val().replace(/,/g, ''));
        });

        // get total of VAT

        var GrandVats = 0;
        // $("#VAT").each(function () {
        $("input[id*='AmountVAT']").each(function () {
            GrandVats = GrandVats + parseFloat($(this).val().replace(/,/g, ''));
        });

        var TotalAmounts = 0;
        // $("#VAT").each(function () {
        $("input[id*='TotalAmount']").each(function () {
            TotalAmounts = TotalAmounts + parseFloat($(this).val().replace(/,/g, ''));
        });
        if (Discount == "") {
            Discount = 0;
        }
        if (TypeofVAT == "1") {
            var TotalWithDiscount = parseFloat(TotalAmounts) - parseFloat(Discount);

            var UnitWithOutVat1 = 0;
            var UnitWithOutVatTotal1 = 0;
            UnitWithOutVat1 = parseFloat(TotalWithDiscount) / 1.12;
            UnitWithOutVatTotal1 = parseFloat(UnitWithOutVat1);

            var UnitCostvalue = (UnitWithOutVatTotal1).toFixed(2).toString();
            var Comma = UnitCostvalue.toString().split(".");
            Comma[0] = Comma[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            $("#GrandTotalWithoutVat").val(Comma.join("."));


            var Vat1 = 0;
            var VatTotal1 = 0;
            Vat1 = parseFloat(UnitCostvalue) * 0.12;
            VatTotal1 = parseFloat(Vat1);


            var Vatvalue = (VatTotal1).toFixed(2).toString();
            var Comma = Vatvalue.toString().split(".");
            Comma[0] = Comma[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            $("#GrandTotalVat").val(Comma.join("."));

            var GrandTOtalAmount = parseFloat(TotalWithDiscount);
            var GrandTOtalAmount = (GrandTOtalAmount).toFixed(2).toString();
            var Comma1 = GrandTOtalAmount.toString().split(".");
            Comma1[0] = Comma1[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            $("#GrandTotal").val(Comma1.join("."));

        }
        else if (TypeofVAT == "2") {
            var TotalWithDiscount = parseFloat(TotalAmounts)
            var UnitWithOutVat1 = 0;
            var UnitWithOutVatTotal1 = 0;
            UnitWithOutVat1 = parseFloat(TotalWithDiscount) - parseFloat(Discount);
            UnitWithOutVatTotal1 = parseFloat(UnitWithOutVat1);

            var UnitCostvalue = (UnitWithOutVatTotal1).toFixed(2).toString();
            var Comma = UnitCostvalue.toString().split(".");
            Comma[0] = Comma[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            $("#GrandTotalWithoutVat").val(Comma.join("."));


            var Vat1 = 0;
            var VatTotal1 = 0;
            Vat1 = parseFloat(UnitCostvalue) * 0.12;
            VatTotal1 = parseFloat(Vat1);


            var Vatvalue = (VatTotal1).toFixed(2).toString();
            var Comma = Vatvalue.toString().split(".");
            Comma[0] = Comma[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            $("#GrandTotalVat").val(Comma.join("."));

            var GrandTOtalAmount = ((parseFloat(TotalWithDiscount) - parseFloat(Discount)) * 1.12);
            var GrandTOtalAmount = (GrandTOtalAmount).toFixed(2).toString();
            var Comma1 = GrandTOtalAmount.toString().split(".");
            Comma1[0] = Comma1[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            $("#GrandTotal").val(Comma1.join("."));

        }
        else {
            var TotalWithDiscount = parseFloat(TotalAmounts) - parseFloat(Discount);

            var GrandTOtalAmount = parseFloat(TotalWithDiscount);
            var GrandTOtalAmount = (GrandTOtalAmount).toFixed(2).toString();
            var Comma1 = GrandTOtalAmount.toString().split(".");
            Comma1[0] = Comma1[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            $("#GrandTotal").val(Comma1.join("."));
        }

    });


    //--------------------------------------------------------------------------------------------- VALIDATION FOR PREVIEW BUTTON -----------------------------------------------------------------------//

    function DoPreview() {
        $("#HiddenPreview").val("PREVIEW");
}

    function DoSave() {
        $("#HiddenPreview").val("SAVED");
}

        //--------------------------------------------------------------------------------------------- ADD TABLE ROW -----------------------------------------------------------------------//

    $(document).ready(function () {

        $("#btnSaveSupplier").prop('disabled', true);
        //add button click event
        $("body").on("click", "#add", function () {

            //clear select data
            var Particular = $('#ParticularName').val();
            var ItemMeasure = $('#ItemMeasure').val();
            var itemDescription = $('#itemDescription').val();
            var Amount = $('#Amount').val();
            var TotalAmount = $('#TotalAmount').val();
            var AmountVAT = $('#AmountVAT').val();
            var AmountWithoutVAT = $('#AmountWithoutVAT').val();
            var Qty = $('#Qty').val();

            if (Particular == "" || ItemMeasure == "" || Qty == "" || Amount == "") {
                sweetAlert("Item row can't be empty.", "", "error");
                //alert("File Shouldn't Be Empty!!");
                return false;
            }
            $('select#ParticularName').select2('destroy');


            var $newRow = $('#mainrow').clone(true).insertBefore('#mainrow');

            $('.pc', $newRow).val($('#ParticularName').val());

            //remove id attribute from new clone row
            $('#ParticularName,#ItemMeasure,#Qty,#AmountWithoutVAT,#AmountVAT,#Amount,#TotalAmount', $newRow);


            //clear select data
            $('#ParticularName').val("");
            $("#Qty").attr('placeholder', "");
            $('#ItemMeasure').val("");
            $('#Amount').val('');
            $('#TotalAmount').val('0.00');
            $('#AmountVAT').val('0.00');
            $('#AmountWithoutVAT').val('0.00');
            $('#Qty').val("");
            $('#itemDescription').val("");
            $('select#ParticularName').select2();
            $("#ParticularName").prop('disabled', false);


        })

        //--------------------------------------------------------------------------------------------- REMOVE / DELETE  TABLE ROW -----------------------------------------------------------------------//

        //remove button click event
        $('#TableSupplierItemList').on('click', '#Remove', function () {
            var trIndex = $(this).closest("tr").index();
            var Discount = $("#Discount").val();
            var TypeofVAT = $("#TypeOfVat").val();
            var count = $('#TableSupplierItemList tbody tr').length;
            if (count > 1) {
                $(this).closest("tr").remove();

               

                var TotalAmounts = 0;
                $("input[id*='TotalAmount']").each(function () {
                    TotalAmounts = TotalAmounts + parseFloat($(this).val().replace(/,/g, ''));
                });


                if (TotalAmounts == "NaN") {
                    TotalAmounts = 0;
                }


                //--------------------------------------------------------------------------------------------- RECALCULATE AFTER REMOVE TABLE ROW -----------------------------------------------------------------------//


               
                if (Discount == "") {
                    Discount = 0;
                }
                if (TypeofVAT == "1") {
                    var TotalWithDiscount = parseFloat(TotalAmounts) - parseFloat(Discount);

                    var UnitWithOutVat1 = 0;
                    var UnitWithOutVatTotal1 = 0;
                    UnitWithOutVat1 = parseFloat(TotalWithDiscount) / 1.12;
                    UnitWithOutVatTotal1 = parseFloat(UnitWithOutVat1);

                    var UnitCostvalue = (UnitWithOutVatTotal1).toFixed(2).toString();
                    var Comma = UnitCostvalue.toString().split(".");
                    Comma[0] = Comma[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                    $("#GrandTotalWithoutVat").val(Comma.join("."));


                    var Vat1 = 0;
                    var VatTotal1 = 0;
                    Vat1 = parseFloat(UnitCostvalue) * 0.12;
                    VatTotal1 = parseFloat(Vat1);


                    var Vatvalue = (VatTotal1).toFixed(2).toString();
                    var Comma = Vatvalue.toString().split(".");
                    Comma[0] = Comma[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                    $("#GrandTotalVat").val(Comma.join("."));

                    var GrandTOtalAmount = parseFloat(TotalWithDiscount);
                    var GrandTOtalAmount = (GrandTOtalAmount).toFixed(2).toString();
                    var Comma1 = GrandTOtalAmount.toString().split(".");
                    Comma1[0] = Comma1[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                    $("#GrandTotal").val(Comma1.join("."));

                }
                else if (TypeofVAT == "2") {
                    var TotalWithDiscount = parseFloat(TotalAmounts)
                    var UnitWithOutVat1 = 0;
                    var UnitWithOutVatTotal1 = 0;
                    UnitWithOutVat1 = parseFloat(TotalWithDiscount) - parseFloat(Discount);
                    UnitWithOutVatTotal1 = parseFloat(UnitWithOutVat1);

                    var UnitCostvalue = (UnitWithOutVatTotal1).toFixed(2).toString();
                    var Comma = UnitCostvalue.toString().split(".");
                    Comma[0] = Comma[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                    $("#GrandTotalWithoutVat").val(Comma.join("."));


                    var Vat1 = 0;
                    var VatTotal1 = 0;
                    Vat1 = parseFloat(UnitCostvalue) * 0.12;
                    VatTotal1 = parseFloat(Vat1);


                    var Vatvalue = (VatTotal1).toFixed(2).toString();
                    var Comma = Vatvalue.toString().split(".");
                    Comma[0] = Comma[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                    $("#GrandTotalVat").val(Comma.join("."));

                    var GrandTOtalAmount = ((parseFloat(TotalWithDiscount) - parseFloat(Discount)) * 1.12);
                    var GrandTOtalAmount = (GrandTOtalAmount).toFixed(2).toString();
                    var Comma1 = GrandTOtalAmount.toString().split(".");
                    Comma1[0] = Comma1[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                    $("#GrandTotal").val(Comma1.join("."));

                }
                else {
                    var TotalWithDiscount = parseFloat(TotalAmounts) - parseFloat(Discount);

                    var GrandTOtalAmount = parseFloat(TotalWithDiscount);
                    var GrandTOtalAmount = (GrandTOtalAmount).toFixed(2).toString();
                    var Comma1 = GrandTOtalAmount.toString().split(".");
                    Comma1[0] = Comma1[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                    $("#GrandTotal").val(Comma1.join("."));
                }



            } else {
                sweetAlert("Item row can't be empty.", "", "error");
                //alert("File Shouldn't Be Empty!!");
                return false;
            }
        });


        //--------------------------------------------------------------------------------------------- SUBMIT / INSERT FORM -----------------------------------------------------------------------//

        $("#SupplierValidation").submit(function (event) {

            var validate = false;
            //Loop through the Table rows and build a JSON array.
            var SupplierDetails = new Array();
            $("#TableSupplierItemList TBODY TR").each(function () {

                var row = $(this);


                var vParticular = $("#Particular", row).val()
                var vQty = $("#Qty", row).val();
                var vAmount = $("#Amount", row).val();
                var vTotalAmount = $("#TotalAmount", row).val();

                if(vParticular == "" || vQty == "" || vAmount == "" || vTotalAmount == "")
                {
                    validate = true;
                }

                var customer = {};

                customer.ParticularName = $("#ParticularName", row).val();
                customer.itemDescription = $("#itemDescription", row).val();
                customer.ItemMeasure = $("#ItemMeasure", row).val();
                customer.Qty = $("#Qty", row).val();
                customer.Amount = $("#Amount", row).val();
                customer.TotalAmount = $("#TotalAmount", row).val();
                customer.AmountVAT = $("#AmountVAT", row).val();
                customer.AmountWithoutVAT = $("#AmountWithoutVAT", row).val();
                SupplierDetails.push(customer);
            });



            var SupplierID = $("#SupplierName").val();
            var ApproverID = $("#ApproverID").val();
            var EndorserID = $("#Endorser").val();
            var Preview = $("#HiddenPreview").val();
            var SupplierAddress = $("#SupplierAddresss").val();
            var DeliveryAddress = $("#DeliveryAddress").val();
            var ListClientReferenceNo = $("#ListClientReferenceNo").val();
            var ProjectName = $("#ProjectName").val();
            var TermsofPayment = $("#TermsofPayment").val();
            var Shippinginstruction = $("#Shippinginstruction").val();
            var GrandTotalWithoutVat = $("#GrandTotalWithoutVat").val();
            var GrandTotalVat = $("#GrandTotalVat").val();
            var GrandTotal = $("#GrandTotal").val();
            var POCReferenceNo = $("#POCReferenceNo").val();
            var POSupplierNumber = $("#POSupplierNumber").val();
            var POSupplierDate = $("#POSupplierDate").val();
            var Discount = $("#Discount").val();
            var TypeOfVat = $("#TypeOfVat").val();
            var PRNumber = $("#PRNumber").val();
            
            

            if (ApproverID == EndorserID) {
                sweetAlert("Invalid Endorser.", "The Endroser and Approver should not be the same.", "error");
                return false;
            }

            else if (POSupplierDate == "" || Shippinginstruction == "" ||
                ProjectName == "" || TermsofPayment == "" ||
                DeliveryAddress == "" || SupplierAddress == "" || EndorserID == "" || TypeOfVat == "" || validate == true) {
                return false;
            }
         
            var action = $("#SupplierValidation").attr("action");
            event.preventDefault();
            event.stopImmediatePropagation();

            if (Preview == "SAVED") {
                var $this = $("#btnSaveSupplier");
                $this.button('loading');
                setTimeout(function () {
                    $this.button('reset');
                }, 10000);
            }

            $.ajax({
                url: action,
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify({
                    SupplierDetails: SupplierDetails,
                    ListClientReferenceNo: ListClientReferenceNo,
                    ProjectName: ProjectName,
                    SupplierID: SupplierID,
                    ApproverID: ApproverID,
                    EndorserID: EndorserID,
                    SupplierAddresss: SupplierAddress,
                    Shippinginstruction: Shippinginstruction,
                    TermsofPayment: TermsofPayment,
                    GrandTotalWithoutVat: GrandTotalWithoutVat,
                    GrandTotalVat: GrandTotalVat,
                    GrandTotal: GrandTotal,
                    POSupplierDate: POSupplierDate,
                    POSupplierNumber: POSupplierNumber,
                    DeliveryAddress: DeliveryAddress,
                    Discount: Discount,
                    HiddenPreview: Preview,
                    TypeOfVat: TypeOfVat,
                    PRNumber: PRNumber,
                }),
                dataType: "json",
                success: function (data) {

                    if (data == 'Information') {

                        sweetAlert("Please fill out required field", "", "error");
                        return false;
                    }
                    else if (data == 'ItemGrid') {
                        sweetAlert("Please fill out empty field!!", "", "error");
                        return false;
                    }
                    else if (data == 'RedirectLogin') {
                        sweetAlert("Your Session has been ended", "Please Refresh the page.", "error").then(function () {
                            // Redirect the user
                            location.reload();
                        });
                    }
                    else if (data == 'SuccessPreview') {

                        window.open("/Report/ReportPreviewSupplier/");
                        var $this = $("#btnPreviewSupplier");
                        $this.button('loading');
                        setTimeout(function () {
                            $("#btnSaveSupplier").prop('disabled', false);
                            $this.button('reset');
                        }, 5000);
                    }
                    else {
                        swal
                            ({
                                 title: "Success",
                                text: data + " Item/s Successfully Added; and PO to SUPPLIER Successfully Saved.",
                                type: "success", allowOutsideClick: false
                            }).then(function () {

                                window.location.href = "/Report/ReportSupplier/";
                            });

                    }
                }
            });
      
        }); //end .submit()

})

    $('#ParticularName').val("");
    $('#ItemMeasure').val("");
    $('#Amount').val('');
    $('#TotalAmount').val('0.00');
    $('#AmountVAT').val('0.00');
    $('#AmountWithoutVAT').val('0.00');
    $('#GrandTotalWithoutVat').val('0.00');
    $('#GrandTotalVat').val('0.00');
    $('#GrandTotal').val('0.00');
    $('#Qty').val("");























