using MealPlannerAPI.Models.DTOs.Create;
using MealPlannerAPI.Models.DTOs.Response;
using MealPlannerAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace MealPlannerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IngredientController(IIngredientService ingredientService) : ApiControllerBase
    {
        private readonly IIngredientService _ingredientService = ingredientService;

        // GET: api/Ingredients
        [HttpGet]
        public async Task<ActionResult<IEnumerable<IngredientResponseDTO>>> GetIngredient()
        {
            var result = await _ingredientService.GetIngredientsAsync();

            return HandleResult(result);
        }

        // GET: api/Ingredient/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<IngredientResponseDTO>> GetIngredientById([FromRoute] int id)
        {
            var result = await _ingredientService.GetIngredientByIdAsync(id);

            return HandleResult(result);
        }

        // PUT: api/Ingredient/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id:int}")]
        public async Task<ActionResult<IngredientResponseDTO>> PutIngredient(CreateIngredientDTO createIngredientDTO, int? id)
        {
            var result = await _ingredientService.PutIngredientAsync(createIngredientDTO, id);

            if (result.IsFailure)
            {
                return HandleResult(result);
            }

            return NoContent();
        }

        // POST: api/Ingredient
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<IngredientResponseDTO>> PostIngredient([FromBody] CreateIngredientDTO createIngredientDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _ingredientService.PostIngredientAsync(createIngredientDTO);

            return HandleResult(result);
        }

        // DELETE: api/Ingredient/5
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<IngredientResponseDTO>> DeleteIngredient(int? id)
        {
            var result = await _ingredientService.DeleteIngredientAsync(id);

            if (result.IsFailure)
            {
                return HandleResult(result);
            }

            return NoContent();
        }
    }
}