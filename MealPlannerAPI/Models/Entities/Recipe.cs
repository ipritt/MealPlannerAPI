using MealPlannerAPI.Models.DTOs.Response;
using System.Text.Json.Serialization;

namespace MealPlannerAPI.Models.Entities
{
    public class Recipe
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Instructions { get; set; } = string.Empty;
        public ICollection<Ingredient> Ingredients { get; set; } = [];
        public ICollection<RecipeIngredients> RecipeIngredients { get; set; } = [];

        public RecipeResponseDTO ToResponseDTO(Recipe recipe)
        {
            return new RecipeResponseDTO
            {
                Id = recipe.Id,
                Name = recipe.Name,
                Instructions = recipe.Instructions,
                RecipeIngredients = [.. recipe.RecipeIngredients
                .Select(ri => new RecipeIngredientResponseDTO
                    {
                        IngredientId = ri.IngredientId,
                        Name = recipe.Ingredients.First(i => i.Id == ri.IngredientId).Name,
                        Category = recipe.Ingredients.First(i => i.Id == ri.IngredientId).Category,
                        Unit = recipe.Ingredients.First(i => i.Id == ri.IngredientId).Unit,
                        Quantity = ri.Quantity
                    }
                )]
            };
        }
    }
}
