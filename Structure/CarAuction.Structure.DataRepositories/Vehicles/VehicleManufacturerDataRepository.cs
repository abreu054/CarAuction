using CarAuction.Business.Dbo;
using CarAuction.Business.Dbo.Models.Vehicles;
using CarAuction.Structure.Dto.Search;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CarAuction.Structure.DataRepositories.Vehicles
{
    internal sealed class VehicleManufacturerDataRepository(ILoggerFactory loggerFactory, ApplicationDbContext dbContext)
        : BaseDataRepository<VehicleManufacturer>(loggerFactory.CreateLogger<VehicleManufacturer>())
    {
        protected override async Task<bool> CreateEntityAsync(VehicleManufacturer entity)
        {
            await dbContext.VehicleManufacturers.AddAsync(entity);
            return await dbContext.SaveChangesAsync() > 0;
        }

        protected override async Task<IEnumerable<VehicleManufacturer>> GetAllEntitiesAsync()
        {
            return await dbContext.VehicleManufacturers.AsNoTracking().ToListAsync();
        }

        protected override async Task<VehicleManufacturer?> GetEntityByIDAsync(int entityID)
        {
            var vehicleManufacturer = await dbContext.VehicleManufacturers
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.VehicleManufacturerID == entityID);

            return vehicleManufacturer is null ? throw new NullReferenceException("VehicleManufacturer not found.") : vehicleManufacturer;
        }

        protected override async Task<IEnumerable<VehicleManufacturer>> SearchEntitiesAsync(BaseSearchParamsDto searchParams)
        {
            var query = dbContext.VehicleManufacturers
                .AsNoTracking()
                .Include(v => v.VehicleModels)
                .AsQueryable();

            if (searchParams is VehicleManufacturerSearchParamsDto vehiclesSearchParams)
            {
                if (!string.IsNullOrWhiteSpace(vehiclesSearchParams.VehicleManufacturerName))
                    query = query.Where(v => v.VehicleManufacturerName == vehiclesSearchParams.VehicleManufacturerName);
            }

            return await query.ToListAsync();
        }

        protected override async Task<bool> UpdateEntityAsync(VehicleManufacturer entity)
        {
            dbContext.VehicleManufacturers.Update(entity);
            return await dbContext.SaveChangesAsync() > 0;
        }

        protected override async Task<bool> DeleteEntityAsync(VehicleManufacturer entity)
        {
            dbContext.VehicleManufacturers.Remove(entity);
            return await dbContext.SaveChangesAsync() > 0;
        }
    }
}