using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Helpers;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using TestsProject.DAL;
using TestsProject.Misc;
using TestsProject.Models.Entities;
using TestsProject.Models.ViewModels;

namespace TestsProject.Controllers
{
	public class TestController : Controller
	{
		private AppContext db = new AppContext();
		// GET: Test
		[CustomAuthorize(Roles = "Student")]
		public ActionResult Index()
		{
			var now = DateTime.Now;
			var username = User.Identity.GetUserName();
			var userIsMemberOf = db.StudentGroupMemberships.Where(g => g.Username == username).Select(c => c.GroupCode).ToList();

			var validTests =
				db.TestParameters.Where(
					t => t.OpenFrom < now && t.OpenTo > now && t.AllowedGroups.Select(g => g.Code).Any(gr => userIsMemberOf.Contains(gr)));
			return View(validTests.ToList());
		}

		public ActionResult Full(int? id) //testStats id
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			var stats = db.TestStats.Find(id);
			if (stats == null)
			{
				return new HttpNotFoundResult();
			}
			var test = db.CompletedTests.First(x => x.TestParameters.Id == stats.TestParameters.Id);
			var user = test.UserId;
			/*if (!User.IsInRole("Teacher") && User.Identity.GetUserId() != user) //can't just see everyone's tests
			{
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
			}*/

;			return View(test);
		}

		[HttpPost]
		[CustomAuthorize(Roles = "Student")]
		public ActionResult Submit()
		{
			//first we check if the test was submitted in time
			//js should autosubmit, but make sure in case of hackers/noscript
			var testParameters = db.TestParameters.Find(int.Parse(Request["TestParameters.Id"]));
			var startTime = DateTime.Parse(Request["startDate"]);
			var now = DateTime.Now;
			var diff = now - startTime;
			var timeAllowed = testParameters.TimeLimit + 0.5;  //with some leeway
			if (diff.TotalMinutes > timeAllowed)
			{
				return RedirectToAction("TestFail", "Student");
			}

			var completedTest = new CompletedTest {TestParameters = testParameters, UserId = User.Identity.GetUserId(),SubmittedQuestions = new List<FilledInQuestion>()};

			//the request will contain used question ids as comma separated strings
			var q = Request["item.QuestionId"];
			var questions = q.Split(',').ToList();
			//List<int> ids = questions.Select(int.Parse).ToList();
			//now cycle through the questions and evaluate answers
			double totalScore=0;
			int maximumScore = 0;
			int unansweredQuestions = 0;
			int correctQuestions = 0;
			int incorrectQuestions = 0;
			int partiallyCorrectQuestions = 0;
			int partiallyIncorrectQuestions = 0;
			foreach (var id in questions)
			{
				var question = db.Questions.Find(int.Parse(id));
				var filledInQuestion = new FilledInQuestion {Question = question, SelectedAnswers = new List<Answer>()};
				maximumScore += question.NumPoints;
				var questionStats = StatsManager.FindQuestionStats(question, db) ?? StatsManager.AddQuestionStats(question, db);
				//how many correct answers did the question have?				
				int correctAmount = question.Answers.Count(x => x.IsCorrect);
				int optionsCorrect = 0;
				int optionsIncorrect = 0;
				string ansSelected = Request[id];
				//like questions, could be comma separated
				if (ansSelected != null)
				{
					List<string> answers = ansSelected.Split(',').ToList();
					foreach (var ans in answers)
					{
						//each answer belongs to one question, so we can just check if it's correct
						var answer = db.Answers.Find(int.Parse(ans));
						if (answer.IsCorrect)
						{
							optionsCorrect++;
						}
						else
						{
							optionsIncorrect++;
						}

						filledInQuestion.SelectedAnswers.Add(answer);

						//update the stats
						var answerStats = StatsManager.FindAnswerStats(answer, db) ?? StatsManager.AddAnswerStats(answer, db);
						answerStats.TimesSelected++;
						db.SaveChanges();
					}
					
					var questionScore = PointCalculator.CalculatePoints(question.NumPoints, optionsCorrect, optionsIncorrect, correctAmount);
					filledInQuestion.Score = questionScore;
					totalScore += questionScore;

					if (optionsCorrect == 0 && optionsIncorrect == 0)
					{
						unansweredQuestions++;   //just in case this can actually happen
						questionStats.TimesUnanswered++;
					}
					if (optionsIncorrect == 0)
					{
						correctQuestions++;
						questionStats.TimesAnsweredCorrectly++;
					}
					else if (optionsCorrect == 0)
					{
						incorrectQuestions++;
						questionStats.TimesAnsweredIncorrectly++;
					}
					else if (questionScore > 0)
					{
						partiallyCorrectQuestions++;
						questionStats.TimesAnsweredPartiallyCorrectly++;
					}
					else
					{
						partiallyIncorrectQuestions++;
						questionStats.TimesAnsweredPartiallyIncorrectly++;
					}
				}
				else
				{
					unansweredQuestions++;
					questionStats.TimesUnanswered++;
				}
				questionStats.TimesAnswered++;
				completedTest.SubmittedQuestions.Add(filledInQuestion);
				db.SaveChanges();
			}

			var completionStats = new TestStats
			{
				Correct = correctQuestions,
				PartiallyCorrect = partiallyCorrectQuestions,
				PartiallyIncorrect = partiallyIncorrectQuestions,
				Incorrect = incorrectQuestions,
				Unanswered = unansweredQuestions,
				TotalScore = totalScore,
				MaximumScore = maximumScore,
				UserId = User.Identity.Name,
				StartTime = startTime,
				TestParameters = testParameters
			};

			db.CompletedTests.Add(completedTest);
			db.TestStats.Add(completionStats);
			db.SaveChanges();
			return RedirectToAction("PastTest","Student",new {id=completionStats.Id});
		}

		//GET: Take/1
		[CustomAuthorize(Roles = "Student")]
		public ActionResult Take(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			var testParameters = db.TestParameters.Find(id);
			if (testParameters == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.NotFound);
			}
			var viewModel = new TakeTestVM();
			viewModel.MultipleChoiceQuestions = new List<Question>();
			viewModel.SingleChoiceQuestions = new List<Question>();
			var validTopics = testParameters.Topics.Select(t => t.TopicId);
			var validQuestions = db.Questions.Where(q => validTopics.Contains(q.Topic.TopicId)).ToList();

			if (validQuestions.Count > testParameters.NumQuestions)
			{
				Random r = new Random();
				for (var i = 0; i < testParameters.NumQuestions; i++)
				{
					var rn = r.Next(0, validQuestions.Count());
					var question = validQuestions.ElementAt(rn);
					if (question.Answers.Count(a => a.IsCorrect) == 1)
					{
						viewModel.SingleChoiceQuestions.Add(validQuestions.ElementAt(rn));
					}
					else
					{
						viewModel.MultipleChoiceQuestions.Add(validQuestions.ElementAt(rn));
					}
					validQuestions.RemoveAt(rn);
				}
			}
			else
			{
				viewModel.SingleChoiceQuestions.AddRange(validQuestions.Where(q => q.Answers.Count(a => a.IsCorrect) == 1));
				viewModel.MultipleChoiceQuestions.AddRange(validQuestions.Where(q => q.Answers.Count(a => a.IsCorrect) != 1));
			}
			viewModel.TestParameters = testParameters;
			return View(viewModel);
		}
	}
}