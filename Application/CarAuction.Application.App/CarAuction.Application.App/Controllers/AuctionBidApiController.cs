using CarAuction.Structure.Dto.Write;
using CarAuction.Structure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CarAuction.Application.App.Controllers
{
    [Authorize]
    [Route("api/auctionbids")]
    [ApiController]
    public class AuctionBidApiController(UserManager<IdentityUser> userManager, IAuctionBidWriteService auctionBidWriteService) : ControllerBase
    {
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(CreateEntityResponseDto), StatusCodes.Status401Unauthorized, "application/json")]
        [ProducesResponseType(typeof(CreateEntityResponseDto), StatusCodes.Status400BadRequest, "application/json")]
        public async Task<IActionResult> CreateAuctionBidAsync([FromBody] CreateAuctionBidRequestDto auctionBidRequestDto)
        {
            var currentUser = await userManager.GetUserAsync(HttpContext.User);
            if (currentUser is null)
                return Unauthorized(new CreateEntityResponseDto(success: false, message: "Cannot find user"));

            auctionBidRequestDto.UserID = currentUser.Id;
            var createEntityResponseDto = await auctionBidWriteService.CreateAuctionBidAsync(auctionBidRequestDto);
            if (createEntityResponseDto.Success)
                return Created();
            else
                return BadRequest(createEntityResponseDto);
        }
    }
}