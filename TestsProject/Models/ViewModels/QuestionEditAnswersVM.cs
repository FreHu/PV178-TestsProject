using System.Collections.Generic;
using TestsProject.Models.Entities;

namespace TestsProject.Models.ViewModels
{
	public class QuestionEditAnswersVM
	{
		public Question Question { get; set; }
		public List<Answer> Answers { get; set; }
	}
}