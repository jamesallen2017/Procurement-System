/*
 *  Document   : base_forms_validation.js
 *  Author     : pixelcave
 *  Description: Custom JS code used in Form Validation Page
 */

var BaseFormValidation = function() {
    // Init Bootstrap Forms Validation, for more examples you can check out https://github.com/jzaefferer/jquery-validation
    var initValidationBootstrap = function(){
        jQuery('.js-validation-bootstrap').validate({
            ignore: [],
            errorClass: 'help-block animated fadeInDown',
            errorElement: 'div',
            errorPlacement: function(error, e) {
                jQuery(e).parents('.form-group > div').append(error);
            },
            highlight: function(e) {
                var elem = jQuery(e);

                elem.closest('.form-group').removeClass('has-error').addClass('has-error');
                elem.closest('.help-block').remove();
            },
            success: function(e) {
                var elem = jQuery(e);

                elem.closest('.form-group').removeClass('has-error');
                elem.closest('.help-block').remove();
            },
            rules: {
                'val-username': {
                    required: true,
                    minlength: 3
                },
                'val-email': {
                    required: true,
                    email: true
                },
                'val-password': {
                    required: true,
                    minlength: 5
                },
                'val-confirm-password': {
                    required: true,
                    equalTo: '#val-password'
                },
                'val-Select2': {
                    required: true
                },
                'val-Select2-multiple': {
                    required: true,
                    minlength: 2
                },
                'val-suggestions': {
                    required: true,
                    minlength: 5
                },
                'val-skill': {
                    required: true
                },
                'val-currency': {
                    required: true,
                    currency: ['$', true]
                },
                'val-website': {
                    required: true,
                    url: true
                },
                'val-phoneus': {
                    required: true,
                    phoneUS: true
                },
                'val-digits': {
                    required: true,
                    digits: true
                },
                'val-number': {
                    required: true,
                    number: true
                },
                'val-range': {
                    required: true,
                    range: [1, 5]
                },
                'val-terms': {
                    required: true
                }
            },
            messages: {
                'val-username': {
                    required: 'Enter a username',
                    minlength: 'Your username must consist of at least 3 characters'
                },
                'val-email': 'Enter a valid email address',
                'val-password': {
                    required: 'provide a password',
                    minlength: 'Your password must be at least 5 characters long'
                },
                'val-confirm-password': {
                    required: 'provide a password',
                    minlength: 'Your password must be at least 5 characters long',
                    equalTo: 'Enter the same password as above'
                },
                'val-Select2': 'Select a value!',
                'val-Select2-multiple': 'Select at least 2 values!',
                'val-suggestions': 'What can we do to become better?',
                'val-skill': 'Select a skill!',
                'val-currency': 'Enter a price!',
                'val-website': 'Enter your website!',
                'val-phoneus': 'Enter a US phone!',
                'val-digits': 'Enter only digits!',
                'val-number': 'Enter a number!',
                'val-range': 'Enter a number between 1 and 5!',
                'val-terms': 'You must agree to the service terms!'
            }
        });
    };

    // Init Material Forms Validation, for more examples you can check out https://github.com/jzaefferer/jquery-validation
    var initValidationMaterial = function(){
        jQuery('.js-validation-material').validate({
            ignore: [],
            errorClass: 'help-block text-right animated fadeInDown',
            errorElement: 'div',
            errorPlacement: function(error, e) {
                jQuery(e).parents('.form-group > div').append(error);
            },
            highlight: function(e) {
                var elem = jQuery(e);

                elem.closest('.form-group').removeClass('has-error').addClass('has-error');
                elem.closest('.help-block').remove();
            },
            success: function(e) {
                var elem = jQuery(e);

                elem.closest('.form-group').removeClass('has-error');
                elem.closest('.help-block').remove();
            },
            rules: {
                'FirstName': {
                    required: true,
                    minlength: 3,

                },
                'LastName': {
                    required: true,
                    minlength: 3,

                },
                'UserName': {
                    required: true,
                    minlength: 5,

                },
                'EmployeeID': {
                    required: true,

                },
                'Location': {
                    required: true,

                },
                'Position': {
                    required: true,

                },
                'ApproverID': {
                    required: true,
                },
                'DepartmentID': {
                    required: true
                },
                'UserGroupID': {
                    required: true,
                },
                'UserRoleID': {
                    required: true
                },
                'Client': {
                    required: true,
                },
                'ClientAddress': {
                    required: true,
                },
               
                'ContactPerson': {
                    required: true,

                },
                'SupplierAddress': {
                    required: true,
                },
                'Supplier': {
                    required: true,
                },
                'ClientNameList': {
                    required: true
                },
                'ProjectName': {
                    required: true,

                },
                'TermsofPayment': {
                    required: true
                },
                'SupplierName': {
                    required: true,

                },
                'Shippinginstruction': {
                    required: true,
                },
                'Approver': {
                    required: true
                },
                'POSupplierDate': {
                    required: true
                },
                'SignProposal':
                {
                    required: true,

                },
                'ItemName':
                {
                    required: true,
                },
                'ItemDescription':
                {
                    required: true,

                },
                'ItemMeasure':
                {
                    required: true,
                },
                'ClientStatus':
                {
                    required: true,
                },
                'Endorser':
                {
                    required: true,
                },
                'ListLocationAddress':
                {
                    required: true,
                },
                'ClientID':
               {
                   required: true,
                },
                'DeliveryNo':
                {
                    required: true,
                },
                'DeliveryDate':
                {
                    required: true,
                },
                'ACC_Particular':
                {
                    required: true,
                },
                'ACC_CheckDate':
                {
                    required: true,
                },
                'ACC_ATICVNO':
                {
                    required: true,
                },
                'ACC_AmountPaid':
                {
                    required: true,
                },
                'ACC_InvoiceNo':
                {
                    required: true,
                },
                'ACC_BillNo':
                {
                    required: true,
                },
                'ACC_BillDate':
                {
                    required: true,
                },
                'ACC_BillAmount':
                {
                    required: true,
                },
                'ACC_Remarks':
                {
                    required: true,
                },
                'Particular':
                {
                    required: true,
                },
                'Qty':
                {
                    required: true,
                },
                'Amount':
                {
                    required: true,
                },
                'ProjectDetails':
                {
                    required: true,
                },
                'CompletionDate':
                {
                    required: true,
                },
                'StartDate':
                {
                    required: true,
                },
                'POReferenceNo':
                {
                    required: true,
                },
                'BillingType':
                {
                    required: true,
                },
                'TypeOfNumber':
                {
                    required: true,
                },
                'TypeOfVat':
                {
                    required: true,
                },
                'ItemID':
                {
                    required: true,
                },
                'DisplayAmount':
                {
                    required: true,
                },
                'ClientLocation':
                {
                    required: true,
                },
                'DRReasonRemarks':
                {
                    required: true,
                },
                'ParticularName':
                {
                    required: true,
                },
                'DeliveryAddress':
                {
                    required: true,
                },

                'ClientName':
                {
                    required: true,
                },
                'ProjectDescription':
                {
                    required: true,
                },
                'CanvessedBy':
                {
                    required: true,
                },
                'DateNeeded':
                {
                    required: true,
                },
                'EndorserID':
                {
                    required: true,
                },
                
                
                

            },
            messages: {
                'FirstName': {
                    required: 'Enter first name',
                    minlength: 'Your first name must consist of at least 3 characters'
                },
                'LastName': 'Enter last name',

                'UserName': {
                    required: 'Enter username',
                    minlength: 'Your username must be at least 5 characters long'
                },
                'EmployeeID': {
                    required: 'Enter a valid employee ID',
                },
                'Location': {
                    required: 'Enter a valid location',
                },
                'SignProposal': {
                    required:  function(element) {
                        if ($("#PONumber").val().length <= 0) {
                            return "Enter a valid quotation number ";
                        
                        } else {
                            return false;
                        }
                    }
                },
                'ApproverID': {
                    required: 'Choose your approver'
                },
                
                'EndorserID': 'Select Endorser.',
                'DateNeeded': 'Select Date Needed.',
                'CanvessedBy': 'Select Canvassed by.',
                'ProjectDescription': 'Enter Project Description.',
                'ClientName': 'Enter Client Name.',
                'ClientLocation': 'Select your client location.',
                'DisplayAmount': 'Enter Amount.',
                'ItemID': 'Enter an Item name.',
                'TypeOfVat': 'Select your type of Vat.',
                'Endorser': 'Choose your endorser.',
                'BillingType': 'Choose Billing Type.',
                'TypeOfNumber': 'Choose Billing Type.',
                'ListLocationAddress': 'Select location',
                'ItemMeasure': 'Select unit of measure.',
                'ClientStatus': 'Select status.',
                'ItemDescription': 'Enter a item description.',
                'ItemName': 'Enter an Item name.',
                'DepartmentID': 'Select department.',
                'UserGroupID': 'Select user group.',
                'UserRoleID': 'Select user role.',
                'Client': 'Enter a valid company name.',
                'ClientAddress': 'Enter a valid company address.',
                'ContactPerson': 'Enter a valid contact person.',
                'Supplier': 'Enter a valid Company Name.',
                'SupplierAddress': 'Enter a valid supplier address.',
                'ClientNameList': 'Select client.',
                'ClientID': 'Select client!',
                'ProjectName': 'Enter a valid project name.',
                'POSupplierNumber': 'Enter a valid PO number.',
                'POSupplierDate': 'Enter a valid PO date',
                'TermsofPayment': 'Enter a Terms of paymen.t',
                'DeliveryNo': 'Enter valid delivery receipt no.',
                'DeliveryDate': 'Select date received.',
                'ACC_Particular': 'Enter a valid particular.',
                'ACC_CheckDate': 'Select check date.',
                'ACC_ATICVNO': 'Enter a valid ATI CV No.',
                'ACC_AmountPaid': 'Enter amount paid.',
                'ACC_InvoiceNo': 'Enter invoice No.',
                'ACC_BillNo': 'Enter a valid bill No.',
                'ACC_BillDate': 'Select bill date.',
                'ACC_BillAmount': 'Enter bill amount.',
                'ACC_Remarks': 'Enter a valid Reason / Remark.',
                'Particular': 'Select item.',
                'Qty': 'Enter quantity.',
                'Amount': 'Enter Amount.',
                'ProjectDetails': 'Enter a valid project detail.',
                'CompletionDate': 'Select completion date.',
                'StartDate': 'Select start date.',
                'POReferenceNo': 'Enter a valid PO Reference.',
                'DRReasonRemarks': 'Enter a reason.',
                'ParticularName': 'Enter an Item name.',
                'DeliveryAddress': 'Enter a Delivery Address.',
                
                
            }
        });
    };

    return {
        init: function () {
            // Init Bootstrap Forms Validation
            initValidationBootstrap();

            // Init Material Forms Validation
            initValidationMaterial();

            // Init Validation on Select2 change
            jQuery('.js-Select2').on('change', function(){
                jQuery(this).valid();
            });
        }
    };
}();

// Initialize when page loads
jQuery(function(){ BaseFormValidation.init(); });
