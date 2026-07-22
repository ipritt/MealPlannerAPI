using MealPlannerAPI.Models.DTOs.Create;
using MealPlannerAPI.Models.DTOs.Response;
using MealPlannerAPI.Models.DTOs.Update;
using MealPlannerAPI.Models.Utility;

namespace MealPlannerAPI.Services
{
    public interface IIngredientService
    {
        Task<Result<IEnumerable<IngredientResponseDTO>>> GetIngredientsAsync();
        Task<Result<IngredientResponseDTO>> GetIngredientByIdAsync(int id);
        Task<Result<IngredientResponseDTO>> CreateIngredientAsync(CreateIngredientDTO createIngredientDTO);
        Task<Result<IngredientResponseDTO>> UpdateIngredientAsync(UpdateIngredientDTO updateIngredientDTO, int? id);
        Task<Result<IngredientResponseDTO>> DeleteIngredientAsync(int? id);
    }
}
