using MealPlannerAPI.Context;
using MealPlannerAPI.Models.DTOs.Create;
using MealPlannerAPI.Models.DTOs.Response;
using MealPlannerAPI.Models.DTOs.Request;
using MealPlannerAPI.Models.Utility;
using Microsoft.EntityFrameworkCore;
using System.Data;
using MealPlannerAPI.Models.Entities;

namespace MealPlannerAPI.Services
{
    public class IngredientService(IDbContextFactory<PlannerContext> contextFactory) : IIngredientService
    {
        private readonly IDbContextFactory<PlannerContext> _contextFactory = contextFactory;

        public async Task<Result<IEnumerable<IngredientResponseDTO>>> GetIngredientsAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var allIngredients = await context.Ingredients
                .Include(i => i.Recipes)
                .ToListAsync();

            var inStockAmount = await context.Inventory
                .Include(inv => inv.Ingredient)
                .Where(inv => inv.IngredientId == inv.Ingredient.Id)
                .Select(inv => inv.InStockAmount)
                .FirstOrDefaultAsync();

            var ingredientResponseDTO = allIngredients
                .Select(i => i.ToResponseDTO(i, inStockAmount)).ToList();

            // TODO: handle validations in separate validation layer or service,
            // and return a list of errors
            var errors = new List<Error>();

            if (ingredientResponseDTO == null || ingredientResponseDTO.Count == 0)
            {
                errors.Add(new Error("Ingredients", "No ingredients found.", ErrorType.NotFound));
            }

            if (errors.Count != 0)
            {
                return Result<IEnumerable<IngredientResponseDTO>>.Failure(errors);
            }

            return Result<IEnumerable<IngredientResponseDTO>>.Success(ingredientResponseDTO);
        }

        public async Task<Result<IngredientResponseDTO>> GetIngredientByIdAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var ingredient = await context.Ingredients
                .Include(i => i.Recipes)
                .FirstOrDefaultAsync(i => i.Id == id);

            var inStockAmount = await context.Inventory
                .Include(i => i.Ingredient)
                .Where(inv => inv.IngredientId == id)
                .Select(inv => inv.InStockAmount)
                .FirstOrDefaultAsync();

            // TODO: handle validations in separate validation layer or service,
            // and return a list of errors
            var errors = new List<Error>();

            if (ingredient == null)
            {
                errors.Add(new Error($"Ingredient.{id}", "Ingredient not found.", ErrorType.NotFound));
            }

            if (errors.Count != 0)
            {
                return Result<IngredientResponseDTO>.Failure(errors);
            }

            return Result<IngredientResponseDTO>.Success(ingredient?.ToResponseDTO(ingredient, inStockAmount));
        }

        public async Task<Result<IngredientResponseDTO>> UpdateIngredientAsync(IngredientRequestDTO ingredientRequestDTO, int? id)
        {
            if (id == null)
            {
                return Result<IngredientResponseDTO>.Failure([new("Ingredient.Id", "ID mismatch.", ErrorType.BadRequest)]);
            }

            using var context = await _contextFactory.CreateDbContextAsync();

            context.Entry(new Ingredient
            {
                Id = (int)id,
                Name = ingredientRequestDTO.Name,
                Category = ingredientRequestDTO.Category,
                Unit = ingredientRequestDTO.Unit
            }).State = EntityState.Modified;

            // TODO: handle validations in separate validation layer or service,
            // and return a list of errors
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
                    errors.Add(new Error($"Ingredient.{id}", "Ingredient not found.", ErrorType.NotFound));
                }
                else
                {
                    throw;
                }

                if (errors.Count != 0)
                {
                    return Result<IngredientResponseDTO>.Failure(errors);
                }
            }

            return Result<IngredientResponseDTO>.Success(new IngredientResponseDTO());
        }

        public async Task<Result<IngredientResponseDTO>> CreateIngredientAsync(IngredientRequestDTO ingredientRequestDTO)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            // TODO: Implement proper ingredient existence check using a unique identifier (GUID cache) or other criteria
            // Check HybridCache library
            var ingredientExists = await context.Ingredients
                .AnyAsync(i => string.Equals(i.Name, ingredientRequestDTO.Name));

            // TODO: handle validations in separate validation layer or service,
            // and return a list of errors
            var errors = new List<Error>();

            if (ingredientExists)
            {
                errors.Add(new Error($"Ingredient.Name = {ingredientRequestDTO.Name}", 
                    "Ingredient already exists.", ErrorType.Conflict));

                return Result<IngredientResponseDTO>.Failure(errors);
            }

            var ingredient = new Ingredient
            {
                Name = ingredientRequestDTO.Name,
                Category = ingredientRequestDTO.Category,
                Unit = ingredientRequestDTO.Unit
            };

            // TODO: Add exception handling here
            await context.Ingredients.AddAsync(ingredient);
            await context.SaveChangesAsync();

            return Result<IngredientResponseDTO>.Success
            (
                ingredient.ToResponseDTO(await context.Ingredients
                    .Include(i => i.Recipes)
                    .FirstAsync(i => i.Name == ingredientRequestDTO.Name)
                    , null)
            );
        }

        public async Task<Result<IngredientResponseDTO>> DeleteIngredientAsync(int? id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var ingredient = await context.Ingredients
                .Include(i => i.Recipes).FirstOrDefaultAsync(i => i.Id == id);

            // TODO: handle validations in separate validation layer or service,
            // and return a list of errors
            var errors = new List<Error>();

            if (ingredient == null)
            {
                errors.Add(new Error("Ingredient", "Ingredient not found.", ErrorType.NotFound));
            }
            else if (ingredient.Recipes.Count != 0) // Don't delete the ingredient if it is used in any recipes
            {
                errors.Add(new Error("Ingredient", "Cannot delete an ingredient that is used in recipes.", ErrorType.Conflict));
            }
            else
            {
                context.Ingredients.Remove(ingredient);

                // TODO: Add exception handling here
                await context.SaveChangesAsync();
            }

            if (errors.Count != 0)
            {
                return Result<IngredientResponseDTO>.Failure(errors);
            }

            return Result<IngredientResponseDTO>.Success(ingredient?.ToResponseDTO(ingredient, null));
        }
    }
}
