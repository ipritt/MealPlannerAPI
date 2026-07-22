using MealPlannerAPI.Models.DTOs.Update;

namespace MealPlannerAPI.Models.DTOs.Request
{
    public class RecipeRequestDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Instructions { get; set; } = string.Empty;
        public ICollection<UpdateRecipeIngredientDTO> RecipeIngredients { get; set; } = [];
    }
}
