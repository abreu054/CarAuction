namespace CarAuction.Structure.Dto.Read
{
    public class AuctionBidDetailResponseDto
    {
        public int AuctionID { get; set; }

        public int AuctionBidID { get; set; }

        public double AuctionBidAmount { get; set; }

        public DateTime AuctionBidInsertDate { get; set; }
    }
}