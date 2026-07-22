namespace MealPlannerAPI.Models.DTOs.Request
{
    public class InventoryRequestDTO
    {
        public int IngredientId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal InStockAmount { get; set; }
        public string Unit { get; set; } = string.Empty;
    }
}
