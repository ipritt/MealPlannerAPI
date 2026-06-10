using Microsoft.EntityFrameworkCore;

namespace MealPlannerAPI.Models
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ingredient>().HasData(
                new Ingredient { Id = 1, Name = "Flour", Category = "Baking" },
                new Ingredient { Id = 2, Name = "Sugar", Category = "Baking" },
                new Ingredient { Id = 3, Name = "Eggs", Category = "Dairy" }
            );
            modelBuilder.Entity<Recipe>().HasData(
                new Recipe { Id = 1, Name = "Pancakes", Instructions = "Mix ingredients and cook on griddle.", Unit = "servings" }
            );
            modelBuilder.Entity<RecipeIngredients>().HasData(
                new RecipeIngredients { RecipeId = 1, IngredientId = 1, Quantity = 2, Unit = "cups" },
                new RecipeIngredients { RecipeId = 1, IngredientId = 2, Quantity = 0.5m, Unit = "cups" },
                new RecipeIngredients { RecipeId = 1, IngredientId = 3, Quantity = 2, Unit = "large" }
            );
            modelBuilder.Entity<Inventory>().HasData(
                new Inventory { Id = 1, IngredientId = 1, Quantity = 10, Unit = "cups" },
                new Inventory { Id = 2, IngredientId = 2, Quantity = 5, Unit = "cups" },
                new Inventory { Id = 3, IngredientId = 3, Quantity = 12, Unit = "large" }
            );
        }
    }
}
