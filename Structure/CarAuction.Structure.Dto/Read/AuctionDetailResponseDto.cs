using CarAuction.Business.Core;

namespace CarAuction.Structure.Dto.Read
{
    public class AuctionDetailResponseDto
    {
        public int AuctionID { get; set; }

        public AuctionStatus AuctionStatus { get; set; }

        public DateTime AuctionStartDate { get; set; }

        public DateTime AuctionEndDate { get; set; }

        public double AuctionStartingBid { get; set; }

        public double AuctionCurrentBid { get; set; }

        public double AuctionCurrentMinimunBid { get; set; }

        public int VehicleID { get; set; }

        public string VehicleUniqueIdentifier { get; set; } = string.Empty;

        public int VehicleManufacturerID { get; set; }

        public string VehicleManufacturerName { get; set; } = string.Empty;

        public int VehicleModelID { get; set; }

        public string VehicleModelName { get; set; } = string.Empty;
    }
}