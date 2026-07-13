using MealPlannerAPI.Models.DTOs.Create;
using MealPlannerAPI.Models.DTOs.Response;
using MealPlannerAPI.Models.Utility;

namespace MealPlannerAPI.Services
{
    public interface IIngredientService
    {
        Task<Result<IEnumerable<IngredientResponseDTO>>> GetIngredientsAsync();
        Task<Result<IngredientResponseDTO>> GetIngredientByIdAsync(int id);
        Task<Result<IngredientResponseDTO>> PostIngredientAsync(CreateIngredientDTO createIngredientDTO);
        Task<Result<IngredientResponseDTO>> PutIngredientAsync(CreateIngredientDTO createIngredientDTO, int? id);
        Task<Result<IngredientResponseDTO>> DeleteIngredientAsync(int? id);
    }
}
