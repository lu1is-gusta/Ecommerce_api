using FluentValidation;

namespace EcommerceApi.Application.UseCases.CreateOrder;

public class CreateOrderValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.Buyer)
            .NotNull().WithMessage("An order must have a buyer.");

        When(x => x.Buyer is not null, () =>
        {
            RuleFor(x => x.Buyer.Name)
                .NotEmpty().WithMessage("Buyer name is required.")
                .MaximumLength(200);

            RuleFor(x => x.Buyer.Email)
                .NotEmpty().WithMessage("Buyer email is required.")
                .EmailAddress().WithMessage("Buyer email is invalid.")
                .MaximumLength(300);
        });

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("An order must have at least one product.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId)
                .NotEmpty().WithMessage("Each item must reference a product.");
            item.RuleFor(i => i.Quantity)
                .GreaterThan(0).WithMessage("Item quantity must be greater than zero.");
        });
    }
}
