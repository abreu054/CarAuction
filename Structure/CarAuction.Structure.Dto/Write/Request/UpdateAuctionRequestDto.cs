using CarAuction.Business.Core;

namespace CarAuction.Structure.Dto.Write
{
    public class UpdateAuctionRequestDto
    {
        public int AuctionID { get; set; }

        public AuctionStatus AuctionStatus { get; set; }

        public DateTime? AuctionStartDate { get; set; }

        public DateTime? AuctionEndDate { get; set; }
    }
}