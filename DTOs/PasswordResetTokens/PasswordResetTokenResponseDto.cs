namespace UsersManagementApi.DTOs.PasswordResetTokens
{
    public class PasswordResetTokenResponseDto
    {
        public Guid TokenId { get; set; }
        public Guid UserId { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsUsed { get; set; }
    }
}
