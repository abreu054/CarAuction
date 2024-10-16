using System.ComponentModel.DataAnnotations;

namespace CarAuction.Business.Dbo.Models.Vehicles
{
    public class VehicleModel
    {
        [Key]
        public int VehicleModelID { get; set; }

        [Required]
        public string VehicleModelName { get; set; } = string.Empty;

        #region Relationships

        public int? VehicleManufacturerID { get; set; }

        public VehicleManufacturer? VehicleManufacturer { get; set; }

        public ICollection<Vehicle> Vehicles { get; set; } = [];

        #endregion
    }
}