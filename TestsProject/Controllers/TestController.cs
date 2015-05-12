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
	/// <summary>
	/// Handles taking, submitting and displaying the results of a test.
	/// </summary>
	public class TestController : Controller
	{
		private AppContext db = new AppContext();

		/// <summary>
		/// Displays tests the user can take right now. (Checks if he is in the right group and the test is currently open)
		/// </summary>
		/// <returns></returns>
		[CustomAuthorize(Roles = "Student")]
		public ActionResult Index()
		{
			var now = DateTime.Now;
			var username = User.Identity.GetUserName();
			var userIsMemberOf = this.db.StudentGroupMemberships.Where(g => g.Username == username).Select(c => c.GroupCode).ToList();

			var validTests =
				this.db.TestParameters.Where(
					t => t.OpenFrom < now && t.OpenTo > now && t.AllowedGroups.Select(g => g.Code).Any(gr => userIsMemberOf.Contains(gr)));
			return this.View(validTests.ToList());
		}

		/// <summary>
		/// Displays a previously completed test, showing the user's answers and scores
		/// </summary>
		/// <param name="id">id of the test stats</param>
		/// <returns></returns>
		public ActionResult Full(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			var stats = this.db.TestStats.Find(id);
			if (stats == null)
			{
				return new HttpNotFoundResult();
			}

			var test = this.db.CompletedTests.First(x => x.TestParameters.Id == stats.TestParameters.Id);
			var user = test.UserId;
			return this.View(test);
		}

		/// <summary>
		/// This method evaluates the results when a test is submitted.
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		[CustomAuthorize(Roles = "Student")]
		public ActionResult Submit()
		{
			var testParameters = this.db.TestParameters.Find(int.Parse(Request["TestParameters.Id"]));

			// js should autosubmit, but make sure in case of hackers/noscript
			DateTime start = DateTime.Parse(Request["startDate"]);
			if (!this.CheckIfSubmittedInTime(start, testParameters.TimeLimit))
			{
				return this.RedirectToAction("TestFail", "Student");
			}

			var completedTest = new CompletedTest
			{
				TestParameters = testParameters,
				UserId = User.Identity.GetUserId(),
				SubmittedQuestions = new List<FilledInQuestion>()
			};

			// the request will contain used question ids as comma separated strings
			var q = Request["item.QuestionId"];
			var questionIds = q.Split(',').ToList();

			var statsHolder = new TestStatsHolder();
			this.EvaluateQuestions(questionIds, ref statsHolder, ref completedTest);

			var completionStats = new TestStats
			{
				Correct = statsHolder.CorrectQuestions,
				PartiallyCorrect = statsHolder.PartiallyCorrectQuestions,
				PartiallyIncorrect = statsHolder.PartiallyIncorrectQuestions,
				Incorrect = statsHolder.IncorrectQuestions,
				Unanswered = statsHolder.UnansweredQuestions,
				TotalScore = statsHolder.TotalScore,
				MaximumScore = statsHolder.MaximumScore,
				UserId = User.Identity.Name,
				StartTime = start,
				TestParameters = testParameters
			};

			this.db.CompletedTests.Add(completedTest);
			this.db.TestStats.Add(completionStats);
			this.db.SaveChanges();
			return this.RedirectToAction("PastTest", "Student", new { id = completionStats.Id });
		}
		/// <summary>
		/// Sets up the test with the specified id. This means selecting a random subset of the valid questions and shuffling them.
		/// If there are not enough questions in the database, the test will use all it can.
		/// The questions are separated to multiple choice and single choice,  so the answers can be displayed as checkboxes/radios respectively.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>

		[CustomAuthorize(Roles = "Student")]
		public ActionResult Take(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			var testParameters = this.db.TestParameters.Find(id);

			if (testParameters == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.NotFound);
			}

			var viewModel = new TakeTestVM
			{
				MultipleChoiceQuestions = new List<Question>(),
				SingleChoiceQuestions = new List<Question>()
			};
			var validTopics = testParameters.Topics.Select(t => t.TopicId);
			var validQuestions = this.db.Questions.Where(q => validTopics.Contains(q.Topic.TopicId)).ToList();

			if (validQuestions.Count > testParameters.NumQuestions)
			{
				this.ShuffleQuestions(testParameters, validQuestions, viewModel);
			}
			else
			{
				viewModel.SingleChoiceQuestions.AddRange(validQuestions.Where(q => q.Answers.Count(a => a.IsCorrect) == 1));
				viewModel.MultipleChoiceQuestions.AddRange(validQuestions.Where(q => q.Answers.Count(a => a.IsCorrect) != 1));
			}

			viewModel.TestParameters = testParameters;
			return this.View(viewModel);
		}

		private bool CheckIfSubmittedInTime(DateTime start, int timeLimitInMinutes)
		{
			var now = DateTime.Now;
			var diff = now - start;
			var timeAllowed = timeLimitInMinutes;
			return diff.TotalMinutes <= timeAllowed;
		}

		/// <summary>
		/// Goes through all the questions in the test, evaluates stats and scores
		/// </summary>
		/// <param name="questionIds">A list of the question ids retrieved from the form hidden fields</param>
		/// <param name="stats">Container for stat-related variables</param>
		/// <param name="completedTest">Model containing details required to reproduce the test when a user wants to review his answers.</param>
		private void EvaluateQuestions(List<string> questionIds, ref TestStatsHolder stats, ref CompletedTest completedTest)
		{
			foreach (var id in questionIds)
			{
				var question = this.db.Questions.Find(int.Parse(id));
				var filledInQuestion = new FilledInQuestion { Question = question, SelectedAnswers = new List<Answer>() };
				stats.MaximumScore += question.NumPoints;
				var questionStats = StatsManager.FindQuestionStats(question, this.db) ?? StatsManager.AddQuestionStats(question, this.db);

				// how many correct answers did the question have?				
				int optionsCorrect = 0;
				int optionsIncorrect = 0;

				string ansSelected = Request[id];
				int correctAmount = question.Answers.Count(x => x.IsCorrect);

				if (ansSelected == null)
				{
					stats.UnansweredQuestions++;
					questionStats.TimesUnanswered++;
				}
				else
				{
					List<string> answers = ansSelected.Split(',').ToList();

					foreach (var ans in answers)
					{
						// each answer belongs to one question, so we can just check if it's correct
						var answer = this.db.Answers.Find(int.Parse(ans));
						filledInQuestion.SelectedAnswers.Add(answer);
						this.EvaluateAnswer(answer, ref optionsCorrect, ref optionsIncorrect);

						var answerStats = StatsManager.FindAnswerStats(answer, this.db) ?? StatsManager.AddAnswerStats(answer, this.db);
						answerStats.TimesSelected++;
						this.db.SaveChanges();
					}

					var questionScore = PointCalculator.CalculatePoints(question.NumPoints, optionsCorrect, optionsIncorrect, correctAmount);
					filledInQuestion.Score = questionScore;

					this.UpdateStats(optionsCorrect, optionsIncorrect, stats, questionStats, questionScore);
				}

				questionStats.TimesAnswered++;
				completedTest.SubmittedQuestions.Add(filledInQuestion);
				this.db.SaveChanges();
			}
		}

		/// <summary>
		/// Updates stat-related objects
		/// </summary>
		/// <param name="optionsCorrect">The number of options that were selected correctly</param>
		/// <param name="optionsIncorrect">The number of options that were selected incorrectly</param>
		/// <param name="stats">Contains stats for this particular test</param>
		/// <param name="questionStats">Contains stats for all instances of the question answered</param>
		/// <param name="questionScore"></param>
		private void UpdateStats(int optionsCorrect, int optionsIncorrect, TestStatsHolder stats, QuestionStats questionStats, double questionScore)
		{
			stats.TotalScore += questionScore;

			if (optionsIncorrect == 0)
			{
				stats.CorrectQuestions++;
				questionStats.TimesAnsweredCorrectly++;
			}
			else if (optionsCorrect == 0)
			{
				stats.IncorrectQuestions++;
				questionStats.TimesAnsweredIncorrectly++;
			}
			else if (questionScore > 0)
			{
				stats.PartiallyCorrectQuestions++;
				questionStats.TimesAnsweredPartiallyCorrectly++;
			}
			else
			{
				stats.PartiallyIncorrectQuestions++;
				questionStats.TimesAnsweredPartiallyIncorrectly++;
			}

			// this should not happen because ansSelected would be null and I'm checking for it before, but just in case I missed something
			if (optionsCorrect == 0 && optionsIncorrect == 0)
			{
				stats.UnansweredQuestions++;
				questionStats.TimesUnanswered++;
			}
		}

		private void EvaluateAnswer(Answer answer, ref int optionsCorrect, ref int optionsIncorrect)
		{
			if (answer.IsCorrect)
			{
				optionsCorrect++;
			}
			else
			{
				optionsIncorrect++;
			}
		}

		private void ShuffleQuestions(TestParameters testParameters, List<Question> validQuestions, TakeTestVM viewModel)
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
	}

	/// <summary>
	/// A container for variables, so that it's not necessary to pass them all as arguments
	/// </summary>
	public class TestStatsHolder
	{
		public double TotalScore { get; set; }
		public int MaximumScore { get; set; }
		public int UnansweredQuestions { get; set; }
		public int CorrectQuestions { get; set; }
		public int IncorrectQuestions { get; set; }
		public int PartiallyCorrectQuestions { get; set; }
		public int PartiallyIncorrectQuestions { get; set; }

		public TestStatsHolder()
		{
			this.TotalScore = 0;
			this.MaximumScore = 0;
			this.UnansweredQuestions = 0;
			this.CorrectQuestions = 0;
			this.IncorrectQuestions = 0;
			this.PartiallyCorrectQuestions = 0;
			this.PartiallyIncorrectQuestions = 0;
		}
	}
}