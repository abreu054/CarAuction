using CarAuction.Structure.Dto.Write;

namespace CarAuction.Structure.Services.Vehicles
{
    /// <summary>
    /// Service responsible for creating Vehicles
    /// </summary>
    public interface IVehicleWriteService
    {
        Task<CreateEntityResponseDto> CreateVehicleAsync(CreateVehicleRequestDto vehicleDto);
    }
}