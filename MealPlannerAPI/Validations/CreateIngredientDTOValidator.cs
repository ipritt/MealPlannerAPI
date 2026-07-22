using FluentValidation;
using MealPlannerAPI.Models.DTOs.Request;

namespace MealPlannerAPI.Validations
{
    public class CreateIngredientDTOValidator : AbstractValidator<IngredientRequestDTO>
    {
        public CreateIngredientDTOValidator() 
        {
            RuleFor(x => x)
                .NotNull()
                .WithMessage("CreateIngredientDTO object cannot be null.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

            RuleFor(x => x.Category)
                .NotEmpty()
                .WithMessage("Category is required.")
                .MaximumLength(100).WithMessage("Category must not exceed 100 characters.");

            RuleFor(x => x.Unit)
                .NotEmpty()
                .WithMessage("Unit is required.")
                .MaximumLength(50).WithMessage("Unit must not exceed 50 characters.");
        }
    }
}
