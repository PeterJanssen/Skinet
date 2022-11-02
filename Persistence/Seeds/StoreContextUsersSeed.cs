using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models.AccountModels.AppUserModels;
using Microsoft.AspNetCore.Identity;

namespace Persistence.Seeds
{
    public class StoreContextUsersSeed
    {
        public static async Task SeedUsersAsync(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            if (!userManager.Users.Any())
            {
                var users = new List<AppUser>
                {
                    new AppUser
                    {
                        DisplayName = "Bob",
                        Email = "bob@test.com",
                        UserName = "bob@test.com",
                        DateOfBirth= DateTime.Parse("1995-02-07"),
                        Address = new Address
                        {
                            FirstName = "Bob",
                            LastName = "Bobbity",
                            Street = "10 The Street",
                            City = "New York",
                            State = "NY",
                            ZipCode = "90210"
                        }
                    },
                    new AppUser
                    {
                        DisplayName = "Amber",
                        Email = "amber@test.com",
                        UserName = "amber@test.com",
                        DateOfBirth= DateTime.Parse("1990-02-07"),
                        Address = new Address
                        {
                            FirstName = "Amber",
                            LastName = "Ambrosia",
                            Street = "10 The Street",
                            City = "New York",
                            State = "NY",
                            ZipCode = "90210"
                        }
                    },
                    new AppUser
                    {
                        DisplayName = "Admin",
                        Email = "admin@test.com",
                        UserName = "admin@test.com"
                    }
                };

                var roles = new List<AppRole>
                {
                    new AppRole {Name = "Admin"},
                    new AppRole {Name = "Member"}
                };

                foreach (var role in roles)
                {
                    await roleManager.CreateAsync(role);
                }

                foreach (var user in users)
                {
                    await userManager.CreateAsync(user, "Pa$$w0rd");
                    await userManager.AddToRoleAsync(user, "Member");
                    if (user.Email == "admin@test.com") await userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }
    }
}