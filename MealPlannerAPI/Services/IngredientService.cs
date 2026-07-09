using MealPlannerAPI.Context;
using MealPlannerAPI.Models;
using MealPlannerAPI.Models.DTOs.Create;
using MealPlannerAPI.Models.DTOs.Response;
using Microsoft.EntityFrameworkCore;

namespace MealPlannerAPI.Services
{
    public class IngredientService(IDbContextFactory<PlannerContext> contextFactory) : IIngredientService
    {
        private readonly IDbContextFactory<PlannerContext> _contextFactory = contextFactory;

        public async Task<bool> IngredientExistsAsync(int? id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            return await context.Ingredients.AnyAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<IngredientResponseDTO>> GetIngredientsAsync()
        {
            return await GetAllIngredientsAsync();
        }

        public async Task<IngredientResponseDTO?> GetIngredientByIdAsync(int id)
        {
            return GetAllIngredientsAsync()
                .Result?.Where(inv => inv.Id == id).FirstOrDefault();
        }

        public async Task<IngredientResponseDTO?> PostIngredientAsync(CreateIngredientDTO createIngredientDTO)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var ingredient = new Ingredient
            {
                Name = createIngredientDTO.Name,
                Category = createIngredientDTO.Category,
                Unit = createIngredientDTO.Unit
            };

            await context.Ingredients.AddAsync(ingredient);
            await context.SaveChangesAsync();

            return new IngredientResponseDTO
            {
                Id = ingredient.Id,
                Name = ingredient.Name,
                Category = ingredient.Category,
                Unit= ingredient.Unit
            };
        }

        private async Task<IEnumerable<IngredientResponseDTO>> GetAllIngredientsAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            return await context.Ingredients
                .Include(i => i.Recipes)
                .Select(i => new IngredientResponseDTO
                {
                    Id = i.Id,
                    Name = i.Name,
                    Category = i.Category,
                    Unit = i.Unit,
                    UsedInRecipes = i.Recipes.Select(r => r.Id).ToList()
                }).ToListAsync();
        }
    }
}
