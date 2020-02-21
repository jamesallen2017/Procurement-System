using System.Web;
using System.Web.Optimization;

namespace PO_PurchasingUI
{
	public class BundleConfig
	{
		// For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
		public static void RegisterBundles(BundleCollection bundles)
		{
			bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
						"~/Scripts/jquery-3.4.1.min.js",
						"~/assets/js/core/bootstrap.min.js",
						"~/Scripts/jquery.signalR-2.4.1.min.js",
						"~/assets/js/core/jquery.slimscroll.min.js",
						"~/assets/js/core/jquery.scrollLock.min.js",
						"~/assets/js/core/jquery.appear.min.js",
						"~/assets/js/core/jquery.countTo.min.js",
						"~/assets/js/core/jquery.placeholder.min.js",
						"~/assets/js/core/js.cookie.min.js",
						"~/Scripts/jstree.min.js",
						"~/Scripts/bootbox.min.js",
                        "~/Scripts/ bootbox.locales.min.js",
						"~/assets/js/plugins/jquery-tags-input/jquery.tagsinput.min.js",
						"~/assets/js/plugins/bootstrap-datepicker/bootstrap-datepicker.min.js",
						"~/assets/js/plugins/bootstrap-datetimepicker/moment.min.js",
						"~/assets/js/plugins/datatables/jquery.dataTables.min.js",
						"~/assets/js/plugins/masked-inputs/jquery.maskedinput.min.js",
						"~/assets/js/plugins/sweetalert2/sweetalert2.min.js",
						"~/assets/js/plugins/select2/select2.full.min.js",
						"~/assets/js/plugins/datatables/jquery.dataTables.min.js",
						"~/assets/js/pages/base_tables_datatables.js",
                        "~/assets/js/plugins/slick/slick.min.js",
                        "~/assets/js/plugins/chartjsv2/Chart.min.js",
                        "~/assets/js/pages/base_comp_chartjs_v2.js",
						"~/assets/js/app.js"
						));

			bundles.Add(new ScriptBundle("~/bundles/LoginJquery").Include(
						"~/assets/js/core/jquery.min.js",
						"~/assets/js/core/bootstrap.min.js",
						"~/assets/js/core/jquery.slimscroll.min.js",
						"~/assets/js/core/jquery.scrollLock.min.js",
						"~/assets/js/core/jquery.appear.min.js",
						"~/assets/js/core/jquery.countTo.min.js",
						"~/assets/js/core/jquery.placeholder.min.js",
						"~/assets/js/core/js.cookie.min.js",
						"~/Scripts/bootbox.min.js",
						"~/Scripts/ bootbox.locales.min.js",
						"~/assets/js/app.js"

						));

			bundles.Add(new StyleBundle("~/Content/LoginCss").Include(
					  "~/Content/themes/default/style.min.css",
					  "~/assets/css/bootstrap.min.css",
					  "~/assets/css/oneui.css",
					  "~/assets/js/plugins/jquery-ui-1.12.1/jquery-ui.min.css"

					  ));


			bundles.Add(new ScriptBundle("~/bundles/JSLoginFormValidation").Include(
						"~/assets/js/plugins/jquery-validation/jquery.validate.min.js",
						"~/assets/js/plugins/jquery-validation/additional-methods.min.js",
						"~/assets/js/pages/base_pages_login.js"));

			bundles.Add(new ScriptBundle("~/bundles/JSFormValidation").Include(
						"~/assets/js/plugins/jquery-validation/jquery.validate.min.js",
						"~/assets/js/plugins/jquery-validation/additional-methods.min.js",
						"~/assets/js/pages/base_forms_validation.js",
						"~/assets/js/SystemFunctionalities/GlobalFunction.js"));

