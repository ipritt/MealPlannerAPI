namespace MealPlannerAPI.Models.DTOs.Response
{
    public class RecipeResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Instructions { get; set; } = string.Empty;
        public ICollection<RecipeIngredientResponseDTO> RecipeIngredients { get; set; } = [];
    }
}
