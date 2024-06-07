using BTMBackend.Dtos.UserDto;
using BTMBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace BTMBackend.Data.Repos
{
    public interface IUserRepo
    {
        public Task<LogInResponseDto?> Login(string userName, string password);
        public Task Create(User user);
        public void Update(User user);
        public Task<bool> UpdateLastLoginDateById(int id);
        public Task<bool> SaveChanges();
        public Task<bool> IsUserExist(string username);
        public Task<bool> IsUserVerified(int userId);
        Task<List<int>> GetAllSupervisorId();
        public Task<User?> GetById(int userId);
        public Task<bool> UpdateStatus(int userId);
        public Task UpdatePassword(string password , int UserId);
        public Task<User?> GetUserByPhoneNumber(string phoneNumber);
        Task<CreateTokenRequestDto?> GetUserAndRole(int userId);
    }

    public class UserRepo : IUserRepo
    {
        private readonly DataContext _context;
        public UserRepo(DataContext context)
        {
            _context = context;
        }

        public async Task Create(User user)
        {
            await _context.AddAsync(user);
        }

        public async Task<LogInResponseDto?> Login(string userName, string password)
        {
            string FullName = "your_access_token_here";
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == userName && u.Password == password);

            if (user == null)
                return null;

            var getRole = await _context.Roles.FirstOrDefaultAsync(x => x.Id == user.RoleId);

            if (getRole == null)
                return null;

            var getCustomer = await _context.Customers.FirstOrDefaultAsync(x => x.UserId == user.Id);
            if (getCustomer != null)
            {
                FullName = getCustomer.FirstName + " " + getCustomer.LastName;
            }
            var getEmployee = await _context.Employees.FirstOrDefaultAsync(x => x.UserId == user.Id);
            if (getEmployee != null)
            {
                FullName = getEmployee.FirstName + " " + getEmployee.LastName;
            }

            return new LogInResponseDto()
            {
                Id = user.Id,
                Name = FullName,
                PositionAr = getRole.TitleAr,
                PositionEn = getRole.TitleEn,
                Token = "your_access_token_here"
            };
        }



        public async Task<bool> IsUserExist(string username)
        {
            return await _context.Users.AnyAsync(x => x.Username == username);
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

        public void Update(User user)
        {
            _context.Users.Update(user);
        }

        public async Task<bool> UpdateLastLoginDateById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                user.LastLoginDateTime = DateTime.Now;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> IsUserVerified(int userId)
        {
            return await _context.Users.AnyAsync(x=>x.Id == userId && x.IsVerified);
        }

        public async Task UpdatePassword(string password, int UserId)
        {
            var getUser = await _context.Users.FirstOrDefaultAsync(x=>x.Id == UserId);
            if (getUser != null)
            {
                getUser.Password = password;
                getUser.UpdatedAt = DateTime.Now;
                getUser.IsVerified = true;
                _context.Users.Update(getUser);
            }

        }

        public async Task<User?> GetUserByPhoneNumber(string phoneNumber)
        {
            return await _context.Users.FirstOrDefaultAsync(x=>x.Username == phoneNumber);
        }

        public async Task<User?> GetById(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> UpdateStatus(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return false;
            }
            user.IsActive = !user.IsActive;
            var result = await SaveChanges();
            return result;
        }

        public async Task<CreateTokenRequestDto?> GetUserAndRole(int userId)
        {
            var result = await _context.Users
                .Where(user => user.Id == userId)
                .Join(_context.Roles,
                    user => user.RoleId,
                    role => role.Id,
                    (user, role) => new CreateTokenRequestDto
                    {
                        Role = role.TitleEn,
                        UserId = user.Id,
                    })
                .FirstOrDefaultAsync();

            return result;
        }

        public async Task<List<int>> GetAllSupervisorId()
        {
            var result = await (from user in _context.Users
                                join role in _context.Roles on user.RoleId equals role.Id
                                where role.TitleEn == "Supervisor"
                                select user.Id).ToListAsync();

            return result;
        }

    }
}
