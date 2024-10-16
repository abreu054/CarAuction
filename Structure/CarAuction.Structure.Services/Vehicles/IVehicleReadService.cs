using CarAuction.Structure.Dto.Read;
using CarAuction.Structure.Dto.Search;

namespace CarAuction.Structure.Services.Vehicles
{
    /// <summary>
    /// Service responsible for searching Vehicles
    /// </summary>
    public interface IVehicleReadService
    {
        Task<IEnumerable<VehicleDetailResponseDto>> SearchVehiclesAsync(VehiclesSearchParamsDto searchParamsDto);
    }
}