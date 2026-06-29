namespace TailorSoftAPI.DTOs.PasswordResetTokens
{
    public class UpdatePasswordResetTokenDto
    {
        public Guid TokenId { get; set; }
        public bool IsUsed { get; set; }
    }
}
