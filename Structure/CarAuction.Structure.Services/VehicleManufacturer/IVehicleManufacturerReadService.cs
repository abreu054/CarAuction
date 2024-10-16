using CarAuction.Structure.Dto.Read;
using CarAuction.Structure.Dto.Search;

namespace CarAuction.Structure.Services.VehicleManufacturers
{
    /// <summary>
    /// Service responsible for searching Manufacturers
    /// </summary>
    public interface IVehicleManufacturerReadService
    {
        Task<IEnumerable<VehicleManufacturerDetailResponseDto>> SearchVehicleManufacturerAsync(VehicleManufacturerSearchParamsDto searchParamsDto);
    }
}