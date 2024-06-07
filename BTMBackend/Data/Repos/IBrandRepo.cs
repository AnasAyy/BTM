using BTMBackend.Dtos.BrandDto;
using BTMBackend.Models;
using BTMBackend.Utilities;
using Microsoft.EntityFrameworkCore;

namespace BTMBackend.Data.Repos
{
    public interface IBrandRepo
    {
        public Task CreateAsync(Brand brand);
        public Task<IQueryable<Brand>> GetAllBrands();
        public Task<List<GetForHomePageResponce>> GetAllBrandsFroHomePa();
        public Task<IQueryable<Brand>> SearchByName(string query);

        public Task<Brand> GetBrandById(int id);
        public Task<bool> GetBrandByNameForAdd(string nameAr, string nameEn);
        public Task<bool> GetBrandByNameForUpdate(int id, string nameAr, string nameEn);
        public Task<bool> Update(Brand brand);
        public Task<bool> DeleteBrand(int id);
        public Task<bool> SaveChangesAsync();
    }

    public class BrandRepo(DataContext context, UploadFileService uploadFileService) : IBrandRepo
    {
        private readonly DataContext _context = context;
        private readonly UploadFileService _uploadFileService = uploadFileService;
        

        public async Task CreateAsync(Brand brand)
        {
            await _context.Brands.AddAsync(brand);
        }

        public Task<bool> DeleteBrand(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<Brand>> GetAllBrands()
        {
            throw new NotImplementedException();
        }

        

        public async Task<List<GetForHomePageResponce>> GetAllBrandsFroHomePa()
        {
            var query = from b in _context.Brands
                        where b.IsActive == true
                        select new GetForHomePageResponce
                        {
                            Id = b.Id,
                            Logo = b.Logo,
                        };

            var brandResponses = await query.ToListAsync();

            foreach (var brandResponse in brandResponses)
            {
                
                var photo = await _uploadFileService.ConvertFileToByteArrayAsync(brandResponse.Logo);

                
                brandResponse.Photo = photo;
            }

            return brandResponses.Select(b => new GetForHomePageResponce
            {
                Id = b.Id,
                Photo = b.Photo
            }).ToList();
        
        }

        public Task<Brand> GetBrandById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> GetBrandByNameForAdd(string nameAr, string nameEn)
        {
            return await _context.Brands.AnyAsync(b => b.NameAr == nameAr || b.NameEn == nameEn);
        }

        public Task<bool> GetBrandByNameForUpdate(int id, string nameAr, string nameEn)
        {
            throw new NotImplementedException();
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

        public Task<IQueryable<Brand>> SearchByName(string query)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(Brand brand)
        {
            throw new NotImplementedException();
        }
    }
}
