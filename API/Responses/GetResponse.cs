namespace API.Responses
{
    class GetMultipleResponse<T>
    {
        // Count of all records
        public int Count { get; set; }
        // Current page
        public int Page { get; set; }
        // Number of records per page
        public int PerPage { get; set; } = 25;
        // Collection of records based on page and perPage
        public List<T> Collection { get; set; }
    }

    class GetOneResponse<T>
    {
        public T Item { get; set; }
    }
}