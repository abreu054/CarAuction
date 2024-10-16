using CarAuction.Structure.Dto.Read;
using CarAuction.Structure.Dto.Write;
using CarAuction.Structure.Services.VehicleManufacturers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CarAuction.Application.App.Controllers
{
    [Route("api/manufacturers")]
    [ApiController]
    public class ManufacturersApiController(
        SignInManager<IdentityUser> signInManager,
        IVehicleManufacturerReadService vehicleManufacturerReadService,
        IVehicleManufacturerWriteService vehicleManufacturerWriteService)
        : ControllerBase
    {
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(VehicleManufacturerDetailResponseDto), StatusCodes.Status200OK, "application/json")]
        public async Task<IActionResult> GetManufacturerAsync([FromRoute] int id)
        {
            var manufacturer = (await vehicleManufacturerReadService
                .SearchVehicleManufacturerAsync(new Structure.Dto.Search.VehicleManufacturerSearchParamsDto()
                {
                    EntityID = id
                })).FirstOrDefault();

            return Ok(manufacturer);
        }

        [HttpGet("list")]
        [ProducesResponseType(typeof(VehicleManufacturerDetailResponseDto[]), StatusCodes.Status200OK, "application/json")]
        public async Task<IActionResult> GetManufacturersAsync()
        {
            var manufacturers = await vehicleManufacturerReadService.SearchVehicleManufacturerAsync(new Structure.Dto.Search.VehicleManufacturerSearchParamsDto());
            return Ok(manufacturers);
        }

        //[Authorize]
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(CreateEntityResponseDto), StatusCodes.Status400BadRequest, "application/json")]
        [ProducesResponseType(typeof(CreateEntityResponseDto), StatusCodes.Status401Unauthorized, "application/json")]
        public async Task<IActionResult> CreateManufacturerAsync([FromBody] CreateVehicleManufacturerRequestDto createVehicleManufacturerDto)
        {
            if (!signInManager.IsSignedIn(HttpContext.User))
                return Unauthorized(new CreateEntityResponseDto(false, "Cannot perform this action"));

            var createEntityResponse = await vehicleManufacturerWriteService.CreateVehicleManufacturerAsync(createVehicleManufacturerDto);
            if (createEntityResponse.Success)
                return Created();
            else
                return BadRequest(createEntityResponse);
        }
    }
}