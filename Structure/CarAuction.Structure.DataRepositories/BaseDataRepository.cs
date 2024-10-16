using CarAuction.Structure.Dto.Search;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CarAuction.Structure.DataRepositories
{
    /// <summary>
    /// Base abstract repository
    /// <para>All repositories must inherit from this</para>
    /// </summary>
    /// <typeparam name="T">The type of Entity for the repo</typeparam>
    internal abstract class BaseDataRepository<T>(ILogger<T> logger) : IDataRepository<T> where T : class
    {
        private static readonly SemaphoreSlim _inserDataSemaphore = new(1, 1);
        private static readonly SemaphoreSlim _updateDataSemaphore = new(1, 1);

        // Create
        public async Task<(bool success, string message)> CreateAsync(T entity)
        {
            try
            {
                await _inserDataSemaphore.WaitAsync(TimeSpan.FromSeconds(30));

                return (await CreateEntityAsync(entity), string.Empty);
            }
            catch (DbUpdateException ex)
            when (ex.InnerException != null &&
            (ex.InnerException.Message.StartsWith("Violation of PRIMARY KEY") || ex.InnerException.Message.StartsWith("Cannot insert duplicate key row")))
            {
                logger.LogError(ex, "Violation of primary key while trying to Add a new entity of type {EntityType}", typeof(T).Name);
                return (false, "Entity with the same Identifier already exists");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.LogError(ex, "Concurrency error while trying to Add a new entity of type {EntityType}", typeof(T).Name);
                return (false, "Multiples requests for this operation at the same time");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while trying to Add a new entity of type {EntityType}", typeof(T).Name);
                return (false, ex.Message);
            }
            finally
            {
                _inserDataSemaphore.Release();
            }
        }

        protected abstract Task<bool> CreateEntityAsync(T entity);

        // Read
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            try
            {
                return await GetAllEntitiesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while trying to Get all entities of type {EntityType}", typeof(T).Name);
                return [];
            }
        }

        protected abstract Task<IEnumerable<T>> GetAllEntitiesAsync();

        public async Task<T?> GetByIDAsync(int entityID)
        {
            try
            {
                return await GetEntityByIDAsync(entityID);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while trying to Get entity of type {EntityType} with ID {EntityID}", typeof(T).Name, entityID);
                return null;
            }
        }

        protected abstract Task<T?> GetEntityByIDAsync(int entityID);

        public async Task<IEnumerable<T>> SearchAsync(BaseSearchParamsDto searchParams)
        {
            try
            {
                return await SearchEntitiesAsync(searchParams);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while trying to Search entities of type {EntityType}", typeof(T).Name);
                return [];
            }
        }

        /// <summary>
        /// Searches for all entities that match a certain criteria
        /// </summary>
        /// <param name="searchParams">Criteria to search for, can be the default, or any descending class, like <see cref="VehiclesSearchParamsDto"/></param>
        /// <returns>Entities found with that matched the search criteria</returns>
        protected abstract Task<IEnumerable<T>> SearchEntitiesAsync(BaseSearchParamsDto searchParams);

        // Update
        public async Task<(bool success, string message)> UpdateAsync(T entity)
        {
            try
            {
                await _updateDataSemaphore.WaitAsync(TimeSpan.FromSeconds(30));

                return (await UpdateEntityAsync(entity), string.Empty);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.LogError(ex, "Concurrency error while trying to Update entity of type {EntityType}", typeof(T).Name);
                return (false, "Multiples requests for this operation at the same time");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while trying to Update entity of type {EntityType}", typeof(T).Name);
                return (false, ex.Message);
            }
            finally
            {
                _updateDataSemaphore.Release();
            }
        }

        protected abstract Task<bool> UpdateEntityAsync(T entity);

        // Delete
        public async Task<(bool success, string message)> DeleteAsync(T entity)
        {
            try
            {
                return (await DeleteEntityAsync(entity), string.Empty);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while trying to Delete entity of type {EntityType}", typeof(T).Name);
                return (false, ex.Message);
            }
        }

        protected abstract Task<bool> DeleteEntityAsync(T entity);
    }
}