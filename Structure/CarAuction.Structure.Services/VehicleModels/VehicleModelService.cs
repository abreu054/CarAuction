using CarAuction.Business.Dbo.Models.Vehicles;
using CarAuction.Structure.DataRepositories;
using CarAuction.Structure.Dto.Read;
using CarAuction.Structure.Dto.Search;
using CarAuction.Structure.Dto.Write;
using Microsoft.Extensions.Logging;

namespace CarAuction.Structure.Services.VehicleModels
{
    internal sealed class VehicleModelService(ILogger<VehicleModelService> logger,
        IDataRepository<VehicleModel> vehicleModelRepository,
        IDataRepository<VehicleManufacturer> vehicleManufacturerRepository)
        : IVehicleModelReadService, IVehicleModelWriteService
    {
        public async Task<CreateEntityResponseDto> CreateVehicleModelAsync(CreateVehicleModelRequestDto vehicleModelDto)
        {
            logger.LogInformation("Trying to Create new vehicle model");

            if (string.IsNullOrWhiteSpace(vehicleModelDto.VehicleModelName))
                return new(false, "Model name cannot be empty");

            if (vehicleModelDto.VehicleManufacturerID <= 0)
                return new(false, "Vehicle manufacturer cannot be empty");

            var vehicleManufacturer = await vehicleManufacturerRepository.GetByIDAsync(vehicleModelDto.VehicleManufacturerID);
            if (vehicleManufacturer is null)
                return new(false, "Vehicle manufacturer was not found");

            var existingVehicleModel = await vehicleModelRepository.SearchAsync(new VehicleModelSearchParamsDto()
            {
                VehicleModelName = vehicleModelDto.VehicleModelName,
                VehicleManufacturerID = vehicleModelDto.VehicleManufacturerID
            });
            if (existingVehicleModel.Any()) return new(false, "A model with the same name already exists");

            var newVehicleModel = new VehicleModel()
            {
                VehicleModelName = vehicleModelDto.VehicleModelName,
                VehicleManufacturerID = vehicleManufacturer.VehicleManufacturerID
            };

            var (success, message) = await vehicleModelRepository.CreateAsync(newVehicleModel);
            return new(success, success ? "Vehicle model created with success" : message);
        }

        public async Task<IEnumerable<VehicleModelDetailResponseDto>> SearchVehicleModelsAsync(VehicleModelSearchParamsDto searchParamsDto)
        {
            var vehicles = await vehicleModelRepository.SearchAsync(searchParamsDto);
            if (vehicles is null || !vehicles.Any())
                return [];

            return vehicles.Select(vehicle => new VehicleModelDetailResponseDto()
            {
                VehicleModelID = vehicle.VehicleModelID,
                VehicleModelName = vehicle.VehicleModelName,

                VehicleManufacturerID = vehicle.VehicleManufacturerID.Value,
                VehicleManufacturerName = vehicle.VehicleManufacturer.VehicleManufacturerName
            });
        }
    }
}