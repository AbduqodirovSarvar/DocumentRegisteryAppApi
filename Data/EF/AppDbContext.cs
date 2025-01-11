using DocumentRegisteryAppApi.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DocumentRegisteryAppApi.Data.EF
{
    public class AppDbContext(
        DbContextOptions<AppDbContext> options
        ) : DbContext(options)
    {
        public DbSet<DocumentEntity> Documents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<DocumentEntity>().HasKey(e => e.Id);
        }
    }
}