			bundles.Add(new StyleBundle("~/Content/css").Include(
                           "~/Content/themes/default/style.min.css",
                           "~/assets/js/plugins/slick/slick.min.css",
                           "~/assets/js/plugins/slick/slick-theme.min.css",
                           "~/assets/js/plugins/datatables/jquery.dataTables.min.css",
						   "~/assets/js/plugins/jquery-tags-input/jquery.tagsinput.min.css",
						  "~/assets/js/plugins/bootstrap-datepicker/bootstrap-datepicker3.min.css",
						  "~/assets/js/plugins/sweetalert2/sweetalert2.min.css",
						  "~/assets/js/plugins/select2/select2.min.css",
						  "~/assets/js/plugins/select2/select2-bootstrap.min.css",
						  "~/assets/css/bootstrap.min.css",
						  "~/assets/css/oneui.css",
						   "~/assets/js/plugins/jquery-ui-1.12.1/jquery-ui.min.css"
						  ));

			bundles.Add(new ScriptBundle("~/bundles/CreateSupplier").Include(
						"~/assets/js/SystemFunctionalities/CreateSupplier.js"));

			bundles.Add(new ScriptBundle("~/bundles/EditSupplier").Include(
						"~/assets/js/SystemFunctionalities/EditSupplier.js"));

			bundles.Add(new ScriptBundle("~/bundles/CreateClient").Include(
						"~/assets/js/SystemFunctionalities/CreateClient.js"));

			bundles.Add(new ScriptBundle("~/bundles/EditClient").Include(
						"~/assets/js/SystemFunctionalities/EditClient.js"));

			bundles.Add(new ScriptBundle("~/bundles/CreateCOC").Include(
						"~/assets/js/SystemFunctionalities/CreateCOC.js"));

			bundles.Add(new ScriptBundle("~/bundles/EditCOC").Include(
						"~/assets/js/SystemFunctionalities/EditCOC.js"));

			bundles.Add(new ScriptBundle("~/bundles/Acc_ClientMonitoring").Include(
						"~/assets/js/SystemFunctionalities/Acc_ClientMonitoring.js"));

			bundles.Add(new ScriptBundle("~/bundles/Acc_SupplierMonitroing").Include(
						"~/assets/js/SystemFunctionalities/Acc_SupplierMonitoring.js"));

			bundles.Add(new ScriptBundle("~/bundles/CreatePayables").Include(
						"~/assets/js/SystemFunctionalities/CreateaPayables.js"));

			bundles.Add(new ScriptBundle("~/bundles/CreateReceivables").Include(
						"~/assets/js/SystemFunctionalities/CreateReceivables.js"));

			bundles.Add(new ScriptBundle("~/bundles/EditPayables").Include(
						"~/assets/js/SystemFunctionalities/EditPayables.js"));

			bundles.Add(new ScriptBundle("~/bundles/EditReceivables").Include(
						"~/assets/js/SystemFunctionalities/EditReceivables.js"));

			bundles.Add(new ScriptBundle("~/bundles/DRReceiving").Include(
						"~/assets/js/SystemFunctionalities/DRReceiving.js"));

			bundles.Add(new ScriptBundle("~/bundles/DRReleasing").Include(
						"~/assets/js/SystemFunctionalities/DRReleasing.js"));

			bundles.Add(new ScriptBundle("~/bundles/ReleasedLogs").Include(
						"~/assets/js/SystemFunctionalities/ReleasedLogs.js"));

			bundles.Add(new ScriptBundle("~/bundles/ReceivedLogs").Include(
						"~/assets/js/SystemFunctionalities/ReceivedLogs.js"));

			bundles.Add(new ScriptBundle("~/bundles/FormsControl").Include(
						"~/assets/js/SystemFunctionalities/FormControl.js"));

			bundles.Add(new ScriptBundle("~/bundles/SMTP").Include(
						"~/assets/js/SystemFunctionalities/SMTP.js"));

			bundles.Add(new ScriptBundle("~/bundles/ItemMaster").Include(
						"~/assets/js/SystemFunctionalities/ItemMaster.js"));

			bundles.Add(new ScriptBundle("~/bundles/RountingApproval").Include(
						"~/assets/js/SystemFunctionalities/RoutingApproval.js"));

			bundles.Add(new ScriptBundle("~/bundles/COCRoutingApproval").Include(
						"~/assets/js/SystemFunctionalities/COCRoutingApproval.js"));
		}
	}
}
