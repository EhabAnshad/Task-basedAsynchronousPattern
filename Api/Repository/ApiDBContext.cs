using System.ComponentModel.DataAnnotations.Schema;
using Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Repository
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Batch>()
                .HasOne(p => p.Order)
                .WithMany(b => b.CompletedPerBatch)
                .HasForeignKey(p => p.OrderId)
                .HasConstraintName("ForeignKey_Post_Blog");
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<Batch> Batches { get; set; }
    }
}
