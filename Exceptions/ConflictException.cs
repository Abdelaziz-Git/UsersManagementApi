namespace TailorSoftAPI.Exceptions
{
    public sealed class ConflictException(string message)
        : AppException(message, System.Net.HttpStatusCode.Conflict)
    {

    }
}
