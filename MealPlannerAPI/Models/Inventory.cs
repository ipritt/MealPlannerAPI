namespace MealPlannerAPI.Models
{
    public class Inventory
    {
        public int Id { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;


        public int IngredientId { get; set; }
        public virtual Ingredient Ingredient { get; set; }
    }
}
