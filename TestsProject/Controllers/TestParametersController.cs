using System.Collections.Generic;
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
	public class TestParametersController : Controller
    {
        private AppContext db = new AppContext();

        // GET: TestParametersme
        public ActionResult Index()
        {
            return View(db.TestParameters.ToList());
        }

	    public ActionResult NewTopic(int? id)
	    {
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
		    var vm = new NewTopicVM();
		    vm.TestParameters = db.TestParameters.Find(id);
		    vm.Topics = db.Topics.ToList();
			return View(vm);
	    }

	    public ActionResult NewGroup(int? id)
	    {
		    if (id == null)
		    {
			    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
		    }
		    var parameters = db.TestParameters.Find(id);
		    var vm = new EditGroupsVM
		    {
			    TestParameters = parameters,
			    StudentGroups = db.StudentGroups.ToList()
		    };
		    return View(vm);
	    }

	    [HttpPost]
	    public ActionResult NewTopic()
	    {
		    var parameters = db.TestParameters.Find(int.Parse(Request["parametersId"]));
		    var topicIds = new List<int>(db.Topics.Select(t => t.TopicId));
		    foreach (var id in topicIds)
		    {
			    var reqId = Request[id.ToString()];
			    if (reqId == "on")
			    {
					//nemam a chcem mat
				    var temp = parameters.Topics.FirstOrDefault(t => t.TopicId == id);
				    if (temp == null)
				    {
					    parameters.Topics.Add(db.Topics.Find(id));
				    }
			    }

			    else
			    {
					//mam a nechcem mat
				    var temp = parameters.Topics.FirstOrDefault(t => t.TopicId == id);
				    if (temp != null)
				    {
					    parameters.Topics.Remove(temp);
				    }
			    }
		    }
			db.SaveChanges();
			var vm = new NewTopicVM();
		    vm.TestParameters = parameters;
			vm.Topics = db.Topics.ToList();
		    return View(vm);
	    }

	    [HttpPost]
	    public ActionResult NewGroup()
	    {
		    var parameters = db.TestParameters.Find(int.Parse(Request["parametersId"]));
		    var groupCodes = new List<string> (db.StudentGroups.Select(g => g.Code).ToList());
			foreach (var code in groupCodes)
			{
				var reqId = Request[code];
				if (reqId == "on")
				{
					//nemam a chcem mat
					var temp = parameters.AllowedGroups.FirstOrDefault(g=>g.Code == code);
					if (temp == null)
					{
						parameters.AllowedGroups.Add(db.StudentGroups.Find(code));
					}
				}

				else
				{
					//mam a nechcem mat
					var temp = parameters.AllowedGroups.FirstOrDefault(g=> g.Code == code);
					if (temp != null)
					{
						parameters.AllowedGroups.Remove(temp);
					}
				}
			}
			db.SaveChanges();
			var vm = new EditGroupsVM();
			vm.TestParameters = parameters;
			vm.StudentGroups = db.StudentGroups.ToList();
			return View(vm);
	    }

        // GET: TestParameters/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestParameters testParameters = db.TestParameters.Find(id);
            if (testParameters == null)
            {
                return HttpNotFound();
            }
            return View(testParameters);
        }

        // GET: TestParameters/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TestParameters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,TimeLimit,NumQuestions,OpenFrom,OpenTo")] TestParameters testParameters)
        {
            if (ModelState.IsValid)
            {
                db.TestParameters.Add(testParameters);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(testParameters);
        }

        // GET: TestParameters/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestParameters testParameters = db.TestParameters.Find(id);
            if (testParameters == null)
            {
                return HttpNotFound();
            }
            return View(testParameters);
        }

        // POST: TestParameters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,TimeLimit,NumQuestions,OpenFrom,OpenTo")] TestParameters testParameters)
        {
            if (ModelState.IsValid)
            {
                db.Entry(testParameters).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(testParameters);
        }

        // GET: TestParameters/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestParameters testParameters = db.TestParameters.Find(id);
            if (testParameters == null)
            {
                return HttpNotFound();
            }
            return View(testParameters);
        }

        // POST: TestParameters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TestParameters testParameters = db.TestParameters.Find(id);
            db.TestParameters.Remove(testParameters);
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
