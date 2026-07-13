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
                .Select(i => MapResponseFromEntity(i)).ToListAsync();

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

            var errors = new List<Error>();

            if (ingredient == null)
            {
                errors.Add(new Error($"Ingredient.{id}", "Ingredient not found.", ErrorType.NotFound));
            }

            if (errors.Count != 0)
            {
                return Result<IngredientResponseDTO>.Failure(errors);
            }

            return Result<IngredientResponseDTO>.Success(MapResponseFromEntity(ingredient));
        }

        public async Task<Result<IngredientResponseDTO>> PutIngredientAsync(CreateIngredientDTO createIngredientDTO, int? id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            context.Entry(MapEntityFromResponse(createIngredientDTO, id)).State = EntityState.Modified;

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

            var errors = new List<Error>();

            if (ingredientExists)
            {
                errors.Add(new Error($"Ingredient.Name = {createIngredientDTO.Name}", 
                    "Ingredient already exists.", ErrorType.Conflict));

                return Result<IngredientResponseDTO>.Failure(errors);
            }

            var ingredient = MapEntityFromResponse(createIngredientDTO, null);

            await context.Ingredients.AddAsync(ingredient);
            await context.SaveChangesAsync();

            return Result<IngredientResponseDTO>.Success
            (
                MapResponseFromEntity(await context.Ingredients
                    .Include(i => i.Recipes)
                    .FirstOrDefaultAsync(i => i.Name == createIngredientDTO.Name))
            );
        }

        public async Task<Result<IngredientResponseDTO>> DeleteIngredientAsync(int? id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var ingredient = await context.Ingredients
                .Include(i => i.Recipes).FirstOrDefaultAsync(i => i.Id == id);

            var errors = new List<Error>();

            if (ingredient == null)
            {
                errors.Add(new Error("Ingredient", "Ingredient not found.", ErrorType.NotFound));
            }

            // Don't delete the ingredient if it is used in any recipes
            if (ingredient == null || ingredient.Recipes.Count != 0)
            {
                errors.Add(new Error("Ingredient", "Cannot delete ingredient that is used in recipes.", ErrorType.Conflict));
            }

            if (errors.Count != 0)
            {
                return Result<IngredientResponseDTO>.Failure(errors);
            }

            context.Ingredients.Remove(ingredient ?? new Ingredient());
            await context.SaveChangesAsync();

            return Result<IngredientResponseDTO>.Success(MapResponseFromEntity(ingredient));
        }


        #region Private Methods

        // TODO: Consider using AutoMapper for mapping between entities
        // or handle these mappings in the models themselves, or in a separate mapping class
        private static IngredientResponseDTO MapResponseFromEntity(Ingredient? ingredient)
        {
            return ingredient == null ? new IngredientResponseDTO() : new IngredientResponseDTO
            {
                Id = ingredient.Id,
                Name = ingredient.Name,
                Category = ingredient.Category,
                Unit = ingredient.Unit,
                UsedInRecipes = [.. ingredient.Recipes.Select(r => r.Id)]
            };
        }

        private static Ingredient MapEntityFromResponse(CreateIngredientDTO createIngredientDTO, int? id)
        {
            return new Ingredient
            {
                Id = id ?? 0,
                Name = createIngredientDTO.Name,
                Category = createIngredientDTO.Category,
                Unit = createIngredientDTO.Unit
            };
        }

        #endregion Private Methods
    }
}
