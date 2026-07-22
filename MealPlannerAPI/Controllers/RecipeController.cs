using MealPlannerAPI.Models.DTOs.Create;
using MealPlannerAPI.Models.DTOs.Request;
using MealPlannerAPI.Models.DTOs.Response;
using MealPlannerAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace MealPlannerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipeController(IRecipeService recipeService) : ApiControllerBase
    {
        private readonly IRecipeService _recipeService = recipeService;

        // GET: api/Recipe
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecipeResponseDTO>>> GetRecipes()
        {
            var result = await _recipeService.GetRecipesAsync();

            return HandleResult(result, false);
        }

        // GET: api/Recipe/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<RecipeResponseDTO>> GetRecipeById([FromRoute] int id)
        {
            var result = await _recipeService.GetRecipeByIdAsync(id);

            return HandleResult(result, false);
        }

        // PUT: api/Recipe/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id:int}")]
        public async Task<ActionResult<RecipeResponseDTO>> UpdateRecipe(
            [FromBody] RecipeRequestDTO recipeDTO, [FromRoute] int? id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _recipeService.UpdateRecipeAsync(recipeDTO, id);

            return HandleResult(result, true);
        }

        // POST: api/Recipe
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RecipeResponseDTO>> CreateRecipe(
            [FromBody] RecipeRequestDTO recipeDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _recipeService.CreateRecipeAsync(recipeDTO);

            return HandleResult(result, false);
        }

        // DELETE: api/Recipe/5
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<RecipeResponseDTO>> DeleteRecipe([FromRoute] int? id)
        {
            var result = await _recipeService.DeleteRecipeAsync(id);

            return HandleResult(result, true);
        }
    }
}