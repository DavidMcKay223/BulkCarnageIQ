using BulkCarnageIQ.Core.Carnage;
using BulkCarnageIQ.Core.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BulkCarnageIQ.Infrastructure.Persistence
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<FoodItem> FoodItems { get; set; } = null!;
        public DbSet<MealEntry> MealEntries { get; set; } = null!;
        public DbSet<GroceryListItem> GroceryListItems { get; set; } = null!;
        public DbSet<GroupFoodItem> GroupFoodItems { get; set; } = null!;
        public DbSet<GroupFoodItemEntry> GroupFoodItemEntries { get; set; } = null!;
        public DbSet<WeightLog> WeightLogs { get; set; } = null!;
        public DbSet<UserProfile> UserProfiles { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FoodItem>()
                .HasKey(f => f.RecipeName);

            modelBuilder.Entity<GroupFoodItemEntry>()
                .HasOne(e => e.GroupFoodItem)
                .WithMany(g => g.Entries)
                .HasForeignKey(e => e.GroupFoodItemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserProfile>()
                .HasKey(f => f.UserName);
        }
    }
}
