using System.Text.Json.Serialization;

namespace MealPlannerAPI.Models
{
    public class Recipe
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Instructions { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;

        public List<RecipeIngredients> RecipeIngredients  { get; } = [];
    }
}
