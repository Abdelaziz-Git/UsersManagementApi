namespace UsersManagementApi.DTOs.Authentication
{
    public class LogoutRequestDto
    {
        public string Email { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
