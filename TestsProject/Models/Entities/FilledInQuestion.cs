using System.Collections.Generic;

namespace TestsProject.Models.Entities
{
	public class FilledInQuestion
	{
		public int Id { get; set; }
		public virtual Question Question { get; set; }
		public virtual ICollection<Answer> SelectedAnswers { get; set; }
		public double Score { get; set; }
	}
}