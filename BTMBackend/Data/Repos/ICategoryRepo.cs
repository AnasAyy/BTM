using BTMBackend.Dtos.CategoryDto;
using BTMBackend.Dtos.PublicListDto;
using BTMBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace BTMBackend.Data.Repos
{
    public interface ICategoryRepo
    {
        public Task CreateAsync(Category category);
        public Task<IQueryable<GetAllCategoryResponceDto>> SearchByName(string Query);
        public Task<bool> GetCategoryByNameForAddAsync(string nameAR,string nameEN);
        public void Update(Category category);
        public Task<bool> GetCategoryByNameForUpdateAsync(string nameAR, string nameEN, int id);
        public Task<Category?> GetByIdAsync(int id);
        public Task<IQueryable<GetAllCategoryResponceDto>> GetAllAsync();
        public Task<List<AppGetAllCategoryResponseDto>> AppGetAllAsync();
        public Task<bool> SaveChangesAsync();
    }

    public class CategoryRepo(DataContext context) : ICategoryRepo
    {
        private readonly DataContext _context = context;

        public async Task CreateAsync(Category category)
        {
            try
            {
                await _context.Categories.AddAsync(category);
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();
                Console.WriteLine(ex.Message);
            }
        }
        public void Update(Category category)
        {
            try
            {
                _context.Categories.Update(category);
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();
                Console.WriteLine(ex.Message);
            }
        }
        public async Task<bool> GetCategoryByNameForAddAsync(string nameAR, string nameEN)
        {
            return await _context.Categories.AnyAsync(x => x.NameAr == nameAR || x.NameEn == nameEN);
        }
        public async Task<bool> GetCategoryByNameForUpdateAsync(string nameAR, string nameEN, int id)
        {
            return await _context.Categories.AnyAsync(x => (x.NameAr == nameAR || x.NameEn == nameEN) && x.Id != id);
        }
        public async Task<Category?> GetByIdAsync(int id)
        {
            var x = await _context.Categories.SingleOrDefaultAsync(x => x.Id == id);
            _context.ChangeTracker.Clear();
            return x;
        }
        public async Task<IQueryable<GetAllCategoryResponceDto>> GetAllAsync()
        {
            var query = from c in _context.Categories
                        join u in _context.Users on c.UserId equals u.Id
                        join e in _context.Employees on u.Id equals e.UserId
                        orderby c.Id descending
                        select new GetAllCategoryResponceDto
                        {
                            Id = c.Id,
                            NameAr = c.NameAr,
                            NameEn = c.NameEn,
                            Status = c.IsActive,
                            CreatedAt = c.CreatedAt,
                            CreatedBy = $"{e.FirstName} {e.LastName}"
                        };

            return await Task.FromResult(query);
        }



        public async Task<IQueryable<GetAllCategoryResponceDto>> SearchByName(string Query)
        {
            var query = from c in _context.Categories
                        join u in _context.Users on c.UserId equals u.Id
                        join e in _context.Employees on u.Id equals e.UserId
                        where c.NameAr.Contains(Query) || c.NameEn.Contains(Query)
                        orderby c.Id descending
                        select new GetAllCategoryResponceDto
                        {
                            Id = c.Id,
                            NameAr = c.NameAr,
                            NameEn = c.NameEn,
                            Status = c.IsActive,
                            CreatedAt = c.CreatedAt,
                            CreatedBy = $"{e.FirstName} {e.LastName}"
                        };

            return await Task.FromResult(query);
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

        public async Task<List<AppGetAllCategoryResponseDto>> AppGetAllAsync()
        {
            var result = await _context.Categories.Where(x=>x.IsActive)
                .Select(x=> new AppGetAllCategoryResponseDto()
                {
                    Id = x.Id,
                    NameAr = x.NameAr,
                    NameEn = x.NameEn,
                }).ToListAsync();
            return result;
        }
    }

}
