using CarAuction.Business.Dbo.Models.Auctions;
using CarAuction.Business.Dbo.Models.Vehicles;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CarAuction.Business.Dbo
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<IdentityUser>(options)
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Vehicles

            builder.Entity<Vehicle>()
                .HasIndex(v => v.VehicleUniqueIdentifier)
                .IsUnique();

            builder.Entity<Vehicle>()
                .HasIndex(v => v.VehicleType);

            builder.Entity<Vehicle>()
                .HasOne(v => v.VehicleManufacturer)
                .WithMany(vm => vm.Vehicles)
                .HasForeignKey(v => v.VehicleManufacturerID);            

            builder.Entity<Vehicle>()
                .HasOne(v => v.VehicleModel)
                .WithMany(vm => vm.Vehicles)
                .HasForeignKey(v => v.VehicleModelID);

            builder.Entity<VehicleModel>()
                .HasOne(vm => vm.VehicleManufacturer)
                .WithMany(vm => vm.VehicleModels)
                .HasForeignKey(vm => vm.VehicleManufacturerID);

            // ----

            // Auctions

            builder.Entity<Auction>()
                .HasOne(a => a.Vehicle)
                .WithMany(v => v.Auctions)
                .HasForeignKey(a => a.VehicleID)
                .IsRequired(false);

            builder.Entity<Auction>()
                .HasOne(a => a.CurrentAuctionBid)
                .WithOne()
                .HasForeignKey<Auction>(a => a.CurrentAuctionBidID)
                .IsRequired(false);

            builder.Entity<AuctionBid>()
                .HasOne(ab => ab.Auction)
                .WithMany(a => a.AuctionBids)
                .HasForeignKey(ab => ab.AuctionID);

            builder.Entity<AuctionBid>()
                .HasOne(ab => ab.User);

            // ----

            base.OnModelCreating(builder);
        }

        public DbSet<Vehicle> Vehicles { get; set; }

        public DbSet<VehicleManufacturer> VehicleManufacturers { get; set; }

        public DbSet<VehicleModel> VehicleModels { get; set; }

        public DbSet<Auction> Auctions { get; set; }

        public DbSet<AuctionBid> AuctionBids { get; set; }
    }
}