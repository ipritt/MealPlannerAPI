namespace MealPlannerAPI.Models
{
    public class RecipeIngredients
    {
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; } = null!;
        public int IngredientId { get; set; }
        public Ingredient Ingredient { get; set; } = null!;

        public decimal Quantity { get; set; }
        public string Unit { get; set; }
    }
}
