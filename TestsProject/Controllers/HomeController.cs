using System.Web.Mvc;

namespace TestsProject.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			
			return View();
		}

		public ActionResult About()
		{
			ViewBag.Message = "This application was created as an end-of-semester project for the course PV178 at FI Muni";

			return View();
		}
	}
}