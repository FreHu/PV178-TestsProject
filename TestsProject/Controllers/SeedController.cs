using System.Web.Mvc;
using TestsProject.DAL;
using TestsProject.Migrations;
using TestsProject.Misc;

namespace TestsProject.Controllers
{
	//[CustomAuthorize(Roles = "Master")]
	public class SeedController : Controller
    {
		private AppContext db = new AppContext();
		private ApplicationDbContext applicationDbContext = new ApplicationDbContext();
        // GET: Seed
        public ActionResult Index()
        {
			//this is probably bad practice and should be done through the package manager console
			//however, I am unable to see exception details there.

			//to run seed on users
/*		
			Configuration configuration = new Configuration();
			configuration.SeedDebug(applicationDbContext);
 
			//to run seed on other things
*/			
			AppInitializer initializer = new AppInitializer();
			initializer.SeedDebug(db);
	
            return RedirectToAction("Index","Home");
        }
    }
}