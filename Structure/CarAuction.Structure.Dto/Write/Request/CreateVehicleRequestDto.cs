using CarAuction.Business.Core;
using System.ComponentModel.DataAnnotations;

namespace CarAuction.Structure.Dto.Write
{
    public class CreateVehicleRequestDto
    {
        [Required]
        public string VehicleUniqueIdentifier { get; set; } = string.Empty;

        [Required]
        public VehicleType VehicleType { get; set; }

        public int VehicleYear { get; set; }

        public double? VehicleLoadCapacity { get; set; }

        public int? VehicleNumberOfDoors { get; set; }

        public int? VehicleNumberOfSeats { get; set; }

        public double VehicleStartingBid { get; set; }

        [Required]
        public int VehicleManufacturerID { get; set; }

        [Required]
        public int VehicleModelID { get; set; }
    }
}