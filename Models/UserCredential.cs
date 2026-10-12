using System;
using System.Collections.Generic;

namespace UsersManagementApi.Models;

public partial class UserCredential
{
    public Guid CredentialId { get; set; }

    public Guid UserId { get; set; }

    public string PasswordHash { get; set; } = null!;

    public DateTime? LastPasswordChangeDate { get; set; }

    public int? FailedLoginAttempts { get; set; }

    public DateTime? AccountLockedUntil { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual User User { get; set; } = null!;
}
