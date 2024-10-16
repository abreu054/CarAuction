using System.ComponentModel.DataAnnotations;

namespace CarAuction.Structure.Dto.Write
{
    public class CreateVehicleManufacturerRequestDto
    {
        [Required]
        public string VehicleManufacturerName { get; set; } = string.Empty;
    }
}