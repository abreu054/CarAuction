using CarAuction.Business.Core;
using CarAuction.Business.Dbo;
using CarAuction.Business.Dbo.Models.Vehicles;
using CarAuction.Structure.DataRepositories;
using CarAuction.Structure.Dto.Write;
using CarAuction.Structure.Services;
using CarAuction.Structure.Services.Vehicles;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace CarAuction.Application.ConsoleApp
{
    internal class Program
    {
        internal static IServiceProvider? _serviceProvider;
        internal static IConfiguration? _configuration;

        private static async Task Main(string[] args)
        {
            ConfigureServices();

            // Test integrations

            var insertTestData = _configuration!.GetValue<bool>("PopulateData");
            if (insertTestData)
            {
                await InsertTestDataAsync();
                await CreateAuctionBidAsync();
            }

            await CreateDuplicateVehicleAsync();
        }

        private static void ConfigureServices()
        {
            var configurationManager = new ConfigurationManager();
            configurationManager.AddJsonFile($"appsettings.json");

            ServiceCollection serviceCollection = new ServiceCollection();

            serviceCollection.AddLogging(options =>
            {
                options.AddConsole();
                options.SetMinimumLevel(LogLevel.Information);
            });

            var connectionString = configurationManager.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            serviceCollection.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString, builder => builder.MigrationsAssembly(Assembly.GetAssembly(typeof(ApplicationDbContext)).FullName));
            });

            serviceCollection.AddIdentityCore<IdentityUser>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            // Register services from Projects

            ServicesFactory.RegisterServices(serviceCollection, configurationManager);

            // ----

            _configuration = configurationManager;
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        private static async Task InsertTestDataAsync()
        {
            var dbContext = _serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = _serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            // Create or Get VehicleManufacturer
            VehicleManufacturer? vehicleManufacturer = default;
            if (!dbContext.VehicleManufacturers.Any())
            {
                var entityEntry = dbContext.VehicleManufacturers.Add(new VehicleManufacturer()
                {
                    VehicleManufacturerName = "Seat"
                });

                dbContext.SaveChanges();
                vehicleManufacturer = entityEntry.Entity;
            }
            else
            {
                vehicleManufacturer = dbContext.VehicleManufacturers.First();
            }

            // Create or Get VehicleModel
            VehicleModel? vehicleModel = default;
            if (vehicleManufacturer != null)
            {
                if (!dbContext.VehicleModels.Any())
                {
                    var entityEntry = dbContext.VehicleModels.Add(new VehicleModel()
                    {
                        VehicleModelName = "Leon",
                        VehicleManufacturer = vehicleManufacturer
                    });

                    dbContext.SaveChanges();
                    vehicleModel = entityEntry.Entity;
                }
                else
                {
                    vehicleModel = dbContext.VehicleModels.First();
                }
            }

            // Create or Get Vehicle
            Vehicle? vehicle = default;
            if (vehicleManufacturer != null && vehicleModel != null)
            {
                if (!dbContext.Vehicles.Any())
                {
                    var entityEntry = dbContext.Vehicles.Add(new Vehicle()
                    {
                        VehicleUniqueIdentifier = "STLN2020HB",
                        VehicleType = VehicleType.Hatchback,
                        VehicleNumberOfDoors = 5,
                        VehicleYear = 2020,
                        VehicleStartingBid = 19000,
                        VehicleManufacturer = vehicleManufacturer,
                        VehicleModel = vehicleModel,
                        VehicleInsertDate = DateTime.UtcNow
                    });

                    dbContext.SaveChanges();
                    vehicle = entityEntry.Entity;
                }
                else
                {
                    vehicle = dbContext.Vehicles.First();
                }
            }

            // Create or Get test User
            var userEmail = _configuration.GetValue<string>("TestData:User:Email");
            var userPassword = _configuration.GetValue<string>("TestData:User:Password");

            if (string.IsNullOrWhiteSpace(userEmail))
                throw new ArgumentNullException(nameof(userEmail));
            else if (string.IsNullOrWhiteSpace(userPassword))
                throw new ArgumentNullException(nameof(userPassword));

            IdentityUser? user = default;
            if (!userManager.Users.Any(u => u.Email == userEmail))
            {
                var createUserResult = await userManager.CreateAsync(new IdentityUser(userName: userEmail)
                {
                    Email = userEmail,
                    EmailConfirmed = true
                }, userPassword);

                if (createUserResult.Succeeded)
                    user = await userManager.FindByEmailAsync(userEmail);
            }
            else
            {
                user = await userManager.FindByEmailAsync(userEmail);
            }

            // Create Auction
            if (user != null && vehicle != null && !dbContext.Auctions.Any())
            {
                dbContext.Auctions.Add(new Business.Dbo.Models.Auctions.Auction()
                {
                    AuctionStartDate = DateTime.UtcNow,
                    AuctionEndDate = DateTime.UtcNow.AddMonths(1),
                    AuctionStatus = AuctionStatus.Active,

                    Vehicle = vehicle
                });

                dbContext.SaveChanges();
            }
        }

        public static async Task CreateDuplicateVehicleAsync()
        {
            var vehicleRepository = _serviceProvider.GetRequiredService<IDataRepository<Vehicle>>();
            var vehicleService = _serviceProvider.GetRequiredService<IVehicleWriteService>();

            var vehicle = (await vehicleRepository.GetAllAsync()).FirstOrDefault();
            if (vehicle is null) return;

            var createEntityResponse = await vehicleService.CreateVehicleAsync(new CreateVehicleRequestDto()
            {
                VehicleUniqueIdentifier = vehicle.VehicleUniqueIdentifier,
                VehicleModelID = vehicle.VehicleModelID.GetValueOrDefault()
            });

            if (createEntityResponse.Success)
                throw new InvalidOperationException("A duplicate vehicle should not be possible to create");
        }

        public static async Task CreateAuctionBidAsync()
        {
            var userManager = _serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var auctionService = _serviceProvider.GetRequiredService<IAuctionReadService>();
            var auctionBidService = _serviceProvider.GetRequiredService<IAuctionBidWriteService>();

            var userEmail = _configuration.GetValue<string>("TestData:User:Email");
            if (userEmail is null) throw new ArgumentNullException(nameof(userEmail));

            var auction = (await auctionService.SearchAuctionsAsync(new Structure.Dto.Search.AuctionSearchParamsDto()
            {
                AuctionStatus = Business.Core.AuctionStatus.Active
            })).FirstOrDefault();
            if (auction is null) throw new ArgumentNullException(nameof(auction));

            var user = await userManager.FindByEmailAsync(userEmail);

            var bidDto = new CreateAuctionBidRequestDto()
            {
                UserID = user.Id,
                AuctionID = auction.AuctionID,
                AuctionBidAmount = 20008
            };

            var createEntityResponse = await auctionBidService.CreateAuctionBidAsync(bidDto);
        }
    }
}