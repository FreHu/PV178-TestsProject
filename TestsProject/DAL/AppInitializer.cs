using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using TestsProject.Misc;
using TestsProject.Models.Entities;

namespace TestsProject.DAL
{
	public class AppInitializer : DropCreateDatabaseIfModelChanges<AppContext>
	{
		private readonly ApplicationDbContext applicationDbContext = new ApplicationDbContext();

		public void SeedDebug(AppContext context)
		{
			//todo remove this eventually
			Seed(context);
		}

		protected override void Seed(AppContext db)
		{
			var generalTopic = new Topic { Name = "General" };
			var basicMathTopic = new Topic { Name = "Basic Math", BaseTopic = null };
			var advancedMathTopic = new Topic { Name = "Advanced Math", BaseTopic = basicMathTopic };
			var personalQuestionsTopic = new Topic { Name = "Personal Questions", BaseTopic = generalTopic };
			var topics = new List<Topic>
			{
				generalTopic,basicMathTopic,advancedMathTopic,personalQuestionsTopic
			};
			topics.ForEach(s => db.Topics.Add(s));
			db.SaveChanges();


			var question = new Question { QuestionText = "$$1+1$$", Explanation = "This is basic stuff, you should not need an explanation.", Topic = basicMathTopic, NumPoints = 1 };
			var question2 = new Question { QuestionText = "How was your day?", Explanation = "That's too bad!", Topic = personalQuestionsTopic, NumPoints = 1 };
			var question3 = new Question { QuestionText = @"$$\int^2_0 1 dx $$", Explanation = "Use Wolfram Alpha.", Topic = advancedMathTopic, NumPoints = 5 };
			var question4 = new Question
			{
				QuestionText = "What do you think about this website?",
				Explanation = "There are no wrong answers here.",
				Topic = personalQuestionsTopic,
				NumPoints = 1
			};

			var question100 = new Question { QuestionText = "$$17*23$$", Explanation = "none", Topic = basicMathTopic, NumPoints = 1};
			var question101 = new Question { QuestionText = "$$7*39$$", Explanation = "none", Topic = basicMathTopic, NumPoints = 1};
			var question102 = new Question { QuestionText = "$$7+46$$", Explanation = "none", Topic = basicMathTopic, NumPoints = 1};
			var question103 = new Question { QuestionText = "$$33-47$$", Explanation = "none", Topic = basicMathTopic, NumPoints = 1 };
			var question104 = new Question { QuestionText = "$$44*33$$", Explanation = "none", Topic = basicMathTopic, NumPoints = 1 };
			var question105 = new Question { QuestionText = "$$42*17$$", Explanation = "none", Topic = basicMathTopic, NumPoints = 1 };
			var question106 = new Question { QuestionText = "$$25-14$$", Explanation = "none", Topic = basicMathTopic, NumPoints = 1 };
			var question107 = new Question { QuestionText = "$$38+39$$", Explanation = "none", Topic = basicMathTopic, NumPoints = 1 };
			var question108 = new Question { QuestionText = "$$50*18$$", Explanation = "none", Topic = basicMathTopic, NumPoints = 1 };
			var question109 = new Question { QuestionText = "$$28+46$$", Explanation = "none", Topic = basicMathTopic, NumPoints = 1 };
			var questions = new List<Question>
			{
				question,
				question2,
				question3,
				question4,
				question100,
				question101,
				question102,
				question103,
				question104,
				question105,
				question106,
				question107,
				question108,
				question109
			};

			questions.ForEach(s => db.Questions.Add(s));
			questions.ForEach(q => StatsManager.AddQuestionStats(q,db));
			var answers = new List<Answer>
			{
				new Answer {Question = question, Text = "2", IsCorrect = true},
				new Answer {Question = question, Text = "3", IsCorrect = false},
				new Answer {Question = question3, Text = "2", IsCorrect = true},
				new Answer {Question = question3, Text = "3-1", IsCorrect = true},
				new Answer {Question = question3, Text = "17", IsCorrect = false},
				new Answer {Question = question4, Text="It's decent", IsCorrect=true },
				new Answer {Question = question4, Text = "It's very good.", IsCorrect = true},
				new Answer {Question = question4, Text = "It's terrible",IsCorrect=true },
				new Answer {Question = question2, Text = "Good", IsCorrect = true},
				new Answer {Question = question2, Text = "Not so good", IsCorrect = false},
				new Answer {Question = question100, Text = "391", IsCorrect = true},new Answer {Question = question100, Text = "392", IsCorrect = false},new Answer {Question = question100, Text = "390", IsCorrect = false},
new Answer {Question = question101, Text = "273", IsCorrect = true},new Answer {Question = question101, Text = "274", IsCorrect = false},new Answer {Question = question101, Text = "272", IsCorrect = false},
new Answer {Question = question102, Text = "53", IsCorrect = true},new Answer {Question = question102, Text = "54", IsCorrect = false},new Answer {Question = question102, Text = "52", IsCorrect = false},
new Answer {Question = question103, Text = "-14", IsCorrect = true},new Answer {Question = question103, Text = "-13", IsCorrect = false},new Answer {Question = question103, Text = "-15", IsCorrect = false},
new Answer {Question = question104, Text = "1452", IsCorrect = true},new Answer {Question = question104, Text = "1453", IsCorrect = false},new Answer {Question = question104, Text = "1451", IsCorrect = false},
new Answer {Question = question105, Text = "714", IsCorrect = true},new Answer {Question = question105, Text = "715", IsCorrect = false},new Answer {Question = question105, Text = "713", IsCorrect = false},
new Answer {Question = question106, Text = "11", IsCorrect = true},new Answer {Question = question106, Text = "12", IsCorrect = false},new Answer {Question = question106, Text = "10", IsCorrect = false},
new Answer {Question = question107, Text = "77", IsCorrect = true},new Answer {Question = question107, Text = "78", IsCorrect = false},new Answer {Question = question107, Text = "76", IsCorrect = false},
new Answer {Question = question108, Text = "900", IsCorrect = true},new Answer {Question = question108, Text = "901", IsCorrect = false},new Answer {Question = question108, Text = "899", IsCorrect = false},
new Answer {Question = question109, Text = "74", IsCorrect = true},new Answer {Question = question109, Text = "75", IsCorrect = false},new Answer {Question = question109, Text = "73", IsCorrect = false}
			};

			answers.ForEach(s => db.Answers.Add(s));
			answers.ForEach(a => StatsManager.AddAnswerStats(a,db));
			db.SaveChanges();

			var jano = applicationDbContext.Users.FirstOrDefault(user => user.UserName == "jano");
			var zuza = applicationDbContext.Users.FirstOrDefault(user => user.UserName == "zuza");
			var fero = applicationDbContext.Users.FirstOrDefault(user => user.UserName == "fero");

			var sg1 = new StudentGroup { Code = "VRG", Name = "Great students", Description = "The best of the best." };
			var sg2 = new StudentGroup { Code = "TRB", Name = "Terrible students", Description = "Won't make it to the next smester." };

			var groups = new List<StudentGroup> { sg1, sg2 };
			groups.ForEach(s => db.StudentGroups.Add(s));
			db.SaveChanges();

			var sgms = new List<StudentGroupMembership>
			{
				new StudentGroupMembership{GroupCode = "VRG", Username="jano"},
				new StudentGroupMembership{GroupCode = "VRG", Username="zuza"},
				new StudentGroupMembership{GroupCode = "TRB", Username="jano"},
				new StudentGroupMembership{GroupCode = "TRB", Username="fero"},
			};

			sgms.ForEach(s => db.StudentGroupMemberships.Add(s));
			db.SaveChanges();
			var tp = new TestParameters
			{
				Name = "Math quiz",
				NumQuestions = 10,
				AllowedGroups = new List<StudentGroup> { sg1 },
				OpenFrom = new DateTime(1994, 12, 25),
				OpenTo = new DateTime(2015, 12, 14),
				Topics = new List<Topic>{basicMathTopic,advancedMathTopic},
				TimeLimit = 15
			};

			var tp2 = new TestParameters
			{
				Name = "Closed test",
				NumQuestions = 5,
				AllowedGroups = new List<StudentGroup> { sg1},
				OpenFrom = new DateTime(1994, 12, 25),
				OpenTo = new DateTime(2013, 12, 14),
				TimeLimit = 60,
			};
			var tp3 = new TestParameters
			{
				Name = "Do me first!",
				NumQuestions = 5,
				AllowedGroups = new List<StudentGroup> {sg1, sg2 },
				OpenFrom = new DateTime(1994, 12, 25),
				OpenTo = new DateTime(2017, 12, 14),
				Topics = new List<Topic> {personalQuestionsTopic, generalTopic},
				TimeLimit = 60,
			};
			db.TestParameters.Add(tp);
			db.TestParameters.Add(tp2);
			db.TestParameters.Add(tp3);

			db.SaveChanges();
		}
	}
}