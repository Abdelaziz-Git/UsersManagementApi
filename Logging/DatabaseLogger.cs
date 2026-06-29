using System.Data;
using Dapper;
using TailorSoftAPI.Data;

namespace TailorSoftAPI.Logging
{
    public class DatabaseLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly LogLevel _minimumLogLevel;

        public DatabaseLogger(string categoryName, IServiceScopeFactory scopeFactory, LogLevel minimumLogLevel)
        {
            _categoryName = categoryName;
            _scopeFactory = scopeFactory;
            _minimumLogLevel = minimumLogLevel;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= _minimumLogLevel;
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            var message = formatter(state, exception);
            var exceptionMessage = exception?.ToString() ?? string.Empty;

            // Fire and forget - log to database asynchronously
            _ = LogToDatabaseAsync(logLevel, message, exceptionMessage);
        }

        private async Task LogToDatabaseAsync(LogLevel logLevel, string message, string? exceptionMessage)
        {
            using var scope = _scopeFactory.CreateScope();

            var context = scope.ServiceProvider
                .GetRequiredService<DapperContext>();

            if (context == null)
                return;

            using var connection = context.CreateConnection();
            if (connection == null)
                return;

            var parameters = new DynamicParameters();
            parameters.Add("@LogLevel", logLevel.ToString());
            parameters.Add("@Category", _categoryName);
            parameters.Add("@Message", message);
            parameters.Add("@Exception", exceptionMessage);
            parameters.Add("@Timestamp", DateTime.UtcNow);

            await connection.ExecuteAsync(
                "SP_Logs_Insert",
                parameters,
                commandType: CommandType.StoredProcedure);
        }
    }
}