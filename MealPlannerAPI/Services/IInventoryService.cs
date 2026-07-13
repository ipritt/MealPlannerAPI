using MealPlannerAPI.Models.DTOs.Create;
using MealPlannerAPI.Models.DTOs.Response;
using MealPlannerAPI.Models.Utility;

namespace MealPlannerAPI.Services
{
    public interface IInventoryService
    {
        Task<Result<IEnumerable<InventoryResponseDTO>>> GetInventoryAsync();
        Task<Result<InventoryResponseDTO>> GetInventoryByIdAsync(int id);
        Task<Result<InventoryResponseDTO>> PostInventoryAsync(CreateInventoryDTO createInventoryDTO);
        Task<Result<InventoryResponseDTO>> PutInventoryAsync(CreateInventoryDTO createInventoryDTO, int? id);
        Task<Result<InventoryResponseDTO>> DeleteInventoryAsync(int? id);
    }
}
