
function DoPreview() {
    $("#HiddenPreview").val("PREVIEW");
}
function DoSave() {
    $("#HiddenPreview").val("SAVED");
}
$(document).ready(function () {
    $("#btnSaveCoC").prop('disabled', true);
})
$("#CoCValidation").on('submit', function (event) {

    var HiddenPreview = $("#HiddenPreview").val()
    var ClientNameList = $("#ClientID").val()
    var COC_StartDate = $("#COC_StartDate").val()
    var POReferenceNo = $("#POReferenceNo").val()
    var Approver = $("#ApproverID").val()
    var ProjectName = $("#ProjectName").val()
    var COC_CompletionDate = $("#COC_CompletionDate").val()
    var ProjectDetails = $("#ProjectDetails").val()
    var COC_ControlNo = $("#COC_ControlNo").val()
    var COC_Resposible = $("#COC_Resposible").val()
    var NumberID = $("#NumberID").val()
    var POC_ReferenceNo = $("#POC_ReferenceNo").val()
    var ClientAddress = $("#ClientAddress").val()
    var ClientLocation = $("#ClientLocation").val()
    

    if (ClientNameList == ""  || ProjectName == "" || ProjectDetails == "") {
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
            StartDate: COC_StartDate,
            ProjectName: ProjectName,
            CompletionDate: COC_CompletionDate,
            ProjectDetails: ProjectDetails,
            POReferenceNo: POReferenceNo,
            ApproverID: Approver,
            COC_ControlNo: COC_ControlNo,
            COC_Resposible: COC_Resposible,
            HiddenPreview: HiddenPreview,
            NumberID: NumberID,
            POC_ReferenceNo: POC_ReferenceNo,
            ClientAddress: ClientAddress,
            ClientLocation: ClientLocation,
        }),
        dataType: "json",
        success: function (data) {
            if (data == "Error") {
                swal("Sorry", "Please Contact our administrator for assistance", "error")
                return false;
            }
            else if (data == "SuccessPreview") {
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
                return
            }
            else
            {
                swal
              ({
                   title: "Success",
                  text: "Certificate of Completion Successfully Updated.",
                  type: "success", allowOutsideClick: false
              }).then(function () {
                  // Redirect the user
                  window.location.href = '/Home/EditCertificateOfCompletion'
                  window.open("/Report/COC_Report/");

              });
            }
          
        }
    });

})