namespace UsersManagementApi.DTOs.Common
{
    public class FailedLoginRequestDto
    {
        public int MaxAttempts { get; set; } = 5;
        public int LockoutDurationMinutes { get; set; } = 30;
    }
}
