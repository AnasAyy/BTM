using BTMBackend.Dtos.AuthorityDto;
using BTMBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace BTMBackend.Data.Repos
{
    public interface IAuthorityRepo
    {
        public Task<Role?> GetRoleByIdAsync(int roleId);
        public Task<int> GetCustomerRoleId();
        public void CreateRole(Role role);
        public Task<string?> GetRoleNameEnByUserId(int userId);
        public void UpdateRole(Role role);
        public Task<bool> CheckRoles();
        public Task<IQueryable<GetAllRoleResponseDto>> GetAllRoles();
        public Task<List<GetAllRoleResponseDto>> GetAllEmployeeRoles();
        public Task<Permission?> GetPermissionByIdAsync(int permissionId);
        public Task<List<RolePermission>> GetPermissionsByRoleIdAsync(int roleId);
        public Task<bool> IsRoleHavePermission(int roleId, int permissionId);
        public Task<bool> IsUserHavePermission(int userId, int permissionId);
        public Task<bool> IsRoleTitleExist(string titleAr, string titleEn);
        public void CreatePermission(Permission permission);
        public void UpdatePermission(Permission permission);
        public Task<bool> SaveChanges();
    }
    public class AuthorityRepo(DataContext context) : IAuthorityRepo
    {
        private readonly DataContext _context = context;

        public async Task<bool> CheckRoles()
        {
            return await _context.Roles.AnyAsync();
        }

        public async void CreatePermission(Permission permission)
        {
            await _context.Permissions.AddAsync(permission);
        }

        public async void CreateRole(Role role)
        {
            await _context.Roles.AddAsync(role);
        }

        public async Task<List<GetAllRoleResponseDto>> GetAllEmployeeRoles()
        {
            var result = await _context.Roles
                .Where(x=>x.TitleEn != "Customer")
                .Select(r => new GetAllRoleResponseDto
            {
                Id = r.Id,
                TitleAr = r.TitleAr,
                TitleEn = r.TitleEn,
            }).ToListAsync();
            return result;
        }

        public async Task<IQueryable<GetAllRoleResponseDto>> GetAllRoles()
        {
            var result = await _context.Roles.Select(r => new GetAllRoleResponseDto
            {
                Id = r.Id,
                TitleAr = r.TitleAr,
                TitleEn = r.TitleEn,
            }).ToListAsync();
            return result.AsQueryable();
        }

        public async Task<int> GetCustomerRoleId()
        {
            var customerRoleId = await _context.Roles.FirstOrDefaultAsync(x => x.TitleAr == "عميل");
            return customerRoleId == null ? 0 : customerRoleId.Id;
        }

        public Task<Permission?> GetPermissionByIdAsync(int permissionId)
        {
            return _context.Permissions.FirstOrDefaultAsync(x => x.Id == permissionId);
        }

        public async Task<List<RolePermission>> GetPermissionsByRoleIdAsync(int roleId)
        {
            return await _context.RolePermissions.Where(x => x.RoleId == roleId).ToListAsync();
        }

        public Task<Role?> GetRoleByIdAsync(int roleId)
        {
            return _context.Roles.FirstOrDefaultAsync(x => x.Id == roleId);
        }

        public Task<string?> GetRoleNameEnByUserId(int userId)
        {
            var result = _context.Users
                .Where(user => user.Id == userId)
                .Join(_context.Roles,
                    user => user.RoleId,
                    role => role.Id,
                    (user, role) => role.TitleEn)
                .FirstOrDefaultAsync();

            return result;
        }

        public async Task<bool> IsRoleHavePermission(int roleId, int permissionId)
        {
            return await _context.RolePermissions.AnyAsync(x => x.RoleId == roleId && x.PermissionId == permissionId);
        }

        public async Task<bool> IsRoleTitleExist(string titleAr, string titleEn)
        {
            return await _context.Roles.AnyAsync(x => x.TitleAr == titleAr || x.TitleEn == titleEn);
        }

        public async Task<bool> IsUserHavePermission(int userId, int permissionId)
        {
            var getUserRoleId = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (getUserRoleId == null)
            {
                return false;
            }

            return await _context.RolePermissions.AnyAsync(x => x.RoleId == getUserRoleId.RoleId && x.PermissionId == permissionId);
        }

        public async Task<bool> SaveChanges()
        {
            try
            {
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _context.ChangeTracker.Clear();
            }
            return false;
        }

        public void UpdatePermission(Permission permission)
        {
            _context.Permissions.Update(permission);
        }

        public void UpdateRole(Role role)
        {
            _context.Roles.Update(role);
        }


    }
}
