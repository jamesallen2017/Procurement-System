$(document).ready(function () {

    $("#SMTPForm").submit(function (event) {


        var SMTP_To = $("#SMTP_To").val();
        var SMTP_Subject = $("#SMTP_Subject").val();
        var SMTP_Body = $("#SMTP_Body").val();



        var url = $("#SMTPForm").attr("action");
        event.preventDefault();
        event.stopImmediatePropagation();

        $.ajax({

            url: url,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({
                SMTP_To: SMTP_To,
                SMTP_Subject: SMTP_Subject,
                SMTP_Body: SMTP_Body,
            }),
            dataType: "json",
            success: function (data) {
                if (data != 'success') {

                    return false;
                }
                else {
                    swal
                        ({
                            title: "Congratulation!",
                            text: "Email Successfully sent.",
                            type: "success", allowOutsideClick: false
                        }).then(function () {
                            // Redirect the user
                            location.reload();
                        });

                }
            }

        });

    });
});