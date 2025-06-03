namespace Domain.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.Entities;

    public interface IAdmRepository
    {
        Task<Adm> GetByIdAsync(Guid id);
        Task<IEnumerable<Adm>> GetAllAsync();
        Task AddAsync(Adm adm);
        Task UpdateAsync(Adm adm);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<Adm> GetByEmailAsync(string email);
    }
}