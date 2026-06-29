namespace TailorSoftAPI.Models
{
    public class RolePermission
    {
        public Guid RolePermissionId { get; set; }

        public Guid RoleId { get; set; }

        public Guid PermissionId { get; set; }

        public DateTime GrantedDate { get; set; }
    }
}
