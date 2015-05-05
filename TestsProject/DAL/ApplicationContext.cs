using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using TestsProject.Models.Entities;

namespace TestsProject.DAL
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
		public ApplicationDbContext()
			: base("DefaultConnection", throwIfV1Schema: false)
		{
			this.Configuration.LazyLoadingEnabled = true;
		}

		public static ApplicationDbContext Create()
		{
			return new ApplicationDbContext();
		}
	}
}