using CarAuction.Business.Dbo.Models.Vehicles;
using FluentValidation;

namespace CarAuction.Business.Validators
{
    public class VehicleValidator : AbstractValidator<Vehicle>
    {
        public VehicleValidator()
        {
            RuleFor(v => v.VehicleManufacturerID)
                .NotNull()
                .GreaterThan(0)
                .WithMessage("Manufacturer mandatory");

            RuleFor(v => v.VehicleModelID)
                .NotNull()
                .GreaterThan(0)
                .WithMessage("Model mandatory");

            RuleFor(v => v.VehicleYear)
                .GreaterThan(0)
                .WithMessage("Year mandatory");

            RuleFor(v => v.VehicleYear)
                .LessThanOrEqualTo(DateTime.UtcNow.Year)
                .WithMessage("Year cannot be in the future");

            RuleFor(v => v.VehicleStartingBid)
                .GreaterThan(0)
                .WithMessage("Starting bid mandatory");

            RuleFor(v => v.VehicleNumberOfDoors)
                .NotNull()
                .GreaterThan(0)
                .WithMessage("Number of doors mandatory")
                .When(v => v.VehicleType == Core.VehicleType.Hatchback || v.VehicleType == Core.VehicleType.Sedan);

            RuleFor(v => v.VehicleNumberOfSeats)
                .NotNull()
                .GreaterThan(0)
                .WithMessage("Number of seats mandatory")
                .When(v => v.VehicleType == Core.VehicleType.SUV);

            RuleFor(v => v.VehicleLoadCapacity)
                .NotNull()
                .GreaterThan(0)
                .WithMessage("Load capacity mandatory")
                .When(v => v.VehicleType == Core.VehicleType.Truck);
        }
    }
}