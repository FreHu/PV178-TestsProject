using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TestsProject.Models.Entities
{
	public class TestParameters
	{
		public int Id { get; set; }

		[Required]
		public string Name { get; set; }

		[DefaultValue(60)]
		//in minutes
		//0 = no limit
		[DisplayName("Time Limit")]
		public int TimeLimit { get; set; }
		[Required]
		[DefaultValue(10)]
		[DisplayName("Number of Questions")]
		public int NumQuestions { get; set; }

		[Required]
		[DisplayName("Open From")]
		[DisplayFormat(DataFormatString = "{0:dd.MM.yyyy, HH:mm}")]
		public DateTime OpenFrom { get; set; }
		[DisplayName("Open To")]
		[DisplayFormat(DataFormatString = "{0:dd.MM.yyyy, HH:mm}")]
		public DateTime OpenTo { get; set; }
		[DisplayName("Allowed Groups")]
		public virtual ICollection<StudentGroup> AllowedGroups { get; set; }
		public virtual ICollection<Topic> Topics { get; set; }
	}
}