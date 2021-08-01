using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataLayer.Model
{
    public class DBInitializer
    {
        public async static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = new AfrostichContext(serviceProvider.GetRequiredService<DbContextOptions<AfrostichContext>>());
            context.Database.EnsureCreated();
            // Look for any students.
            if (await context.ROLE.AnyAsync())
            {
                return;   // DB has been seeded
            }

            var roles = new Role[]
              {
                    new Role{ Active = true, Name = "Seller"},
                    new Role{ Active = true, Name = "Customer"},
                    
              };
            foreach (Role role in roles)
            {
                context.Add(role);
            }
            await context.SaveChangesAsync();

            var genders = new Gender[]
            {
                new Gender{ Active = true, Name = "Male"},
                new Gender{ Active = true, Name = "Female"},
            };
            foreach (Gender gender in genders)
            {
                context.Add(gender);
            }
            await context.SaveChangesAsync();


           // var securityQuestions = new SecurityQuestion[]
           //{
           //     new SecurityQuestion{ Active = true, Name = "Mother's Maiden Name"},
           //     new SecurityQuestion{ Active = true, Name = "Favorite Food"},
           //     new SecurityQuestion{ Active = true, Name = "First Car"},
           //     new SecurityQuestion{ Active = true, Name = "Last Vacation"},
           //};
           // foreach (SecurityQuestion securityQuestion in securityQuestions)
           // {
           //     context.Add(securityQuestion);
           // }
           // await context.SaveChangesAsync();

        }

    }
}
