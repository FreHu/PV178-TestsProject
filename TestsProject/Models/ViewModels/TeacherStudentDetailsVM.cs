using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestsProject.Models.Entities;

namespace TestsProject.Models.ViewModels
{
	public class TeacherStudentDetailsVM
	{
		public string Name { get; set; }
		public string Email { get; set; }
		public ICollection<TestStats> CompletedTests { get; set; }
	}
}