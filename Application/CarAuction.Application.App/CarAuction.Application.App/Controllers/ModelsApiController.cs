using CarAuction.Structure.Dto.Read;
using CarAuction.Structure.Dto.Write;
using CarAuction.Structure.Services.VehicleModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CarAuction.Application.App.Controllers
{
    [Route("api/models")]
    [ApiController]
    public class ModelsApiController(
        SignInManager<IdentityUser> signInManager,
        IVehicleModelReadService vehicleModelReadService,
        IVehicleModelWriteService vehicleModelWriteService)
        : ControllerBase
    {
        [HttpGet("list")]
        [ProducesResponseType(typeof(VehicleModelDetailResponseDto[]), StatusCodes.Status200OK, "application/json")]
        public async Task<IActionResult> GetModelsAsync()
        {
            var models = await vehicleModelReadService.SearchVehicleModelsAsync(new Structure.Dto.Search.VehicleModelSearchParamsDto());
            return Ok(models);
        }

        //[Authorize]
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(CreateEntityResponseDto), StatusCodes.Status400BadRequest, "application/json")]
        [ProducesResponseType(typeof(CreateEntityResponseDto), StatusCodes.Status401Unauthorized, "application/json")]
        public async Task<IActionResult> CreateModelAsync([FromBody] CreateVehicleModelRequestDto createVehicleModelRequestDto)
        {
            if (!signInManager.IsSignedIn(HttpContext.User))
                return Unauthorized(new CreateEntityResponseDto(false, "Cannot perform this action"));

            var createEntityResponse = await vehicleModelWriteService.CreateVehicleModelAsync(createVehicleModelRequestDto);
            if (createEntityResponse.Success)
                return Created();
            else
                return BadRequest(createEntityResponse);
        }
    }
}