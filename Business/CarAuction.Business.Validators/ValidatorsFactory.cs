using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CarAuction.Business.Validators
{
    public static class ValidatorsFactory
    {
        public static void RegisterServices(IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddSingleton(new VehicleValidator());
            serviceCollection.AddSingleton(new AuctionValidator());
            serviceCollection.AddSingleton(new AuctionBidValidator());
        }
    }
}