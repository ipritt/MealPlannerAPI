using MealPlannerAPI.Models.DTOs.Create;
using MealPlannerAPI.Models.DTOs.Response;

namespace MealPlannerAPI.Services
{
    public interface IIngredientService
    {
        Task<bool> IngredientExistsAsync(int? id);
        Task<IEnumerable<IngredientResponseDTO>> GetIngredientsAsync();
        Task<IngredientResponseDTO?> GetIngredientByIdAsync(int id);
        Task<IngredientResponseDTO?> PostIngredientAsync(CreateIngredientDTO createIngredientDTO);
    }
}
