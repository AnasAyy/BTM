using BTMBackend.Dtos.StatisticDto;
using BTMBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace BTMBackend.Data.Repos
{
    public interface IStatisticRepo
    {
        public Task CreateAsync(Statistic statistic);
        public Task<bool> CheckName(string name);
        public Task<IQueryable<GetAllStatisticResponseDto>> SearchByName(string name);

        public void Update(Statistic statistic);
        public Task<Statistic?> GetByIdAsync(int id);
        public Task<bool> GetItemByNameForUpdateAsync(string name, int id);
        public Task<IQueryable<GetAllStatisticResponseDto>> GetAllAsync();
        public Task<IQueryable<GetAllForHomePageResponseDto>> GetAllForHomePageAsync();
        public Task<bool> SaveChangesAsync();
    }
    public class StatisticRepo : IStatisticRepo
    {
        private readonly DataContext _context;
        public StatisticRepo(DataContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Statistic statistic)
        {
            try
            {
                await _context.Statistics.AddAsync(statistic);
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();
                Console.WriteLine(ex.Message);
            }
        }

        public void Update(Statistic statistic)
        {
            try
            {
                _context.Statistics.Update(statistic);
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<bool> CheckName(string name)
        {
            return await _context.Statistics.AnyAsync(x => x.NameAr == name || x.NameEn==name);
        }
        public async Task<IQueryable<GetAllStatisticResponseDto>> GetAllAsync()
        {
            var query = from stat in _context.Statistics
                        join user in _context.Users on stat.UserId equals user.Id
                        join employee in _context.Employees on user.Id equals employee.UserId
                        select new GetAllStatisticResponseDto
                        {
                            Id = stat.Id,
                            NameAr = stat.NameAr,
                            NameEn = stat.NameEn,
                            FakeValue = stat.FakeValue,
                            RealValue = stat.RealValue,
                            Status = stat.Status,
                            CreatedAt = stat.CreatedAt,
                            CreatedBy = $"{employee.FirstName} {employee.LastName}"
                        };

            return await Task.FromResult(query);
        }
        public async Task<IQueryable<GetAllStatisticResponseDto>> SearchByName(string name)
        {
            var query = from stat in _context.Statistics
                        join user in _context.Users on stat.UserId equals user.Id
                        join employee in _context.Employees on user.Id equals employee.UserId
                        where stat.NameAr.Contains(name) || stat.NameEn.Contains(name)
                        select new GetAllStatisticResponseDto
                        {
                            Id = stat.Id,
                            NameAr = stat.NameAr,
                            NameEn = stat.NameEn,
                            FakeValue = stat.FakeValue,
                            RealValue = stat.RealValue,
                            Status = stat.Status,
                            CreatedAt = stat.CreatedAt,
                            CreatedBy = $"{employee.FirstName} {employee.LastName}"
                        };

            return await Task.FromResult(query);
        }
        public async Task<Statistic?> GetByIdAsync(int id)
        {
            var x = await _context.Statistics.SingleOrDefaultAsync(x => x.Id == id);
            _context.ChangeTracker.Clear();
            return x;
        }

        public async Task<bool> GetItemByNameForUpdateAsync(string name, int id)
        {
            return await _context.Statistics
                .AnyAsync(x => (x.NameAr == name || x.NameEn == name) &&  x.Id != id);
        }
        public async Task<bool> SaveChangesAsync()
        {
            try
            {
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();
                Console.WriteLine(ex.Message);
            }

            return false;
        }

        public async Task<IQueryable<GetAllForHomePageResponseDto>> GetAllForHomePageAsync()
        {
            var query = from stat in _context.Statistics
                        select new GetAllForHomePageResponseDto
                        {
                            NameAr = stat.NameAr,
                            NameEn = stat.NameEn,
                            Value = stat.Status ? stat.RealValue : stat.FakeValue
                        };

            return await Task.FromResult(query);
        }
    }
}
