namespace MealPlannerAPI.Models.Entities
{
    public class Recipe
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Instructions { get; set; } = string.Empty;
        public ICollection<Ingredient> Ingredients { get; set; } = [];
        public ICollection<RecipeIngredients> RecipeIngredients { get; set; } = [];
    }
}
