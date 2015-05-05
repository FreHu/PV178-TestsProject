using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using TestsProject.DAL;
using TestsProject.Misc;
using TestsProject.Models.Entities;
using TestsProject.Models.ViewModels;

namespace TestsProject.Controllers
{
	[CustomAuthorize(Roles = "Teacher")]
    public class StudentGroupController : Controller
    {
        private AppContext db = new AppContext();

        // GET: StudentGroup
        public ActionResult Index()
        {
            return View(db.StudentGroups.ToList());
        }

        // GET: StudentGroup/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentGroup studentGroup = db.StudentGroups.Find(id);
            if (studentGroup == null)
            {
                return HttpNotFound();
            }
			
	        using (var userContext = new ApplicationDbContext())
	        {
		        var memberNames = db.StudentGroupMemberships.Where(g => g.GroupCode == id).Select(x => x.Username).ToList();
		        var memberIds = userContext.Users.Where(x => memberNames.Contains(x.UserName)).Select(x=>x.Id).ToList();
				var vm = new StudentGroupDetailsVM
				{
					MemberNames = memberNames,
					MemberIds = memberIds,
					StudentGroup = studentGroup
				};
				return View(vm);
			}
        }

        // GET: StudentGroup/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: StudentGroup/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Code,Name,Description")] StudentGroup studentGroup)
        {
            if (ModelState.IsValid)
            {
	            var x = db.StudentGroups.Find(studentGroup.Code);
	            if (db.StudentGroups.Find(studentGroup.Code) == null)
	            {
		            db.StudentGroups.Add(studentGroup);
		            db.SaveChanges();
	            }
	            else
	            {
					//todo code is unique, error message of some sort
	            }
                
                return RedirectToAction("Index");
            }

            return View(studentGroup);
        }

        // GET: StudentGroup/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentGroup studentGroup = db.StudentGroups.Find(id);
            if (studentGroup == null)
            {
                return HttpNotFound();
            }
            return View(studentGroup);
        }

        // POST: StudentGroup/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Code,Name,Description")] StudentGroup studentGroup)
        {
            if (ModelState.IsValid)
            {
                db.Entry(studentGroup).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(studentGroup);
        }

        // GET: StudentGroup/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentGroup studentGroup = db.StudentGroups.Find(id);
            if (studentGroup == null)
            {
                return HttpNotFound();
            }
            return View(studentGroup);
        }

        // POST: StudentGroup/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            StudentGroup studentGroup = db.StudentGroups.Find(id);
            db.StudentGroups.Remove(studentGroup);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
