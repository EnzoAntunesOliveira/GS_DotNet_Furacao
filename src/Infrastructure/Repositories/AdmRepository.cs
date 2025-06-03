using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class AdmRepository : IAdmRepository
    {
        private readonly GlobalSolutionDbContext _context;

        public AdmRepository(GlobalSolutionDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Adm adm)
        {
            await _context.Adms.AddAsync(adm);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.Adms.FindAsync(id);
            if (entity != null)
            {
                _context.Adms.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Adms.AnyAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Adm>> GetAllAsync()
        {
            return await _context.Adms.ToListAsync();
        }

        public async Task<Adm> GetByEmailAsync(string email)
        {
            return await _context.Adms.FirstOrDefaultAsync(a => a.Email == email);
        }

        public async Task<Adm> GetByIdAsync(Guid id)
        {
            return await _context.Adms.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task UpdateAsync(Adm adm)
        {
            _context.Adms.Update(adm);
            await _context.SaveChangesAsync();
        }
    }
}