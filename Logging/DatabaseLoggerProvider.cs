using UsersManagementApi.Logging;

public class DatabaseLoggerProvider : ILoggerProvider
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly LogLevel _minimumLogLevel;

    public DatabaseLoggerProvider(
        IServiceScopeFactory scopeFactory,
        LogLevel minimumLogLevel = LogLevel.Information)
    {
        _scopeFactory = scopeFactory;
        _minimumLogLevel = minimumLogLevel;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new DatabaseLogger(
            categoryName,
            _scopeFactory,
            _minimumLogLevel);
    }

    public void Dispose()
    {
    }
}