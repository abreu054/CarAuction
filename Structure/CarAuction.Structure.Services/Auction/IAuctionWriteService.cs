using CarAuction.Structure.Dto.Write;

namespace CarAuction.Structure.Services
{
    /// <summary>
    /// Service responsible for creating and updating Auctions
    /// </summary>
    public interface IAuctionWriteService
    {
        Task<CreateEntityResponseDto> CreateAuctionAsync(CreateAuctionRequestDto auctionDto);

        Task<UpdateEntityResponseDto> UpdateAuctionAsync(UpdateAuctionRequestDto updateAuctionDto);
    }
}