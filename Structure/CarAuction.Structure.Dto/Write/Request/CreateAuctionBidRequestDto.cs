namespace CarAuction.Structure.Dto.Write
{
    public class CreateAuctionBidRequestDto
    {
        public int AuctionID { get; set; }

        public double AuctionBidAmount { get; set; }

        public string UserID { get; set; } = string.Empty;
    }
}