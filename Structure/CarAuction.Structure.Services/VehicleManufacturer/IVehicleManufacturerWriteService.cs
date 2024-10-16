using CarAuction.Structure.Dto.Write;

namespace CarAuction.Structure.Services.VehicleManufacturers
{
    /// <summary>
    /// Service responsible for creating Manufacturers
    /// </summary>
    public interface IVehicleManufacturerWriteService
    {
        Task<CreateEntityResponseDto> CreateVehicleManufacturerAsync(CreateVehicleManufacturerRequestDto vehicleManufacturerDto);
    }
}