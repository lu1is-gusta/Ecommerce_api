using FluentValidation;

namespace EcommerceApi.Application.UseCases.UpdateBuyer;

public class UpdateBuyerValidator : AbstractValidator<UpdateBuyerRequest>
{
    public UpdateBuyerValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Buyer name is required.")
            .MaximumLength(200);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Buyer email is required.")
            .EmailAddress().WithMessage("Buyer email is invalid.")
            .MaximumLength(300);
    }
}
