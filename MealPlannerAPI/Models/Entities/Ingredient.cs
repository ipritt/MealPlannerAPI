using MealPlannerAPI.Models.DTOs.Response;

namespace MealPlannerAPI.Models.Entities
{
    public class Ingredient()
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Unit { get; internal set; } = string.Empty;
        public ICollection<Recipe> Recipes { get; set; } = [];

        public IngredientResponseDTO ToResponseDTO(Ingredient ingredient, decimal? inStockAmount)
        {
            return new IngredientResponseDTO
            {
                Id = ingredient.Id,
                Name = ingredient.Name,
                Category = ingredient.Category,
                Unit = ingredient.Unit,
                InStockAmount = inStockAmount ?? 0,
                UsedInRecipes = [.. ingredient.Recipes.Select(r => r.Id)]
            };
        }
    }
}
