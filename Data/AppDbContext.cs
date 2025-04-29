using Authorization_Refreshtoken.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Authorization_Refreshtoken.Data
{
    public class AppDbContext:IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {
            
        }

        public DbSet<Items> Items { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Items>().HasKey(x => x.Id);
            builder.Entity<AppUser>().HasKey(x => x.Id);
            builder.Entity<AppUser>().Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Entity<Items>().Property(x => x.Name).HasMaxLength(40);
            builder.Entity<Items>().Property(x => x.Gender).HasMaxLength(6);
        }
    }
}
