using MealPlannerAPI.Models.DTOs.Create;
using MealPlannerAPI.Models.DTOs.Response;

namespace MealPlannerAPI.Services
{
    public interface IRecipeService
    {
        Task<bool> RecipeExistsAsync(int? id);
        Task<IEnumerable<RecipeResponseDTO>> GetRecipesAsync();
        Task<RecipeResponseDTO?> GetRecipeByIdAsync(int id);
        Task<RecipeResponseDTO?> PostRecipeAsync(CreateRecipeDTO recipeDTO);
    }
}
