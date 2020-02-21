$('#TableSupplierItemList').on('change', '#item_ParticularName', function () {

    var count = $(this).closest("tr");
    var ItemName = $("#item_ParticularName", count).val();
    var POCReferenceNo = $("#POCReferenceNo").val();

    
    $("#item_ParticularName",count).prop('disabled', true);

    setTimeout(function () {
        $("#item_ParticularName",count).prop('disabled', false);
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

            $("#item_ItemMeasure", count).val(data.ItemMeasure);
            $("#item_Qty", count).attr('placeholder', data.Qty);
            $("#item_itemDescription", count).val(data.itemDescription); // Render data into textbox 
        }
    });

})



function getClientAddress() {
    var SupplierID = $("#LocationAddressID").val();

    $("#LocationAddressID").prop('disabled', true);

    setTimeout(function () {
        $("#LocationAddressID").prop('disabled', false);
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
                $("#DeliveryAddress").val(data);
        }

    });

};



function getSupplierAddress() {
    var SupplierID = $("#Supplier").val();
    
    $("#Supplier").prop('disabled', true);

    setTimeout(function () {
        $("#Supplier").prop('disabled', false);
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

                $('#DivSupplierAddresss').removeClass('form-material-primary floating')
                $('#DivSupplierAddresss').addClass('form-material')
                $("#SupplierAddresss").val(data);
            }
            else {
                $('#DivSupplierAddresss').removeClass('form-material')
                $('#DivSupplierAddresss').addClass('form-material-primary floating')
                $("#SupplierAddresss").val("");
            }
        }
    });
};


