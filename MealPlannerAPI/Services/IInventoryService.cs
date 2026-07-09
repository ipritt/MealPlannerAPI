using MealPlannerAPI.Models.DTOs.Create;
using MealPlannerAPI.Models.DTOs.Response;
using Microsoft.AspNetCore.Mvc;

namespace MealPlannerAPI.Services
{
    public interface IInventoryService
    {
        Task<bool> InventoryExistsAsync(int? id);
        Task<IEnumerable<InventoryResponseDTO>> GetInventoryAsync();
        Task<InventoryResponseDTO?> GetInventoryByIdAsync(int id);
        Task<InventoryResponseDTO?> PostInventoryAsync(CreateInventoryDTO inventoryDTO);

        Task<InventoryResponseDTO?> DeleteInventoryAsync(int? id);
    }
}
