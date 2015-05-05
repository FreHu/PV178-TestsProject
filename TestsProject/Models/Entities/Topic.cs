using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TestsProject.Models.Entities
{
	public class Topic
	{
		public int TopicId { get; set; }
		[Display(Name = "Topic")]
		public string Name { get; set; }

		public virtual ICollection<Question> Questions { get; set; }

		public int? BaseTopicId { get; set; }
		[Display(Name = "Base Topic")]
		public virtual Topic BaseTopic { get; set; }

		public virtual ICollection<TestParameters> TestParameters { get; set; }
	}
}