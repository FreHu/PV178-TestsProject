using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TestsProject.Models.Entities
{
	public class Question
	{
		[Display(Name = "ID")]
		public int QuestionId { get; set; }

		[Display(Name = "Question")]
		[Required(ErrorMessage = "The question text field is required.")]
		public string QuestionText { get; set; }
		[Display(Name = "Explanation")]
		public string Explanation { get; set; }

		[DefaultValue(1)]
		[Display(Name = "Points")]
		public int NumPoints { get; set; }

		public int TopicId { get; set; }
		public virtual Topic Topic { get; set; }
		public virtual ICollection<Answer> Answers { get; set; }
	}
}