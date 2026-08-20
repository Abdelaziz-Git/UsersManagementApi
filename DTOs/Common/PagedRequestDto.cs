namespace UsersManagementApi.DTOs.Common
{
    public class PagedRequestDto
    {
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public string? SearchTerm { get; set; }
    }
}
