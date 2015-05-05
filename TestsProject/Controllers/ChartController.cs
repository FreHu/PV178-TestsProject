using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using TestsProject.DAL;

namespace TestsProject.Controllers
{
    public class ChartController : Controller
    {
	    private AppContext db = new AppContext();
        // GET: Chart
        public ActionResult Index()
        {
            return new HttpStatusCodeResult(HttpStatusCode.MethodNotAllowed);
        }

		public ActionResult ChartTestResults(int? id)
		{
			var stats = db.TestStats.Find(id);
			var chart = new Chart(width: Misc.Constants.ChartWidth, height: Misc.Constants.ChartHeight);
			chart.AddTitle(stats.UserId + " - " + stats.TestParameters.Name);
			chart.AddSeries(
				name: "Scores",
				xValue: new[] { "Correct", "Partially Correct", "Partially Incorrect", "Incorrect", "Unanswered" },
				yValues: new[] { stats.Correct, stats.PartiallyCorrect, stats.PartiallyIncorrect, stats.Incorrect, stats.Unanswered });
			chart.Write("png");
			return PartialView();
		}

	    public ActionResult ChartQuestionStats(int? id)
	    {
		    var qstats = db.QuestionStats.Find(id);
		    var astats = db.AnswerStats.Where(a => a.Answer.QuestionId == id).ToList();
		    var ansNumbers = new List<string>();
		    var ansValues = new List<int>();
		    for (var i = 0; i < astats.Count(); i++)
		    {
			    ansNumbers.Add(i.ToString());
				ansValues.Add(astats.ElementAt(i).TimesSelected);
		    }
		    var chart = new Chart(width: Misc.Constants.ChartWidth, height: Misc.Constants.ChartHeight)
			    .AddTitle("Question " + qstats.Question.QuestionId)
			    .AddSeries("Answers",
				    xValue: ansNumbers.ToArray(),
				    yValues: ansValues.ToArray())
			    .Write();
			return PartialView();
	    }
	}
}