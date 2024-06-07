using BTMBackend.Dtos.ExternalLinkDto;
using BTMBackend.Dtos.PublicListDto;
using BTMBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace BTMBackend.Data.Repos
{
    public interface IExternalLinkRepo
    {
        //public Task CreateAsync(ExternalLink externalLink);
        //public Task<List<GetTypeResponseDto>> GetTypeAsync();
        public void Update(ExternalLink externalLink);
        public Task<List<Links>> GetByAllLinksAsync();
        //public Task<Uri?> GetStoreLinkAsync();
        public Task<IQueryable<GetAllLinksResponseDto>> GetAllLinksAsync();
        public Task<IQueryable<GetAllLinksResponseDto>> SearchByName(string Query);
        //public Task<bool> CheckyTypeAsync(int type);
        //public Task<bool> GetLinkForAddAsync(string name, Uri link);
        public Task<ExternalLink?> GetByIdAsync(int id);
        public Task<bool> GetLinkForUpdateAsync(Uri link, int id);
        public Task<bool> SaveChangesAsync();
    }

    public class ExternalLinkRepo : IExternalLinkRepo
    {
        readonly DataContext _context;
        public ExternalLinkRepo(DataContext context)
        {
            _context = context;
        }
        //public async Task CreateAsync(ExternalLink externalLink)
        //{
        //    try
        //    {
        //        await _context.ExternalLinks.AddAsync(externalLink);
        //    }
        //    catch (Exception ex)
        //    {
        //        _context.ChangeTracker.Clear();
        //        Console.WriteLine(ex.Message);
        //    }
        //}
        //public async Task<List<GetTypeResponseDto>> GetTypeAsync()
        //{
        //    var query = from p in _context.PublicLists
        //                where (p.Code == "ExternalLink") && p.Type != 0
        //                select new GetTypeResponseDto
        //                {
        //                    Id = p.Id,
        //                    NameAR = p.NameAR,
        //                    NameEN = p.NameEN
        //                };

        //    return await Task.FromResult(query.ToList());
        //}
        public void Update(ExternalLink externalLink)
        {
            try
            {
                _context.ExternalLinks.Update(externalLink);
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();
                Console.WriteLine(ex.Message);
            }
        }
        public async Task<List<Links>> GetByAllLinksAsync()
        {
            var externalLinks = await _context.ExternalLinks
                .Select(x => new { NameEn = x.NameEn, Link = x.Link })
                .ToListAsync();

            return externalLinks.Select(x => new Links { Name = x.NameEn, Link = x.Link }).ToList();
        }
        public async Task<IQueryable<GetAllLinksResponseDto>> GetAllLinksAsync()
        {
            var query = from externalLink in _context.ExternalLinks
                        join user in _context.Users on externalLink.UserId equals user.Id
                        join employee in _context.Employees on user.Id equals employee.UserId
                        select new GetAllLinksResponseDto
                        {
                            Id = externalLink.Id,
                            NameAr = externalLink.NameAr,
                            NameEn = externalLink.NameEn,
                            Link = externalLink.Link,
                            CreatedAt = externalLink.CreatedAt,
                            CreatedBy = $"{employee.FirstName} {employee.LastName}"
                        };
            return await Task.FromResult(query);
        }
        //public async Task<bool> CheckyTypeAsync(int type)
        //{
        //    return await _context.PublicLists.AnyAsync(x => x.Id == type && x.Type != 0);
        //}
        //public async Task<bool> GetLinkForAddAsync(string name, Uri link)
        //{
        //    return await _context.ExternalLinks.AnyAsync(x => x.Name == name || x.Link == link);
        //}

        public async Task<bool> GetLinkForUpdateAsync(Uri link, int id)
        {
            return await _context.ExternalLinks
                .AnyAsync(x =>
                x.Link == link && x.Id != id);
        }
        public async Task<ExternalLink?> GetByIdAsync(int id)
        {
            var x = await _context.ExternalLinks.SingleOrDefaultAsync(x => x.Id == id);
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

        public async Task<IQueryable<GetAllLinksResponseDto>> SearchByName(string Query)
        {
            var query = from externalLink in _context.ExternalLinks
                        join user in _context.Users on externalLink.UserId equals user.Id
                        join employee in _context.Employees on user.Id equals employee.UserId
                        where externalLink.NameAr.Contains(Query) ||  externalLink.NameEn.Contains(Query)
                        select new GetAllLinksResponseDto
                        {
                            Id = externalLink.Id,
                            NameAr = externalLink.NameAr,
                            NameEn = externalLink.NameEn,
                            Link = externalLink.Link,
                            CreatedAt = externalLink.CreatedAt,
                            CreatedBy = $"{employee.FirstName} {employee.LastName}"
                        };
            return await Task.FromResult(query);
        }
    }

    public class Links() {
        public string Name { get; set; } = null!;
        public Uri Link { get; set; }=null!;
    }

}
