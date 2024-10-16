using CarAuction.Business.Core;

namespace CarAuction.Structure.Dto.Write
{
    public class CreateAuctionRequestDto
    {
        public AuctionStatus AuctionStatus { get; set; }

        public DateTime AuctionStartDate { get; set; }

        public DateTime AuctionEndDate { get; set; }

        public int VehicleID { get; set; }
    }
}