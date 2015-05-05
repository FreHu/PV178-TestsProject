using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TestsProject.Models.Entities
{
	public class AnswerStats
	{
		public int Id { get; set; }
		public virtual Answer Answer { get; set; }
		[Display(Name="Times selected")]
		public int TimesSelected { get; set; }
	}
}