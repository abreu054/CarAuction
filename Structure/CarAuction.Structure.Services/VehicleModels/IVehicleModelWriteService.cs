using CarAuction.Structure.Dto.Write;

namespace CarAuction.Structure.Services.VehicleModels
{
    /// <summary>
    /// Service responsible for creating VehicleModels
    /// </summary>
    public interface IVehicleModelWriteService
    {
        Task<CreateEntityResponseDto> CreateVehicleModelAsync(CreateVehicleModelRequestDto vehicleModelDto);
    }
}