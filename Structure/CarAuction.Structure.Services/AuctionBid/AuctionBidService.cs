using CarAuction.Business.Dbo.Models.Auctions;
using CarAuction.Business.Validators;
using CarAuction.Structure.DataRepositories;
using CarAuction.Structure.Dto.Read;
using CarAuction.Structure.Dto.Write;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CarAuction.Structure.Services
{
    public sealed class AuctionBidService(ILogger<AuctionBidService> logger,
        AuctionBidValidator validator,
        UserManager<IdentityUser> userManager,
        IDataRepository<Auction> auctionsRepository,
        IDataRepository<AuctionBid> auctionBidRepository)
        : IAuctionBidReadService, IAuctionBidWriteService
    {
        public async Task<CreateEntityResponseDto> CreateAuctionBidAsync(CreateAuctionBidRequestDto auctionBidDto)
        {
            logger.LogInformation("Trying to Create new AuctionBid");

            if (auctionBidDto.AuctionID <= 0)
                return new(false, "Auction ID cannot be empty");

            var auction = await auctionsRepository.GetByIDAsync(auctionBidDto.AuctionID);
            if (auction is null) return new(false, "There is no auction with the provided ID");

            if (auction.AuctionStatus == Business.Core.AuctionStatus.Inactive)
                return new(false, "Auction must be active");

            var user = await userManager.FindByIdAsync(auctionBidDto.UserID);
            if (user is null) return new(false, "There is no user with the provided ID");

            var auctionBid = new AuctionBid()
            {
                AuctionBidAmount = auctionBidDto.AuctionBidAmount,
                AuctionBidInsertDate = DateTime.UtcNow,

                UserID = user.Id,
                AuctionID = auction.AuctionID
            };

            var validationResult = await validator.ValidateAsync(auctionBid);
            if (!validationResult.IsValid) return new(false, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

            if (auction.CurrentAuctionBid != null && auction.CurrentAuctionBid.AuctionBidAmount >= auctionBid.AuctionBidAmount)
            {
                return new(false, "There is already an existing bid with higher value");
            }
            else if (auction.CurrentAuctionBid is null && auction.Vehicle.VehicleStartingBid > auctionBid.AuctionBidAmount)
            {
                return new(false, "The bid must be higher than the minimum required");
            }

            var (success, message) = await auctionBidRepository.CreateAsync(auctionBid);
            return new(success, success ? "Bid created with success" : message);
        }

        public async Task<AuctionBidDetailResponseDto?> GetAuctionCurrentBidAsync(int auctionID)
        {
            var auctionBid = (await auctionsRepository.GetByIDAsync(auctionID))?.CurrentAuctionBid;
            if (auctionBid is null) return null;

            return new AuctionBidDetailResponseDto()
            {
                AuctionID = auctionBid.AuctionID,
                AuctionBidID = auctionBid.AuctionBidID,
                AuctionBidAmount = auctionBid.AuctionBidAmount,
                AuctionBidInsertDate = auctionBid.AuctionBidInsertDate
            };
        }
    }
}