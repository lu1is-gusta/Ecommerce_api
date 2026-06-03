using FluentValidation;

namespace EcommerceApi.Application.UseCases.UpdateOrder;

public class UpdateOrderValidator : AbstractValidator<UpdateOrderRequest>
{
    public UpdateOrderValidator()
    {
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
