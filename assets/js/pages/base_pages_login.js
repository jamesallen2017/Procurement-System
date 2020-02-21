/*
 *  Document   : base_pages_login.js
 *  Author     : pixelcave
 *  Description: Custom JS code used in Login Page
 */

var BasePagesLogin = function() {
    // Init Login Form Validation, for more examples you can check out https://github.com/jzaefferer/jquery-validation
    var initValidationLogin = function(){
        jQuery('.js-validation-login').validate({
            errorClass: 'help-block text-right animated fadeInDown',
            errorElement: 'div',
            errorPlacement: function(error, e) {
                jQuery(e).parents('.form-group > div').append(error);
            },
            highlight: function(e) {
                jQuery(e).closest('.form-group').removeClass('has-error').addClass('has-error');
                jQuery(e).closest('.help-block').remove();
            },
            success: function(e) {
                jQuery(e).closest('.form-group').removeClass('has-error');
                jQuery(e).closest('.help-block').remove();
            },
            rules: {
                'UserName': {
                    required: true,
                    minlength: 3
                },
                'Password': {
                    required: true,
                    minlength: 5
                },
                'NewPassword': {
                    required: true,
                    minlength: 5
                },
                'ConfirmationPassword': {
                    required: true,
                    minlength: 5,
                    equalTo: '#NewPassword'
                },
                'ServerName': {
                    required: true,
                },
                'ServerUsername': {
                    required: true,
                },
                'ServerPassword': {
                    required: true,
                },
                'AdminUsername': {
                    required: true,
                },
                'AdminPassword': {
                    required: true,
                },
            },
            messages: {
                'ServerName': {
                    required: 'Enter Server Name.',
                },
                'ServerUsername': {
                    required: 'Enter Server Username.',
                },
                'ServerPassword': {
                    required: 'Enter Server password.',
                },
                'AdminUsername': {
                    required: 'Enter Admin Username.',
                },
                'AdminPassword': {
                    required: 'Enter Admin Password.',
                },
                'UserName': {
                    required: 'Enter a username.',
                    minlength: 'Your username must consist of at least 3 characters.'
                },
                'Password': {
                    required: 'Enter a password.',
                    minlength: 'Your password must be at least 5 characters long.'
                },
                'NewPassword': {
                    required: 'Enter a new password.',
                    minlength: 'Your password must be at least 5 characters long.'
                },
                'ConfirmationPassword': {
                    required: 'Enter confirm new password.',
                    minlength: 'Your password must be at least 5 characters long.',
                    equalTo: 'Your New password and confirm new password do not match, Please try again'
                }
            }
        });
    };

    return {
        init: function () {
            // Init Login Form Validation
            initValidationLogin();
        }
    };
}();

// Initialize when page loads
jQuery(function(){ BasePagesLogin.init(); });