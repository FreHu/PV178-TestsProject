namespace TestsProject.Models.Entities
{
	public class QuestionStats
	{
		public int Id { get; set; }
		public int QuestionId { get; set; }
		public virtual Question Question { get; set; }
		//we can't just sum timesAnswered on its answers because of mutliple choice questions
		public int TimesAnswered { get; set; }
		public int TimesAnsweredCorrectly { get; set; }
		public int TimesAnsweredIncorrectly { get; set; }
		public int TimesAnsweredPartiallyCorrectly { get; set; }
		public int TimesAnsweredPartiallyIncorrectly { get; set; }
		public int TimesUnanswered { get; set; }
	}
}