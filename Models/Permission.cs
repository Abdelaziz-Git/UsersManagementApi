using System;
using System.Collections.Generic;

namespace UsersManagementApi.Models;

public partial class Permission
{
    public Guid PermissionId { get; set; }

    public string PermissionName { get; set; } = null!;

    public string? Description { get; set; }

    public string Module { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
