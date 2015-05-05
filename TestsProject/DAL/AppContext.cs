using System.Data.Entity;
using TestsProject.Models.Entities;
using TestsProject.Models.ViewModels;

namespace TestsProject.DAL
{
	public class AppContext : DbContext
	{
		public AppContext()
			: base("DefaultConnection")
		{
		
		}
		
		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Topic>()
				.HasOptional(x => x.BaseTopic)
				.WithMany()
				.HasForeignKey(x => x.BaseTopicId);

			modelBuilder.Entity<Topic>()
				.HasMany(x => x.TestParameters)
				.WithMany(x => x.Topics);

			modelBuilder.Entity<TestParameters>()
				.HasMany(x => x.AllowedGroups)
				.WithMany(x => x.TestParameters);
		}

		public DbSet<StudentGroup> StudentGroups { get; set; }

		public DbSet<StudentGroupMembership> StudentGroupMemberships { get; set; }

		public DbSet<Question> Questions { get; set; }

		public DbSet<Topic> Topics { get; set; }

		public DbSet<Answer> Answers { get; set; }

		public DbSet<TestParameters> TestParameters { get; set; }

		public DbSet<AnswerStats> AnswerStats { get; set; }
		public DbSet<QuestionStats> QuestionStats { get; set; }
		public DbSet<TestStats> TestStats { get; set; }
		public DbSet<CompletedTest> CompletedTests { get; set; }
	}
}