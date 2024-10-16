using System.ComponentModel.DataAnnotations;

namespace CarAuction.Structure.Dto.Write
{
    public class CreateVehicleModelRequestDto
    {
        [Required]
        public string VehicleModelName { get; set; } = string.Empty;

        public int VehicleManufacturerID { get; set; }
    }
}