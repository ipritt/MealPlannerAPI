using MealPlannerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MealPlannerAPI.Context
{
    public class PlannerContext(DbContextOptions<PlannerContext> options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Recipe>()
                .HasMany(r => r.Ingredients)
                .WithMany(r => r.Recipes)
                .UsingEntity<RecipeIngredients>();

            modelBuilder.Entity<RecipeIngredients>()
                .Property(ri => ri.Quantity)
                .HasPrecision(6, 2);

            modelBuilder.Entity<Inventory>()
                .Property(inv => inv.InStockAmount)
                .HasPrecision(6, 2);

            modelBuilder.Seed();
        }

        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<RecipeIngredients> RecipeIngredients { get; set; }
        public DbSet<Inventory> Inventory { get; set; }
    }
}
