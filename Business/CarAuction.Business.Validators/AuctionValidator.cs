using CarAuction.Business.Dbo.Models.Auctions;
using FluentValidation;

namespace CarAuction.Business.Validators
{
    public class AuctionValidator : AbstractValidator<Auction>
    {
        public AuctionValidator()
        {
            RuleFor(a => a.VehicleID)
                .NotEmpty()
                .NotNull()
                .WithMessage("Vehicle cannot be empty");

            RuleFor(a => a.AuctionEndDate)
                .GreaterThan(a => a.AuctionStartDate)
                .WithMessage("Auction end date must be bigger than start date");
        }
    }
}