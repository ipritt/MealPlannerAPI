using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MealPlannerAPI.Models;

[Route("api/[controller]")]
[ApiController]
public class InventoriesController : ControllerBase
{
    private readonly IDbContextFactory<PlannerContext> _contextFactory;

    public InventoriesController(IDbContextFactory<PlannerContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    // GET: api/Inventory
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Inventory>>> GetInventory()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Inventory.ToListAsync();
    }

    // GET: api/Inventory/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Inventory>> GetInventory(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var inventory = await context.Inventory.FindAsync(id);

        if (inventory == null)
        {
            return NotFound();
        }

        return inventory;
    }

    // PUT: api/Inventory/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutInventory(int? id, Inventory inventory)
    {
        if (id != inventory.Id)
        {
            return BadRequest();
        }

        using var context = await _contextFactory.CreateDbContextAsync();
        context.Entry(inventory).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await InventoryExists(id))
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

    // POST: api/Inventory
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Inventory>> PostInventory(Inventory inventory)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        context.Inventory.Add(inventory);
        await context.SaveChangesAsync();

        return CreatedAtAction("GetInventory", new { id = inventory.Id }, inventory);
    }

    // DELETE: api/Inventory/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteInventory(int? id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var inventory = await context.Inventory.FindAsync(id);
        if (inventory == null)
        {
            return NotFound();
        }

        context.Inventory.Remove(inventory);
        await context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<bool> InventoryExists(int? id) 
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Inventory.AnyAsync(e => e.Id == id);
    }
}
