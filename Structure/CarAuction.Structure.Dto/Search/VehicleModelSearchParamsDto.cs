namespace CarAuction.Structure.Dto.Search
{
    public class VehicleModelSearchParamsDto : BaseSearchParamsDto
    {
        public string VehicleModelName { get; set; } = string.Empty;

        public int VehicleManufacturerID { get; set; }
    }
}