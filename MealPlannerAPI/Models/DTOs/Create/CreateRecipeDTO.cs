using MealPlannerAPI.Models.Entities;

namespace MealPlannerAPI.Models.DTOs.Create
{
    public class CreateRecipeDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Instructions { get; set; } = string.Empty;
        public ICollection<CreateIngredientDTO> Ingredients { get; set; } = [];

        public Recipe ToEntity(CreateRecipeDTO createRecipeDTO, int? id)
        {
            return new Recipe
            {
                Id = id ?? 0,
                Name = createRecipeDTO.Name,
                Instructions = createRecipeDTO.Instructions,
                Ingredients = [.. createRecipeDTO.Ingredients.Select(i => new Ingredient
                {
                    Name = i.Name,
                    Category = i.Category,
                    Unit = i.Unit
                })]
            };
        }
    }
}
