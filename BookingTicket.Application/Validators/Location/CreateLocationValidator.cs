using BookingTicket.Application.DTOs.Location;
using FluentValidation;

namespace BookingTicket.Application.Validators.Location
{
    public class CreateLocationValidator : AbstractValidator<CreateLocationDto>
    {
        public CreateLocationValidator()
        {
            RuleFor(x => x.LocationName)
                .NotEmpty().WithMessage("Location name is required.")
                .MaximumLength(100)
                .WithMessage("Location name must not exceed 100 characters.");
        }
    }
}
