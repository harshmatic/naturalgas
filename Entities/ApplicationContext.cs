using System.Linq;
using ESPL.NG.Entities.Core;
// using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace ESPL.NG.Entities
{
    //public class ApplicationContext : IdentityDbContext<AppUser>
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            //Database.Migrate();
        }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<AppUser> AppUser { get; set; }
        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {

            foreach (var relationship in modelbuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
            base.OnModelCreating(modelbuilder);

            //Addeing Index on Customer
            modelbuilder.Entity<Customer>()
            .HasIndex(p => new { p.IsDelete, p.CustomerName, p.CustomerEmail })
            .IsUnique();
        }

    }
}