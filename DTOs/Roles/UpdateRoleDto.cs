namespace UsersManagementApi.DTOs.Roles
{
    public class UpdateRoleDto
    {
        public string RoleName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
