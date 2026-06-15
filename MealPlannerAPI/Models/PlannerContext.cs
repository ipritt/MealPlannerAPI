using Microsoft.EntityFrameworkCore;

namespace MealPlannerAPI.Models
{
    public class PlannerContext : DbContext
    {
        public PlannerContext(DbContextOptions<PlannerContext> options)
            : base(options)
        {
          
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RecipeIngredients>()
                .HasKey(ri => new { ri.RecipeId, ri.IngredientId });

            modelBuilder.Seed();
        }

        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Inventory> Inventory { get; set; }
    }
}
