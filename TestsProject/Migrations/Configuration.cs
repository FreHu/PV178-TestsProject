using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using TestsProject.DAL;
using TestsProject.Models.Entities;

namespace TestsProject.Migrations
{
	internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
	{
		public Configuration()
		{
			AutomaticMigrationsEnabled = true;
			ContextKey = "Projekt_Odpovedniky.Models.ApplicationDbContext";
		}

		public void SeedDebug(ApplicationDbContext context)
		{
			Seed(context);
		}

		protected override void Seed(ApplicationDbContext context)
		{
			//  This method will be called after migrating to the latest version.
			// Update-Database -ConnectionStringName DefaultConnection -Verbose to run seed explicitly

			var roleStore = new RoleStore<IdentityRole>(context);
			var roleManager = new RoleManager<IdentityRole>(roleStore);
			var teacher = new IdentityRole
			{
				Name = "Teacher"
			};
			var student = new IdentityRole
			{
				Name = "Student"
			};
			roleManager.Create(teacher);
			roleManager.Create(student);

			var userStore = new UserStore<ApplicationUser>(context);
			var userManager = new UserManager<ApplicationUser>(userStore);

			var hasher = new PasswordHasher();
			var users = new List<ApplicationUser> 
			{ 
				new ApplicationUser
                {
	                PasswordHash = hasher.HashPassword("Heslo6!"), Email = "teacher@gmail.com", UserName = "teacher",  SecurityStamp = Guid.NewGuid().ToString()
                },
                new ApplicationUser
                {
	                PasswordHash = hasher.HashPassword("Heslo6!"), Email = "student@gmail.com", UserName = "student",  SecurityStamp = Guid.NewGuid().ToString()
                },
				new ApplicationUser
                {
	                PasswordHash = hasher.HashPassword("Heslo6!"), Email = "fero@gmail.com", UserName = "fero",  SecurityStamp = Guid.NewGuid().ToString()
                },
				new ApplicationUser
                {
	                PasswordHash = hasher.HashPassword("Heslo6!"), Email = "jano@gmail.com", UserName = "jano",  SecurityStamp = Guid.NewGuid().ToString()
                },
				new ApplicationUser
                {
	                PasswordHash = hasher.HashPassword("Heslo6!"), Email = "zuza@gmail.com", UserName = "zuza",  SecurityStamp = Guid.NewGuid().ToString()
                }
            };

			users.ForEach(user => context.Users.AddOrUpdate(user));
			context.SaveChanges();
			try
			{
				userManager.AddToRole(users[0].Id, "Teacher");
				userManager.AddToRole(users[1].Id, "Student");

				userManager.AddToRole(users[2].Id, "Student");
				userManager.AddToRole(users[3].Id, "Student");
				userManager.AddToRole(users[4].Id, "Student");
			}
			catch (DbEntityValidationException dbEx)
			{
				foreach (var validationErrors in dbEx.EntityValidationErrors)
				{
					foreach (var validationError in validationErrors.ValidationErrors)
					{
						System.Diagnostics.Debug.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
					}
				}
			}
			context.SaveChanges();
		}
	}
}
