namespace MealPlannerAPI.Models.DTOs.Create
{
    public class CreateRecipeDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Instructions { get; set; } = string.Empty;
        public ICollection<CreateIngredientDTO> Ingredients { get; set; } = [];
    }
}
