using System;
using System.Collections.Generic;

namespace UsersManagementApi.Models;

public partial class ErrorLog
{
    public Guid ErrorLogId { get; set; }

    public string? ProcedureName { get; set; }

    public int? ErrorNumber { get; set; }

    public string? ErrorMessage { get; set; }

    public int? ErrorSeverity { get; set; }

    public int? ErrorState { get; set; }

    public int? ErrorLine { get; set; }

    public DateTime? CreatedAt { get; set; }
}
