using System.Collections.Generic;
using TestsProject.Models.Entities;

namespace TestsProject.Models.ViewModels
{
	public class QuestionDetailsVM
	{
		public Question Question { get; set; }
		public QuestionStats QuestionStats { get; set; }
		public IList<AnswerStats> AnswerStats { get; set; }
	}
}