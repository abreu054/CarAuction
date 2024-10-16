using System.ComponentModel.DataAnnotations;

namespace CarAuction.Business.Dbo.Models.Vehicles
{
    public class VehicleManufacturer
    {
        [Key]
        public int VehicleManufacturerID { get; set; }

        [Required]
        public string VehicleManufacturerName { get; set; } = string.Empty;

        #region Relationships

        public ICollection<Vehicle> Vehicles { get; set; } = [];

        public ICollection<VehicleModel> VehicleModels { get; set; } = [];

        #endregion
    }
}