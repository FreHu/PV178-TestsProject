using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestsProject.DAL;
using TestsProject.Models.Entities;

namespace TestsProject.Misc
{
	public class StatsManager
	{
		public static void RemoveQuestionStats(Question question, AppContext db)
		{
			var stats = db.QuestionStats.FirstOrDefault(q => q.Question.QuestionId == question.QuestionId);
			if (stats != null)
			{
				db.QuestionStats.Remove(stats);
				db.SaveChanges();
			}
			else
			{
				System.Diagnostics.Debug.WriteLine("RemoveQuestionStats was called on nonexisting QuestionStats");
			}
		}

		public static QuestionStats AddQuestionStats(Question question, AppContext db)
		{
			var stats = db.QuestionStats.FirstOrDefault(q => q.Question.QuestionId == question.QuestionId);
			if (stats == null)
			{
				stats = new QuestionStats
				{
					Question = question,
					TimesAnswered = 0,
					TimesAnsweredCorrectly = 0,
					TimesAnsweredIncorrectly = 0,
					TimesAnsweredPartiallyCorrectly = 0,
					TimesAnsweredPartiallyIncorrectly = 0,
					TimesUnanswered = 0
				};
				db.QuestionStats.Add(stats);
				db.SaveChanges();
			}
			else
			{
				System.Diagnostics.Debug.WriteLine("AddQuestionStats was called, but it already exists for this question.");
			}
			return stats;
		}

		public static void RemoveAnswerStats(Answer answer, AppContext db)
		{
			var stats = db.AnswerStats.FirstOrDefault(a => a.Answer.AnswerId == answer.AnswerId);
			if (stats != null)
			{
				db.AnswerStats.Remove(stats);
				db.SaveChanges();
			}
			else
			{
				System.Diagnostics.Debug.WriteLine("RemoveAnswerStats was called on nonexisting AnswerStats");
			}
		}

		public static AnswerStats AddAnswerStats(Answer answer, AppContext db)
		{
			var stats = db.AnswerStats.FirstOrDefault(a => a.Answer.AnswerId == answer.AnswerId);
			if (stats == null)
			{
				stats = new AnswerStats
				{
					Answer = answer,
					TimesSelected = 0
				};
				db.AnswerStats.Add(stats);
				db.SaveChanges();
			}
			else
			{
				System.Diagnostics.Debug.WriteLine("AddAnswerStats was called but it already exists for this answer.");
			}
			return stats;
		}

		public static AnswerStats FindAnswerStats(Answer answer, AppContext db)
		{
			return db.AnswerStats.FirstOrDefault(a => a.Answer.AnswerId == answer.AnswerId);
		}

		public static QuestionStats FindQuestionStats(Question question, AppContext db)
		{
			return db.QuestionStats.FirstOrDefault(q => q.Question.QuestionId == question.QuestionId);
		}
	}
}