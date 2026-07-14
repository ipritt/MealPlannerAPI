using MealPlannerAPI.Models.DTOs.Response;

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
                Ingredients = [.. recipe.Ingredients.Select(i => new IngredientResponseDTO
                {
                    Id = i.Id,
                    Name = i.Name,
                    Category = i.Category,
                    Unit = i.Unit,
                    UsedInRecipes = [.. i.Recipes.Select(r => r.Id)]
                })]
            };
        }
    }
}
