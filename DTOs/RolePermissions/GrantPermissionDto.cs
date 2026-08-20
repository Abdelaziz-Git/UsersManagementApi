namespace UsersManagementApi.DTOs.RolePermissions
{
    public class GrantPermissionDto
    {
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }
    }
}
