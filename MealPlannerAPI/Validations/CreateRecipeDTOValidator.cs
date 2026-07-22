using FluentValidation;
using MealPlannerAPI.Models.DTOs.Request;

namespace MealPlannerAPI.Validations
{
    public class RecipeDTOValidator : AbstractValidator<RecipeRequestDTO>
    {
        public RecipeDTOValidator() 
        {
            RuleFor(x => x)
                .NotNull()
                .WithMessage("RecipeDTO object cannot be null.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

            RuleFor(x => x.Instructions)
                .MaximumLength(500).WithMessage("Instructions must not exceed 500 characters.");

            //RuleFor(x => x.Ingredients)
            //    .NotEmpty()
            //    .WithMessage("At least one ingredient is required.");
            //    .Must(ingredients => ingredients.All(i => i.Quantity > 0))
            //    .WithMessage("All ingredients must have a quantity greater than zero.");
        }
    }
}
