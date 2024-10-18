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
                AuctionStartDate = new DateTime(auctionDto.AuctionStartDate.Year, auctionDto.AuctionStartDate.Month, auctionDto.AuctionStartDate.Day, 0, 0, 0),
                AuctionEndDate = new DateTime(auctionDto.AuctionEndDate.Year, auctionDto.AuctionEndDate.Month, auctionDto.AuctionEndDate.Day, 0, 0, 0),

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

            // To ensure that if ScheduledJob fails to update the AuctionStatus, we still only return Live auctions
            if(searchParamsDto.AuctionStatus != null && searchParamsDto.AuctionStatus == Business.Core.AuctionStatus.Active)
            {
                auctions = auctions.Where(a => a.AuctionStartDate < DateTime.UtcNow && a.AuctionEndDate > DateTime.UtcNow);
            }

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
                existingAuction.AuctionStartDate = new DateTime(
                    updateAuctionDto.AuctionStartDate.Value.Year,
                    updateAuctionDto.AuctionStartDate.Value.Month,
                    updateAuctionDto.AuctionStartDate.Value.Day,
                    0, 0, 0);

                updateAuction = true;
            }

            if (updateAuctionDto.AuctionEndDate != null && updateAuctionDto.AuctionEndDate != existingAuction.AuctionEndDate)
            {
                existingAuction.AuctionStartDate = new DateTime(
                    updateAuctionDto.AuctionEndDate.Value.Year,
                    updateAuctionDto.AuctionEndDate.Value.Month,
                    updateAuctionDto.AuctionEndDate.Value.Day,
                    0, 0, 0);

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