using CarAuction.Business.Core;

namespace CarAuction.Structure.Dto.Read
{
    public class VehicleDetailResponseDto
    {
        public int VehicleID { get; set; }

        public string VehicleUniqueIdentifier { get; set; } = string.Empty;

        public VehicleType VehicleType { get; set; }

        public int VehicleYear { get; set; }

        public double VehicleStartingBid { get; set; }

        public int? VehicleNumberOfDoors { get; set; }

        public int? VehicleNumberOfSeats { get; set; }

        public double? VehicleLoadCapacity { get; set; }

        public string VehicleManufacturerName { get; set; } = string.Empty;

        public string VehicleModelName { get; set; } = string.Empty;
    }
}