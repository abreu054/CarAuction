using CarAuction.Business.Validators;
using CarAuction.Structure.DataRepositories;
using CarAuction.Structure.Services.VehicleManufacturers;
using CarAuction.Structure.Services.VehicleModels;
using CarAuction.Structure.Services.Vehicles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CarAuction.Structure.Services
{
    public static class ServicesFactory
    {
        public static void RegisterServices(IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddScoped<IVehicleReadService, VehicleService>();
            serviceCollection.AddScoped<IVehicleWriteService, VehicleService>();

            serviceCollection.AddScoped<IVehicleManufacturerReadService, VehicleManufacturerService>();
            serviceCollection.AddScoped<IVehicleManufacturerWriteService, VehicleManufacturerService>();

            serviceCollection.AddScoped<IVehicleModelReadService, VehicleModelService>();
            serviceCollection.AddScoped<IVehicleModelWriteService, VehicleModelService>();

            serviceCollection.AddScoped<IAuctionReadService, AuctionService>();
            serviceCollection.AddScoped<IAuctionWriteService, AuctionService>();

            serviceCollection.AddScoped<IAuctionBidReadService, AuctionBidService>();
            serviceCollection.AddScoped<IAuctionBidWriteService, AuctionBidService>();

            DataRepositoriesFactory.RegisterServices(serviceCollection, configuration);
            ValidatorsFactory.RegisterServices(serviceCollection, configuration);
        }
    }
}