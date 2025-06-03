using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IAdmService
    {
        Task<Adm> GetByIdAsync(Guid id);
        Task<IEnumerable<Adm>> GetAllAsync();
        Task<Adm> CreateAsync(string nome, string email, string senha);
        Task UpdateAsync(Guid id, string nome, string email, string senha);
        Task DeleteAsync(Guid id);
        Task<Adm> AuthenticateAsync(string email, string senha);
    }
}