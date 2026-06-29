using Microsoft.Data.SqlClient;
using System.Net;

namespace TailorSoftAPI.Exceptions
{
    /// <summary>
    /// Thrown when a database operation fails
    /// </summary>
    public sealed class DatabaseException(string message, Exception? innerException = null)
        : AppException(message, HttpStatusCode.InternalServerError)
    {
        public DatabaseException(string message, SqlException innerException)
            : this(message, (Exception)innerException) { }
    }
}
