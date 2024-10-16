using CarAuction.Structure.Dto.Search;

namespace CarAuction.Structure.DataRepositories
{
    /// <summary>
    /// Force common CRUD operations for all data repositories.
    /// </summary>
    public interface IDataRepository<T> where T : class
    {
        /// <summary>
        /// Tries to create the Entity of the specified type, multi thread safe
        /// </summary>
        /// <param name="entity">The Entity to create</param>
        /// <returns>Operation result, message if fail</returns>
        Task<(bool success, string message)> CreateAsync(T entity);

        /// <summary>
        /// Tries to get the Entity of the specified type by its ID
        /// </summary>
        /// <param name="entityID">The ID of the entity</param>
        /// <returns>The Entity found, or null</returns>
        Task<T?> GetByIDAsync(int entityID);

        /// <summary>
        /// Gets all Entities of the specified type
        /// </summary>
        /// <returns>All Entities of the type, or empty</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Searchs for the Entity with the provided params, the query operator is always AND
        /// </summary>
        /// <param name="searchParams">Can be any class that inherits from BaseSearchParams</param>
        /// <returns>Entities that matched the criterias</returns>
        Task<IEnumerable<T>> SearchAsync(BaseSearchParamsDto searchParams);

        /// <summary>
        /// Tries to update the Entity of the specified type, multi thread safe
        /// </summary>
        /// <param name="entity">The Entity to update</param>
        /// <returns>Operation result, message if fail</returns>
        Task<(bool success, string message)> UpdateAsync(T entity);

        /// <summary>
        /// Tries to delete the Entity of the specified type
        /// </summary>
        /// <param name="entity">The Entity to delete</param>
        /// <returns>Operation result, message if fail</returns>
        Task<(bool success, string message)> DeleteAsync(T entity);                
    }
}