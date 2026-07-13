namespace MealPlannerAPI.Models.Entities
{
    public class RecipeIngredients
    {
        public int RecipeId { get; set; }
        public int IngredientId { get; set; }
        public decimal Quantity { get; set; }
    }
}