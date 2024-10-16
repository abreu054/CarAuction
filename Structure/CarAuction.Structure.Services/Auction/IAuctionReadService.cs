using CarAuction.Structure.Dto.Read;
using CarAuction.Structure.Dto.Search;

namespace CarAuction.Structure.Services
{
    /// <summary>
    /// Service responsible for searching Auctions
    /// </summary>
    public interface IAuctionReadService
    {
        Task<IEnumerable<AuctionDetailResponseDto>> SearchAuctionsAsync(AuctionSearchParamsDto searchParamsDto);
    }
}