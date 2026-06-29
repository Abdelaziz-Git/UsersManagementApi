namespace TailorSoftAPI.DTOs.RolePermissions
{
    public class RolePermissionResponseDto
    {
        public Guid RolePermissionId { get; set; }
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }
        public DateTime GrantedDate { get; set; }
    }
}
