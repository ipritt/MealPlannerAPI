using MealPlannerAPI.Models.Entities;

namespace MealPlannerAPI.Models.DTOs.Create
{
    public class CreateInventoryDTO
    {
        public int IngredientId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal InStockAmount { get; set; }
        public string Unit { get; set; } = string.Empty;

        public Inventory ToEntity(CreateInventoryDTO createInventoryDTO, int? id)
        {
            return new Inventory
            {
                Id = id ?? 0,
                IngredientId = createInventoryDTO.IngredientId,
                Name = createInventoryDTO.Name,
                InStockAmount = createInventoryDTO.InStockAmount,
                Unit = createInventoryDTO.Unit
            };
        }
    }
}
