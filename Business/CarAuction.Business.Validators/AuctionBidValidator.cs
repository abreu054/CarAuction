using CarAuction.Business.Dbo.Models.Auctions;
using FluentValidation;

namespace CarAuction.Business.Validators
{
    public class AuctionBidValidator : AbstractValidator<AuctionBid>
    {
        public AuctionBidValidator()
        {
            RuleFor(ab => ab.AuctionBidAmount)
                .GreaterThan(0)
                .WithMessage("Bid must be higher than 0");

            RuleFor(ab => ab.AuctionID)
                .GreaterThan(0)
                .WithMessage("Auction cannot be null");

            RuleFor(ab => ab.UserID)
                .NotNull()
                .NotEmpty()
                .WithMessage("User cannot be null");
        }
    }
}