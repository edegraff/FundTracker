using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FundPortfolio.Startup))]
namespace FundPortfolio
{
	public partial class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			ConfigureAuth(app);
		}
	}
}