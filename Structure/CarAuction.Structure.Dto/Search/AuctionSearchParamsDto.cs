using CarAuction.Business.Core;

namespace CarAuction.Structure.Dto.Search
{
    public class AuctionSearchParamsDto : BaseSearchParamsDto
    {
        public int VehicleID { get; set; }

        public AuctionStatus? AuctionStatus { get; set; }

        public VehicleType? VehicleType { get; set; }
        
        public int VehicleYear { get; set; }

        public int VehicleManufacturerID { get; set; }

        public int VehicleModelID { get; set; }
    }
}