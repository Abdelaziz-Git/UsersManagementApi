using System;
using System.Collections.Generic;

namespace UsersManagementApi.Models;

public partial class UserSession
{
    public Guid SessionId { get; set; }

    public Guid UserId { get; set; }

    public string RefreshTokenHash { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public DateTime ExpiryDate { get; set; }

    public bool IsRevoked { get; set; }

    public DateTime? RevokedDate { get; set; }

    public virtual User User { get; set; } = null!;
}
