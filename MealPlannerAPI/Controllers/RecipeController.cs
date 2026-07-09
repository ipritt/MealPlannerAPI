using MealPlannerAPI.Context;
using MealPlannerAPI.Models.DTOs.Create;
using MealPlannerAPI.Models.DTOs.Response;
using MealPlannerAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MealPlannerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipeController(IDbContextFactory<PlannerContext> contextFactory,
        IRecipeService recipeService) : ControllerBase
    {
        private readonly IDbContextFactory<PlannerContext> _contextFactory = contextFactory;
        private readonly IRecipeService _recipeService = recipeService;

        // GET: api/Recipe
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecipeResponseDTO>>> GetRecipes()
        {
            return Ok(await _recipeService.GetRecipesAsync());
        }

        // GET: api/Recipe/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<RecipeResponseDTO>> GetRecipeById([FromRoute] int id)
        {
            var recipe = _recipeService.GetRecipeByIdAsync(id);

            if (recipe == null)
            {
                return NotFound();
            }

            return Ok(recipe);
        }

        // PUT: api/Recipe/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRecipe(int? id, RecipeResponseDTO recipe)
        {
            if (id != recipe.Id)
            {
                return BadRequest();
            }

            using var context = await _contextFactory.CreateDbContextAsync();
            context.Entry(recipe).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _recipeService.RecipeExistsAsync(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Recipe
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RecipeResponseDTO>> PostRecipe([FromBody] CreateRecipeDTO createRecipeDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var responseDTO = await _recipeService.PostRecipeAsync(createRecipeDTO);

            return CreatedAtAction(nameof(GetRecipeById), new { id = responseDTO?.Id }, responseDTO);
        }

        // DELETE: api/Recipe/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecipe(int? id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var recipe = await context.Recipes.FindAsync(id);

            if (recipe == null)
            {
                return NotFound();
            }

            context.Recipes.Remove(recipe);
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}