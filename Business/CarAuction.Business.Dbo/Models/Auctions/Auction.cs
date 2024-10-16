using CarAuction.Business.Core;
using CarAuction.Business.Dbo.Models.Vehicles;
using System.ComponentModel.DataAnnotations;

namespace CarAuction.Business.Dbo.Models.Auctions
{
    public class Auction
    {
        [Key]
        public int AuctionID { get; set; }

        [Required]
        [EnumDataType(typeof(AuctionStatus))]
        public AuctionStatus AuctionStatus { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime AuctionStartDate { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime AuctionEndDate { get; set; }

        #region Relationships

        public int? CurrentAuctionBidID { get; set; }

        public AuctionBid? CurrentAuctionBid { get; set; }

        public int? VehicleID { get; set; }

        public Vehicle? Vehicle { get; set; }

        public ICollection<AuctionBid> AuctionBids { get; set; } = [];

        #endregion
    }
}