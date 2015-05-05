using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TestsProject.Models.Entities
{
	public class TestStats
	{
		public int Id { get; set; }

		public string UserId { get; set; }

		public int Correct { get; set; }
		public int Incorrect { get; set; }
		public int Unanswered { get; set; }
		[Display(Name="Partially correct")]
		public int PartiallyCorrect { get; set; }
		[Display(Name="Partially incorrect")]
		public int PartiallyIncorrect { get; set; }

		[DisplayName("Maximum score")]
		public int MaximumScore { get; set; }
		[Display(Name="Total score")]
		public double TotalScore { get; set; }
		[DisplayName("Test")]
		public virtual TestParameters TestParameters { get; set; }

		[DisplayFormat(DataFormatString = "{0:dd.MM.yyyy, HH:mm}")]
		public DateTime StartTime { get; set; }
	}
}