using CarAuction.Structure.Dto.Read;
using CarAuction.Structure.Dto.Search;
using CarAuction.Structure.Dto.Write;
using CarAuction.Structure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CarAuction.Application.App.Controllers
{
    [Route("api/auctions")]
    [ApiController]
    public class AuctionsApiController(
        SignInManager<IdentityUser> signInManager,
        IAuctionReadService auctionReadService,
        IAuctionWriteService auctionWriteService) : ControllerBase
    {
        //[Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AuctionDetailResponseDto), StatusCodes.Status200OK, "application/json")]
        public async Task<IActionResult> GetAuctionAsync([FromRoute] int id)
        {
            var auction = (await auctionReadService.SearchAuctionsAsync(new AuctionSearchParamsDto()
            {
                EntityID = id
            })).FirstOrDefault();

            return Ok(auction);
        }

        [HttpPost("search")]
        [ProducesResponseType(typeof(AuctionDetailResponseDto[]), StatusCodes.Status200OK, "application/json")]
        public async Task<IActionResult> SearchAuctionsAsync([FromBody] AuctionSearchParamsDto searchParamsDto)
        {
            var auctions = await auctionReadService.SearchAuctionsAsync(searchParamsDto);
            return Ok(auctions.ToArray());
        }

        //[Authorize]
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(CreateEntityResponseDto), StatusCodes.Status400BadRequest, "application/json")]
        [ProducesResponseType(typeof(CreateEntityResponseDto), StatusCodes.Status401Unauthorized, "application/json")]
        public async Task<IActionResult> CreateAuctionAsync([FromBody] CreateAuctionRequestDto createAuctionRequestDto)
        {
            if (!signInManager.IsSignedIn(HttpContext.User))
                return Unauthorized(new CreateEntityResponseDto(false, "Cannot perform this action"));

            var createEntityResponse = await auctionWriteService.CreateAuctionAsync(createAuctionRequestDto);
            if (createEntityResponse.Success)
                return Created();
            else
                return BadRequest(createEntityResponse);
        }

        //[Authorize]
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(UpdateEntityResponseDto), StatusCodes.Status400BadRequest, "application/json")]
        [ProducesResponseType(typeof(UpdateEntityResponseDto), StatusCodes.Status401Unauthorized, "application/json")]
        public async Task<IActionResult> PatchAuctionAsync([FromBody] UpdateAuctionRequestDto updateAuctionRequestDto)
        {
            if (!signInManager.IsSignedIn(HttpContext.User))
                return Unauthorized(new UpdateEntityResponseDto(false, "Cannot perform this action"));

            var updateEntityResponseDto = await auctionWriteService.UpdateAuctionAsync(updateAuctionRequestDto);
            if (updateEntityResponseDto.Success)
                return NoContent();
            else
                return BadRequest(updateEntityResponseDto);
        }
    }
}