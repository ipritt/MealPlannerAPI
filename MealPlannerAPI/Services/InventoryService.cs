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

        public async Task<InventoryResponseDTO?> PostInventoryAsync(CreateInventoryDTO inventoryDTO)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var inventory = new Inventory
            {
                IngredientId = inventoryDTO.IngredientId,
                Name = inventoryDTO.Name,
                InStockAmount = inventoryDTO.InStockAmount,
                Unit = inventoryDTO.Unit
            };

            await context.Inventory.AddAsync(inventory);
            await context.SaveChangesAsync();

            return new InventoryResponseDTO
            {
                Id = inventory.Id,
                IngredientId = inventoryDTO.IngredientId,
                Name = inventory.Name,
                InStockAmount = inventory.InStockAmount,
                Unit = inventory.Unit
            };
        }

        private async Task<IEnumerable<InventoryResponseDTO>> GetAllInventoryAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            return await context.Inventory
                .Include(inv => inv.Ingredient)
                .Select(inv => new InventoryResponseDTO
                {
                    Id = inv.Id,
                    IngredientId = inv.Ingredient.Id,
                    Name = inv.Ingredient.Name,
                    InStockAmount = inv.InStockAmount,
                    Unit = inv.Ingredient.Unit,
                }).ToListAsync();
        }
    }
}
