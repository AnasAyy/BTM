using BTMBackend.Dtos.PublicListDto;
using BTMBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace BTMBackend.Data.Repos
{
    public interface IPublicListRepo
    {
        public Task CreateAsync(PublicList publicList);
        public void Update(PublicList publicList);
        public  Task<IQueryable<GetAllItemsResponseDto>> SearchMainByName(string name);
        public  Task<IQueryable<GetAllItemsResponseDto>> GetMainItemAsync();

        public Task<IQueryable<GetAllItemsResponseDto>> GetAllAsync(string code);
        public Task<IQueryable<GetAllItemsResponseDto>> SearchByName(string name , string code);
        public Task<bool> CheckyTypeAsync(string Code);
        public Task<bool> GetItemByNameForAddAsync(string nameAR, string nameEN);
        public Task<bool> GetItemByNameForUpdateAsync(string nameAR, string nameEN, int type, int id);
        public Task<PublicList?> GetByIdAsync(int id);
        public Task<List<GetPublicListResponseDto>> GetAllCounties();
        public Task<List<GetPublicListResponseDto>> GetItems(string code);
        public Task<bool> SaveChangesAsync();
    }


    public class PublicRepo : IPublicListRepo
    {
        private readonly DataContext _context;

        public PublicRepo(DataContext context)
        {
            _context = context;
        }


        public async Task CreateAsync(PublicList publicList)
        {
            try
            {
                await _context.PublicLists.AddAsync(publicList);
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();
                Console.WriteLine(ex.Message);
            }
        }

        public void Update(PublicList publicList)
        {
            try
            {
                _context.PublicLists.Update(publicList);
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<IQueryable<GetAllItemsResponseDto>> GetMainItemAsync()
        {

            var query = from p in _context.PublicLists
                        where p.Type==0
                        orderby p.Id descending
                        select new GetAllItemsResponseDto
                        {
                            Id = p.Id,
                            NameAR = p.NameAR,
                            NameEN = p.NameEN,
                            Status = p.Status,
                            Code = p.Code,
                            CreateAt = p.CreatedAt,
                        };

            return await Task.FromResult(query);

        }

        public async Task<IQueryable<GetAllItemsResponseDto>> SearchMainByName(string name)
        {

            var query = from p in _context.PublicLists
                        where p.Type == 0 && (p.NameAR.Contains(name) || p.NameEN.Contains(name))
                        orderby p.Id descending
                        select new GetAllItemsResponseDto
                        {
                            Id = p.Id,
                            NameAR = p.NameAR,
                            NameEN = p.NameEN,
                            Status = p.Status,
                            Code = p.Code,
                            CreateAt = p.CreatedAt,
                        };

            return await Task.FromResult(query);

        }

        public async Task<IQueryable<GetAllItemsResponseDto>> GetAllAsync(string code)
        {
            var query = from p in _context.PublicLists
                        where p.Code == code && p.Type != 0 
                        orderby p.Id descending
                        select new GetAllItemsResponseDto
                        {
                            Id = p.Id,
                            NameAR = p.NameAR,
                            NameEN = p.NameEN,
                            Status = p.Status,
                            Code = p.Code,
                            CreateAt = p.CreatedAt,
                        };

            return await Task.FromResult(query);
        }

        public async Task<PublicList?> GetByIdAsync(int id)
        {
            var x = await _context.PublicLists.SingleOrDefaultAsync(x => x.Id == id);
            _context.ChangeTracker.Clear();
            return x;
        }

        public async Task<bool> GetItemByNameForAddAsync(string nameAR, string nameEN)
        {
            return await _context.PublicLists.AnyAsync(x => x.NameAR == nameAR || x.NameEN == nameEN);
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

        public async Task<bool> CheckyTypeAsync(string Code)
        {
            return await _context.PublicLists.AnyAsync(x => x.Code == Code && x.Type == 0);
        }

        public async Task<bool> GetItemByNameForUpdateAsync(string nameAR, string nameEN, int type, int id)
        {
            return await _context.PublicLists
                .AnyAsync(x => x.NameAR == nameAR && x.NameEN == nameEN && x.Type == type && x.Id != id);
        }

        public async Task<List<GetPublicListResponseDto>> GetAllCounties()
        {
            var query = from p in _context.PublicLists
                        where p.Type == 0 && p.Code.Contains("County") && p.Status == true
                        select new GetPublicListResponseDto
                        {
                            Id = p.Id,
                            NameAR = p.NameAR,
                            NameEN = p.NameEN,
                            Code = p.Code,
                        };

            return await query.ToListAsync();
        }

        public async Task<List<GetPublicListResponseDto>> GetItems(string code)
        {
            var query = from p in _context.PublicLists
                        where p.Type == 1 && p.Code == code && p.Status == true
                        orderby p.Id descending
                        select new GetPublicListResponseDto
                        {
                            Id = p.Id,
                            NameAR = p.NameAR,
                            NameEN = p.NameEN,
                        };

            return await query.ToListAsync();
        }

        public async Task<IQueryable<GetAllItemsResponseDto>> SearchByName(string code, string name)
        {
            var query = from p in _context.PublicLists
                        where p.Code == code && p.Type != 0 && p.Status == true && (p.NameAR.Contains(name) || p.NameEN.Contains(name))
                        orderby p.Id descending
                        select new GetAllItemsResponseDto
                        {
                            Id = p.Id,
                            NameAR = p.NameAR,
                            NameEN = p.NameEN,
                            Status = p.Status,
                            CreateAt = p.CreatedAt,
                        };

            return await Task.FromResult(query);
        }
    }
}
