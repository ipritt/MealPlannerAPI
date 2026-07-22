using MealPlannerAPI.Context;
using MealPlannerAPI.Models.DTOs.Request;
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
                .Include(r => r.RecipeIngredients)
                .Select(r => r.ToResponseDTO(r))
                .ToListAsync();

            // TODO: handle validations in separate validation layer or service,
            // and return a list of errors
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
                .Include(r => r.RecipeIngredients)
                .FirstOrDefaultAsync(r => r.Id == id);

            // TODO: handle validations in separate validation layer or service,
            // and return a list of errors
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

        public async Task<Result<RecipeResponseDTO>> UpdateRecipeAsync(RecipeRequestDTO recipeRequestDTO, int? id)
        {
            if (id == null)
            {
                return Result<RecipeResponseDTO>.Failure([new("Recipe.Id", "ID mismatch.", ErrorType.BadRequest)]);
            }

            using var context = await _contextFactory.CreateDbContextAsync();

            var errors = new List<Error>();

            var existingRecipesEntity = await context.Recipes
                .Include(r => r.Ingredients)
                .Include(r => r.RecipeIngredients)
                .FirstAsync(r => r.Id == id);

            if (existingRecipesEntity == null)
            {
                return Result<RecipeResponseDTO>.Failure([new($"Recipe.{id}", "Recipe not found.", ErrorType.NotFound)]);
            }

            // Entity Framework doesn't like implicitly updating child collections
            // when there is a payload (Quantity) in a many-to-many join table (RecipeIngredients),
            // so we have to explicitly do so.
            foreach (var updatedRecipeIngredient in recipeRequestDTO.RecipeIngredients)
            {
                var existingRecipeIngredient = existingRecipesEntity.RecipeIngredients
                    .FirstOrDefault(ri => ri.IngredientId == updatedRecipeIngredient.IngredientId && ri.RecipeId == id);

                if (existingRecipeIngredient != null)
                {
                    context.Entry(existingRecipeIngredient).CurrentValues.SetValues(updatedRecipeIngredient);
                }
                else
                {
                    existingRecipesEntity.RecipeIngredients.Add(new RecipeIngredients
                    {
                        IngredientId = updatedRecipeIngredient.IngredientId,
                        Quantity = updatedRecipeIngredient.Quantity
                    });
                }
            }

            // Remove RecipeIngredients that are not in the recipeDTO
            foreach (var existingRecipeIngredient in existingRecipesEntity.RecipeIngredients)
            {
                if (!recipeRequestDTO.RecipeIngredients.Any(ri => ri.IngredientId == existingRecipeIngredient.IngredientId))
                {
                    context.RecipeIngredients.Remove(existingRecipeIngredient);
                }
            }

            context.Entry(existingRecipesEntity);

            try
            {
                // TODO: Add exception handling here
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            if (errors.Count != 0)
            {
                return Result<RecipeResponseDTO>.Failure(errors);
            }

            return Result<RecipeResponseDTO>.Success(existingRecipesEntity.ToResponseDTO(existingRecipesEntity));
        }

        public async Task<Result<RecipeResponseDTO>> CreateRecipeAsync(RecipeRequestDTO recipeRequestDTO)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var recipeExists = await context.Recipes
                .AnyAsync(r => string.Equals(r.Name, recipeRequestDTO.Name));

            var errors = new List<Error>();

            // There could be different versions of basically the same recipe,
            // so only really worried about the name being the same, not the other properties.
            // If the name is the same, then it is considered a duplicate.
            // I may revisit this later and add a more robust check for duplicates, but for now, this will do.
            if (recipeExists)
            {
                errors.Add(new Error($"Recipe.Name = {recipeRequestDTO.Name}",
                    "Recipe already exists.", ErrorType.Conflict));

                return Result<RecipeResponseDTO>.Failure(errors);
            }

            var newRecipeEntity = new Recipe
            {
                Name = recipeRequestDTO.Name,
                Instructions = recipeRequestDTO.Instructions
            };

            // Entity Framework doesn't like implicitly adding child collections
            // when there is a payload (Quantity) in a many-to-many join table (RecipeIngredients),
            // so we have to explicitly do so.
            foreach (var updatedRecipeIngredient in recipeRequestDTO.RecipeIngredients)
            {
                var existingIngredient = context.Ingredients
                    .FirstOrDefault(ri => ri.Id == updatedRecipeIngredient.IngredientId);
                
                if (existingIngredient != null)
                {
                    newRecipeEntity.RecipeIngredients.Add(new RecipeIngredients
                    {
                        IngredientId = updatedRecipeIngredient.IngredientId,
                        Quantity = updatedRecipeIngredient.Quantity
                    });
                }
                else
                {
                    errors.Add(new Error($"Ingredient.Id = {existingIngredient?.Id}",
                        "Ingredient not found. Please add the ingredient before attempting to use it in a recipe.", ErrorType.NotFound ));

                    return Result<RecipeResponseDTO>.Failure(errors);
                }
            }

            context.Add(newRecipeEntity);

            // TODO: Add exception handling here
            await context.SaveChangesAsync();

            if (errors.Count != 0)
            {
                return Result<RecipeResponseDTO>.Failure(errors);
            }

            return Result<RecipeResponseDTO>.Success(newRecipeEntity.ToResponseDTO(newRecipeEntity));
        }

        public async Task<Result<RecipeResponseDTO>> DeleteRecipeAsync(int? id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var recipeExists = await context.Recipes
                .AnyAsync(r => r.Id == id);

            var recipe = new Recipe();

            // TODO: handle validations in separate validation layer or service,
            // and return a list of errors
            var errors = new List<Error>();

            if (!recipeExists)
            {
                errors.Add(new Error("Recipe", "Recipe not found.", ErrorType.NotFound));
            }
            else
            {
                recipe = await context.Recipes.FirstAsync(r => r.Id == id);
                context.Recipes.Remove(recipe);

                // TODO: Add exception handling here;
                await context.SaveChangesAsync();
            }

            if (errors.Count != 0)
            {
                return Result<RecipeResponseDTO>.Failure(errors);
            }

            return Result<RecipeResponseDTO>.Success(recipe.ToResponseDTO(recipe));
        }
    }
}
