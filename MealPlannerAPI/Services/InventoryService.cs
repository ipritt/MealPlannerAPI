using MealPlannerAPI.Context;
using MealPlannerAPI.Models;
using MealPlannerAPI.Models.DTOs.Create;
using MealPlannerAPI.Models.DTOs.Response;
using Microsoft.EntityFrameworkCore;

namespace MealPlannerAPI.Services
{
    public class InventoryService(IDbContextFactory<PlannerContext> contextFactory) : IInventoryService
    {
        private readonly IDbContextFactory<PlannerContext> _contextFactory = contextFactory;

        public async Task<bool> InventoryExistsAsync(int? id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            return await context.Inventory.AnyAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<InventoryResponseDTO>> GetInventoryAsync()
        {
            return await GetAllInventoryAsync();
        }

        public async Task<InventoryResponseDTO?> GetInventoryByIdAsync(int id)
        {
            return GetAllInventoryAsync()
                .Result?.Where(inv => inv.Id == id).FirstOrDefault();
        }

        public async Task<InventoryResponseDTO?> PostInventoryAsync(CreateInventoryDTO createInventoryDTO)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var inventory = MapEntityFromResponse(createInventoryDTO);

            await context.Inventory.AddAsync(inventory);
            await context.SaveChangesAsync();

            return MapResponseFromEntity(inventory);
        }

        public async Task<InventoryResponseDTO?> DeleteInventoryAsync(int? id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var inventory = await context.Inventory.FindAsync(id);

            if (inventory == null)
            {
                return null;
            }

            context.Inventory.Remove(inventory);
            await context.SaveChangesAsync();

            return MapResponseFromEntity(inventory);
        }


        #region Private Methods
        private async Task<IEnumerable<InventoryResponseDTO>> GetAllInventoryAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            return await context.Inventory
                .Include(inv => inv.Ingredient)
                .Select(inv => MapResponseFromEntity(inv)).ToListAsync();
        }

        private static InventoryResponseDTO MapResponseFromEntity(Inventory inventory)
        {
            return new InventoryResponseDTO
            {
                Id = inventory.Id,
                IngredientId = inventory.IngredientId,
                Name = inventory.Name,
                InStockAmount = inventory.InStockAmount,
                Unit = inventory.Unit
            };
        }

        private static Inventory MapEntityFromResponse(CreateInventoryDTO createInventoryDTO)
        {
            return new Inventory
            {
                IngredientId = createInventoryDTO.IngredientId,
                Name = createInventoryDTO.Name,
                InStockAmount = createInventoryDTO.InStockAmount,
                Unit = createInventoryDTO.Unit
            };
        }

        #endregion Private Methods
    }
}
