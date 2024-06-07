using BTMBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace BTMBackend.Data.Repos
{
    public interface IOTPMessageRepo
    {
        public Task Create(OTPMessage message);
        public Task<bool> CheckIfValid(string otpCode, string PhoneNumber);
        public void Update(OTPMessage message);
        public Task<bool> SaveChanges();
        public Task Delete(string otpCode, string PhoneNumber);
        public Task DeleteOld();
        
    }
    public class OTPMessageRepo(DataContext context) : IOTPMessageRepo
    {
        private readonly DataContext _context = context;

        public async Task<bool> CheckIfValid(string otpCode, string PhoneNumber)
        {
            return await _context.OTPMessages.AnyAsync(x=>x.PhoneNumber == PhoneNumber && x.Code == otpCode && x.ExpirationDatetime > DateTime.Now && x.Status == 0);
        }

        public async Task Create(OTPMessage message)
        {
            await _context.OTPMessages.AddAsync(message);
        }

        public async Task Delete(string otpCode, string phoneNumber)
        {
            var otpMessage = await _context.OTPMessages
                .Where(x => x.Code == otpCode && x.PhoneNumber == phoneNumber)
                .FirstOrDefaultAsync();

            if (otpMessage != null)
            {
                _context.OTPMessages.Remove(otpMessage);
            }
        }

        public async Task DeleteOld()
        {
            DateTime currentTime = DateTime.Now;
            int minutesToAdd = 5;
            DateTime newTime = currentTime.AddMinutes(minutesToAdd);

            var oldData = await _context.OTPMessages
                .Where(x => x.ExpirationDatetime < newTime)
                .ToListAsync();

            if (oldData.Count > 0)
            {
                _context.OTPMessages.RemoveRange(oldData);
                await _context.SaveChangesAsync();
            }
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

        public void Update(OTPMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
