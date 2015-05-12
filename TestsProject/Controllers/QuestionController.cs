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
	public class QuestionController : Controller
	{
		private AppContext db = new AppContext();

		// GET: Question
		public ActionResult Index()
		{
			var questions = db.Questions.Include(q => q.Topic);
			return View(questions.ToList());
		}

		public ActionResult EditAnswers(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			var question = db.Questions.Find(id);
			if (question == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.NotFound);
			}

			var vm = new QuestionEditAnswersVM
			{
				Question = question,
				Answers = question.Answers.ToList()
			};
			return View(vm);
		}

		public ActionResult EditAnswersDelete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			
			Answer answer = db.Answers.Find(id);
			StatsManager.RemoveAnswerStats(answer,db);
			db.Answers.Remove(answer);
			db.SaveChanges();
			if (System.Web.HttpContext.Current.Request.UrlReferrer != null)
				return Redirect(System.Web.HttpContext.Current.Request.UrlReferrer.ToString());
			return RedirectToAction("Index");

		}

		[HttpPost]
		public ActionResult EditAnswers()
		{
			var questionId = int.Parse(Request["questionId"]);
			var question = db.Questions.Find(questionId);
			var answerText = Request["answerText"];
			bool isCorrect = Request["isCorrect"] == "on";

			if (answerText != null)
			{
				var answer = new Answer
				{
					Text = answerText,
					IsCorrect = isCorrect,
					Question = question,
					QuestionId = questionId
				};
				db.Answers.Add(answer);
				StatsManager.AddAnswerStats(answer, db);
				db.SaveChanges();
			}
			else
			{
				ViewBag.ErrorMessage = "You need to provide an answer.";
			}

			
			var vm = new QuestionEditAnswersVM()
			{
				Question = question,
				Answers = question.Answers.ToList()
			};
			return View(vm);
		}

		// GET: Question/Details/5
		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Question question = db.Questions.Find(id);
			if (question == null)
			{
				return HttpNotFound();
			}

			var vm = new QuestionDetailsVM
			{
				AnswerStats = db.AnswerStats.Where(x => x.Answer.QuestionId == question.QuestionId).ToList(),
				QuestionStats = db.QuestionStats.FirstOrDefault(x => x.Question.QuestionId == question.QuestionId),
				Question = question
			};
			return View(vm);
		}

		// GET: Question/Create
		public ActionResult Create()
		{
			ViewBag.TopicId = new SelectList(db.Topics, "TopicId", "Name");
			return View();
		}

		// POST: Question/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "QuestionId,QuestionText,Explanation,NumPoints,TopicId")] Question question)
		{
			if (ModelState.IsValid)
			{
				db.Questions.Add(question);
				db.SaveChanges();
				StatsManager.AddQuestionStats(question, db);
				return RedirectToAction("Index");
			}

			ViewBag.TopicId = new SelectList(db.Topics, "TopicId", "Name", question.TopicId);
			return View(question);
		}

		// GET: Question/Edit/5
		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Question question = db.Questions.Find(id);
			if (question == null)
			{
				return HttpNotFound();
			}
			ViewBag.TopicId = new SelectList(db.Topics, "TopicId", "Name", question.TopicId);
			return View(question);
		}

		// POST: Question/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "QuestionId,QuestionText,Explanation,NumPoints,TopicId")] Question question)
		{
			if (ModelState.IsValid)
			{
				db.Entry(question).State = EntityState.Modified;
				db.SaveChanges();
				return RedirectToAction("Index");
			}
			ViewBag.TopicId = new SelectList(db.Topics, "TopicId", "Name", question.TopicId);
			return View(question);
		}

		// GET: Question/Delete/5
		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Question question = db.Questions.Find(id);
			if (question == null)
			{
				return HttpNotFound();
			}
			return View(question);
		}

		// POST: Question/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			Question question = db.Questions.Find(id);
			StatsManager.RemoveQuestionStats(question,db);
			db.Questions.Remove(question);
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
