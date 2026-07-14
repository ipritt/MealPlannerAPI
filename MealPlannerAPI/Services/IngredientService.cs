using MealPlannerAPI.Context;
using MealPlannerAPI.Models.DTOs.Create;
using MealPlannerAPI.Models.DTOs.Response;
using MealPlannerAPI.Models.Entities;
using MealPlannerAPI.Models.Utility;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace MealPlannerAPI.Services
{
    public class IngredientService(IDbContextFactory<PlannerContext> contextFactory) : IIngredientService
    {
        private readonly IDbContextFactory<PlannerContext> _contextFactory = contextFactory;

        public async Task<Result<IEnumerable<IngredientResponseDTO>>> GetIngredientsAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var ingredientResponseDTO = await context.Ingredients
                .Include(i => i.Recipes)
                .Select(i => i.ToResponseDTO(i)).ToListAsync();

            // TODO: handle validations in separate validation layer or service,
            // and return a list of errors instead of throwing exceptions
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

            // TODO: handle validations in separate validation layer or service,
            // and return a list of errors instead of throwing exceptions
            var errors = new List<Error>();

            if (ingredient == null)
            {
                errors.Add(new Error($"Ingredient.{id}", "Ingredient not found.", ErrorType.NotFound));
            }

            if (errors.Count != 0)
            {
                return Result<IngredientResponseDTO>.Failure(errors);
            }

            return Result<IngredientResponseDTO>.Success(ingredient?.ToResponseDTO(ingredient));
        }

        public async Task<Result<IngredientResponseDTO>> PutIngredientAsync(CreateIngredientDTO createIngredientDTO, int? id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            context.Entry(createIngredientDTO.ToEntity(createIngredientDTO, id)).State = EntityState.Modified;

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

        public async Task<Result<IngredientResponseDTO>> PostIngredientAsync(CreateIngredientDTO createIngredientDTO)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            // TODO: Implement proper ingredient existence check using a unique identifier (GUID cache) or other criteria
            // Check HybridCache library
            var ingredientExists = await context.Ingredients
                .AnyAsync(i => string.Equals(i.Name, createIngredientDTO.Name));

            // TODO: handle validations in separate validation layer or service,
            // and return a list of errors instead of throwing exceptions
            var errors = new List<Error>();

            if (ingredientExists)
            {
                errors.Add(new Error($"Ingredient.Name = {createIngredientDTO.Name}", 
                    "Ingredient already exists.", ErrorType.Conflict));

                return Result<IngredientResponseDTO>.Failure(errors);
            }

            var ingredient = createIngredientDTO.ToEntity(createIngredientDTO, null);

            // TODO: Add exception handling here
            await context.Ingredients.AddAsync(ingredient);
            await context.SaveChangesAsync();

            return Result<IngredientResponseDTO>.Success
            (
                ingredient.ToResponseDTO(await context.Ingredients
                    .Include(i => i.Recipes)
                    .FirstAsync(i => i.Name == createIngredientDTO.Name))
            );
        }

        public async Task<Result<IngredientResponseDTO>> DeleteIngredientAsync(int? id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var ingredient = await context.Ingredients
                .Include(i => i.Recipes).FirstOrDefaultAsync(i => i.Id == id);

            // TODO: handle validations in separate validation layer or service,
            // and return a list of errors instead of throwing exceptions
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

            return Result<IngredientResponseDTO>.Success(ingredient?.ToResponseDTO(ingredient));
        }
    }
}
