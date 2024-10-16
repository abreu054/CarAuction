using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CarAuction.Business.Dbo.Models.Auctions
{
    public class AuctionBid
    {
        [Key]
        public int AuctionBidID { get; set; }

        [Required]
        public double AuctionBidAmount { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime AuctionBidInsertDate { get; set; }

        #region Relationships

        // We could move all AuctionBids to an History table for Auctions that are no longer active

        public int AuctionID { get; set; }

        public Auction? Auction { get; set; }

        public string UserID { get; set; } = string.Empty;

        public IdentityUser? User { get; set; }

        #endregion
    }
}