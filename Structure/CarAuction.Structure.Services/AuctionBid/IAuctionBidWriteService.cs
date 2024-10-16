using CarAuction.Structure.Dto.Write;

namespace CarAuction.Structure.Services
{
    /// <summary>
    /// Service responsible for creating AuctionBids
    /// </summary>
    public interface IAuctionBidWriteService
    {
        Task<CreateEntityResponseDto> CreateAuctionBidAsync(CreateAuctionBidRequestDto auctionBidDto);
    }
}