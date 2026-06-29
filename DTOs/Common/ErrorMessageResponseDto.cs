namespace TailorSoftAPI.DTOs.Common
{
    public class ErrorMessageResponseDto
    {
        public string? ErrorMessage {  get; init; }
        public ErrorMessageResponseDto(string? ErrorMessage)
        {
            this.ErrorMessage = ErrorMessage;
        }
    }
}
