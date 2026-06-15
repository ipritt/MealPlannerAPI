using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MealPlannerAPI.Models;

[Route("api/[controller]")]
[ApiController]
public class IngredientsController : ControllerBase
{
    private readonly IDbContextFactory<PlannerContext> _contextFactory;
    public IngredientsController(IDbContextFactory<PlannerContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    // GET: api/Ingredient
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Ingredient>>> GetIngredient()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Ingredients
            .Include(i => i.RecipeIngredients)
            .ThenInclude(ri => ri.Recipe)
            .ToListAsync();
    }

    // GET: api/Ingredient/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Ingredient>> GetIngredient(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var ingredient = await context.Ingredients.FindAsync(id);

        if (ingredient == null)
        {
            return NotFound();
        }

        return ingredient;
    }

    // PUT: api/Ingredient/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutIngredient(int? id, Ingredient ingredient)
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
            if (!await IngredientExists(id))
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
    public async Task<ActionResult<Ingredient>> PostIngredient(Ingredient ingredient)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        context.Ingredients.Add(ingredient);
        await context.SaveChangesAsync();

        return CreatedAtAction("GetIngredient", new { id = ingredient.Id }, ingredient);
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

    private async Task<bool> IngredientExists(int? id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Ingredients.AnyAsync(e => e.Id == id);
    }
}
