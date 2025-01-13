namespace coworking.Dtos.Base
{
    public class PagedListInputDto
    {
        /// <summary>
        /// Number of page to paginated starting from 1
        /// </summary>
        public int? Page { get; set; }
        /// <summary>
        /// Maximum number of records to display in the list.
        /// If not specified a DefaultPageSize be returned.
        /// </summary>
        public int? PageSize { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int DefaultPageSize => 10;

        /*
        /// <summary>
        /// Property for make the sorting, if null order by Id, otherwise by the property send if not exist will throw error
        /// exception
        /// </summary>
        public string? OrderByProp { get; set; }

        /// <summary>
        /// value asc or desc, by default desc
        /// </summary>
        public string? OrderByCriteria { get; set; }

        public string? SearchByProp { get; set; }
        public string? SearchCriteria { get; set; }
        */
    }
}
