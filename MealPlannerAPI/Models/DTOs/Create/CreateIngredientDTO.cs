using MealPlannerAPI.Models.Entities;

namespace MealPlannerAPI.Models.DTOs.Create
{
    public class CreateIngredientDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;

        public Ingredient ToEntity(CreateIngredientDTO createIngredientDTO, int? id)
        {
            return new Ingredient
            {
                Id = id ?? 0,
                Name = createIngredientDTO.Name,
                Category = createIngredientDTO.Category,
                Unit = createIngredientDTO.Unit
            };
        }
    }
}
