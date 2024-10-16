using CarAuction.Structure.Dto.Read;
using CarAuction.Structure.Dto.Search;
using CarAuction.Structure.Dto.Write;
using CarAuction.Structure.Services.Vehicles;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CarAuction.Application.App.Controllers
{
    [Route("api/vehicles")]
    [ApiController]
    public class VehiclesApiController(
        SignInManager<IdentityUser> signInManager,
        IVehicleReadService vehicleReadService,
        IVehicleWriteService vehicleWriteService)
        : ControllerBase
    {
        [HttpGet("list")]
        [ProducesResponseType(typeof(VehicleDetailResponseDto[]), StatusCodes.Status200OK, "application/json")]
        public async Task<IActionResult> GetVehiclesAsync()
        {
            var vehicles = await vehicleReadService.SearchVehiclesAsync(new VehiclesSearchParamsDto());
            return Ok(vehicles);
        }

        [HttpPost("search")]
        [ProducesResponseType(typeof(VehicleDetailResponseDto[]), StatusCodes.Status200OK, "application/json")]
        public async Task<IActionResult> SearchVehiclesAsync([FromBody] VehiclesSearchParamsDto vehiclesSearchParamsDto)
        {
            var vehicles = await vehicleReadService.SearchVehiclesAsync(vehiclesSearchParamsDto);
            return Ok(vehicles);
        }

        //[Authorize]
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(CreateEntityResponseDto), StatusCodes.Status400BadRequest, "application/json")]
        [ProducesResponseType(typeof(CreateEntityResponseDto), StatusCodes.Status401Unauthorized, "application/json")]
        public async Task<IActionResult> CreateVehicleAsync([FromBody] CreateVehicleRequestDto createVehicleRequestDto)
        {
            if (!signInManager.IsSignedIn(HttpContext.User))
                return Unauthorized(new CreateEntityResponseDto(false, "Cannot perform this action"));

            var createEntityResponse = await vehicleWriteService.CreateVehicleAsync(createVehicleRequestDto);
            if (createEntityResponse.Success)
                return Created();
            else
                return BadRequest(createEntityResponse);
        }
    }
}