namespace UsersManagementApi.DTOs.UserCredentials
{
    public class CreateUserCredentialDto
    {
        public Guid UserId { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
    }
}
