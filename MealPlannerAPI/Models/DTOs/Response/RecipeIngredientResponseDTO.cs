namespace MealPlannerAPI.Models.DTOs.Response
{
    public class RecipeIngredientResponseDTO
    {
        public int IngredientId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Unit { get; internal set; } = string.Empty;
        public decimal Quantity { get; set; }
    }
}
