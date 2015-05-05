using System.Collections.Generic;
using TestsProject.Models.Entities;

namespace TestsProject.Models.ViewModels
{
	public class StudentGroupMembershipViewModel
	{
		public List<StudentGroup> StudentGroups { get; set; }
		public List<StudentGroupMembership> Memberships { get; set; }
	}
}