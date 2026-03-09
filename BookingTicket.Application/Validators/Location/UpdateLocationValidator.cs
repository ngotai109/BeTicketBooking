using BookingTicket.Application.DTOs.Location;
using FluentValidation;

namespace BookingTicket.Application.Validators.Location
{
    public class UpdateLocationValidator : AbstractValidator<UpdateLocationDto>
    {
        public UpdateLocationValidator()
        {
            RuleFor(x => x.LocationName)
                .NotEmpty().WithMessage("Location name is required.")
                .MaximumLength(100)
                .WithMessage("Location name must not exceed 100 characters.");
        }
    }
}
