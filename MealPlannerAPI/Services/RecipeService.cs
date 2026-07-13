using MealPlannerAPI.Context;
using MealPlannerAPI.Models.DTOs.Create;
using MealPlannerAPI.Models.DTOs.Response;
using MealPlannerAPI.Models.Entities;
using MealPlannerAPI.Models.Utility;
using Microsoft.EntityFrameworkCore;

namespace MealPlannerAPI.Services
{
    public class RecipeService(IDbContextFactory<PlannerContext> contextFactory) : IRecipeService
    {
        private readonly IDbContextFactory<PlannerContext> _contextFactory = contextFactory;

        public async Task<Result<IEnumerable<RecipeResponseDTO>>> GetRecipesAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var recipeResponseDTO = await context.Recipes
                .Include(r => r.Ingredients)
                .Select(r => MapResponseFromEntity(r)).ToListAsync();

            var errors = new List<Error>();

            if (recipeResponseDTO == null || recipeResponseDTO.Count == 0)
            {
                errors.Add(new Error("Recipes", "No recipes found.", ErrorType.NotFound));
            }

            if (errors.Count != 0)
            {
                return Result<IEnumerable<RecipeResponseDTO>>.Failure(errors);
            }

            return Result<IEnumerable<RecipeResponseDTO>>.Success(recipeResponseDTO);
        }

        public async Task<Result<RecipeResponseDTO>> GetRecipeByIdAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var recipe = await context.Recipes
                .Include(r => r.Ingredients)
                .FirstOrDefaultAsync(r => r.Id == id);

            var errors = new List<Error>();

            if (recipe == null)
            {
                errors.Add(new Error($"Recipe.{id}", "Recipe not found.", ErrorType.NotFound));
            }

            if (errors.Count != 0)
            {
                return Result<RecipeResponseDTO>.Failure(errors);
            }

            return Result<RecipeResponseDTO>.Success(MapResponseFromEntity(recipe));
        }

        public async Task<Result<RecipeResponseDTO>> PutRecipeAsync(CreateRecipeDTO createRecipeDTO, int? id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            context.Entry(MapEntityFromResponse(createRecipeDTO, id)).State = EntityState.Modified;

            var errors = new List<Error>();

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // TODO: Implement proper recipe existence check using a unique identifier (GUID cache) or other criteria
                // Check HybridCache library
                if (!await context.Recipes.AnyAsync(r => r.Id == id))
                {
                    errors.Add(new Error($"Recipe.{id}", "Recipe not found.", ErrorType.NotFound));
                }
                else
                {
                    throw;
                }

                if (errors.Count != 0)
                {
                    return Result<RecipeResponseDTO>.Failure(errors);
                }
            }

            return Result<RecipeResponseDTO>.Success(new RecipeResponseDTO());
        }

        public async Task<Result<RecipeResponseDTO>> PostRecipeAsync(CreateRecipeDTO createRecipeDTO)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            // TODO: Implement proper ingredient existence check using a unique identifier (GUID cache) or other criteria
            // Check HybridCache library
            var recipeExists = await context.Recipes
                .AnyAsync(r => string.Equals(r.Name, createRecipeDTO.Name));

            var errors = new List<Error>();

            if (recipeExists)
            {
                errors.Add(new Error($"Recipe.Name = {createRecipeDTO.Name}",
                    "Recipe already exists.", ErrorType.Conflict));

                return Result<RecipeResponseDTO>.Failure(errors);
            }

            var recipe = MapEntityFromResponse(createRecipeDTO, null);

            await context.Recipes.AddAsync(recipe);
            await context.SaveChangesAsync();

            return Result<RecipeResponseDTO>.Success
            (
                MapResponseFromEntity(await context.Recipes
                    .Include(r => r.Ingredients)
                    .FirstOrDefaultAsync(r => r.Name == createRecipeDTO.Name))
            );
        }

        public async Task<Result<RecipeResponseDTO>> DeleteRecipeAsync(int? id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var recipe = await context.Recipes
                .Include(r => r.Ingredients)
                .FirstOrDefaultAsync(r => r.Id == id);

            var errors = new List<Error>();

            if (recipe == null)
            {
                errors.Add(new Error("Recipe", "Recipe not found.", ErrorType.NotFound));
            }

            // Don't delete the recipe if it has any ingredients
            if (recipe == null || recipe.Ingredients.Count != 0)
            {
                errors.Add(new Error("Recipe", "Cannot delete recipe that has ingredients.", ErrorType.Conflict));
            }

            if (errors.Count != 0)
            {
                return Result<RecipeResponseDTO>.Failure(errors);
            }

            context.Recipes.Remove(recipe ?? new Recipe());
            await context.SaveChangesAsync();

            return Result<RecipeResponseDTO>.Success(MapResponseFromEntity(recipe));
        }


        #region Private Methods

        // TODO: Consider using AutoMapper for mapping between entities
        // or handle these mappings in the models themselves, or in a separate mapping class
        private static RecipeResponseDTO MapResponseFromEntity(Recipe? recipe)
        {
            return recipe == null ? new RecipeResponseDTO() : new RecipeResponseDTO
            {
                Id = recipe.Id,
                Name = recipe.Name,
                Instructions = recipe.Instructions,
                Ingredients = [.. recipe.Ingredients.Select(i => new IngredientResponseDTO
                {
                    Id = i.Id,
                    Name = i.Name,
                    Category = i.Category,
                    Unit = i.Unit,
                    UsedInRecipes = [.. i.Recipes.Select(r => r.Id)]
                })]
            };
        }

        private static Recipe MapEntityFromResponse(CreateRecipeDTO createRecipeDTO, int? id)
        {
            return new Recipe
            {
                Id = id ?? 0,
                Name = createRecipeDTO.Name,
                Instructions = createRecipeDTO.Instructions,
                Ingredients = [.. createRecipeDTO.Ingredients.Select(i => new Ingredient
                {
                    Name = i.Name,
                    Category = i.Category,
                    Unit = i.Unit
                })]
            };
        }

        #endregion Private Methods
    }
}
