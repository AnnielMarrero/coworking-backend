namespace coworking.Dtos.Base
{
    public class PagedResultDto<Entity>
    {
        public int CurrentPage { get; set; }

        public int PageCount { get; set; }

        public int PageSize { get; set; }

        public int RecordCount { get; set; }

        public List<Entity> Results { get; set; } = new();

    }
}
