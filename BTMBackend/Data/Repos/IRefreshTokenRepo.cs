using BTMBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace BTMBackend.Data.Repos
{
    public interface IRefreshTokenRepo
    {
        Task<bool> Create(RefreshToken refreshToken);
        Task<bool> Delete(RefreshToken refreshToken);
        Task<RefreshToken?> GetByToken(string refreshToken);
    }
    public class RefreshTokenRepo(DataContext context) : IRefreshTokenRepo
    {
        private readonly DataContext _context = context;

        public async Task<bool> Create(RefreshToken refreshToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken);
            return await SaveChangres();
        }

        public async Task<bool> Delete(RefreshToken refreshToken)
        {
           _context.RefreshTokens.Remove(refreshToken);
            return await SaveChangres();
        }

        public async Task<RefreshToken?> GetByToken(string refreshToken)
        {
            return await _context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == refreshToken);
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
    }
}
