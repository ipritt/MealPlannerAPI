using MealPlannerAPI.Models.DTOs.Request;
using MealPlannerAPI.Models.DTOs.Response;
using MealPlannerAPI.Models.Utility;

namespace MealPlannerAPI.Services
{
    public interface IIngredientService
    {
        Task<Result<IEnumerable<IngredientResponseDTO>>> GetIngredientsAsync();
        Task<Result<IngredientResponseDTO>> GetIngredientByIdAsync(int id);
        Task<Result<IngredientResponseDTO>> CreateIngredientAsync(IngredientRequestDTO createIngredientDTO);
        Task<Result<IngredientResponseDTO>> UpdateIngredientAsync(IngredientRequestDTO updateIngredientDTO, int? id);
        Task<Result<IngredientResponseDTO>> DeleteIngredientAsync(int? id);
    }
}
