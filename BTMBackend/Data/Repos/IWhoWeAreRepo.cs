using BTMBackend.Dtos.WhoWeAreDto;
using BTMBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace BTMBackend.Data.Repos
{
    public interface IWhoWeAreRepo
    {
        public void Update(WhoWeAre whoWeAre);
        public Task<bool> SaveChangesAsync();
        public Task<WhoWeAre?> GetByIdAsync(int id);

        public Task<WhoWeAre> Get();
        public Task<WhoWeAreResponceDto> GetForHome();


    }

    public class WhoWeAreRepo : IWhoWeAreRepo
    {
        private readonly DataContext _context;
        public WhoWeAreRepo(DataContext context)
        {
            _context = context;
        }

        public void Update(WhoWeAre whoWeAre)
        {
            try
            {
                _context.WhoWeAres.Update(whoWeAre);
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();
                Console.WriteLine(ex.Message);
            }
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

        public async Task<WhoWeAre> Get()
        {
            var result = await _context.WhoWeAres.FirstOrDefaultAsync();

            if (result == null)
            {
                throw new InvalidOperationException("The result is null.");
            }

            return result;
        }

        public async Task<WhoWeAreResponceDto> GetForHome()
        {
            var result = await _context.WhoWeAres
                .Select(w => new WhoWeAreResponceDto
                {
                    DescriptionAr = w.DescriptionAr,
                    DescriptionEn = w.DescriptionEn
                })
                .FirstOrDefaultAsync();

            if (result == null)
            {
                throw new InvalidOperationException("The result is null.");
            }

            return result;
        }

        public async Task<WhoWeAre?> GetByIdAsync(int id)
        {
            var x = await _context.WhoWeAres.SingleOrDefaultAsync(x => x.Id == id);
            _context.ChangeTracker.Clear();
            return x;
        }
    }
}
