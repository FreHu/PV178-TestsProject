using System.Collections.Generic;
using TestsProject.Models.Entities;

namespace TestsProject.Models.ViewModels
{
	public class TakeTestVM
	{
		public TestParameters TestParameters { get; set; }
		public List<Question> SingleChoiceQuestions { get; set; }
		public List<Question> MultipleChoiceQuestions { get; set; } //also zero choice questions
	}

	public class NewTopicVM
	{
		public TestParameters TestParameters { get; set; }
		public List<Topic> Topics { get; set; }
	}

	public class EditGroupsVM
	{
		public TestParameters TestParameters { get; set; }
		public List<StudentGroup> StudentGroups { get; set; }
	}
}