using CarAuction.Business.Dbo.Models.Vehicles;
using CarAuction.Business.Validators;
using CarAuction.Structure.DataRepositories;
using CarAuction.Structure.Dto.Read;
using CarAuction.Structure.Dto.Search;
using CarAuction.Structure.Dto.Write;
using Microsoft.Extensions.Logging;

namespace CarAuction.Structure.Services.Vehicles
{
    public sealed class VehicleService(ILogger<VehicleService> logger,
        VehicleValidator validator,
        IDataRepository<Vehicle> vehiclesRepository,
        IDataRepository<VehicleModel> vehicleModelsRepository)
        : IVehicleReadService, IVehicleWriteService
    {
        public async Task<CreateEntityResponseDto> CreateVehicleAsync(CreateVehicleRequestDto vehicleDto)
        {
            logger.LogInformation("Trying to Create new vehicle");

            if (string.IsNullOrWhiteSpace(vehicleDto.VehicleUniqueIdentifier))
                return new(false, "Vehicle identifier cannot be empty");

            var existingVehicle = await vehiclesRepository.SearchAsync(new VehiclesSearchParamsDto()
            {
                VehicleUniqueIdentifier = vehicleDto.VehicleUniqueIdentifier,
            });
            if (existingVehicle.Any()) return new(false, "A vehicle with the same Identifier already exists");

            if (vehicleDto.VehicleModelID <= 0) return new(false, "Vehicle model cannot be empty");

            var vehicleModel = await vehicleModelsRepository.GetByIDAsync(vehicleDto.VehicleModelID);
            if (vehicleModel is null) return new(false, "Vehicle model was not found");

            var newVehicle = new Vehicle()
            {
                VehicleUniqueIdentifier = vehicleDto.VehicleUniqueIdentifier,
                VehicleType = vehicleDto.VehicleType,
                VehicleStartingBid = vehicleDto.VehicleStartingBid,
                VehicleNumberOfSeats = vehicleDto.VehicleNumberOfSeats,
                VehicleNumberOfDoors = vehicleDto.VehicleNumberOfDoors,
                VehicleYear = vehicleDto.VehicleYear,
                VehicleLoadCapacity = vehicleDto.VehicleLoadCapacity,
                VehicleInsertDate = DateTime.UtcNow,

                VehicleManufacturerID = vehicleModel.VehicleManufacturerID,
                VehicleModelID = vehicleModel.VehicleModelID,
            };

            var validationResult = await validator.ValidateAsync(newVehicle);
            if (!validationResult.IsValid) return new(false, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

            var (success, message) = await vehiclesRepository.CreateAsync(newVehicle);
            return new(success, success ? "Vehicle created with success" : message);
        }

        public async Task<IEnumerable<VehicleDetailResponseDto>> SearchVehiclesAsync(VehiclesSearchParamsDto searchParamsDto)
        {
            var vehicles = await vehiclesRepository.SearchAsync(searchParamsDto);
            if (vehicles is null || !vehicles.Any())
                return [];

            return vehicles.Select(vehicle => new VehicleDetailResponseDto()
            {
                VehicleID = vehicle.VehicleID,
                VehicleUniqueIdentifier = vehicle.VehicleUniqueIdentifier,
                VehicleType = vehicle.VehicleType,

                VehicleYear = vehicle.VehicleYear,
                VehicleStartingBid = vehicle.VehicleStartingBid,
                VehicleNumberOfDoors = vehicle.VehicleNumberOfDoors,
                VehicleNumberOfSeats = vehicle.VehicleNumberOfSeats,
                VehicleLoadCapacity = vehicle.VehicleLoadCapacity,

                VehicleManufacturerName = vehicle.VehicleManufacturer?.VehicleManufacturerName ?? string.Empty,
                VehicleModelName = vehicle.VehicleModel?.VehicleModelName ?? string.Empty
            });
        }

        //TODO If we implement the DeleteVehicle method, we need to verify if there isnt any active auction for the vehicle
    }
}