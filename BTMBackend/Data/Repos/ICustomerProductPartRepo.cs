using BTMBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace BTMBackend.Data.Repos
{
    public interface ICustomerProductPartRepo
    {
        public Task<bool> Create(CustomerProductPart productPart);
        public Task<bool> CreateRange(CustomerProductPart[] productPart);
        public Task<bool> Update(CustomerProductPart productPart);
        public Task<bool> Delete(CustomerProductPart productPart);
        public Task<CustomerProductPart?> GetById (int id);
        public Task<CustomerProductPart?> GetByOrderId (int id);
    }
    public class CustomerProductPartRepo(DataContext context) : ICustomerProductPartRepo
    {
        private readonly DataContext _context = context;

        public async Task<bool> Create(CustomerProductPart productPart)
        {
            await _context.CustomerProductParts.AddAsync(productPart);
            var result = await SaveChanges();
            return result;
        }

        public async Task<bool> CreateRange(CustomerProductPart[] productPart)
        {
            await _context.CustomerProductParts.AddRangeAsync(productPart);
            var result = await SaveChanges();
            return result;
        }

        public async Task<bool> Delete(CustomerProductPart productPart)
        {
            _context.CustomerProductParts.Remove(productPart);
            var result = await SaveChanges();
            return result;
        }

        public async Task<CustomerProductPart?> GetById(int id)
        {
            return await _context.CustomerProductParts.FirstOrDefaultAsync(x=>x.Id == id);
        }

        public async Task<CustomerProductPart?> GetByOrderId(int id)
        {
            return await _context.CustomerProductParts.FirstOrDefaultAsync(x => x.OrderId == id);
        }

        public async Task<bool> Update(CustomerProductPart productPart)
        {
            _context.CustomerProductParts.Update(productPart);
            var result = await SaveChanges();
            return result;
        }

        private async Task<bool> SaveChanges()
        {
            try
            {
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
    }
}
