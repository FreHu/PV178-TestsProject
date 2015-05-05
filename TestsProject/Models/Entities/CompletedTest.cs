using System.Collections.Generic;

namespace TestsProject.Models.Entities
{
	public class CompletedTest
	{
		public int  Id { get; set; }
		public string UserId { get; set; }
		public virtual TestParameters TestParameters { get; set; }
		public virtual ICollection<FilledInQuestion> SubmittedQuestions { get; set; }
	}
}