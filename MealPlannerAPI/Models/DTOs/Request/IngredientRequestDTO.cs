using MealPlannerAPI.Models.DTOs.Create;
using MealPlannerAPI.Models.Entities;

namespace MealPlannerAPI.Models.DTOs.Request
{
    public class IngredientRequestDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
    }
}
