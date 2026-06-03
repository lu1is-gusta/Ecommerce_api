using FluentValidation;

namespace EcommerceApi.Application.UseCases.CreateProduct;

public class CreateProductValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(300);

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Product price must be greater than zero.");
    }
}
