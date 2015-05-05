using System.ComponentModel.DataAnnotations;

namespace TestsProject.Models.Entities
{
	public class Answer
	{
		public int AnswerId { get; set; }
		[Required(ErrorMessage = "The answer must have a question associated to it.")]
		public int QuestionId { get; set; }
		[Required(ErrorMessage = "The answer must have text.")]
		public string Text { get; set; }
		public bool IsCorrect { get; set; }

		public virtual Question Question { get; set; }
	}
}