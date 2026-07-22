using System.Text.Json.Serialization;

namespace MealPlannerAPI.Models.DTOs.Response
{
    public class IngredientResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Unit { get; internal set; } = string.Empty;
        public decimal InStockAmount { get; set; }
        public List<int> UsedInRecipes { get; set; } = [];
    }
}
