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
            modelBuilder.Entity<Recipe>().HasMany(r => r.RecipeIngredients)
                .WithOne(ri => ri.Recipe)
                .HasForeignKey(ri => ri.RecipeId);

            modelBuilder.Entity<Ingredient>().HasMany(i => i.RecipeIngredients)
                .WithOne(ri => ri.Ingredient)
                .HasForeignKey(ri => ri.IngredientId);

            modelBuilder.Entity<RecipeIngredients>(entity =>
            {
                entity.HasKey(ri => new { ri.RecipeId, ri.IngredientId });
                entity.HasOne(ri => ri.Recipe)
                    .WithMany(r => r.RecipeIngredients)
                    .HasForeignKey(ri => ri.RecipeId);
                entity.HasOne(ri => ri.Ingredient)
                    .WithMany(i => i.RecipeIngredients)
                    .HasForeignKey(ri => ri.IngredientId);
                entity.Property(i => i.Quantity).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<Inventory>(entity =>
            {
                entity.HasOne(i => i.Ingredient)
                    .WithMany()
                    .HasForeignKey(i => i.IngredientId);
                entity.Property(i => i.Quantity).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Seed();
        }

        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<RecipeIngredients> RecipeIngredients { get; set; }
        public DbSet<Inventory> Inventory { get; set; }
    }
}
