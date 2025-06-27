using ExpenseTracker.API.Data;
using ExpenseTracker.API.Models;
using ExpenseTracker.API.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.API.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ExpenseTrackerContext _context;

        public RefreshTokenRepository(ExpenseTrackerContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);
        }
        
        public async Task<RefreshToken> AddAsync(RefreshToken entity)
        {
            await _context.RefreshTokens.AddAsync(entity);
            return entity;
        }

        public RefreshToken Update(RefreshToken entity)
        {
            _context.RefreshTokens.Update(entity);
            return entity;
        }
        
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}