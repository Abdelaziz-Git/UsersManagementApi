namespace UsersManagementApi.DTOs.Roles
{
    public class RoleResponseDto
    {
        public Guid RoleId { get; set; }

        public string RoleName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime CreatedDate { get; set; }

    }
}
