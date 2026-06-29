namespace TailorSoftAPI.DTOs.Permissions
{
    public class CreatePermissionDto
    {
        public string PermissionName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Module { get; set; } = string.Empty;

    }
}
