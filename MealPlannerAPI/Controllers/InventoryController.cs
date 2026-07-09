using MealPlannerAPI.Context;
using MealPlannerAPI.Models;
using MealPlannerAPI.Models.DTOs.Create;
using MealPlannerAPI.Models.DTOs.Response;
using MealPlannerAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MealPlannerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController(IDbContextFactory<PlannerContext> contextFactory, 
        IInventoryService inventoryService) : ControllerBase
    {
        private readonly IDbContextFactory<PlannerContext> _contextFactory = contextFactory;
        private readonly IInventoryService _inventoryService = inventoryService;

        // GET: api/Inventory
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryResponseDTO>>> GetInventory()
        {
            return Ok(await _inventoryService.GetInventoryAsync());
        }

        // GET: api/Inventory/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<InventoryResponseDTO>> GetInventoryById([FromRoute] int id)
        {
            var inventory = await _inventoryService.GetInventoryByIdAsync(id);

            if (inventory == null)
            {
                return NotFound();
            }

            return Ok(inventory);
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
                if (!await _inventoryService.InventoryExistsAsync(id))
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
        public async Task<ActionResult<InventoryResponseDTO>> PostInventory([FromBody] CreateInventoryDTO inventory)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var responseDTO = await _inventoryService.PostInventoryAsync(inventory);

            return CreatedAtAction(nameof(GetInventoryById), new { id = responseDTO?.Id }, responseDTO);
        }

        // DELETE: api/Inventory/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInventory(int? id)
        {
            var inventory = await _inventoryService.DeleteInventoryAsync(id);

            if (inventory == null)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}