function GetTypeOfVat() {

    $("#TableSupplierItemList TBODY TR").each(function () {

        var Type = $("#TypeOfVat").val();
        var Discount = $("#Discount").val();
        var row = $(this);
        var TotalAmount = $("#item_DisplayTotalAmount", row).val().replace(/,/g, '');

        if (Type == "0") {
            var TotalAmounts = 0;
            // $("#VAT").each(function () {
            $("input[id*='TotalAmount']").each(function () {
                TotalAmounts = TotalAmounts + parseFloat($(this).val().replace(/,/g, ''));
            });
            if (Discount == "") {
                Discount = 0;
            }

            var TotalWithDiscount = (parseFloat(TotalAmounts) - parseFloat(Discount));

            var GrandTOtalAmount = parseFloat(TotalWithDiscount);
            var GrandTOtalAmount = (GrandTOtalAmount).toFixed(2).toString();
            var Comma1 = GrandTOtalAmount.toString().split(".");
            Comma1[0] = Comma1[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            $("#DisplayGrandTotal").val(Comma1.join("."));

            $("#item_DisplayAmountWithoutVAT", row).val((0).toFixed(2).toString());
            $("#item_DisplayAmountVAT",row).val((0).toFixed(2).toString());
            $("#DisplayGrandTotalWithoutVat").val((0).toFixed(2).toString());
            $("#DisplayGrandTotalVat").val((0).toFixed(2).toString());
        }
        else {
           

            var TotalAmounts = 0;
            // $("#VAT").each(function () {
            $("input[id*='TotalAmount']").each(function () {
                TotalAmounts = TotalAmounts + parseFloat($(this).val().replace(/,/g, ''));
            });
            if (Discount == "") {
                Discount = 0;
            }
            if (Type == "1") {
                $("#item_DisplayAmountWithoutVAT", row).val((0).toFixed(2).toString());
                $("#item_DisplayAmountVAT", row).val((0).toFixed(2).toString());
                $("#DisplayGrandTotalWithoutVat").val((0).toFixed(2).toString());
                $("#DisplayGrandTotalVat").val((0).toFixed(2).toString());

                var UnitWithOutVat1 = 0;
                var UnitWithOutVatTotal1 = 0;
                UnitWithOutVat1 = parseFloat(TotalAmount) / 1.12;
                UnitWithOutVatTotal1 = parseFloat(UnitWithOutVat1);

                var value1 = (UnitWithOutVatTotal1).toFixed(2).toString();
                var Comma3 = value1.toString().split(".");
                Comma3[0] = Comma3[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                //$("input[id*='AmountWithoutVAT']", row).val(Comma3.join("."));
                $("#item_DisplayAmountWithoutVAT", row).val(Comma3.join("."));


                var Vat1 = 0;
                var VatTotal1 = 0;
                Vat1 = parseFloat(value1) * 0.12;
                VatTotal1 = parseFloat(Vat1);

                var Vatvalue1 = (VatTotal1).toFixed(2).toString();
                var Comma3 = Vatvalue1.toString().split(".");
                Comma3[0] = Comma3[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                //$("input[id*='VAT']", row).val(Comma3.join("."));
                $("#item_DisplayAmountVAT", row).val(Comma3.join("."));


                var TotalWithDiscount = parseFloat(TotalAmounts) - parseFloat(Discount);


                var UnitWithOutVat1 = 0;
                var UnitWithOutVatTotal1 = 0;
                UnitWithOutVat1 = parseFloat(TotalWithDiscount) / 1.12;
                UnitWithOutVatTotal1 = parseFloat(UnitWithOutVat1);

                var UnitCostvalue = (UnitWithOutVatTotal1).toFixed(2).toString();
                var Comma = UnitCostvalue.toString().split(".");
                Comma[0] = Comma[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                $("#DisplayGrandTotalWithoutVat").val(Comma.join("."));

                var Vat1 = 0;
                var VatTotal1 = 0;
                Vat1 = parseFloat(UnitCostvalue) * 0.12;
                VatTotal1 = parseFloat(Vat1);

                var Vatvalue = (VatTotal1).toFixed(2).toString();
                var Comma = Vatvalue.toString().split(".");
                Comma[0] = Comma[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                $("#DisplayGrandTotalVat").val(Comma.join("."));

                var TotalWithDiscount = parseFloat(TotalAmounts) - parseFloat(Discount);

                var GrandTOtalAmount = parseFloat(TotalWithDiscount);
                var GrandTOtalAmount = (GrandTOtalAmount).toFixed(2).toString();
                var Comma1 = GrandTOtalAmount.toString().split(".");
                Comma1[0] = Comma1[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                $("#DisplayGrandTotal").val(Comma1.join("."));
            }
            else if (Type == "2") {

                $("#item_DisplayAmountWithoutVAT", row).val((0).toFixed(2).toString());
                $("#item_DisplayAmountVAT", row).val((0).toFixed(2).toString());
                $("#DisplayGrandTotalWithoutVat").val((0).toFixed(2).toString());
                $("#DisplayGrandTotalVat").val((0).toFixed(2).toString());

                var TotalWithDiscount = parseFloat(TotalAmounts);

                var UnitWithOutVat1 = 0;
                var UnitWithOutVatTotal1 = 0;
                UnitWithOutVat1 = parseFloat(TotalWithDiscount);
                UnitWithOutVatTotal1 = parseFloat(UnitWithOutVat1);

                var value1 = (UnitWithOutVatTotal1).toFixed(2).toString();
                var Comma3 = value1.toString().split(".");
                Comma3[0] = Comma3[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                $("input[id*='item_DisplayAmountWithoutVAT']", row).val(Comma3.join("."));
                $("#item_DisplayAmountWithoutVAT", row).val(Comma3.join("."));


                var Vat1 = 0;
                var VatTotal1 = 0;
                Vat1 = parseFloat(value1) * 0.12;
                VatTotal1 = parseFloat(Vat1);

                var Vatvalue1 = (VatTotal1).toFixed(2).toString();
                var Comma3 = Vatvalue1.toString().split(".");
                Comma3[0] = Comma3[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                //$("input[id*='VAT']", row).val(Comma3.join("."));
                $("#item_DisplayAmountVAT", row).val(Comma3.join("."));


                var UnitWithOutVat1 = 0;
                var UnitWithOutVatTotal1 = 0;
                UnitWithOutVat1 = parseFloat(TotalWithDiscount) - parseFloat(Discount);
                UnitWithOutVatTotal1 = parseFloat(UnitWithOutVat1);

                var UnitCostvalue = (UnitWithOutVatTotal1).toFixed(2).toString();
                var Comma = UnitCostvalue.toString().split(".");
                Comma[0] = Comma[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                $("#DisplayGrandTotalWithoutVat").val(Comma.join("."));

                var Vat1 = 0;
                var VatTotal1 = 0;
                Vat1 = parseFloat(UnitCostvalue) * 0.12;
                VatTotal1 = parseFloat(Vat1);

                var Vatvalue = (VatTotal1).toFixed(2).toString();
                var Comma = Vatvalue.toString().split(".");
                Comma[0] = Comma[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                $("#DisplayGrandTotalVat").val(Comma.join("."));

                var TotalWithDiscount = (parseFloat(TotalWithDiscount) - parseFloat(Discount))* 1.12;

                var GrandTOtalAmount = parseFloat(TotalWithDiscount);
                var GrandTOtalAmount = (GrandTOtalAmount).toFixed(2).toString();
                var Comma1 = GrandTOtalAmount.toString().split(".");
                Comma1[0] = Comma1[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                $("#DisplayGrandTotal").val(Comma1.join("."));

            }
        }

    });

}


$("#Discount").on('keyup', function () {
    var Amount = $('#item_DisplayAmount').val();
    var Qty = $('#item_Qty').val();
        var TypeofVAT = $("#TypeOfVat").val();
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
            $("#DisplayGrandTotalWithoutVat").val(Comma.join("."));

            var Vat1 = 0;
            var VatTotal1 = 0;
            Vat1 = parseFloat(UnitCostvalue) * 0.12;
            VatTotal1 = parseFloat(Vat1);

            var Vatvalue = (VatTotal1).toFixed(2).toString();
            var Comma = Vatvalue.toString().split(".");
            Comma[0] = Comma[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            $("#DisplayGrandTotalVat").val(Comma.join("."));

            var TotalWithDiscount = parseFloat(TotalAmounts) - parseFloat(Discount);

            var GrandTOtalAmount = parseFloat(TotalWithDiscount);
            var GrandTOtalAmount = (GrandTOtalAmount).toFixed(2).toString();
            var Comma1 = GrandTOtalAmount.toString().split(".");
            Comma1[0] = Comma1[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            $("#DisplayGrandTotal").val(Comma1.join("."));
        }
        else if (TypeofVAT == "2") {
            var TotalWithDiscount = parseFloat(TotalAmounts);

            var UnitWithOutVat1 = 0;
            var UnitWithOutVatTotal1 = 0;
            UnitWithOutVat1 = parseFloat(TotalWithDiscount) - parseFloat(Discount);
            UnitWithOutVatTotal1 = parseFloat(UnitWithOutVat1);

            var UnitCostvalue = (UnitWithOutVatTotal1).toFixed(2).toString();
            var Comma = UnitCostvalue.toString().split(".");
            Comma[0] = Comma[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            $("#DisplayGrandTotalWithoutVat").val(Comma.join("."));

            var Vat1 = 0;
            var VatTotal1 = 0;
            Vat1 = parseFloat(UnitCostvalue) * 0.12;
            VatTotal1 = parseFloat(Vat1);

            var Vatvalue = (VatTotal1).toFixed(2).toString();
            var Comma = Vatvalue.toString().split(".");
            Comma[0] = Comma[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            $("#DisplayGrandTotalVat").val(Comma.join("."));

            var TotalWithDiscount = (parseFloat(TotalWithDiscount) - parseFloat(Discount)) * 1.12;

            var GrandTOtalAmount = parseFloat(TotalWithDiscount);
            var GrandTOtalAmount = (GrandTOtalAmount).toFixed(2).toString();
            var Comma1 = GrandTOtalAmount.toString().split(".");
            Comma1[0] = Comma1[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            $("#DisplayGrandTotal").val(Comma1.join("."));

        }
        else {

            var TotalWithDiscount = (parseFloat(TotalAmounts) - parseFloat(Discount)) ;

            var GrandTOtalAmount = parseFloat(TotalWithDiscount);
            var GrandTOtalAmount = (GrandTOtalAmount).toFixed(2).toString();
            var Comma1 = GrandTOtalAmount.toString().split(".");
            Comma1[0] = Comma1[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            $("#DisplayGrandTotal").val(Comma1.join("."));
        }
     
    }

})



$("input[id*='item_Qty']").on('keyup', function () {
    var qty = $(this).val().replace(/,/g, '');
    var row = $(this).closest("tr");
        var TypeofVAT = $("#TypeOfVat").val();
    var Discount = $("#Discount").val();
    var price = $("#item_DisplayAmount", row).val().replace(/,/g, '');

    if (qty != "'" && price != "") {
        var subTotal = parseInt(qty) * parseFloat(price);

        if (!isNaN(subTotal)) {

            var i = (subTotal).toFixed(2).toString();
            var Comma2 = i.toString().split(".");
            Comma2[0] = Comma2[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            //$("input[id*='TotalAmount']", row).val(Comma2.join("."));
            $("#item_DisplayTotalAmount", row).val(Comma2.join("."));

            if (TypeofVAT == "1") {

                var UnitWithOutVat1 = 0;
                var UnitWithOutVatTotal1 = 0;
                UnitWithOutVat1 = parseFloat(i) / 1.12;
                UnitWithOutVatTotal1 = parseFloat(UnitWithOutVat1);

                var value1 = (UnitWithOutVatTotal1).toFixed(2).toString();
                var Comma3 = value1.toString().split(".");
                Comma3[0] = Comma3[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                //$("input[id*='AmountWithoutVAT']", row).val(Comma3.join("."));
                $("#item_DisplayAmountWithoutVAT", row).val(Comma3.join("."));


                var Vat1 = 0;
                var VatTotal1 = 0;
                Vat1 = parseFloat(value1) * 0.12;
                VatTotal1 = parseFloat(Vat1);

                var Vatvalue1 = (VatTotal1).toFixed(2).toString();
                var Comma3 = Vatvalue1.toString().split(".");
                Comma3[0] = Comma3[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                //$("input[id*='VAT']", row).val(Comma3.join("."));
                $("#item_DisplayAmountVAT", row).val(Comma3.join("."));
            }

            else if (TypeofVAT == "2" ) {

                var UnitWithOutVat1 = 0;
                var UnitWithOutVatTotal1 = 0;
                UnitWithOutVat1 = parseFloat(i);
                UnitWithOutVatTotal1 = parseFloat(UnitWithOutVat1);

                var value1 = (UnitWithOutVatTotal1).toFixed(2).toString();
                var Comma3 = value1.toString().split(".");
                Comma3[0] = Comma3[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                //$("input[id*='AmountWithoutVAT']", row).val(Comma3.join("."));
                $("#item_DisplayAmountWithoutVAT", row).val(Comma3.join("."));


                var Vat1 = 0;
                var VatTotal1 = 0;
                Vat1 = parseFloat(value1) * 0.12;
                VatTotal1 = parseFloat(Vat1);

                var Vatvalue1 = (VatTotal1).toFixed(2).toString();
                var Comma3 = Vatvalue1.toString().split(".");
                Comma3[0] = Comma3[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                //$("input[id*='VAT']", row).val(Comma3.join("."));
                $("#item_DisplayAmountVAT", row).val(Comma3.join("."));
            }

        }
        else {

            $("input[id*='item_DisplayGrandTotal']", row).val((0).toFixed(2).toString());
            $("input[id*='item_DisplayTotalAmount']", row).val((0).toFixed(2).toString());
            $("input[id*='item_DisplayAmountWithoutVAT']", row).val((0).toFixed(2).toString());
            $("input[id*='item_DisplayAmountVAT']", row).val((0).toFixed(2).toString());
            $("input[id*='item_DisplayGrandTotalWithoutVat']", row).val((0).toFixed(2).toString());
            $("input[id*='item_DisplayGrandTotalVat']", row).val((0).toFixed(2).toString());
        }
    }

    else {

        $("input[id*='item_DisplayGrandTotal']", row).val((0).toFixed(2).toString());
        $("input[id*='item_DisplayTotalAmount']", row).val((0).toFixed(2).toString());
        $("input[id*='item_DisplayAmountWithoutVAT']", row).val((0).toFixed(2).toString());
        $("input[id*='item_DisplayAmountVAT']", row).val((0).toFixed(2).toString());
        $("input[id*='item_DisplayGrandTotalWithoutVat']", row).val((0).toFixed(2).toString());
        $("input[id*='item_DisplayGrandTotalVat']", row).val((0).toFixed(2).toString());
    }




    // get total Amount Without VAT

   

    var TotalAmounts = 0;
    // $("#VAT").each(function () {
    $("input[id*='item_DisplayTotalAmount']").each(function () {
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
        $("#DisplayGrandTotalWithoutVat").val(Comma.join("."));

        var Vat1 = 0;
        var VatTotal1 = 0;
        Vat1 = parseFloat(UnitCostvalue) * 0.12;
        VatTotal1 = parseFloat(Vat1);

        var Vatvalue = (VatTotal1).toFixed(2).toString();
        var Comma = Vatvalue.toString().split(".");
        Comma[0] = Comma[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        $("#DisplayGrandTotalVat").val(Comma.join("."));

        var TotalWithDiscount = parseFloat(TotalAmounts) - parseFloat(Discount);

        var GrandTOtalAmount = parseFloat(TotalWithDiscount);
        var GrandTOtalAmount = (GrandTOtalAmount).toFixed(2).toString();
        var Comma1 = GrandTOtalAmount.toString().split(".");
        Comma1[0] = Comma1[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        $("#DisplayGrandTotal").val(Comma1.join("."));
    }
    else if (TypeofVAT == "2") {
        var TotalWithDiscount = parseFloat(TotalAmounts);

        var UnitWithOutVat1 = 0;
        var UnitWithOutVatTotal1 = 0;
        UnitWithOutVat1 = parseFloat(TotalWithDiscount) - parseFloat(Discount);
        UnitWithOutVatTotal1 = parseFloat(UnitWithOutVat1);

        var UnitCostvalue = (UnitWithOutVatTotal1).toFixed(2).toString();
        var Comma = UnitCostvalue.toString().split(".");
        Comma[0] = Comma[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        $("#DisplayGrandTotalWithoutVat").val(Comma.join("."));

        var Vat1 = 0;
        var VatTotal1 = 0;
        Vat1 = parseFloat(UnitCostvalue) * 0.12;
        VatTotal1 = parseFloat(Vat1);

        var Vatvalue = (VatTotal1).toFixed(2).toString();
        var Comma = Vatvalue.toString().split(".");
        Comma[0] = Comma[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        $("#DisplayGrandTotalVat").val(Comma.join("."));

        var TotalWithDiscount = (parseFloat(TotalWithDiscount) - parseFloat(Discount)) * 1.12;

        var GrandTOtalAmount = parseFloat(TotalWithDiscount);
        var GrandTOtalAmount = (GrandTOtalAmount).toFixed(2).toString();
        var Comma1 = GrandTOtalAmount.toString().split(".");
        Comma1[0] = Comma1[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        $("#DisplayGrandTotal").val(Comma1.join("."));

    }
   
    else {

        var TotalWithDiscount = (parseFloat(TotalAmounts) - parseFloat(Discount));

        var GrandTOtalAmount = parseFloat(TotalWithDiscount);
        var GrandTOtalAmount = (GrandTOtalAmount).toFixed(2).toString();
        var Comma1 = GrandTOtalAmount.toString().split(".");
        Comma1[0] = Comma1[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        $("#DisplayGrandTotal").val(Comma1.join("."));
    }
});

$("input[id*='item_DisplayAmount']").on('keyup', function () {
    var price = $(this).val().replace(/,/g, '');
    var row = $(this).closest("tr");
        var TypeofVAT = $("#TypeOfVat").val();
    var Discount = $("#Discount").val();
    var qty = $("input[id*='item_Qty']", row).val().replace(/,/g, '');

    if (qty != "'" && price != "") {
        var subTotal = parseInt(qty) * parseFloat(price);

        if (!isNaN(subTotal)) {

            var i = (subTotal).toFixed(2).toString();
            var Comma2 = i.toString().split(".");
            Comma2[0] = Comma2[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            //$("input[id*='TotalAmount']", row).val(Comma2.join("."));
            $("#item_DisplayTotalAmount", row).val(Comma2.join("."));
            if (TypeofVAT == "1") {

                var UnitWithOutVat1 = 0;
                var UnitWithOutVatTotal1 = 0;
                UnitWithOutVat1 = parseFloat(i) / 1.12;
                UnitWithOutVatTotal1 = parseFloat(UnitWithOutVat1);

                var value1 = (UnitWithOutVatTotal1).toFixed(2).toString();
                var Comma3 = value1.toString().split(".");
                Comma3[0] = Comma3[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                //$("input[id*='AmountWithoutVAT']", row).val(Comma3.join("."));
                $("#item_DisplayAmountWithoutVAT", row).val(Comma3.join("."));


                var Vat1 = 0;
                var VatTotal1 = 0;
                Vat1 = parseFloat(value1) * 0.12;
                VatTotal1 = parseFloat(Vat1);

                var Vatvalue1 = (VatTotal1).toFixed(2).toString();
                var Comma3 = Vatvalue1.toString().split(".");
                Comma3[0] = Comma3[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                //$("input[id*='VAT']", row).val(Comma3.join("."));
                $("#item_DisplayAmountVAT", row).val(Comma3.join("."));
            }

            else if (TypeofVAT == "2" ) {

                var UnitWithOutVat1 = 0;
                var UnitWithOutVatTotal1 = 0;
                UnitWithOutVat1 = parseFloat(i);
                UnitWithOutVatTotal1 = parseFloat(UnitWithOutVat1);

                var value1 = (UnitWithOutVatTotal1).toFixed(2).toString();
                var Comma3 = value1.toString().split(".");
                Comma3[0] = Comma3[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                //$("input[id*='AmountWithoutVAT']", row).val(Comma3.join("."));
                $("#item_DisplayAmountWithoutVAT", row).val(Comma3.join("."));


                var Vat1 = 0;
                var VatTotal1 = 0;
                Vat1 = parseFloat(value1) * 0.12;
                VatTotal1 = parseFloat(Vat1);

                var Vatvalue1 = (VatTotal1).toFixed(2).toString();
                var Comma3 = Vatvalue1.toString().split(".");
                Comma3[0] = Comma3[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                //$("input[id*='VAT']", row).val(Comma3.join("."));
                $("#item_DisplayAmountVAT", row).val(Comma3.join("."));
            }
        }
        else {


            $("input[id*='item_DisplayGrandTotal']", row).val((0).toFixed(2).toString());
            $("input[id*='item_DisplayTotalAmount']", row).val((0).toFixed(2).toString());
            $("input[id*='item_DisplayAmountWithoutVAT']", row).val((0).toFixed(2).toString());
            $("input[id*='item_DisplayAmountVAT']", row).val((0).toFixed(2).toString());
            $("input[id*='item_DisplayGrandTotalWithoutVat']", row).val((0).toFixed(2).toString());
            $("input[id*='item_DisplayGrandTotalVat']", row).val((0).toFixed(2).toString());
        }
    }

    else {


        $("input[id*='item_DisplayGrandTotal']", row).val((0).toFixed(2).toString());
        $("input[id*='item_DisplayTotalAmount']", row).val((0).toFixed(2).toString());
        $("input[id*='item_DisplayAmountWithoutVAT']", row).val((0).toFixed(2).toString());
        $("input[id*='item_DisplayAmountVAT']", row).val((0).toFixed(2).toString());
        $("input[id*='item_DisplayGrandTotalWithoutVat']", row).val((0).toFixed(2).toString());
        $("input[id*='item_DisplayGrandTotalVat']", row).val((0).toFixed(2).toString());
    }

    // get total Amount Without VAT


    var TotalAmounts = 0;
    // $("#VAT").each(function () {
    $("input[id*='item_DisplayTotalAmount']").each(function () {
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
        $("#DisplayGrandTotalWithoutVat").val(Comma.join("."));

        var Vat1 = 0;
        var VatTotal1 = 0;
        Vat1 = parseFloat(UnitCostvalue) * 0.12;
        VatTotal1 = parseFloat(Vat1);

        var Vatvalue = (VatTotal1).toFixed(2).toString();
        var Comma = Vatvalue.toString().split(".");
        Comma[0] = Comma[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        $("#DisplayGrandTotalVat").val(Comma.join("."));

        var TotalWithDiscount = parseFloat(TotalAmounts) - parseFloat(Discount);

        var GrandTOtalAmount = parseFloat(TotalWithDiscount);
        var GrandTOtalAmount = (GrandTOtalAmount).toFixed(2).toString();
        var Comma1 = GrandTOtalAmount.toString().split(".");
        Comma1[0] = Comma1[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        $("#DisplayGrandTotal").val(Comma1.join("."));
    }
    else if (TypeofVAT == "2") {
        var TotalWithDiscount = parseFloat(TotalAmounts);

        var UnitWithOutVat1 = 0;
        var UnitWithOutVatTotal1 = 0;
        UnitWithOutVat1 = parseFloat(TotalWithDiscount) - parseFloat(Discount);
        UnitWithOutVatTotal1 = parseFloat(UnitWithOutVat1);

        var UnitCostvalue = (UnitWithOutVatTotal1).toFixed(2).toString();
        var Comma = UnitCostvalue.toString().split(".");
        Comma[0] = Comma[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        $("#DisplayGrandTotalWithoutVat").val(Comma.join("."));

        var Vat1 = 0;
        var VatTotal1 = 0;
        Vat1 = parseFloat(UnitCostvalue) * 0.12;
        VatTotal1 = parseFloat(Vat1);

        var Vatvalue = (VatTotal1).toFixed(2).toString();
        var Comma = Vatvalue.toString().split(".");
        Comma[0] = Comma[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        $("#DisplayGrandTotalVat").val(Comma.join("."));

        var TotalWithDiscount = (parseFloat(TotalWithDiscount) - parseFloat(Discount)) * 1.12;

        var GrandTOtalAmount = parseFloat(TotalWithDiscount);
        var GrandTOtalAmount = (GrandTOtalAmount).toFixed(2).toString();
        var Comma1 = GrandTOtalAmount.toString().split(".");
        Comma1[0] = Comma1[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        $("#DisplayGrandTotal").val(Comma1.join("."));

    }
    else {

        var TotalWithDiscount = (parseFloat(TotalAmounts) - parseFloat(Discount));

        var GrandTOtalAmount = parseFloat(TotalWithDiscount);
        var GrandTOtalAmount = (GrandTOtalAmount).toFixed(2).toString();
        var Comma1 = GrandTOtalAmount.toString().split(".");
        Comma1[0] = Comma1[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        $("#DisplayGrandTotal").val(Comma1.join("."));
    }

});

function DoPreview() {
    $("#HiddenPreview").val("PREVIEW");
}
function DoSave() {
    $("#HiddenPreview").val("SAVED");
}

$(document).ready(function () {
    $("#btnSaveSupplier").prop('disabled', true);

    //add button click event

    $("body").on("click", "#add", function () {

        //clear select data
        var Particular = $('#item_ParticularName').val();
        var ItemMeasure = $('#item_ItemMeasure').val();
        var Amount = $('#item_DisplayAmount').val();
        var Qty = $('#item_Qty').val();
        var TotalAmount = $('#item_DisplayTotalAmount').val();
        var AmountVAT = $('#item_DisplayAmountVAT').val();
        var AmountWithoutVAT = $('#item_DisplayAmountWithoutVAT').val();

        if (Particular == "" || ItemMeasure == "" || Qty == "" || Amount == "" ) {
            sweetAlert("Item row can't be empty.", "", "error");
            //alert("File Shouldn't Be Empty!!");
            return false;
        }
        $('select#item_ParticularName').select2('destroy');

        var $newRow = $('#mainrow').clone(true).appendTo('#TableSupplierItemList');
        $('.pc', $newRow).val($('#item_ParticularName').val());

        //remove id attribute from new clone row
        $('#item_itemDescription,#item_ParticularName,#item_ItemMeasure,#item_Qty,#item_DisplayAmountWithoutVAT,#item_DisplayAmountVAT,#item_DisplayAmount,#item_DisplayTotalAmount', $newRow);


        //clear select data
        $('#item_ParticularName').val('');
        $('#item_ItemMeasure').val('');
        $('#item_DisplayAmount').val('');
        $('#item_itemDescription').val('');
        $('#item_DisplayTotalAmount').val('0.00');
        $('#Item_DisplayAmountVAT').val('0.00');
        $('#item_DisplayAmountWithoutVAT').val('0.00');
        $('#item_Qty').val('');
        $('select#item_ParticularName').select2();
        $("#item_ParticularName").prop('disabled', false);

    })

    //remove button click event
    $('#TableSupplierItemList').on('click', '#Remove', function () {

        var trIndex = $(this).closest("tr").index();
        var Discount = $("#Discount").val();
        var TypeofVAT = $("#TypeOfVat").val();

        var count = $('#TableSupplierItemList tbody tr').length;
        if (count > 1) {
            $(this).closest("tr").remove();

            var TotalAmounts = 0;
            $("input[id*='item_DisplayTotalAmount']").each(function () {
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
                $("#DisplayGrandTotalWithoutVat").val(Comma.join("."));

                var Vat1 = 0;
                var VatTotal1 = 0;
                Vat1 = parseFloat(UnitCostvalue) * 0.12;
                VatTotal1 = parseFloat(Vat1);

                var Vatvalue = (VatTotal1).toFixed(2).toString();
                var Comma = Vatvalue.toString().split(".");
                Comma[0] = Comma[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                $("#DisplayGrandTotalVat").val(Comma.join("."));

                var TotalWithDiscount = parseFloat(TotalAmounts) - parseFloat(Discount);

                var GrandTOtalAmount = parseFloat(TotalWithDiscount);
                var GrandTOtalAmount = (GrandTOtalAmount).toFixed(2).toString();
                var Comma1 = GrandTOtalAmount.toString().split(".");
                Comma1[0] = Comma1[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                $("#DisplayGrandTotal").val(Comma1.join("."));
            }
            else if (TypeofVAT == "2") {
                var TotalWithDiscount = parseFloat(TotalAmounts);

                var UnitWithOutVat1 = 0;
                var UnitWithOutVatTotal1 = 0;
                UnitWithOutVat1 = parseFloat(TotalWithDiscount) - parseFloat(Discount);
                UnitWithOutVatTotal1 = parseFloat(UnitWithOutVat1);

                var UnitCostvalue = (UnitWithOutVatTotal1).toFixed(2).toString();
                var Comma = UnitCostvalue.toString().split(".");
                Comma[0] = Comma[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                $("#DisplayGrandTotalWithoutVat").val(Comma.join("."));

                var Vat1 = 0;
                var VatTotal1 = 0;
                Vat1 = parseFloat(UnitCostvalue) * 0.12;
                VatTotal1 = parseFloat(Vat1);

                var Vatvalue = (VatTotal1).toFixed(2).toString();
                var Comma = Vatvalue.toString().split(".");
                Comma[0] = Comma[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                $("#DisplayGrandTotalVat").val(Comma.join("."));

                var TotalWithDiscount = (parseFloat(TotalWithDiscount) - parseFloat(Discount)) * 1.12;

                var GrandTOtalAmount = parseFloat(TotalWithDiscount);
                var GrandTOtalAmount = (GrandTOtalAmount).toFixed(2).toString();
                var Comma1 = GrandTOtalAmount.toString().split(".");
                Comma1[0] = Comma1[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                $("#DisplayGrandTotal").val(Comma1.join("."));

            }
            else {

                var TotalWithDiscount = (parseFloat(TotalAmounts) - parseFloat(Discount));

                var GrandTOtalAmount = parseFloat(TotalWithDiscount);
                var GrandTOtalAmount = (GrandTOtalAmount).toFixed(2).toString();
                var Comma1 = GrandTOtalAmount.toString().split(".");
                Comma1[0] = Comma1[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                $("#DisplayGrandTotal").val(Comma1.join("."));
            }



        } else {
            sweetAlert("Item row can't be empty.", "", "error");
            //alert("File Shouldn't Be Empty!!");
            return false;
        }
    });


    $("#SupplierValidation").submit(function (event) {

        //Loop through the Table rows and build a JSON array.
        var validate = false;
        var SupplierDetails = new Array();
        $("#TableSupplierItemList TBODY TR").each(function () {

            var row = $(this);
            var vParticular  = $("#item_ParticularName", row).val()
            var vQty  = $("input[id*='item_Qty']", row).val();
            var vAmount  = $("input[id*='item_DisplayAmount']", row).val();

            if(vParticular == "" || vQty == "" || vAmount == "" )
            {
                validate = true;
            }

            var customer = {};

            customer.ParticularName = $("#item_ParticularName", row).val();
            customer.itemDescription = $("#item_itemDescription", row).val();
            customer.ItemMeasure = $("#item_ItemMeasure", row).val();
            customer.Qty = $("input[id*='item_Qty']", row).val();
            customer.Amount = $("#item_DisplayAmount", row).val();
            customer.TotalAmount = $("input[id*='item_DisplayTotalAmount']", row).val();
            customer.AmountVAT = $("input[id*='item_DisplayAmountVAT']", row).val();
            customer.AmountWithoutVAT = $("input[id*='item_DisplayAmountWithoutVAT']", row).val();
            SupplierDetails.push(customer);

        });


        var SupplierID = $("#Supplier").val();
        var ApproverID = $("#ApproverID").val();
        var EndorserID = $("#EndorserID").val();
        var Preview = $("#HiddenPreview").val();
        var ListClientReferenceNo = $("#ListClientReferenceNo").val();
        var ProjectName = $("#ProjectName").val();
        var SupplierAddress = $("#SupplierAddresss").val();
        var DeliveryAddress = $("#DeliveryAddress").val();
        var LocationAddressID = $("#LocationAddressID").val();
        var TermsofPayment = $("#TermsofPayment").val();
        var Shippinginstruction = $("#Shippinginstruction").val();
        var GrandTotalWithoutVat = $("#DisplayGrandTotalWithoutVat").val();
        var GrandTotalVat = $("#DisplayGrandTotalVat").val();
        var GrandTotal = $("#DisplayGrandTotal").val();
        var POSupplierNumber = $("#POSupplierNumber").val();
        var POSupplierDate = $("#PODate").val();
        var Discount = $("#Discount").val();
        var Old_ReferenceNo = $("#POSReferenceNo").val();
        var TypeOfVat = $("#TypeOfVat").val();
        var PRNumber = $("#PRNumber").val();
        

            
        if (ApproverID == EndorserID) {
            sweetAlert("Invalid Endorser.", "The Endroser and Approver should not be the same.", "error");
            return false;
        }

        else if (POSupplierDate == "" || Shippinginstruction == "" ||
            LocationAddressID == "" || TermsofPayment == "" ||
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
                SupplierDetails,
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
                Old_ReferenceNo: Old_ReferenceNo,
                LocationAddressID: LocationAddressID,
                TypeOfVat: TypeOfVat,
                PRNumber: PRNumber,
            }),
            dataType: "json",
            success: function (data) {
                if (data == 'Information') {
                    sweetAlert("Fill out required field.", "", "error");
                    return false;
                }
                else if (data == 'ItemGrid') {
                    sweetAlert("Fill out empty field.", "", "error");
                    return false;
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
                else 
                {
                    swal
                        ({
                             title: "Success",
                            text: data + " Item/s Successfully Updated; and PO to SUPPLIER Successfully Updated.",
                            type: "success", allowOutsideClick: false
                        }).then(function () {
                            // Redirect the user
                            window.location.href = '/Report/ReportSupplier/';
                        });

                }
            }
        });



    }); //end .submit()

})

            //LoadItemCategory($('#Particular'));