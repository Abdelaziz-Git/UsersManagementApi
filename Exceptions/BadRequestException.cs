namespace TailorSoftAPI.Exceptions
{
    public sealed class BadRequestException(string message)
        : AppException(message, System.Net.HttpStatusCode.BadRequest)
    {

    }


}
