using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using ESPL.NG.Helpers.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ESPL.NG.Entities
{
    public class IdentityInitializer
    {
        private RoleManager<IdentityRole> _roleMgr;
        private UserManager<AppUser> _userMgr;

        public IdentityInitializer(UserManager<AppUser> userMgr, RoleManager<IdentityRole> roleMgr)
        {
            _userMgr = userMgr;
            _roleMgr = roleMgr;
        }

        public async Task AddAdminUser()
        {
            if (!(await _roleMgr.RoleExistsAsync("SystemAdmin")))
            {
                var role = new IdentityRole("SystemAdmin");
                role.Claims.Add(new IdentityRoleClaim<string>() { ClaimType = "SystemAdmin", ClaimValue = "True" });
                await _roleMgr.CreateAsync(role);
            }

            var adminUser = new AppUser()
            {
                UserName = "espladmin",
                FirstName = "ESPL",
                LastName = "SystemAdmin",
                Email = "espl.admin@eternussolutions.com"
            };

            var adminUserResult = await _userMgr.CreateAsync(adminUser, "Espl@123");
            var adminRoleResult = await _userMgr.AddToRoleAsync(adminUser, "SystemAdmin");

            if (!adminUserResult.Succeeded || !adminRoleResult.Succeeded)
            {
                throw new InvalidOperationException("Failed to build user and roles");
            }
        }

        public async Task AddManagerUser()
        {
            if (!(await _roleMgr.RoleExistsAsync("Manager")))
            {
                var role = new IdentityRole("Manager");
                role.Claims.Add(new IdentityRoleClaim<string>() { ClaimType = "IsManager", ClaimValue = "True" });
                await _roleMgr.CreateAsync(role);
            }

            var user = new AppUser()
            {
                UserName = "esplmanager",
                FirstName = "ESPL",
                LastName = "Manager",
                Email = "espl.manager@eternussolutions.com"
            };

            var userResult = await _userMgr.CreateAsync(user, "Espl@123");
            var roleResult = await _userMgr.AddToRoleAsync(user, "Manager");

            if (!userResult.Succeeded || !roleResult.Succeeded)
            {
                throw new InvalidOperationException("Failed to build user and roles");
            }
        }

        public async Task AddEmployeeUser()
        {
            if (!(await _roleMgr.RoleExistsAsync("Employee")))
            {
                var role = new IdentityRole("Employee");
                role.Claims.Add(new IdentityRoleClaim<string>() { ClaimType = "IsEmployee", ClaimValue = "True" });
                await _roleMgr.CreateAsync(role);
            }

            var user = new AppUser()
            {
                UserName = "esplemployee",
                FirstName = "ESPL",
                LastName = "Employee",
                Email = "espl.employee@eternussolutions.com"
            };

            var userResult = await _userMgr.CreateAsync(user, "Espl@123");
            var roleResult = await _userMgr.AddToRoleAsync(user, "Employee");

            if (!userResult.Succeeded || !roleResult.Succeeded)
            {
                throw new InvalidOperationException("Failed to build user and roles");
            }
        }

        public async Task Seed()
        {
            var user = await _userMgr.FindByNameAsync("tomcruise");

            // Add User
            if (user == null)
            {
                // await AddAdminUser();
                // await AddManagerUser();
                // await AddEmployeeUser();

                // await AddAllEmployees();
                // await AddAllManagers();
                // await AddAllAdmins();

                await AddAllConstables();
                await AddAllSergeant();
                await AddAllSAIG();
                await AddAllDIG();
                await AddAllIG();
                await AddAllASP();
                await AddAllSuperAdmins();
                await AddAllAdmins();

            }
        }

        public async Task AddAllEmployees()
        {
            List<AppUser> allUsers = new List<AppUser>()
            {
                new AppUser()
                {
                    UserName = "esplemployee",
                    Id = "56c385ae-ce46-41d4-b7fe-08df9aef7301",
                    FirstName = "ESPL",
                    LastName = "Employee",
                    Email = "espl.employee@eternussolutions.com"
                },new AppUser()
                {
                    UserName = "esplemployee1",
                    Id = "56c385ae-ce46-41d4-b7fe-08df9aef7302",
                    FirstName = "ESPL 1",
                    LastName = "Employee 1",
                    Email = "espl.employee1@eternussolutions.com"
                },new AppUser()
                {
                    UserName = "esplemployee2",
                    Id = "56c385ae-ce46-41d4-b7fe-08df9aef7303",
                    FirstName = "ESPL 2",
                    LastName = "Employee 2",
                    Email = "espl.employee2@eternussolutions.com"
                },new AppUser()
                {
                    UserName = "esplemployee3",
                    Id = "56c385ae-ce46-41d4-b7fe-08df9aef7304",
                    FirstName = "ESPL 3",
                    LastName = "Employee 3",
                    Email = "espl.employee3@eternussolutions.com"
                }
            };

            if (!(await _roleMgr.RoleExistsAsync("Employee")))
            {
                var role = new IdentityRole("Employee");
                role.Claims.Add(new IdentityRoleClaim<string>()
                {
                    ClaimType = "IsEmployee",
                    ClaimValue = "True"
                });
                await _roleMgr.CreateAsync(role);
            }

            foreach (AppUser user in allUsers)
            {
                var userResult = await _userMgr.CreateAsync(user, "Espl@123");
                var roleResult = await _userMgr.AddToRoleAsync(user, "Employee");

                if (!userResult.Succeeded || !roleResult.Succeeded)
                {
                    throw new InvalidOperationException("Failed to build user and roles");
                }
            }

        }

        public async Task AddAllManagers()
        {
            List<AppUser> allUsers = new List<AppUser>() {
                new AppUser() {
                    UserName = "esplmanager",
                    Id = "56c385ae-ce46-41d4-b7fe-08df9aef7201",
                    FirstName = "ESPL 1",
                    LastName = "Manager",
                    Email = "espl.manager1@eternussolutions.com"
                },
                new AppUser() {
                    UserName = "esplmanager1",
                    Id = "56c385ae-ce46-41d4-b7fe-08df9aef7202",
                    FirstName = "ESPL 2",
                    LastName = "Manager",
                    Email = "espl.manager2@eternussolutions.com"
                },
                new AppUser() {
                    UserName = "esplmanager2",
                    Id = "56c385ae-ce46-41d4-b7fe-08df9aef7203",
                    FirstName = "ESPL 3",
                    LastName = "Manager",
                    Email = "espl.manager3@eternussolutions.com"
                }

            };

            if (!(await _roleMgr.RoleExistsAsync("Manager")))
            {
                var role = new IdentityRole("Manager");
                role.Claims.Add(new IdentityRoleClaim<string>()
                {
                    ClaimType = "IsManager",
                    ClaimValue = "True"
                });
                await _roleMgr.CreateAsync(role);
            }

            foreach (var user in allUsers)
            {
                var userResult = await _userMgr.CreateAsync(user, "Espl@123");
                var roleResult = await _userMgr.AddToRoleAsync(user, "Manager");

                if (!userResult.Succeeded || !roleResult.Succeeded)
                {
                    throw new InvalidOperationException("Failed to build user and roles");
                }
            }
        }

        public async Task AddAllAdmins()
        {
            List<AppUser> allUsers = new List<AppUser>() {
                new AppUser() {
                    UserName = "tomcruise",
                    Id = "56c385ae-ce46-41d4-b7fe-08df9aef7102",
                    FirstName = "Tom",
                    LastName = "Cruise",
                    Email = "tom.cruise@eternussolutions.com"
                },
                // new AppUser() {
                //     UserName = "espladmin1",
                //     Id = "56c385ae-ce46-41d4-b7fe-08df9aef7102",
                //     FirstName = "ESPL1",
                //     LastName = "Admin1",
                //     Email = "espl.admin1@eternussolutions.com"
                // }

            };

            if (!(await _roleMgr.RoleExistsAsync("SystemAdmin")))
            {
                var role = new IdentityRole("SystemAdmin");
                role.Claims.Add(new IdentityRoleClaim<string>()
                {
                    ClaimType = "SystemAdmin",
                    ClaimValue = "True"
                });
                await _roleMgr.CreateAsync(role);
            }

            foreach (var adminUser in allUsers)
            {
                var adminUserResult = await _userMgr.CreateAsync(adminUser, "Espl@123");
                var adminRoleResult = await _userMgr.AddToRoleAsync(adminUser, "SystemAdmin");

                if (!adminUserResult.Succeeded || !adminRoleResult.Succeeded)
                {
                    throw new InvalidOperationException("Failed to build user and roles");
                }
            }

        }

        public async Task AddAllConstables()
        {
            List<AppUser> allUsers = new List<AppUser>()
            {
                new AppUser()
                {
                    Id = "56c385ae-ce46-41d4-b7fe-08df9aef7301",
                    UserName = "tonystark",
                    FirstName = "Tony",
                    LastName = "Stark",
                    Email = "tony.stark@kenyapolice.com"
                },new AppUser()
                {
                    Id = "56c385ae-ce46-41d4-b7fe-08df9aef7302",
                    UserName = "steverogers",
                    FirstName = "Steve",
                    LastName = "Rogers",
                    Email = "steve.rogers@kenyapolice.com"
                }
            };

            if (!(await _roleMgr.RoleExistsAsync("Constable")))
            {
                var role = new IdentityRole("Constable");
                role.Claims.Add(new IdentityRoleClaim<string>()
                {
                    ClaimType = "IsConstable",
                    ClaimValue = "True"
                });
                await _roleMgr.CreateAsync(role);
            }

            await AddUserWithRole(allUsers, "Constable", "Espl@123");

        }

        public async Task AddAllSergeant()
        {
            List<AppUser> allUsers = new List<AppUser>()
            {
                new AppUser()
                {
                    Id = "56c385ae-ce46-41d4-b7fe-08df9aef7303",
                    UserName = "bradpitt",
                    FirstName = "Brad",
                    LastName = "Pitt",
                    Email = "brad.pitt@kenyapolice.com"
                }
            };

            if (!(await _roleMgr.RoleExistsAsync("Sergeant")))
            {
                var role = new IdentityRole("Sergeant");
                role.Claims.Add(new IdentityRoleClaim<string>()
                {
                    ClaimType = "IsSergeant",
                    ClaimValue = "True"
                });
                await _roleMgr.CreateAsync(role);
            }

            await AddUserWithRole(allUsers, "Sergeant", "Espl@123");
        }

        public async Task AddAllSAIG()
        {
            List<AppUser> allUsers = new List<AppUser>()
            {
                new AppUser()
                {
                    Id = "56c385ae-ce46-41d4-b7fe-08df9aef7201",
                    UserName = "angelinajolie",
                    FirstName = "Angelina",
                    LastName = "Jolie",
                    Email = "angelina.jolie@kenyapolice.com"
                }
            };

            if (!(await _roleMgr.RoleExistsAsync("SAIG")))
            {
                var role = new IdentityRole("SAIG");
                role.Claims.Add(new IdentityRoleClaim<string>()
                {
                    ClaimType = "IsSAIG",
                    ClaimValue = "True"
                });
                await _roleMgr.CreateAsync(role);
            }

            await AddUserWithRole(allUsers, "SAIG", "Espl@123");
        }

        public async Task AddAllDIG()
        {
            List<AppUser> allUsers = new List<AppUser>()
            {
                new AppUser()
                {
                    Id = "56c385ae-ce46-41d4-b7fe-08df9aef7202",
                    UserName = "jacksparrow",
                    FirstName = "Jack",
                    LastName = "Sparrow",
                    Email = "jack.sparrow@kenyapolice.com"
                }
            };

            if (!(await _roleMgr.RoleExistsAsync("DIG")))
            {
                var role = new IdentityRole("DIG");
                role.Claims.Add(new IdentityRoleClaim<string>()
                {
                    ClaimType = "IsDIG",
                    ClaimValue = "True"
                });
                await _roleMgr.CreateAsync(role);
            }

            await AddUserWithRole(allUsers, "DIG", "Espl@123");
        }

        public async Task AddAllIG()
        {
            List<AppUser> allUsers = new List<AppUser>()
            {
                new AppUser()
                {
                    Id = "56c385ae-ce46-41d4-b7fe-08df9aef7203",
                    UserName = "johndoe",
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@kenyapolice.com"
                }
            };

            if (!(await _roleMgr.RoleExistsAsync("IG")))
            {
                var role = new IdentityRole("IG");
                role.Claims.Add(new IdentityRoleClaim<string>()
                {
                    ClaimType = "IsIG",
                    ClaimValue = "True"
                });
                await _roleMgr.CreateAsync(role);
            }

            await AddUserWithRole(allUsers, "IG", "Espl@123");
        }

        public async Task AddAllASP()
        {
            List<AppUser> allUsers = new List<AppUser>()
            {
                new AppUser()
                {
                    Id = "56c385ae-ce46-41d4-b7fe-08df9aef7204",
                    UserName = "johnydepp",
                    FirstName = "Johny",
                    LastName = "Depp",
                    Email = "johny.depp@kenyapolice.com"
                }
            };

            if (!(await _roleMgr.RoleExistsAsync("ASP")))
            {
                var role = new IdentityRole("ASP");
                role.Claims.Add(new IdentityRoleClaim<string>() { ClaimType = "IsASP", ClaimValue = "True" });
                await _roleMgr.CreateAsync(role);
            }

            await AddUserWithRole(allUsers, "ASP", "Espl@123");
        }

        public async Task AddAllSuperAdmins()
        {
            List<AppUser> allUsers = new List<AppUser>()
            {
                new AppUser()
                {
                    Id = "56c385ae-ce46-41d4-b7fe-08df9aef7101",
                    UserName = "nickjones",
                    FirstName = "Nick",
                    LastName = "Jones",
                    Email = "nick.jones@kenyapolice.com"
                }
            };

            if (!(await _roleMgr.RoleExistsAsync("SuperAdmin")))
            {
                var role = new IdentityRole("SuperAdmin");
                role.Claims.Add(new IdentityRoleClaim<string>() { ClaimType = "IsSuperAdmin", ClaimValue = "True" });
                await _roleMgr.CreateAsync(role);
            }

            await AddUserWithRole(allUsers, "SuperAdmin", "Espl@123");
        }

        public async Task AddUserWithRole(List<AppUser> allUsers, string roleName, string password)
        {


            foreach (AppUser user in allUsers)
            {
                var userResult = await _userMgr.CreateAsync(user, password);
                var roleResult = await _userMgr.AddToRoleAsync(user, roleName);

                if (!userResult.Succeeded || !roleResult.Succeeded)
                {
                    throw new InvalidOperationException("Failed to build user and roles");
                }
            }
        }

    }
}
