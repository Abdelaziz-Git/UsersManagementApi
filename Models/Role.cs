namespace TailorSoftAPI.Models
{
    public class Role
    {
        public Guid RoleId { get; set; }

        public string RoleName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsDeleted { get; set; }
    }
}
