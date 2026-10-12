using System;
using System.Collections.Generic;

namespace UsersManagementApi.Models;

public partial class RolePermission
{
    public Guid RolePermissionId { get; set; }

    public Guid RoleId { get; set; }

    public Guid PermissionId { get; set; }

    public DateTime GrantedDate { get; set; }

    public virtual Permission Permission { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
