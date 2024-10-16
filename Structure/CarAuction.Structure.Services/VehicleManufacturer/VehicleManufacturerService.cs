using CarAuction.Business.Dbo.Models.Vehicles;
using CarAuction.Structure.DataRepositories;
using CarAuction.Structure.Dto.Read;
using CarAuction.Structure.Dto.Search;
using CarAuction.Structure.Dto.Write;
using Microsoft.Extensions.Logging;

namespace CarAuction.Structure.Services.VehicleManufacturers
{
    internal sealed class VehicleManufacturerService(ILogger<VehicleManufacturerService> logger,
        IDataRepository<VehicleManufacturer> dataRepository)
        : IVehicleManufacturerReadService, IVehicleManufacturerWriteService
    {
        public async Task<CreateEntityResponseDto> CreateVehicleManufacturerAsync(CreateVehicleManufacturerRequestDto vehicleManufacturerDto)
        {
            logger.LogInformation("Trying to Create new vehicle manufacturer");

            if (string.IsNullOrWhiteSpace(vehicleManufacturerDto.VehicleManufacturerName))
                return new(false, "Manufacturer name cannot be empty");

            var existingVehicleManufacturer = await dataRepository.SearchAsync(new VehicleManufacturerSearchParamsDto()
            {
                VehicleManufacturerName = vehicleManufacturerDto.VehicleManufacturerName,
            });
            if (existingVehicleManufacturer.Any()) return new(false, "A manufacturer with the same name already exists");

            var newVehicleManufacturer = new VehicleManufacturer()
            {
                VehicleManufacturerName = vehicleManufacturerDto.VehicleManufacturerName
            };

            var (success, message) = await dataRepository.CreateAsync(newVehicleManufacturer);
            return new(success, success ? "Vehicle manufacturer created with success" : message);
        }

        public async Task<IEnumerable<VehicleManufacturerDetailResponseDto>> SearchVehicleManufacturerAsync(VehicleManufacturerSearchParamsDto searchParamsDto)
        {
            var vehicleManufacturers = await dataRepository.SearchAsync(searchParamsDto);
            if (vehicleManufacturers is null || !vehicleManufacturers.Any())
                return [];

            return vehicleManufacturers.Select(vehicle => new VehicleManufacturerDetailResponseDto()
            {
                VehicleManufacturerID = vehicle.VehicleManufacturerID,
                VehicleManufacturerName = vehicle.VehicleManufacturerName
            });
        }
    }
}