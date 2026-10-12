
using Microsoft.EntityFrameworkCore;
using System.Data;
using UsersManagementApi.Data;
using UsersManagementApi.DTOs.Permissions;
using UsersManagementApi.Interfaces.Repositories;
using UsersManagementApi.Models;

namespace UsersManagementApi.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly Context _context;

        public PermissionRepository(Context efContext)
        {
            _context = efContext;
        }

        public async Task<Guid?> CreateAsync(CreatePermissionDto dto)
        {
            var newPermission = new Permission
            {
                PermissionId = Guid.NewGuid(),
                PermissionName = dto.PermissionName,
                Module = dto.Module,
                Description = dto.Description
            };
            await _context.Permissions.AddAsync(newPermission);
            return await _context.SaveChangesAsync() == 1 ? newPermission.PermissionId : null;
        }

        public async Task<Permission?> GetByIdAsync(Guid id)
        {
            return await _context.Permissions
                .AsNoTracking()
                .Where(p => (p.PermissionId == id) && !(p.IsDeleted))
                .FirstOrDefaultAsync();
        }

        public async Task<List<Permission>> GetAllAsync(string module)
        {
            return await _context.Permissions
                .AsNoTracking()
                .Where(p => p.Module == module && !p.IsDeleted)
                .ToListAsync();
        }

        public async Task<bool> UpdateAsync(Guid id, UpdatePermissionDto dto)
        {
            var permission = await _context.Permissions
                .Where(p => (p.PermissionId == id) && !(p.IsDeleted))
                .FirstOrDefaultAsync();

            if (permission == null)
                return false;

            permission.PermissionName= dto.PermissionName;
            permission.Module = dto.Module;
            permission.Description = dto.Description;

            return await _context.SaveChangesAsync() == 1;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var permission = await _context.Permissions
                 .Where(p => (p.PermissionId == id) && !(p.IsDeleted))
                 .FirstOrDefaultAsync();

            if (permission == null)
                return false;

            permission.IsDeleted = true;

            return await _context.SaveChangesAsync() == 1;
        }
    }
}