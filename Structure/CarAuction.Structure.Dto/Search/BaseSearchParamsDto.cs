namespace CarAuction.Structure.Dto.Search
{
    /// <summary>
    /// Base search parameters, possible to create other SearchDtos if they inherit BaseSearchParamsDto
    /// </summary>
    public class BaseSearchParamsDto
    {
        public int EntityID { get; set; }
    }
}
