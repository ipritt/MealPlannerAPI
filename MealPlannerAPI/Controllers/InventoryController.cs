using MealPlannerAPI.Models.DTOs.Create;
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

            return HandleResult(result);
        }

        // GET: api/Inventory/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<InventoryResponseDTO>> GetInventoryById([FromRoute] int id)
        {
            var result = await _inventoryService.GetInventoryByIdAsync(id);

            return HandleResult(result);
        }

        // PUT: api/Inventory/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<InventoryResponseDTO>> PutInventory(CreateInventoryDTO createInventoryDTO, int? id)
        {
            var result = await _inventoryService.PutInventoryAsync(createInventoryDTO, id);

            if (result.IsFailure)
            {
                return HandleResult(result);
            }

            return NoContent();
        }

        // POST: api/Inventory
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<InventoryResponseDTO>> PostInventory([FromBody] CreateInventoryDTO createInventoryDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _inventoryService.PostInventoryAsync(createInventoryDTO);

            return HandleResult(result);
        }

        // DELETE: api/Inventory/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<InventoryResponseDTO>> DeleteInventory(int? id)
        {
            var result = await _inventoryService.DeleteInventoryAsync(id);

            if (result.IsFailure)
            {
                return HandleResult(result);
            }

            return NoContent();
        }
    }
}