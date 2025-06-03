using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface ISafeHouseService
    {
        Task<SafeHouse> GetByIdAsync(Guid id);
        Task<IEnumerable<SafeHouse>> GetAllAsync();
        Task<SafeHouse> CreateAsync(string cep, string numero, string complemento);
        Task UpdateAsync(Guid id, string cep, string numero, string complemento);
        Task DeleteAsync(Guid id);
    }
}