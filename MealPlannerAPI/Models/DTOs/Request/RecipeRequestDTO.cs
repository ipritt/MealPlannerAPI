using MealPlannerAPI.Models.DTOs.Request;

namespace MealPlannerAPI.Models.DTOs.Request
{
    public class RecipeRequestDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Instructions { get; set; } = string.Empty;
        public ICollection<RecipeIngredientRequestDTO> RecipeIngredients { get; set; } = [];
    }
}
