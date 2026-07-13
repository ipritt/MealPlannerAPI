using MealPlannerAPI.Models.DTOs.Create;
using MealPlannerAPI.Models.DTOs.Response;
using MealPlannerAPI.Models.Utility;

namespace MealPlannerAPI.Services
{
    public interface IRecipeService
    {
        Task<Result<IEnumerable<RecipeResponseDTO>>> GetRecipesAsync();
        Task<Result<RecipeResponseDTO>> GetRecipeByIdAsync(int id);
        Task<Result<RecipeResponseDTO>> PostRecipeAsync(CreateRecipeDTO createRecipeDTO);
        Task<Result<RecipeResponseDTO>> PutRecipeAsync(CreateRecipeDTO createRecipeDTO, int? id);
        Task<Result<RecipeResponseDTO>> DeleteRecipeAsync(int? id);
    }
}
