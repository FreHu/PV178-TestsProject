using System.Collections.Generic;
using TestsProject.Models.Entities;

namespace TestsProject.Models.ViewModels
{
	public class StudentGroupDetailsVM
	{
		public StudentGroup StudentGroup { get; set; }
		public IList<string> MemberIds { get; set; }
		public IList<string> MemberNames { get; set; }
	}
}