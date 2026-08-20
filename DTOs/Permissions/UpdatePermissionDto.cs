namespace UsersManagementApi.DTOs.Permissions
{
    public class UpdatePermissionDto
    {
        public string PermissionName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Module { get; set; } = string.Empty;
    }
}
