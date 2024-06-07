using BTMBackend.Dtos.AdSliderDto;
using BTMBackend.Dtos.PublicDto;
using BTMBackend.Models;
using BTMBackend.Utilities;
using Microsoft.EntityFrameworkCore;

namespace BTMBackend.Data.Repos
{
    public interface IAdSliderRepo
    {
        public Task<bool> Create(AdSlider adSlider);
        public Task<bool> Update(AdSlider adSlider);
        public Task<bool> UpdateStatus(int sliderId);
        public Task<bool> Delete(AdSlider adSlider);
        public Task<List<GetAllSlidersWebResponseDto>> GetAllActive();
        public Task<IQueryable<GetAllSliderResponseDto>> GetAll();
        public Task<IQueryable<GetAllSliderResponseDto>> GetAllByTitle(string title);
        public Task<AdSlider?> GetById(int sliderId);
        public Task<GetSliderDetailsByIdResponseDto?> GetDetailsById(int sliderId);
    }

    public class AdSliderRepo(DataContext context, UploadFileService fileService) : IAdSliderRepo
    {
        private readonly DataContext _context = context;
        private readonly UploadFileService _fileService = fileService;

        public async Task<bool> Create(AdSlider adSlider)
        {
            await _context.AdSliders.AddAsync(adSlider);
            var result = await SaveChangres();
            return result;
        }

        public async Task<List<GetAllSlidersWebResponseDto>> GetAllActive()
        {
            var result = await _context.AdSliders.Where(x => x.IsActive)
                .Select(x => new GetAllSlidersWebResponseDto()
                {
                    Id = x.Id,
                    ImageAr = x.ImageArPath != null ? _fileService.ConvertFileToByteArray(x.ImageArPath) : null,
                    ImageEn = x.ImageEnPath != null ? _fileService.ConvertFileToByteArray(x.ImageEnPath) : null,
                }).ToListAsync();
            return result;
        }

        public async Task<IQueryable<GetAllSliderResponseDto>> GetAll()
        {
            var result = await _context.AdSliders
                .Select(x => new GetAllSliderResponseDto()
                {
                    Id = x.Id,
                    Title = x.Title,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt,
                }).ToListAsync();
            return result.AsQueryable();
        }

        public async Task<bool> Update(AdSlider adSlider)
        {
            _context.AdSliders.Update(adSlider);
            var result = await SaveChangres();
            return result;
        }

        private async Task<bool> SaveChangres()
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

        public async Task<GetSliderDetailsByIdResponseDto?> GetDetailsById(int sliderId)
        {
            var result = await _context.AdSliders
                .Where(slider => slider.Id == sliderId)
                .Join(_context.Users,
                    slider => slider.UserId,
                    user => user.Id,
                    (slider, user) => new { Slider = slider, User = user })
                .Join(_context.Employees,
                    combined => combined.User.Id,
                    employee => employee.UserId,
                    (combined, employee) => new { combined.Slider, Employee = employee })
                .Select(combined => new GetSliderDetailsByIdResponseDto
                {
                    Id = combined.Slider.Id,
                    Title = combined.Slider.Title,
                    ImageAr = combined.Slider.ImageArPath != null ? _fileService.ConvertFileToByteArray(combined.Slider.ImageArPath) : null,
                    ImageEn = combined.Slider.ImageEnPath != null ? _fileService.ConvertFileToByteArray(combined.Slider.ImageEnPath) : null,
                    CreatedAt = combined.Slider.CreatedAt,
                    UpdatedAt = combined.Slider.UpdatedAt,
                    IsActive = combined.Slider.IsActive,
                    UserName = combined.Employee.FirstName + " " + combined.Employee.LastName
                })
                .FirstOrDefaultAsync();

            return result;
        }

        public async Task<AdSlider?> GetById(int sliderId)
        {
            return await _context.AdSliders.FirstOrDefaultAsync(x => x.Id == sliderId);
        }

        public async Task<bool> UpdateStatus(int sliderId)
        {
            var slider = await _context.AdSliders.FindAsync(sliderId);
            if (slider == null)
            {
                return false;
            }
            slider.IsActive = !slider.IsActive;
            var result = await SaveChangres();
            return result;
        }

        public async Task<IQueryable<GetAllSliderResponseDto>> GetAllByTitle(string title)
        {
            var result = await _context.AdSliders
                .Where(x => EF.Functions.Like(x.Title, $"%{title}%"))
                .Select(x => new GetAllSliderResponseDto()
                {
                    Id = x.Id,
                    Title = x.Title,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt,
                }).ToListAsync();
            return result.AsQueryable();
        }

        public async Task<bool> Delete(AdSlider adSlider)
        {
            _context.AdSliders.Remove(adSlider);
            var result = await SaveChangres();
            return result;
        }
    }
}
