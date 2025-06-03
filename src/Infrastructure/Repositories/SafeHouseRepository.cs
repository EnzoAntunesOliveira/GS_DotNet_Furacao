using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class SafeHouseRepository : ISafeHouseRepository
    {
        private readonly GlobalSolutionDbContext _context;

        public SafeHouseRepository(GlobalSolutionDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(SafeHouse safeHouse)
        {
            await _context.SafeHouses.AddAsync(safeHouse);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.SafeHouses.FindAsync(id);
            if (entity != null)
            {
                _context.SafeHouses.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.SafeHouses.AnyAsync(sh => sh.Id == id);
        }

        public async Task<IEnumerable<SafeHouse>> GetAllAsync()
        {
            return await _context.SafeHouses.ToListAsync();
        }

        public async Task<SafeHouse> GetByIdAsync(Guid id)
        {
            return await _context.SafeHouses.FirstOrDefaultAsync(sh => sh.Id == id);
        }

        public async Task UpdateAsync(SafeHouse safeHouse)
        {
            _context.SafeHouses.Update(safeHouse);
            await _context.SaveChangesAsync();
        }
    }
}