using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using TestsProject.DAL;
using TestsProject.Misc;
using TestsProject.Models.Entities;
using TestsProject.Models.ViewModels;

namespace TestsProject.Controllers
{
	[CustomAuthorize(Roles = "Student")]
	public class StudentController : Controller
    {
	    private AppContext db = new AppContext();
        // GET: Student
        public ActionResult Index()
        {
	        return View();
        }

	    public ActionResult PastTests()
	    {
		    var stats = db.TestStats.Where(s => s.UserId == User.Identity.Name);

		    return View(stats.ToList());
	    }

	    public ActionResult PastTest(int? id)
	    {
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
		    var stats = db.TestStats.Find(id);
		    if (stats == null)
		    {
				return new HttpStatusCodeResult(HttpStatusCode.NotFound);
			}
            return View(stats);
		}

		// GET: Student/Code
		public ActionResult Code()
	    {
			var loggedInUser = User.Identity.GetUserName();
		    var groupCode = Request["groupCode"];

		    if (db.StudentGroups.Find(groupCode) == null)
		    {
			    TempData["ErrorMessage"] = "No group with code "+groupCode+" exists.";
		    }
		    else
		    {
			    var sgm = new StudentGroupMembership
			    {
				    GroupCode = groupCode,
				    Username = loggedInUser
			    };
			    if (db.StudentGroupMemberships.FirstOrDefault(m => m.GroupCode == groupCode && m.Username == loggedInUser) ==
					null)
			    {
				    db.StudentGroupMemberships.Add(sgm);
				    db.SaveChanges();
			    }
			    else
			    {
					TempData["ErrorMessage"] = "You are already a member of group "+groupCode;
			    }
		    }

		    return RedirectToAction("Group", "Student");
	    }

		public ActionResult TestFail()
		{
			System.Diagnostics.Debug.WriteLine(Request.UrlReferrer);
			return View();
		}

	    public ActionResult Group()
	    {
			var username = User.Identity.GetUserName();
			var viewModel = new StudentGroupMembershipViewModel();
			viewModel.StudentGroups = db.StudentGroups.ToList();
			viewModel.Memberships = db.StudentGroupMemberships.Where(g => g.Username == username).ToList();
			ViewBag.ErrorMessage = TempData["ErrorMessage"];
			return View(viewModel);
	    }
    }
}