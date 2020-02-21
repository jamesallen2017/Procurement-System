

function DoPreview() {
    $("#HiddenPreview").val("PREVIEW");
}
function DoSave() {
    $("#HiddenPreview").val("SAVED");
}


$(document).ready(function () {
    $("#btnSaveCoC").prop('disabled', true);

    $('#Approver option').each(function () {
        if ($(this).val() == '68') {
            $(this).prop("selected", true);
        }
    });

    $("#CoCValidation").submit(function () {

        var HiddenPreview = $("#HiddenPreview").val();
        var ClientNameList = $("#ClientID").val();
        var StartDate = $("#StartDate").val();
        var ProjectName = $("#ProjectName").val();
        var CompletionDate = $("#CompletionDate").val();
        var ProjectDetails = $("#ProjectDetails").val();
        var POReferenceNo = $("#POReferenceNo").val();
        var Approver = $("#Approver").val();
        var FullNameResponsible = $("#FullNameResponsible").val();
        var COC_Resposible = $("#COC_Resposible").val();
        var POC_ReferenceNo = $("#POC_ReferenceNo").val();
        var ClientAddress = $("#ClientAddress").val();
        var ClientLocation = $("#ClientLocation").val();
        
        


        if (ClientNameList == "" || StartDate == "" || ProjectName == "" || CompletionDate == "" || ProjectDetails == "" ) {
            return false;
        }

        if (HiddenPreview == "SAVED") {
            var $this = $("#btnSaveCoC");
            $this.button('loading');
            setTimeout(function () {
                $this.button('reset');
            }, 10000);
        }
     

        var action = $("#CoCValidation").attr("action");
        event.preventDefault();
        event.stopImmediatePropagation();

        $.ajax({
            url: action,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({
                ClientID: ClientNameList,
                StartDate: StartDate,
                ProjectName: ProjectName,
                CompletionDate: CompletionDate,
                ProjectDetails: ProjectDetails,
                POReferenceNo: POReferenceNo,
                ApproverID: Approver,
                FullNameResponsible: FullNameResponsible,
                HiddenPreview: HiddenPreview,
                COC_Resposible: COC_Resposible,
                POC_ReferenceNo: POC_ReferenceNo,
                ClientAddress: ClientAddress,
                ClientLocation: ClientLocation,
            }),
            dataType: "json",
            success: function (data) {

                if (data == "SuccessPreview") {
                    window.open("/Report/ReportPreviewCertificateOfCompletion/");
                    var $this = $("#btnPreviewCOC");
                    $this.button('loading');
                    setTimeout(function () {
                        $("#btnSaveCoC").prop('disabled', false);
                        $this.button('reset');
                    }, 5000);
                }
                else if (data == "FailedPreview") {
                    sweetAlert("Report failed, Please try again", "", "error");
                    return false;
                }

                else {
                    swal
                        ({
                             title: "Success",
                            text: "Certificate of Completion Successfully Saved.",
                            type: "success", allowOutsideClick: false
                        }).then(function () {
                            // Redirect the user
                            window.location.href = "/Home/CertificateOfCompletion";
                            window.open("/Report/COC_Report/");

                        });
                }

               

            }
        });
    })
})

