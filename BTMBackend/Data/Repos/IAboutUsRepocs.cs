using BTMBackend.Dtos.AboutUsDto;
using BTMBackend.Dtos.CategoryDto;
using BTMBackend.Dtos.Product;
using BTMBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace BTMBackend.Data.Repos
{
    public interface IAboutUsRepocs
    {
        public Task CreateAsync(AboutUs aboutUs);
        public void Update(AboutUs aboutUs);
        public void Delete(AboutUs aboutUs);
        public Task<AboutUs?> GetByIdAsync(int id);
        public Task<IQueryable<GetAllAboutUSResponseDto>> GetAllAsync();
        public  Task<IQueryable<GetAllAboutUSResponseDto>> SearchByName(string name);
        public Task<IQueryable<GetAboutUSForHomePageResponseDto>> GetAsync();
        public Task<bool> SaveChangesAsync();

    }

    public class AboutUsRepo(DataContext context) : IAboutUsRepocs
    {
        private readonly DataContext _context = context;
        public async Task CreateAsync(AboutUs aboutUs)
        {
            try
            {
                await _context.AboutUs.AddAsync(aboutUs);
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();
                Console.WriteLine(ex.Message);
            }
        }
        public void Delete(AboutUs aboutUs)
        {
            try
            {
                _context.AboutUs.Remove(aboutUs);
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();
                Console.WriteLine(ex.Message);
            }
        }
        public async Task<IQueryable<GetAllAboutUSResponseDto>> GetAllAsync()
        {
            var query = from a in _context.AboutUs
                        join u in _context.Users on a.UserId equals u.Id
                        join e in _context.Employees on u.Id equals e.UserId
                        orderby a.Id descending
                        select new GetAllAboutUSResponseDto
                        {
                            Id = a.Id,
                            NameAr = a.NameAr,
                            NameEn = a.NameEn,
                            DescriptionAr = a.DescriptionAr,
                            DescriptionEn = a.DescriptionEn,
                            IsActive = a.IsActive,
                            CreatedAt = a.CreatedAt,
                            UpdatedAt = a.UpdatedAt,
                            CreatedBy = $"{e.FirstName} {e.LastName}"
                        };

            return await Task.FromResult(query);
        }

        public async Task<IQueryable<GetAboutUSForHomePageResponseDto>> GetAsync()
        {
            var query = from a in _context.AboutUs
                         where a.IsActive == true
                         orderby a.Id descending
                         select new GetAboutUSForHomePageResponseDto
                         {
                             Id = a.Id,
                             NameAr = a.NameAr,
                             NameEn = a.NameEn,
                             DescriptionAr = a.DescriptionAr,
                             DescriptionEn = a.DescriptionEn,
                         };

            return await Task.FromResult(query);
        }

        public async Task<AboutUs?> GetByIdAsync(int id)
        {
            var x = await _context.AboutUs.SingleOrDefaultAsync(x => x.Id == id);
            _context.ChangeTracker.Clear();
            return x;
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
        public void Update(AboutUs aboutUs)
        {
            try
            {
                _context.AboutUs.Update(aboutUs);
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<IQueryable<GetAllAboutUSResponseDto>> SearchByName(string Query)
        {
            var query = from a in _context.AboutUs
                        join u in _context.Users on a.UserId equals u.Id
                        join e in _context.Employees on u.Id equals e.UserId
                        where a.NameAr.Contains(Query) || a.NameEn.Contains(Query)
                        orderby a.Id descending
                        select new GetAllAboutUSResponseDto
                        {
                            Id = a.Id,
                            NameAr = a.NameAr,
                            NameEn = a.NameEn,
                            DescriptionAr = a.DescriptionAr,
                            DescriptionEn = a.DescriptionEn,
                            IsActive = a.IsActive,
                            CreatedAt = a.CreatedAt,
                            UpdatedAt = a.UpdatedAt,
                            CreatedBy = $"{e.FirstName} {e.LastName}"
                        };

            return await Task.FromResult(query);
        }

    }
}
