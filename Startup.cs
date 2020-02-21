using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(PO_PurchasingUI.Startup))]

namespace PO_PurchasingUI
{
	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
            app.MapSignalR();
		}
	}
}
