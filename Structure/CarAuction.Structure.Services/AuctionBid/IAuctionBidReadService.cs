using CarAuction.Structure.Dto.Read;

namespace CarAuction.Structure.Services
{
    /// <summary>
    /// Service responsible for searching AuctionsBids
    /// </summary>
    public interface IAuctionBidReadService
    {
        Task<AuctionBidDetailResponseDto?> GetAuctionCurrentBidAsync(int auctionID);
    }
}