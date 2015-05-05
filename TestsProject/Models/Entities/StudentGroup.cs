using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TestsProject.Models.Entities
{
	public class StudentGroup
	{
		[Required]
		[Key]
		public string Code { get; set; }

		[Required]
		public string Name { get; set; }
		public string Description { get; set; }

		public ICollection<TestParameters> TestParameters { get; set; }
	}
}