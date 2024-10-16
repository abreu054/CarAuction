using CarAuction.Business.Core;

namespace CarAuction.Structure.Dto.Search
{
    public class VehiclesSearchParamsDto : BaseSearchParamsDto
    {
        public string VehicleUniqueIdentifier { get; set; } = string.Empty;

        public VehicleType? VehicleType { get; set; }

        public int VehicleManufacturerID { get; set; }

        public int VehicleModelID { get; set; }

        public int VehicleYear { get; set; }

        public bool OnlyInActiveAuctions { get; set; }
    }
}