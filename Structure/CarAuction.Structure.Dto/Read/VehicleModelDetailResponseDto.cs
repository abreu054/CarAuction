namespace CarAuction.Structure.Dto.Read
{
    public class VehicleModelDetailResponseDto
    {
        public int VehicleModelID { get; set; }

        public string VehicleModelName { get; set; } = string.Empty;

        public int VehicleManufacturerID { get; set; }

        public string VehicleManufacturerName { get; set; } = string.Empty;
    }
}