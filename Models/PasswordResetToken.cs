using System;
using System.Collections.Generic;

namespace UsersManagementApi.Models;

public partial class PasswordResetToken
{
    public Guid TokenId { get; set; }

    public Guid UserId { get; set; }

    public string ResetToken { get; set; } = null!;

    public DateTime ExpiryDate { get; set; }

    public bool? IsUsed { get; set; }

    public DateTime? UsedDate { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual User User { get; set; } = null!;
}
