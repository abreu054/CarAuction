using CarAuction.Structure.Dto.Read;
using CarAuction.Structure.Dto.Search;

namespace CarAuction.Structure.Services.VehicleModels
{
    /// <summary>
    /// Service responsible for searching VehicleModels
    /// </summary>
    public interface IVehicleModelReadService
    {
        Task<IEnumerable<VehicleModelDetailResponseDto>> SearchVehicleModelsAsync(VehicleModelSearchParamsDto searchParamsDto);
    }
}