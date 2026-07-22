using MealPlannerAPI.Models.DTOs.Request;
using MealPlannerAPI.Models.DTOs.Response;
using MealPlannerAPI.Models.Utility;

namespace MealPlannerAPI.Services
{
    public interface IRecipeService
    {
        Task<Result<IEnumerable<RecipeResponseDTO>>> GetRecipesAsync();
        Task<Result<RecipeResponseDTO>> GetRecipeByIdAsync(int id);
        Task<Result<RecipeResponseDTO>> CreateRecipeAsync(RecipeRequestDTO recipeDTO);
        Task<Result<RecipeResponseDTO>> UpdateRecipeAsync(RecipeRequestDTO recipeDTO, int? id);
        Task<Result<RecipeResponseDTO>> DeleteRecipeAsync(int? id);
    }
}
