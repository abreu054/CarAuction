using CarAuction.Business.Dbo;
using CarAuction.Business.Dbo.Models.Vehicles;
using CarAuction.Structure.Dto.Search;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CarAuction.Structure.DataRepositories.Vehicles
{
    internal sealed class VehiclesDataRepository(ILoggerFactory loggerFactory, ApplicationDbContext dbContext)
        : BaseDataRepository<Vehicle>(loggerFactory.CreateLogger<Vehicle>())
    {
        protected override async Task<bool> CreateEntityAsync(Vehicle entity)
        {
            await dbContext.Vehicles.AddAsync(entity);
            return await dbContext.SaveChangesAsync() > 0;
        }

        protected override async Task<IEnumerable<Vehicle>> GetAllEntitiesAsync()
        {
            return await dbContext.Vehicles
                .Include(v => v.VehicleManufacturer)
                .Include(v => v.VehicleModel)
                .AsNoTracking()
                .ToListAsync();
        }

        protected override async Task<Vehicle?> GetEntityByIDAsync(int entityID)
        {
            var vehicle = await dbContext.Vehicles
                .Include(v => v.VehicleManufacturer)
                .Include(v => v.VehicleModel)
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.VehicleID == entityID);

            return vehicle is null ? throw new NullReferenceException("Vehicle not found.") : vehicle;
        }

        protected override async Task<IEnumerable<Vehicle>> SearchEntitiesAsync(BaseSearchParamsDto searchParams)
        {
            var query = dbContext.Vehicles
                .AsNoTracking()
                .Include(v => v.VehicleManufacturer)
                .Include(v => v.VehicleModel)
                .Include(v => v.Auctions)
                .AsQueryable();

            if (searchParams is VehiclesSearchParamsDto vehiclesSearchParams)
            {
                if (!string.IsNullOrWhiteSpace(vehiclesSearchParams.VehicleUniqueIdentifier))
                    query = query.Where(v => v.VehicleUniqueIdentifier == vehiclesSearchParams.VehicleUniqueIdentifier);

                if (vehiclesSearchParams.VehicleType != null)
                    query = query.Where(v => v.VehicleType == vehiclesSearchParams.VehicleType);

                if (vehiclesSearchParams.VehicleManufacturerID > 0)
                    query = query.Where(v => v.VehicleManufacturerID == vehiclesSearchParams.VehicleManufacturerID);

                if (vehiclesSearchParams.VehicleModelID > 0)
                    query = query.Where(v => v.VehicleModelID == vehiclesSearchParams.VehicleModelID);

                if (vehiclesSearchParams.VehicleYear > 0)
                    query = query.Where(v => v.VehicleYear == vehiclesSearchParams.VehicleYear);

                if (vehiclesSearchParams.OnlyInActiveAuctions)
                    query = query.Where(v => v.Auctions != null && v.Auctions.Any(a => a.AuctionStatus == Business.Core.AuctionStatus.Active));
            }

            return await query.ToListAsync();
        }

        protected override async Task<bool> UpdateEntityAsync(Vehicle entity)
        {
            dbContext.Vehicles.Update(entity);
            return await dbContext.SaveChangesAsync() > 0;
        }

        protected override async Task<bool> DeleteEntityAsync(Vehicle entity)
        {
            dbContext.Vehicles.Remove(entity);
            return await dbContext.SaveChangesAsync() > 0;
        }
    }
}