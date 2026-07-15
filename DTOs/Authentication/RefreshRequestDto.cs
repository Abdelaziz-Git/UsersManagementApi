namespace TailorSoftAPI.DTOs.Authentication
{
    public class RefreshRequestDto
    {
        public string Email { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
