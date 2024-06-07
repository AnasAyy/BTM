using BTMBackend.Dtos.CustomerDto;
using BTMBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace BTMBackend.Data.Repos
{
    public interface ICustomerRepo
    {
        public Task Create(Customer customer);
        public void Update(Customer customer);
        public Task<Customer?> GetById(int id);
        public Task<int?> GetCustomerId(int userId);
        public Task<string?> GetCustomerPhoneById(int? id);
        public Task<GetCustomerDetailsResponseDto?> GetCustomerDetailsById(int customerId);
        public Task<Customer?> GetByPhoneNumber(string phoneNumber);
        public Task<IQueryable<GetAllCustomersResponseDto>> GetAll();
        public Task<bool> SaveChanges();
    }
    public class CustomerRepo : ICustomerRepo
    {
        private readonly DataContext _context;

        public CustomerRepo(DataContext context)
        {
            _context = context;
        }

        public async Task Create(Customer customer)
        {
            await _context.Customers.AddAsync(customer);
        }

        public async Task<Customer?> GetById(int id)
        {
            return await _context.Customers.FirstOrDefaultAsync(x => x.UserId == id);
        }

        public async Task<Customer?> GetByPhoneNumber(string phoneNumber)
        {
            return await _context.Customers.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
        }

        public async Task<GetCustomerDetailsResponseDto?> GetCustomerDetailsById(int customerId)
        {
            var result = await _context.Customers
                .Where(x => x.Id == customerId)
                .Select(x => new GetCustomerDetailsResponseDto
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    PhoneNumber = x.PhoneNumber,
                    County = x.County,
                    City = x.City,
                    Address = x.Address
                })
                .FirstOrDefaultAsync();

            return result;
        }

        public async Task<int?> GetCustomerId(int userId)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(x => x.UserId == userId);
            return customer?.Id;
        }

        public async Task<string?> GetCustomerPhoneById(int? id)
        {
            var result = await _context.Customers.FirstOrDefaultAsync(x => x.Id == id);
            return result?.PhoneNumber;
        }

        public async Task<bool> SaveChanges()
        {
            try
            {
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _context.ChangeTracker.Clear();
            }
            return false;
        }

        public void Update(Customer customer)
        {
            _context.Customers.Update(customer);
        }

        async Task<IQueryable<GetAllCustomersResponseDto>> ICustomerRepo.GetAll()
        {
            var result = await _context.Customers
                            .Join(_context.PublicLists,
                             customer => customer.City,
                             city => city.Id,
                             (customer, city) => new { customer, city })
                            .Join(_context.PublicLists,
                             combined => combined.customer.County,
                             county => county.Id,
                            (combined, county) => new GetAllCustomersResponseDto
                            {
                                FirstName = combined.customer.FirstName,
                                LastName = combined.customer.LastName,
                                PhoneNumber = combined.customer.PhoneNumber,
                                CountyAr = county.NameAR,
                                CityAr = combined.city.NameAR,
                                CountyEn = county.NameEN,
                                CityEn = combined.city.NameEN,
                                TotalPurchasesAmount = combined.customer.TotalPurchasesAmount
                            })
                        .ToListAsync();


            return result.AsQueryable();
        }


    }
}
