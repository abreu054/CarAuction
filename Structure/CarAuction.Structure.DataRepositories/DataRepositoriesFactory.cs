using CarAuction.Business.Dbo.Models.Auctions;
using CarAuction.Business.Dbo.Models.Vehicles;
using CarAuction.Structure.DataRepositories.Auctions;
using CarAuction.Structure.DataRepositories.Vehicles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CarAuction.Structure.DataRepositories
{
    public class DataRepositoriesFactory
    {
        public static void RegisterServices(IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddScoped<IDataRepository<Vehicle>, VehiclesDataRepository>();
            serviceCollection.AddScoped<IDataRepository<VehicleManufacturer>, VehicleManufacturerDataRepository>();
            serviceCollection.AddScoped<IDataRepository<VehicleModel>, VehicleModelDataRepository>();

            serviceCollection.AddScoped<IDataRepository<Auction>, AuctionDataRepository>();
            serviceCollection.AddScoped<IDataRepository<AuctionBid>, AuctionBidRepository>();
        }
    }
}
