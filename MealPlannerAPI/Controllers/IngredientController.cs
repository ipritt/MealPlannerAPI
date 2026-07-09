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
    public class IngredientController(IDbContextFactory<PlannerContext> contextFactory,
        IIngredientService ingredientService) : ControllerBase
    {
        private readonly IDbContextFactory<PlannerContext> _contextFactory = contextFactory;
        private readonly IIngredientService _ingredientService = ingredientService;

        // GET: api/Ingredients
        [HttpGet]
        public async Task<ActionResult<IEnumerable<IngredientResponseDTO>>> GetIngredient()
        {
            return Ok(await _ingredientService.GetIngredientsAsync());
        }

        // GET: api/Ingredient/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<IngredientResponseDTO>> GetIngredientById([FromRoute] int id)
        {
            var ingredient = await _ingredientService.GetIngredientByIdAsync(id);

            if (ingredient == null)
            {
                return NotFound();
            }

            return Ok(ingredient);
        }

        // PUT: api/Ingredient/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutIngredient(int? id, IngredientResponseDTO ingredient)
        {
            if (id != ingredient.Id)
            {
                return BadRequest();
            }

            using var context = await _contextFactory.CreateDbContextAsync();
            context.Entry(ingredient).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _ingredientService.IngredientExistsAsync(id))
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

        // POST: api/Ingredient
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<IngredientResponseDTO>> PostIngredient([FromBody] CreateIngredientDTO createIngredientDTO)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var responseDTO = await _ingredientService.PostIngredientAsync(createIngredientDTO);

            return CreatedAtAction(nameof(GetIngredientById), new { id = responseDTO?.Id }, responseDTO);
        }

        // DELETE: api/Ingredient/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIngredient(int? id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var ingredient = await context.Ingredients.FindAsync(id);
            if (ingredient == null)
            {
                return NotFound();
            }

            context.Ingredients.Remove(ingredient);
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}