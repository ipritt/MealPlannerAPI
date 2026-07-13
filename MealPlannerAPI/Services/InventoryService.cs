using MealPlannerAPI.Context;
using MealPlannerAPI.Models.DTOs.Create;
using MealPlannerAPI.Models.DTOs.Response;
using MealPlannerAPI.Models.Entities;
using MealPlannerAPI.Models.Utility;
using Microsoft.EntityFrameworkCore;

namespace MealPlannerAPI.Services
{
    public class InventoryService(IDbContextFactory<PlannerContext> contextFactory) : IInventoryService
    {
        private readonly IDbContextFactory<PlannerContext> _contextFactory = contextFactory;

        public async Task<Result<IEnumerable<InventoryResponseDTO>>> GetInventoryAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var inventoryResponseDTO = await context.Inventory
                .Include(inv => inv.Ingredient)
                .Select(inv => MapResponseFromEntity(inv)).ToListAsync();

            var errors = new List<Error>();

            if (inventoryResponseDTO == null || inventoryResponseDTO.Count == 0)
            {
                errors.Add(new Error("Inventory", "No inventory items found.", ErrorType.NotFound));
            }

            if (errors.Count != 0)
            {
                return Result<IEnumerable<InventoryResponseDTO>>.Failure(errors);
            }

            return Result<IEnumerable<InventoryResponseDTO>>.Success(inventoryResponseDTO);
        }           

        public async Task<Result<InventoryResponseDTO>> GetInventoryByIdAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var inventory = await context.Inventory
                .Include(inv => inv.Ingredient)
                .FirstOrDefaultAsync(inv => inv.Id == id);

            var errors = new List<Error>();

            if (inventory == null)
            {
                errors.Add(new Error($"Inventory.{id}", "Inventory item not found.", ErrorType.NotFound));
            }

            if (errors.Count != 0)
            {
                return Result<InventoryResponseDTO>.Failure(errors);
            }

            return Result<InventoryResponseDTO>.Success(MapResponseFromEntity(inventory));
        }

        public async Task<Result<InventoryResponseDTO>> PutInventoryAsync(CreateInventoryDTO createInventoryDTO, int? id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            context.Entry(MapEntityFromResponse(createInventoryDTO, id)).State = EntityState.Modified;

            var errors = new List<Error>();

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // TODO: Implement proper ingredient existence check using a unique identifier (GUID cache) or other criteria
                // Check HybridCache library
                if (!await context.Ingredients.AnyAsync(i => i.Id == id))
                {
                    errors.Add(new Error($"Inventory.{id}", "Inventory item not found.", ErrorType.NotFound));
                }
                else
                {
                    throw;
                }

                if (errors.Count != 0)
                {
                    return Result<InventoryResponseDTO>.Failure(errors);
                }
            }

            return Result<InventoryResponseDTO>.Success(new InventoryResponseDTO());
        }

        public async Task<Result<InventoryResponseDTO>> PostInventoryAsync(CreateInventoryDTO createInventoryDTO)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            // TODO: Implement proper ingredient existence check using a unique identifier (GUID cache) or other criteria
            // Check HybridCache library
            var ingredientExists = await context.Ingredients
                .AnyAsync(i => string.Equals(i.Name, createInventoryDTO.Name));

            var errors = new List<Error>();

            if (ingredientExists)
            {
                errors.Add(new Error($"Inventory.Name = {createInventoryDTO.Name}",
                    "Inventory item already exists.", ErrorType.Conflict));

                return Result<InventoryResponseDTO>.Failure(errors);
            }

            var inventory = MapEntityFromResponse(createInventoryDTO, null);

            await context.Inventory.AddAsync(inventory);
            await context.SaveChangesAsync();

            return Result<InventoryResponseDTO>.Success
            (
                MapResponseFromEntity(await context.Inventory
                    .Include(i => i.Ingredient)
                    .FirstOrDefaultAsync(i => i.Name == createInventoryDTO.Name))
            );
        }

        public async Task<Result<InventoryResponseDTO>> DeleteInventoryAsync(int? id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var inventory = await context.Inventory
                .Include(i => i.Ingredient)
                .FirstOrDefaultAsync(i => i.Id == id);

            var errors = new List<Error>();

            if (inventory == null)
            {
                errors.Add(new Error("Inventory", "Inventory item not found.", ErrorType.NotFound));
            }

            // Don't delete the inventory item if it is used in any recipes
            if (inventory == null || inventory.Ingredient.Recipes.Count != 0)
            {
                errors.Add(new Error("Inventory", "Cannot delete inventory item that is used in recipes.", ErrorType.Conflict));
            }

            if (errors.Count != 0)
            {
                return Result<InventoryResponseDTO>.Failure(errors);
            }

            context.Inventory.Remove(inventory ?? new Inventory());
            await context.SaveChangesAsync();

            return Result<InventoryResponseDTO>.Success(MapResponseFromEntity(inventory));
        }


        #region Private Methods

        // TODO: Consider using AutoMapper for mapping between entities
        // or handle these mappings in the models themselves, or in a separate mapping class
        private static InventoryResponseDTO MapResponseFromEntity(Inventory? inventory)
        {
            return inventory == null ? new InventoryResponseDTO() : new InventoryResponseDTO
            {
                Id = inventory.Id,
                IngredientId = inventory.IngredientId,
                Name = inventory.Ingredient.Name,
                InStockAmount = inventory.InStockAmount,
                Unit = inventory.Ingredient.Unit
            };
        }

        private static Inventory MapEntityFromResponse(CreateInventoryDTO createInventoryDTO, int? id)
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

        #endregion Private Methods
    }
}
