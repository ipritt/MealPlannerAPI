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
                .Select(inv => inv.ToResponseDTO(inv)).ToListAsync();

            // TODO: handle validations in separate validation layer or service,
            // and return a list of errors instead of throwing exceptions
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

            // TODO: handle validations in separate validation layer or service,
            // and return a list of errors instead of throwing exceptions
            var errors = new List<Error>();

            if (inventory == null)
            {
                errors.Add(new Error($"Inventory.{id}", "Inventory item not found.", ErrorType.NotFound));
            }

            if (errors.Count != 0)
            {
                return Result<InventoryResponseDTO>.Failure(errors);
            }

            return Result<InventoryResponseDTO>.Success(inventory?.ToResponseDTO(inventory));
        }

        public async Task<Result<InventoryResponseDTO>> PutInventoryAsync(CreateInventoryDTO createInventoryDTO, int? id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            context.Entry(createInventoryDTO.ToEntity(createInventoryDTO, id)).State = EntityState.Modified;

            // TODO: handle validations in separate validation layer or service,
            // and return a list of errors instead of throwing exceptions
            var errors = new List<Error>();

            try
            {
                // TODO: Add exception handling here
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

            // TODO: handle validations in separate validation layer or service,
            // and return a list of errors instead of throwing exceptions
            var errors = new List<Error>();

            if (ingredientExists)
            {
                errors.Add(new Error($"Inventory.Name = {createInventoryDTO.Name}",
                    "Inventory item already exists.", ErrorType.Conflict));

                return Result<InventoryResponseDTO>.Failure(errors);
            }

            var inventory = createInventoryDTO.ToEntity(createInventoryDTO, null);

            // TODO: Add exception handling here
            await context.Inventory.AddAsync(inventory);
            await context.SaveChangesAsync();

            return Result<InventoryResponseDTO>.Success
            (
                inventory.ToResponseDTO(await context.Inventory
                    .Include(i => i.Ingredient)
                    .FirstAsync(i => i.Name == createInventoryDTO.Name))
            );
        }

        public async Task<Result<InventoryResponseDTO>> DeleteInventoryAsync(int? id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var inventory = await context.Inventory
                .Include(i => i.Ingredient)
                .FirstOrDefaultAsync(i => i.Id == id);

            // TODO: handle validations in separate validation layer or service,
            // and return a list of errors instead of throwing exceptions
            var errors = new List<Error>();

            if (inventory == null)
            {
                errors.Add(new Error("Inventory", "Inventory item not found.", ErrorType.NotFound));
            }
            else if (inventory.Ingredient.Recipes.Count != 0) // Don't delete the inventory item if it is used in any recipes
            {
                errors.Add(new Error("Inventory", "Cannot delete an inventory item that is used in recipes.", ErrorType.Conflict));
            }
            else
            {
                context.Inventory.Remove(inventory);

                // TODO: Add exception handling here
                await context.SaveChangesAsync();
            }

            if (errors.Count != 0)
            {
                return Result<InventoryResponseDTO>.Failure(errors);
            }

            return Result<InventoryResponseDTO>.Success(inventory?.ToResponseDTO(inventory));
        }
    }
}
