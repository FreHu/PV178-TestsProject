using System;
using System.Web.Mvc;

namespace TestsProject.Misc
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class CustomAuthorizeAttribute : AuthorizeAttribute
	{
		private bool _isAuthorized;

		protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
		{
			_isAuthorized = base.AuthorizeCore(httpContext);
			return _isAuthorized;
		}

		public override void OnAuthorization(AuthorizationContext filterContext)
		{
			base.OnAuthorization(filterContext);

			if (!_isAuthorized)
			{
					filterContext.Controller.TempData.Add("RedirectReason", "Unauthorized");
					filterContext.Controller.TempData.Add("RoleRequired", Roles);
			
			}
		}
	}
}