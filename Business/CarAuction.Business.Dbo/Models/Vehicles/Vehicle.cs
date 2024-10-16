using CarAuction.Business.Core;
using CarAuction.Business.Dbo.Models.Auctions;
using System.ComponentModel.DataAnnotations;

namespace CarAuction.Business.Dbo.Models.Vehicles
{
    public class Vehicle
    {
        [Key]
        public int VehicleID { get; set; }

        [Required]
        public string VehicleUniqueIdentifier { get; set; } = string.Empty;

        [Required]
        [EnumDataType(typeof(VehicleType))]
        public VehicleType VehicleType { get; set; }

        [Required]
        public int VehicleYear { get; set; }

        [Required]
        public double VehicleStartingBid { get; set; }

        public int? VehicleNumberOfDoors { get; set; }

        public int? VehicleNumberOfSeats { get; set; }

        public double? VehicleLoadCapacity { get; set; }

        public DateTime VehicleInsertDate { get; set; }

        public DateTime? VehicleUpdateDate { get; set; }

        #region Relationships

        public int? VehicleManufacturerID { get; set; }

        public VehicleManufacturer? VehicleManufacturer { get; set; }

        public int? VehicleModelID { get; set; }

        public VehicleModel? VehicleModel { get; set; }

        public ICollection<Auction>? Auctions { get; set; }

        #endregion
    }
}