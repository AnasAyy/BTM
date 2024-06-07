using BTMBackend.Dtos.CategoryDto;
using BTMBackend.Dtos.ContactUsDto;
using BTMBackend.Dtos.PublicListDto;
using BTMBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace BTMBackend.Data.Repos
{
    public interface IContactUsRepo
    {
        public Task<List<GetTypeResponseDto>> GetTypeAsync();
        public  Task CreateAsync(ContactUs contactUs);
        public Task<IQueryable<GetAllMessagesResponseDto>> GetAllMessage();
        public Task<bool> CheckyTypeAsync(int type);
        public Task<IQueryable<GetAllMessagesResponseDto>> SearchByName(string Query);
        public  Task<bool> SaveChangesAsync();
    }


    public class ContactUsRepo : IContactUsRepo
    {
        private readonly DataContext _context;

        public ContactUsRepo(DataContext context)
        {
            _context = context;
        }

        public async Task<List<GetTypeResponseDto>> GetTypeAsync()
        {
            var query = from p in _context.PublicLists
                        where (p.Code == "Complaint" || p.Code == "Suggition") && p.Type != 0 && p.Status == true
                        select new GetTypeResponseDto
                        {
                            Id = p.Id,
                            NameAR = p.NameAR,
                            NameEN = p.NameEN
                        };

            return await Task.FromResult(query.ToList());
        }
        public async Task CreateAsync(ContactUs contactUs)
        {
            try
            {
                await _context.ContactUs.AddAsync(contactUs);
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<IQueryable<GetAllMessagesResponseDto>> GetAllMessage()
        {
            var query = from c in _context.ContactUs
                        from pl in _context.PublicLists
                        where pl.Id == c.Type
                        orderby c.Id descending
                        select new GetAllMessagesResponseDto
                        {
                            Id = c.Id,
                            TypeAR = pl.NameAR,
                            TypeEN = pl.NameEN,
                            Name = c.Name,
                            PhoneNumber = c.PhoneNumber,
                            Message = c.Message,
                        };

            return await Task.FromResult(query);
        }
        public async Task<bool> CheckyTypeAsync(int type)
        {
            return await _context.PublicLists.AnyAsync(x => x.Id == type && x.Type != 0 );
        }

        public async Task<IQueryable<GetAllMessagesResponseDto>> SearchByName(string Query)
        {
            var query = from c in _context.ContactUs
                        from pl in _context.PublicLists
                        where pl.Id == c.Type
                        && c.Name.Contains(Query) 
                        orderby c.Id descending
                        select new GetAllMessagesResponseDto
                        {
                            Id = c.Id,
                            TypeAR = pl.NameAR,
                            TypeEN = pl.NameEN,
                            Name = c.Name,
                            PhoneNumber = c.PhoneNumber,
                            Message = c.Message,
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
    }

}
