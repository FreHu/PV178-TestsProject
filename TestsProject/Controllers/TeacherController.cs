using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using TestsProject.DAL;
using TestsProject.Misc;

namespace TestsProject.Controllers
{
	[CustomAuthorize(Roles="Teacher")]
	public class TeacherController : Controller
	{
		private AppContext db = new AppContext();
		// GET: Student
		public ActionResult Index()
		{
			var tests = db.TestStats.OrderByDescending(x => x.StartTime).Take(2);
			return View(tests);
		}

		public ActionResult Students()
		{
			using (var context = new ApplicationDbContext())
			{
				using (var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context)))
				{
					var studentRoleId = roleManager.Roles.Where(r => r.Name == "Student").Select(r => r.Id).FirstOrDefault();
					var students = context.Users.Where(x => x.Roles.FirstOrDefault()
					.RoleId == studentRoleId).ToList();
					return View(students);
				}
			}
		}

		public ActionResult Student(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			using (var context = new ApplicationDbContext())
			{
				var user = context.Users.Find(id);
				var vm = new Models.ViewModels.TeacherStudentDetailsVM
				{
					Name = user.UserName,
					Email = user.Email,
					CompletedTests = db.TestStats.Where(x=>x.UserId == user.UserName)
					.OrderByDescending(x=>x.StartTime).ToList()
				};
				return View(vm);
			}
		}
	}
}