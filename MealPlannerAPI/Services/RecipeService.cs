using MealPlannerAPI.Context;
using MealPlannerAPI.Models.DTOs.Create;
using MealPlannerAPI.Models.DTOs.Response;
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
                .Select(r => r.ToResponseDTO(r)).ToListAsync();

            // TODO: handle validations in separate validation layer or service,
            // and return a list of errors instead of throwing exceptions
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

            // TODO: handle validations in separate validation layer or service,
            // and return a list of errors instead of throwing exceptions
            var errors = new List<Error>();

            if (recipe == null)
            {
                errors.Add(new Error($"Recipe.{id}", "Recipe not found.", ErrorType.NotFound));
            }

            if (errors.Count != 0)
            {
                return Result<RecipeResponseDTO>.Failure(errors);
            }

            return Result<RecipeResponseDTO>.Success(recipe?.ToResponseDTO(recipe));
        }

        public async Task<Result<RecipeResponseDTO>> PutRecipeAsync(CreateRecipeDTO createRecipeDTO, int? id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            context.Entry(createRecipeDTO.ToEntity(createRecipeDTO, id)).State = EntityState.Modified;

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

            // TODO: handle validations in separate validation layer or service,
            // and return a list of errors instead of throwing exceptions
            var errors = new List<Error>();

            if (recipeExists)
            {
                errors.Add(new Error($"Recipe.Name = {createRecipeDTO.Name}",
                    "Recipe already exists.", ErrorType.Conflict));

                return Result<RecipeResponseDTO>.Failure(errors);
            }

            var recipe = createRecipeDTO.ToEntity(createRecipeDTO, null);

            // TODO: Add exception handling here
            await context.Recipes.AddAsync(recipe);
            await context.SaveChangesAsync();

            return Result<RecipeResponseDTO>.Success
            (
                recipe.ToResponseDTO(await context.Recipes
                    .Include(i => i.Ingredients)
                    .FirstAsync(i => i.Name == createRecipeDTO.Name))
            );
        }

        public async Task<Result<RecipeResponseDTO>> DeleteRecipeAsync(int? id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var recipe = await context.Recipes
                .FirstOrDefaultAsync(r => r.Id == id);

            // TODO: handle validations in separate validation layer or service,
            // and return a list of errors instead of throwing exceptions
            var errors = new List<Error>();

            if (recipe == null)
            {
                errors.Add(new Error("Recipe", "Recipe not found.", ErrorType.NotFound));
            }
            else
            {
                context.Recipes.Remove(recipe);

                // TODO: Add exception handling here
                await context.SaveChangesAsync();
            }

            if (errors.Count != 0)
            {
                return Result<RecipeResponseDTO>.Failure(errors);
            }

            return Result<RecipeResponseDTO>.Success(recipe?.ToResponseDTO(recipe));
        }
    }
}
