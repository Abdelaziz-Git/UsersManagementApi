using System;
using System.Collections.Generic;

namespace UsersManagementApi.Models;

public partial class Log
{
    public int LogId { get; set; }

    public string LogLevel { get; set; } = null!;

    public string Category { get; set; } = null!;

    public string Message { get; set; } = null!;

    public string? Exception { get; set; }

    public DateTime Timestamp { get; set; }
}
