namespace Domain.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.Entities;

    public interface ISafeHouseRepository
    {
        Task<SafeHouse> GetByIdAsync(Guid id);
        Task<IEnumerable<SafeHouse>> GetAllAsync();
        Task AddAsync(SafeHouse safeHouse);
        Task UpdateAsync(SafeHouse safeHouse);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}