using MealPlannerAPI.Models.Entities;

namespace MealPlannerAPI.Models.DTOs.Update
{
    public class UpdateIngredientDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;

        public Ingredient ToEntity(UpdateIngredientDTO updateIngredientDTO, int id)
        {
            return new Ingredient
            {
                Id = id,
                Name = updateIngredientDTO.Name,
                Category = updateIngredientDTO.Category,
                Unit = updateIngredientDTO.Unit
            };
        }
    }
}
