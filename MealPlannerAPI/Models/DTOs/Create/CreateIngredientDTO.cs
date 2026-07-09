namespace MealPlannerAPI.Models.DTOs.Create
{
    public class CreateIngredientDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
    }
}
