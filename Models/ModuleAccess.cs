using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PO_PurchasingUI.Models
{
	public class ModuleAccess
	{
		public string id { get; set; }
		public string parent { get; set; }
		public string text { get; set; }
		public string selectedID { get; set; }

		public string xMainID { get; set; }
		public string MainID { get; set; }
		public string MainPage { get; set; }

		public string xSubPageID { get; set; }
		public string SubPageID { get; set; }
		public string SubMainID { get; set; }
		public string SubPage { get; set; }

		public List<ModuleAccess> MainList { get;set; }
		public List<ModuleAccess> SubList { get;set; }
		public List<ModuleAccess> EditMainList { get; set; }
		public List<ModuleAccess> EditSubList { get; set; }
		public TreeViewAttribute state { get; set; }

		public List<ModuleAccess> AllModuleAccess { get;set; }
	}
	public class TreeViewAttribute
	{
		public bool selected { get; set; }

	}
}