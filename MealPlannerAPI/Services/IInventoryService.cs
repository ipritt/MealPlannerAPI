using MealPlannerAPI.Models.DTOs.Create;
using MealPlannerAPI.Models.DTOs.Response;

namespace MealPlannerAPI.Services
{
    public interface IInventoryService
    {
        Task<bool> InventoryExistsAsync(int? id);
        Task<IEnumerable<InventoryResponseDTO>> GetInventoryAsync();
        Task<InventoryResponseDTO?> GetInventoryByIdAsync(int id);
        Task<InventoryResponseDTO?> PostInventoryAsync(CreateInventoryDTO inventoryDTO);
    }
}
