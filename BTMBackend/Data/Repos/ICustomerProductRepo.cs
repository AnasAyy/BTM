using BTMBackend.Dtos.OrderDto;
using BTMBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace BTMBackend.Data.Repos
{
    public interface ICustomerProductRepo
    {
        public Task<bool> Create(CustomerProduct customerProduct);
        public Task<bool> Update(CustomerProduct customerProduct);
        public Task<bool> Delete(CustomerProduct customerProduct);
        public Task<List<GetCustomerPartDetailsResponseDto>> GetCustomerPartDetails (int? customerProductId);
        public Task<List<GetCustomerOrderedDeviceResponseDto>> GetCustomerProduct(int? customerId);
        public Task<CustomerProduct?> GetById (int id);
    }
    public class CustomerProductRepo(DataContext context) : ICustomerProductRepo
    {
        private readonly DataContext _context = context;

        public async Task<bool> Create(CustomerProduct customerProduct)
        {
            await _context.CustomerProducts.AddAsync(customerProduct);
            var result = await SaveChanges();
            return result;
        }

        public async Task<bool> Delete(CustomerProduct customerProduct)
        {
            _context.CustomerProducts.Remove(customerProduct);
            var result = await SaveChanges();
            return result;
        }

        public async Task<CustomerProduct?> GetById(int id)
        {
            return await _context.CustomerProducts.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<GetCustomerPartDetailsResponseDto>> GetCustomerPartDetails(int? customerProductId)
        {
            if (customerProductId == null)
                return [];

            var result = await (from cpp in _context.CustomerProductParts
                                join p in _context.Parts on cpp.PartId equals p.Id
                                where cpp.CustomerProductId == customerProductId
                                select new GetCustomerPartDetailsResponseDto
                                {
                                    Id = cpp.Id,
                                    PartNameAr = p.NameAr,
                                    PartNameEn = p.NameEn,
                                    MaintenanceExpirationDate = cpp.MaintenanceDate.ToShortDateString()
                                }).ToListAsync();

            return result;
        }

        public async Task<List<GetCustomerOrderedDeviceResponseDto>> GetCustomerProduct(int? customerId)
        {

            var result = await (from cp in _context.CustomerProducts
                                join p in _context.Products on cp.ProductId equals p.Id
                                where cp.CustomerId == customerId
                                select new GetCustomerOrderedDeviceResponseDto
                                {
                                    Id = cp.Id,
                                    DeviceNameAr = p.NameAr,
                                    DeviceNameEn = p.NameEn,
                                    WarrantyExpirationDate = cp.ExpirationDate.ToShortDateString(),
                                    WarrantyStatusAr = DateTime.Now > cp.ExpirationDate ? "خارج اطار الضمان" : "تحت الضمان",
                                    WarrantyStatusEn = DateTime.Now > cp.ExpirationDate ? "Out Of Warranty" : "Under Warranty"
                                }).ToListAsync();
            if(result.Count > 0 )
            {
                for( int i = 0;i < result.Count; i++ )
                {
                    var partWarrantyCheck = await _context.CustomerProductParts
                    .AnyAsync(x => x.MaintenanceDate < DateTime.Now
                    && x.CustomerProductId == result[i].Id);

                    if(partWarrantyCheck)
                    {
                        result[i].WarrantyStatusAr = "خارج اطار الضمان";
                        result[i].WarrantyStatusEn = "Out Of Warranty";
                    }
                }
                
            }

            return result;
        }

        public async Task<bool> Update(CustomerProduct customerProduct)
        {
            _context.CustomerProducts.Update(customerProduct);
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
