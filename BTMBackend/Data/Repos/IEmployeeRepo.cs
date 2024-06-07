using BTMBackend.Dtos.EmployeeDto;
using BTMBackend.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BTMBackend.Data.Repos
{
    public interface IEmployeeRepo
    {
        Task Create(Employee employee);
        void Update(Employee employee);
        Task<Employee?> GetById(int id);
        Task<Employee?> GetByUserId(int userId);
        Task<int?> GetEmployeeIdByUserId(int userId);
        Task<GetEmployeeResponseDto?> GetEmployeeById(int employeeId);
        Task<IQueryable<GetAllEmployeeResponseDto>> GetByNameOrPhone(string query, int userId);
        Task<Employee?> GetByPhoneNumber(string phoneNumber);
        Task<List<GetEmplyeesResponseDto>> GetSupervisors();
        Task<List<GetEmplyeesResponseDto>> GetCallCenters();
        Task<List<GetEmplyeesResponseDto>> GetTechnician();
        Task<IQueryable<GetSpecificTypeOfEmployeeResponseDto>> GetAllCallCenters();
        Task<IQueryable<GetAllEmployeeResponseDto>> GetAll(int userId);
        Task<IQueryable<GetAllEmployeeResponseDto>> GetAllActive();
        Task<IQueryable<GetAllEmployeeResponseDto>> GetAllDisActive();
        Task<bool> SaveChanges();
    }
    public class EmployeeRepo : IEmployeeRepo
    {
        private readonly DataContext _context;

        public EmployeeRepo(DataContext context)
        {
            _context = context;
        }

        public async Task Create(Employee employee)
        {
            await _context.Employees.AddAsync(employee);
        }

        public async Task<IQueryable<GetAllEmployeeResponseDto>> GetAll(int userId)
        {
            var result = await _context.Employees
                .Join(_context.Users, employee => employee.UserId, user => user.Id, (employee, user) => new { employee, user })
                .Where(combined => combined.user.Id != userId) // Exclude the specific user
                .Join(_context.Roles, combined => combined.user.RoleId, role => role.Id, (combined, role) => new { combined.employee, combined.user, role })
                .Select(data => new GetAllEmployeeResponseDto
                {
                    Id = data.employee.Id,
                    FullName = data.employee.FirstName + " " + data.employee.LastName,
                    PhoneNumber = data.employee.PhoneNumber,
                    PositionAr = data.role.TitleAr,
                    PositionEn = data.role.TitleEn,
                    IsActive = data.role.IsActive
                })
                .ToListAsync();

            return result.AsQueryable();
        }


        public async Task<Employee?> GetById(int id)
        {
            return await _context.Employees.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Employee?> GetByPhoneNumber(string phoneNumber)
        {
            return await _context.Employees.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
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

        public void Update(Employee employee)
        {
            _context.Employees.Update(employee);
        }

        public async Task<IQueryable<GetAllEmployeeResponseDto>> GetAllActive()
        {
            var result = await _context.Employees
                .Join(_context.Users, employee => employee.UserId, user => user.Id, (employee, user) => new { employee, user })
                .Join(_context.Roles, combined => combined.user.RoleId, role => role.Id, (combined, role) => new GetAllEmployeeResponseDto
                {
                    Id = combined.employee.Id,
                    FullName = $"{combined.employee.FirstName} {combined.employee.LastName}",
                    PhoneNumber = combined.employee.PhoneNumber,
                    PositionAr = role.TitleAr,
                    PositionEn = role.TitleEn,
                    IsActive = combined.user.IsActive
                })
                .Where(dto => dto.IsActive).ToListAsync(); // Filter only active employees

            return result.AsQueryable();
        }

        public async Task<IQueryable<GetAllEmployeeResponseDto>> GetAllDisActive()
        {
            var result = await _context.Employees
                .Join(_context.Users, employee => employee.UserId, user => user.Id, (employee, user) => new { employee, user })
                .Join(_context.Roles, combined => combined.user.RoleId, role => role.Id, (combined, role) => new GetAllEmployeeResponseDto
                {
                    Id = combined.employee.Id,
                    FullName = $"{combined.employee.FirstName} {combined.employee.LastName}",
                    PhoneNumber = combined.employee.PhoneNumber,
                    PositionAr = role.TitleAr,
                    PositionEn = role.TitleEn,
                    IsActive = combined.user.IsActive
                })
                .Where(dto => dto.IsActive == false).ToListAsync(); // Filter only active employees

            return result.AsQueryable();
        }

        public async Task<IQueryable<GetSpecificTypeOfEmployeeResponseDto>> GetAllCallCenters()
        {
            var getRoleId = await _context.Roles.FirstOrDefaultAsync(role => role.TitleEn == "Call Center");
            var roleId = getRoleId == null ? 0 : getRoleId.Id;

            var result = await _context.Employees
                .Where(employee => _context.Users.Any(user =>
                    user.Id == employee.UserId &&
                    user.RoleId == roleId))
                .Select(employee => new GetSpecificTypeOfEmployeeResponseDto
                {
                    Id = employee.Id,
                    FullName = employee.FirstName + " " + employee.LastName
                })
                .ToListAsync();

            return result.AsQueryable();
        }

        public async Task<IQueryable<GetAllEmployeeResponseDto>> GetByNameOrPhone(string query , int userId)
        {
            var result = await _context.Employees
                .Join(_context.Users, employee => employee.UserId, user => user.Id, (employee, user) => new { employee, user })
                .Where(combined => combined.user.Id != userId)
                .Join(_context.Roles, combined => combined.user.RoleId, role => role.Id, (combined, role) => new { combined.employee, combined.user, role })
                .Where(x => EF.Functions.Like(x.employee.FirstName, $"%{query}%") || EF.Functions.Like(x.employee.LastName, $"%{query}%") || EF.Functions.Like(x.employee.PhoneNumber, $"%{query}%"))
                .Select(data => new GetAllEmployeeResponseDto
                {
                    Id = data.employee.Id,
                    FullName = data.employee.FirstName + " " + data.employee.LastName,
                    PhoneNumber = data.employee.PhoneNumber,
                    PositionAr = data.role.TitleAr,
                    PositionEn = data.role.TitleEn,
                    IsActive = data.role.IsActive
                })
                .ToListAsync();

            return result.AsQueryable();
        }

        public async Task<GetEmployeeResponseDto?> GetEmployeeById(int employeeId)
        {
            var result = await _context.Employees
                .Where(employee => employee.Id == employeeId)
                .Join(_context.Users,
                    employee => employee.UserId,
                    user => user.Id,
                    (employee, user) => new { employee, user })
                .Join(_context.Roles,
                    combined => combined.user.RoleId,
                    role => role.Id,
                    (combined, role) => new { combined.employee, combined.user, role })
                .Join(_context.PublicLists,
                    combined => combined.employee.County,
                    county => county.Id,
                    (combined, county) => new { combined.employee, combined.user, combined.role, county })
                .Join(_context.PublicLists,
                    combined => combined.employee.City,
                    city => city.Id,
                    (combined, city) => new GetEmployeeResponseDto
                    {
                        Id = combined.employee.Id,
                        FirstName = combined.employee.FirstName,
                        LastName = combined.employee.LastName,
                        PhoneNumber = combined.employee.PhoneNumber,
                        CityAr = city.NameAR,
                        CityEn = city.NameEN,
                        CountyAr = combined.county.NameAR,
                        CountyEn = combined.county.NameEN,
                        Address = combined.employee.Address,
                        RoleAr = combined.role.TitleAr,
                        RoleEn = combined.role.TitleEn
                    })
                .FirstOrDefaultAsync();

            return result;
        }

        public async Task<int?> GetEmployeeIdByUserId(int userId)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(x => x.UserId == userId);
            return employee?.Id;
        }

        public Task<List<GetEmplyeesResponseDto>> GetSupervisors()
        {
            string roleName = "Supervisor";

            var result = from employee in _context.Employees
                         join user in _context.Users on employee.UserId equals user.Id
                         join role in _context.Roles on user.RoleId equals role.Id
                         where role.TitleEn == roleName
                         select new GetEmplyeesResponseDto()
                         {
                             Id = employee.Id,
                             FullName = $"{employee.FirstName} {employee.LastName}"
                         };

            return result.ToListAsync();
        }


        public Task<List<GetEmplyeesResponseDto>> GetCallCenters()
        {
            string roleName = "Call Center";

            var result = from employee in _context.Employees
                         join user in _context.Users on employee.UserId equals user.Id
                         join role in _context.Roles on user.RoleId equals role.Id
                         where role.TitleEn == roleName
                         select new GetEmplyeesResponseDto()
                         {
                             Id = employee.Id,
                             FullName = $"{employee.FirstName} {employee.LastName}"
                         };

            return result.ToListAsync();
        }

        public Task<List<GetEmplyeesResponseDto>> GetTechnician()
        {
            string roleName = "Technician";

            var result = from employee in _context.Employees
                         join user in _context.Users on employee.UserId equals user.Id
                         join role in _context.Roles on user.RoleId equals role.Id
                         where role.TitleEn == roleName
                         select new GetEmplyeesResponseDto()
                         {
                             Id = employee.Id,
                             FullName = $"{employee.FirstName} {employee.LastName}"
                         };

            return result.ToListAsync();
        }

        public async Task<Employee?> GetByUserId(int userId)
        {
            return await _context.Employees.FirstOrDefaultAsync(x => x.UserId == userId);
        }
    }
}
