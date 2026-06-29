namespace TailorSoftAPI.Models
{
    public class Permission
    {
        public Guid PermissionId { get; set; }

        public string PermissionName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Module { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }
    }
}
