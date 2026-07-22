using MealPlannerAPI.Models.DTOs.Request;
using MealPlannerAPI.Models.DTOs.Response;
using MealPlannerAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace MealPlannerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController(IInventoryService inventoryService) : ApiControllerBase
    {
        private readonly IInventoryService _inventoryService = inventoryService;

        // GET: api/Inventory
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryResponseDTO>>> GetInventory()
        {
            var result = await _inventoryService.GetInventoryAsync();

            return HandleResult(result, false);
        }

        // GET: api/Inventory/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<InventoryResponseDTO>> GetInventoryById([FromRoute] int id)
        {
            var result = await _inventoryService.GetInventoryByIdAsync(id);

            return HandleResult(result, false);
        }

        // PUT: api/Inventory/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<InventoryResponseDTO>> PutInventory(
            [FromBody] InventoryRequestDTO inventoryRequestDTO, [FromRoute] int? id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _inventoryService.PutInventoryAsync(inventoryRequestDTO, id);

            return HandleResult(result, true);
        }

        // POST: api/Inventory
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<InventoryResponseDTO>> PostInventory(
            [FromBody] InventoryRequestDTO inventoryRequestDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _inventoryService.PostInventoryAsync(inventoryRequestDTO);

            return HandleResult(result, false);
        }

        // DELETE: api/Inventory/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<InventoryResponseDTO>> DeleteInventory([FromRoute] int? id)
        {
            var result = await _inventoryService.DeleteInventoryAsync(id);

            return HandleResult(result, true);
        }
    }
}