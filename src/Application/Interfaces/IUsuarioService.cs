using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IUsuarioService
    {
        Task<Usuario> GetByIdAsync(Guid id);
        Task<IEnumerable<Usuario>> GetAllAsync();
        Task<Usuario> CreateAsync(string nome, string email, string senha);
        Task UpdateAsync(Guid id, string nome, string email, string senha);
        Task DeleteAsync(Guid id);
        Task<Usuario> AuthenticateAsync(string email, string senha);
    }
}