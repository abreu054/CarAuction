using CarAuction.Business.Dbo;
using CarAuction.Business.Dbo.Models.Auctions;
using CarAuction.Structure.Dto.Search;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CarAuction.Structure.DataRepositories.Auctions
{
    internal sealed class AuctionBidRepository(ILoggerFactory loggerFactory, ApplicationDbContext dbContext)
        : BaseDataRepository<AuctionBid>(loggerFactory.CreateLogger<AuctionBid>())
    {
        protected override async Task<bool> CreateEntityAsync(AuctionBid entity)
        {
            using var transaction = await dbContext.Database.BeginTransactionAsync();

            try
            {
                var auction = dbContext.Auctions.FirstOrDefault(a => a.AuctionID == entity.AuctionID) ??
                    throw new ArgumentNullException($"Auction with ID {entity.AuctionID} not found");

                // We lock the AuctionBid and Auction table to try and prevent duplicate insertions
                await dbContext.AuctionBids.AddAsync(entity);
                await dbContext.SaveChangesAsync();

                auction.CurrentAuctionBidID = entity.AuctionBidID;
                await dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }

            return true;
        }

        protected override async Task<IEnumerable<AuctionBid>> GetAllEntitiesAsync()
        {
            return await dbContext.AuctionBids
                .Include(ab => ab.Auction)
                .ThenInclude(ab => ab.Vehicle)
                .AsNoTracking()
                .ToListAsync();
        }

        protected override async Task<AuctionBid?> GetEntityByIDAsync(int entityID)
        {
            var auctionBid = await dbContext.AuctionBids
                .Include(ab => ab.Auction)
                .ThenInclude(ab => ab.Vehicle)
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.AuctionBidID == entityID);

            return auctionBid is null ? throw new NullReferenceException("Vehicle not found.") : auctionBid;
        }

        protected override async Task<IEnumerable<AuctionBid>> SearchEntitiesAsync(BaseSearchParamsDto searchParams)
        {
            var query = dbContext.AuctionBids
                .Include(ab => ab.Auction)
                .ThenInclude(ab => ab.Vehicle)
                .AsNoTracking()
                .AsQueryable();

            if (searchParams.EntityID > 0)
                query = query.Where(ab => ab.AuctionBidID == searchParams.EntityID);

            if (searchParams is AuctionBidSearchParamsDto auctionBidSearchParams)
            {
                if (auctionBidSearchParams.AuctionID > 0)
                    query = query.Where(ab => ab.AuctionID == auctionBidSearchParams.AuctionID);
            }

            return await query.ToListAsync();
        }

        protected override async Task<bool> UpdateEntityAsync(AuctionBid entity)
        {
            throw new NotSupportedException("Auction bids should not be updated");
        }

        protected override async Task<bool> DeleteEntityAsync(AuctionBid entity)
        {
            throw new NotSupportedException("Auction bids should not be deleted");
        }
    }
}