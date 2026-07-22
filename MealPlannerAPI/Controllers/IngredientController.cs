using MealPlannerAPI.Models.DTOs.Create;
using MealPlannerAPI.Models.DTOs.Response;
using MealPlannerAPI.Models.DTOs.Update;
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

            return HandleResult(result, false);
        }

        // GET: api/Ingredient/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<IngredientResponseDTO>> GetIngredientById([FromRoute] int id)
        {
            var result = await _ingredientService.GetIngredientByIdAsync(id);

            return HandleResult(result, false);
        }

        // PUT: api/Ingredient/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id:int}")]
        public async Task<ActionResult<IngredientResponseDTO>> UpdateIngredient(
            [FromBody] UpdateIngredientDTO updateIngredientDTO, [FromRoute] int? id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _ingredientService.UpdateIngredientAsync(updateIngredientDTO, id);

            return HandleResult(result, true);
        }

        // POST: api/Ingredient
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<IngredientResponseDTO>> CreateIngredient(
            [FromBody] CreateIngredientDTO createIngredientDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _ingredientService.CreateIngredientAsync(createIngredientDTO);

            return HandleResult(result, false);
        }

        // DELETE: api/Ingredient/5
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<IngredientResponseDTO>> DeleteIngredient([FromRoute] int? id)
        {
            var result = await _ingredientService.DeleteIngredientAsync(id);

            return HandleResult(result, true);
        }
    }
}