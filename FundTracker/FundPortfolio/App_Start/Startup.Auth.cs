using System.Web;
using System.Web.Mvc;

using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

namespace FundPortfolio
{
	public partial class Startup
	{
		// For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
		public void ConfigureAuth(IAppBuilder app)
		{
			// Enable the application to use a cookie to store information for the signed in user
			app.UseCookieAuthentication(new CookieAuthenticationOptions
			{
				AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
				LoginPath = new PathString("/Account/Login")
			});
			// Use a cookie to temporarily store information about a user logging in with a third party login provider
			app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
			//app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

			// Uncomment the following lines to enable logging in with third party login providers
			//app.UseMicrosoftAccountAuthentication(
			// clientId: "",
			// clientSecret: "");

			//app.UseTwitterAuthentication(
			// consumerKey: "",
			// consumerSecret: "");

			//app.UseFacebookAuthentication(
			// appId: "",
			// appSecret: "");
			app.UseGoogleAuthentication(
				clientId: "296879124727-v11i4ttrgog7uhjrkkd728c9mfq0ahsp.apps.googleusercontent.com",
				clientSecret: "mQmu-DuUimXgKqcF4FqOE9u_");
		}
	}
}