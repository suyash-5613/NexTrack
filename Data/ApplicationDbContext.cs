using Microsoft.EntityFrameworkCore;
using NexTrack.Models;

namespace NexTrack.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Item> Items { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Self-referencing relationship
            modelBuilder.Entity<Item>()
                .HasOne(i => i.Parent)
                .WithMany(i => i.Children)
                .HasForeignKey(i => i.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Item>()
                .Property(i => i.Status)
                .HasDefaultValue("pending");

            modelBuilder.Entity<Item>()
                .HasIndex(i => i.ParentId);

            modelBuilder.Entity<Item>()
                .HasIndex(i => i.Status);

            // Seed data
            modelBuilder.Entity<Item>().HasData(
                new Item
                {
                    Id = 1,
                    Name = "Raw Material Batch A",
                    Weight = 1000m,
                    ParentId = null,
                    Status = "pending",
                    CreatedAt = new DateTime(2026, 3, 25, 10, 0, 0, DateTimeKind.Utc)
                },
                new Item
                {
                    Id = 2,
                    Name = "Raw Material Batch B",
                    Weight = 500m,
                    ParentId = null,
                    Status = "processed",
                    CreatedAt = new DateTime(2026, 3, 25, 10, 0, 0, DateTimeKind.Utc)
                },
                new Item
                {
                    Id = 3,
                    Name = "Component X",
                    Weight = 200m,
                    ParentId = 2,
                    Status = "pending",
                    CreatedAt = new DateTime(2026, 3, 25, 10, 0, 0, DateTimeKind.Utc)
                },
                new Item
                {
                    Id = 4,
                    Name = "Component Y",
                    Weight = 300m,
                    ParentId = 2,
                    Status = "pending",
                    CreatedAt = new DateTime(2026, 3, 25, 10, 0, 0, DateTimeKind.Utc)
                },
                new Item
                {
                    Id = 5,
                    Name = "Raw Material Batch C",
                    Weight = 1500m,
                    ParentId = null,
                    Status = "pending",
                    CreatedAt = new DateTime(2026, 3, 25, 10, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}
