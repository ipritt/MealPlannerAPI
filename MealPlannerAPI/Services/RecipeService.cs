using MealPlannerAPI.Context;
using MealPlannerAPI.Models;
using MealPlannerAPI.Models.DTOs.Create;
using MealPlannerAPI.Models.DTOs.Response;
using Microsoft.EntityFrameworkCore;

namespace MealPlannerAPI.Services
{
    public class RecipeService(IDbContextFactory<PlannerContext> contextFactory) : IRecipeService
    {
        private readonly IDbContextFactory<PlannerContext> _contextFactory = contextFactory;

        public async Task<bool> RecipeExistsAsync(int? id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            return await context.Recipes.AnyAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<RecipeResponseDTO>> GetRecipesAsync()
        {
            return await GetAllRecipesAsync();
        }

        public async Task<RecipeResponseDTO?> GetRecipeByIdAsync(int id)
        {
            return GetAllRecipesAsync()
                .Result?.Where(inv => inv.Id == id).FirstOrDefault();
        }

        public async Task<RecipeResponseDTO?> PostRecipeAsync(CreateRecipeDTO createRecipeDTO)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var recipe = MapEntityFromResponse(createRecipeDTO);

            await context.Recipes.AddAsync(recipe);
            await context.SaveChangesAsync();

            return MapResponseFromEntity(recipe);
        }

        public async Task<RecipeResponseDTO?> DeleteRecipeAsync(int? id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var recipe = await context.Recipes.FindAsync(id);

            if (recipe == null)
            {
                return null;
            }

            context.Recipes.Remove(recipe);
            await context.SaveChangesAsync();

            return MapResponseFromEntity(recipe);
        }


        #region Private Methods
        private async Task<IEnumerable<RecipeResponseDTO>> GetAllRecipesAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            return await context.Recipes
                .Include(r => r.Ingredients)
                .Select(r => MapResponseFromEntity(r)).ToListAsync();
        }

        private static RecipeResponseDTO MapResponseFromEntity(Recipe recipe)
        {
            return new RecipeResponseDTO
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

        private static Recipe MapEntityFromResponse(CreateRecipeDTO createRecipeDTO)
        {
            return new Recipe
            {
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
