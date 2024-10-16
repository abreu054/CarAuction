using CarAuction.Business.Dbo.Models.Auctions;
using CarAuction.Business.Dbo.Models.Vehicles;
using CarAuction.Business.Validators;
using CarAuction.Structure.DataRepositories;
using CarAuction.Structure.Dto.Read;
using CarAuction.Structure.Dto.Search;
using CarAuction.Structure.Dto.Write;
using Microsoft.Extensions.Logging;

namespace CarAuction.Structure.Services
{
    public sealed class AuctionService(ILogger<AuctionService> logger,
        AuctionValidator validator,
        IDataRepository<Auction> auctionsRepository,
        IDataRepository<Vehicle> vehiclesRepository)
        : IAuctionReadService, IAuctionWriteService
    {
        public async Task<CreateEntityResponseDto> CreateAuctionAsync(CreateAuctionRequestDto auctionDto)
        {
            logger.LogInformation("Trying to create new auction");

            if (auctionDto.VehicleID <= 0) return new(false, "Vehicle mandatory");

            var vehicle = await vehiclesRepository.GetByIDAsync(auctionDto.VehicleID);
            if (vehicle is null) return new(false, $"No vehicle found with ID {auctionDto.VehicleID}");

            // Verify if there isnt already an active auction for the vehicle
            if (auctionDto.AuctionStatus == Business.Core.AuctionStatus.Active)
            {
                var activeAuctionsForVehicle = await auctionsRepository.SearchAsync(new AuctionSearchParamsDto()
                {
                    AuctionStatus = Business.Core.AuctionStatus.Active,
                    VehicleID = auctionDto.VehicleID,
                });

                if (activeAuctionsForVehicle.Any())
                    return new(false, "There is an active auction for this vehicle");
            }

            var auction = new Auction()
            {
                AuctionStatus = auctionDto.AuctionStatus,
                AuctionStartDate = auctionDto.AuctionStartDate,
                AuctionEndDate = auctionDto.AuctionEndDate,

                VehicleID = vehicle.VehicleID
            };

            var validationResult = await validator.ValidateAsync(auction);
            if (!validationResult.IsValid)
            {
                return new(false, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            var createAuctionResult = await auctionsRepository.CreateAsync(auction);
            if (!createAuctionResult.success)
                return new(false, createAuctionResult.message);

            return new(true, "Auction created with success");
        }

        public async Task<IEnumerable<AuctionDetailResponseDto>> SearchAuctionsAsync(AuctionSearchParamsDto searchParamsDto)
        {
            var auctions = await auctionsRepository.SearchAsync(searchParamsDto);
            if (auctions is null || !auctions.Any()) return [];

            return auctions.Select(auction => new AuctionDetailResponseDto()
            {
                AuctionID = auction.AuctionID,
                AuctionStatus = auction.AuctionStatus,
                AuctionStartDate = auction.AuctionStartDate,
                AuctionEndDate = auction.AuctionEndDate,
                AuctionStartingBid = auction.Vehicle?.VehicleStartingBid ?? 0,
                AuctionCurrentBid = auction.CurrentAuctionBid?.AuctionBidAmount ?? 0,
                AuctionCurrentMinimunBid = GetAuctionCurrentMinimunBid(auction),

                VehicleID = auction.VehicleID.GetValueOrDefault(),
                VehicleUniqueIdentifier = auction.Vehicle?.VehicleUniqueIdentifier ?? string.Empty,
                VehicleManufacturerID = auction.Vehicle?.VehicleManufacturerID ?? 0,
                VehicleManufacturerName = auction.Vehicle?.VehicleManufacturer?.VehicleManufacturerName ?? string.Empty,
                VehicleModelID = auction.Vehicle?.VehicleModelID ?? 0,
                VehicleModelName = auction.Vehicle?.VehicleModel?.VehicleModelName ?? string.Empty
            });
        }

        public async Task<UpdateEntityResponseDto> UpdateAuctionAsync(UpdateAuctionRequestDto updateAuctionDto)
        {
            var existingAuction = await auctionsRepository.GetByIDAsync(updateAuctionDto.AuctionID);
            if (existingAuction is null) return new(false, $"No auction found with ID {updateAuctionDto.AuctionID}");

            bool updateAuction = false;

            if (updateAuctionDto.AuctionStatus != existingAuction.AuctionStatus)
            {
                existingAuction.AuctionStatus = updateAuctionDto.AuctionStatus;
                updateAuction = true;
            }

            if (updateAuctionDto.AuctionStartDate != null && updateAuctionDto.AuctionStartDate != existingAuction.AuctionStartDate)
            {
                existingAuction.AuctionStartDate = updateAuctionDto.AuctionStartDate.Value;
                updateAuction = true;
            }

            if (updateAuctionDto.AuctionEndDate != null && updateAuctionDto.AuctionEndDate != existingAuction.AuctionEndDate)
            {
                existingAuction.AuctionEndDate = updateAuctionDto.AuctionEndDate.Value;
                updateAuction = true;
            }

            if (updateAuction)
            {
                // If we are going to activate the auction, we need to verify if there isnt already an active auction for the vehicle
                if (updateAuctionDto.AuctionStatus == Business.Core.AuctionStatus.Active)
                {
                    var activeAuctions = await auctionsRepository.SearchAsync(new AuctionSearchParamsDto()
                    {
                        AuctionStatus = Business.Core.AuctionStatus.Active,
                        VehicleID = existingAuction.VehicleID.GetValueOrDefault()
                    });

                    if (activeAuctions.Any())
                        return new(false, "There is already an active auction for the given vehicle");
                }

                var validationResult = await validator.ValidateAsync(existingAuction);
                if (!validationResult.IsValid)
                {
                    return new(false, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                }

                var updateAuctionResult = await auctionsRepository.UpdateAsync(existingAuction);
                if (!updateAuctionResult.success)
                    return new(false, updateAuctionResult.message);
            }

            return new(true, "Auction updated with success");
        }

        private static double GetAuctionCurrentMinimunBid(Auction auction)
        {
            if (auction.CurrentAuctionBid is null || auction.CurrentAuctionBid.AuctionBidAmount <= 0)
                return auction.Vehicle?.VehicleStartingBid ?? 0;
            else
                return auction.CurrentAuctionBid?.AuctionBidAmount ?? 0;
        }
    }
}