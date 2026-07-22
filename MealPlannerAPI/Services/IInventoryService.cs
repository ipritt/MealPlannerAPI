using MealPlannerAPI.Models.DTOs.Request;
using MealPlannerAPI.Models.DTOs.Response;
using MealPlannerAPI.Models.Utility;

namespace MealPlannerAPI.Services
{
    public interface IInventoryService
    {
        Task<Result<IEnumerable<InventoryResponseDTO>>> GetInventoryAsync();
        Task<Result<InventoryResponseDTO>> GetInventoryByIdAsync(int id);
        Task<Result<InventoryResponseDTO>> PostInventoryAsync(InventoryRequestDTO inventoryRequestDTO);
        Task<Result<InventoryResponseDTO>> PutInventoryAsync(InventoryRequestDTO inventoryRequestDTO, int? id);
        Task<Result<InventoryResponseDTO>> DeleteInventoryAsync(int? id);
    }
}
