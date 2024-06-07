using BTMBackend.Dtos.CategoryDto;
using BTMBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace BTMBackend.Data.Repos
{
    public interface IPartRepo
    {
        public Task CreateAsync(Part part);
        public Task<bool> GetPartByNameForAddAsync(string nameAR, string nameEN, int productId);
        public void Update(Part part);
        public Task<List<Part>?> GetByProductId(int productId);
        public Task<bool> GetPartByNameForUpdateAsync(string nameAR, string nameEN, int id);
        public Task<Part?> GetByIdAsync(int id);
        public Task<bool> SaveChangesAsync();
    }


    public class PartRepo(DataContext context) : IPartRepo
    {
        private readonly DataContext _context = context;

        public async Task CreateAsync(Part part)
        {
            try
            {
                await _context.Parts.AddAsync(part);
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();
                Console.WriteLine(ex.Message);
            }
        }
        public void Update(Part part)
        {
            try
            {
                _context.Parts.Update(part);
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();
                Console.WriteLine(ex.Message);
            }
        }
        public async Task<bool> GetPartByNameForAddAsync(string nameAR, string nameEN,int productId)
        {
            return await _context.Parts.AnyAsync(x => (x.NameAr == nameAR || x.NameEn == nameEN) && x.ProductId== productId);
        }
        public async Task<bool> GetPartByNameForUpdateAsync(string nameAR, string nameEN, int id)
        {
            return await _context.Parts.AnyAsync(x => (x.NameAr == nameAR || x.NameEn == nameEN) && x.Id != id);
        }
        public async Task<Part?> GetByIdAsync(int id)
        {
            var x = await _context.Parts.SingleOrDefaultAsync(x => x.Id == id);
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

        public async Task<List<Part>?> GetByProductId(int productId)
        {
            return await _context.Parts.Where(x => x.ProductId == productId).ToListAsync();
        }
    }
}
