namespace UsersManagementApi.DTOs.UserRoles
{
    public class DeleteUserRoleDto
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
    }
}
