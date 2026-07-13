using MealPlannerAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MealPlannerAPI.Context
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ingredient>().HasData(
                new Ingredient { Id = 1, Name = "Flour", Category = "Baking", Unit = "Cups" },
                new Ingredient { Id = 2, Name = "Sugar", Category = "Baking", Unit = "Cups" },
                new Ingredient { Id = 3, Name = "Eggs", Category = "Protiens", Unit = "Large" }
            );
            modelBuilder.Entity<Recipe>().HasData(
                new Recipe { Id = 1, Name = "Pancakes", Instructions = "Mix ingredients and cook on griddle." },
                new Recipe { Id = 2, Name = "Waffles", Instructions = "Mix ingredients and cook in waffle iron." }
            );
            modelBuilder.Entity<RecipeIngredients>().HasData(
                new RecipeIngredients { RecipeId = 1, IngredientId = 1, Quantity = 2 },
                new RecipeIngredients { RecipeId = 1, IngredientId = 2, Quantity = 0.5M },
                new RecipeIngredients { RecipeId = 1, IngredientId = 3, Quantity = 2 },
                new RecipeIngredients { RecipeId = 2, IngredientId = 1, Quantity = 3.0M },
                new RecipeIngredients { RecipeId = 2, IngredientId = 3, Quantity = 0.75M }
            );
            modelBuilder.Entity<Inventory>().HasData(
                new Inventory { Id = 1, Name="Flour", IngredientId = 1, InStockAmount = 10, Unit = "Cups" },
                new Inventory { Id = 2, Name = "Sugar", IngredientId = 2, InStockAmount = 5, Unit = "Cups" },
                new Inventory { Id = 3, Name = "Eggs", IngredientId = 3, InStockAmount = 12, Unit = "Large" }
            );
        }
    }
}
