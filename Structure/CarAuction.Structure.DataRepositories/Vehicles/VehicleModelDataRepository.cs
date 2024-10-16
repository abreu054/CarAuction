using CarAuction.Business.Dbo;
using CarAuction.Business.Dbo.Models.Vehicles;
using CarAuction.Structure.Dto.Search;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CarAuction.Structure.DataRepositories.Vehicles
{
    internal sealed class VehicleModelDataRepository(ILoggerFactory loggerFactory, ApplicationDbContext dbContext)
        : BaseDataRepository<VehicleModel>(loggerFactory.CreateLogger<VehicleModel>())
    {
        protected override async Task<bool> CreateEntityAsync(VehicleModel entity)
        {
            await dbContext.VehicleModels.AddAsync(entity);
            return await dbContext.SaveChangesAsync() > 0;
        }

        protected override async Task<IEnumerable<VehicleModel>> GetAllEntitiesAsync()
        {
            return await dbContext.VehicleModels.AsNoTracking().ToListAsync();
        }

        protected override async Task<VehicleModel?> GetEntityByIDAsync(int entityID)
        {
            var vehicleModel = await dbContext.VehicleModels
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.VehicleModelID == entityID);

            return vehicleModel is null ? throw new NullReferenceException("VehicleModel not found.") : vehicleModel;
        }

        protected override async Task<IEnumerable<VehicleModel>> SearchEntitiesAsync(BaseSearchParamsDto searchParams)
        {
            var query = dbContext.VehicleModels
                .AsNoTracking()
                .Include(v => v.VehicleManufacturer)
                .AsQueryable();

            if (searchParams is VehicleModelSearchParamsDto vehiclesSearchParams)
            {
                if (!string.IsNullOrWhiteSpace(vehiclesSearchParams.VehicleModelName))
                    query = query.Where(v => v.VehicleModelName == vehiclesSearchParams.VehicleModelName);

                if(vehiclesSearchParams.VehicleManufacturerID > 0)
                    query = query.Where(v => v.VehicleManufacturerID == vehiclesSearchParams.VehicleManufacturerID);
            }

            return await query.ToListAsync();
        }

        protected override async Task<bool> UpdateEntityAsync(VehicleModel entity)
        {
            dbContext.VehicleModels.Update(entity);
            return await dbContext.SaveChangesAsync() > 0;
        }

        protected override async Task<bool> DeleteEntityAsync(VehicleModel entity)
        {
            dbContext.VehicleModels.Remove(entity);
            return await dbContext.SaveChangesAsync() > 0;
        }
    }
}