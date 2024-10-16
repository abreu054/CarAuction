using CarAuction.Business.Dbo;
using CarAuction.Business.Dbo.Models.Auctions;
using CarAuction.Structure.Dto.Search;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CarAuction.Structure.DataRepositories.Auctions
{
    internal sealed class AuctionDataRepository(ILoggerFactory loggerFactory, ApplicationDbContext dbContext)
        : BaseDataRepository<Auction>(loggerFactory.CreateLogger<Auction>())
    {
        protected override async Task<bool> CreateEntityAsync(Auction entity)
        {
            await dbContext.Auctions.AddAsync(entity);
            return await dbContext.SaveChangesAsync() > 0;
        }

        protected override async Task<IEnumerable<Auction>> GetAllEntitiesAsync()
        {
            return await dbContext.Auctions
                .Include(a => a.CurrentAuctionBid)
                .Include(a => a.Vehicle)                
                .AsNoTracking()
                .ToListAsync();
        }

        protected override async Task<Auction?> GetEntityByIDAsync(int entityID)
        {
            var auction = await dbContext.Auctions
                .Include(a => a.CurrentAuctionBid)
                .Include(a => a.Vehicle)
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.AuctionID == entityID);

            return auction is null ? throw new NullReferenceException("Auction not found.") : auction;
        }

        protected override async Task<IEnumerable<Auction>> SearchEntitiesAsync(BaseSearchParamsDto searchParams)
        {
            var query = dbContext.Auctions
                .Include(a => a.CurrentAuctionBid)
                .Include(a => a.Vehicle)
                    .ThenInclude(v => v.VehicleManufacturer)
                .Include(a => a.Vehicle)
                    .ThenInclude(v => v.VehicleModel)
                .Where(a => a.Vehicle != null)
                .AsQueryable();

            if(searchParams.EntityID > 0)
                query = query.Where(ab => ab.AuctionID == searchParams.EntityID);

            if (searchParams is AuctionSearchParamsDto auctionSearchParams)
            {
                if (auctionSearchParams.VehicleID > 0)
                    query = query.Where(a => a.VehicleID == auctionSearchParams.VehicleID);

                if (auctionSearchParams.AuctionStatus != null)
                    query = query.Where(a => a.AuctionStatus == auctionSearchParams.AuctionStatus);

                if(auctionSearchParams.VehicleType != null)
                    query = query.Where(a => a.Vehicle.VehicleType == auctionSearchParams.VehicleType);

                if (auctionSearchParams.VehicleYear > 0)
                    query = query.Where(a => a.Vehicle.VehicleYear == auctionSearchParams.VehicleYear);

                if (auctionSearchParams.VehicleManufacturerID > 0)
                    query = query.Where(a => a.Vehicle.VehicleManufacturerID == auctionSearchParams.VehicleManufacturerID);

                if (auctionSearchParams.VehicleModelID > 0)
                    query = query.Where(a => a.Vehicle.VehicleModelID == auctionSearchParams.VehicleModelID);
            }

            return await query.ToListAsync();
        }

        protected override async Task<bool> UpdateEntityAsync(Auction entity)
        {
            dbContext.Auctions.Update(entity);
            return await dbContext.SaveChangesAsync() > 0;
        }

        protected override async Task<bool> DeleteEntityAsync(Auction entity)
        {
            dbContext.Auctions.Remove(entity);
            return await dbContext.SaveChangesAsync() > 0;
        }
    }
}