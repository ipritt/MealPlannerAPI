using MealPlannerAPI.Models.DTOs.Response;

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

        public InventoryResponseDTO ToResponseDTO(Inventory inventory)
        {
            return new InventoryResponseDTO
            {
                Id = inventory.Id,
                IngredientId = inventory.IngredientId,
                Name = inventory.Ingredient.Name,
                InStockAmount = inventory.InStockAmount,
                Unit = inventory.Ingredient.Unit
            };
        }
    }
}
