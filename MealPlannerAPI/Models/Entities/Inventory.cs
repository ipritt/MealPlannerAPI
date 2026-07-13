namespace MealPlannerAPI.Models.Entities
{
    public class Inventory
    {
        public int Id { get; set; }
        public int IngredientId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal InStockAmount { get; set; }
        public string Unit { get; set; } = string.Empty;
        public virtual Ingredient Ingredient { get; set; } = null!;
    }
}
