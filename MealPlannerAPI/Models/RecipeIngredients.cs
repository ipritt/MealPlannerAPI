using System.Text.Json.Serialization;

namespace MealPlannerAPI.Models
{
    public class RecipeIngredients
    {
        [JsonIgnore]
        public int RecipeId { get; set; }

        public Recipe Recipe { get; set; } = null!;

        [JsonIgnore]
        public int IngredientId { get; set; }

        public Ingredient Ingredient { get; set; } = null!;

        public decimal Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
    }
}
