using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repo;

        public UsuarioService(IUsuarioRepository repo)
        {
            _repo = repo;
        }

        public async Task<Usuario> AuthenticateAsync(string email, string senha)
        {
            var user = await _repo.GetByEmailAsync(email.Trim().ToLower());
            if (user == null || !user.VerificarSenha(senha))
                throw new ServiceException("Email ou senha inválidos.");

            return user;
        }

        public async Task<Usuario> CreateAsync(string nome, string email, string senha)
        {
            var existing = await _repo.GetByEmailAsync(email.Trim().ToLower());
            if (existing != null)
                throw new ServiceException("Já existe um usuário com esse e-mail.");

            var entity = new Usuario(nome, email, senha);
            await _repo.AddAsync(entity);
            return entity;
        }

        public async Task DeleteAsync(Guid id)
        {
            var exists = await _repo.ExistsAsync(id);
            if (!exists)
                throw new ServiceException("Usuário não encontrado.");

            await _repo.DeleteAsync(id);
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<Usuario> GetByIdAsync(Guid id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                throw new ServiceException("Usuário não encontrado.");

            return entity;
        }

        public async Task UpdateAsync(Guid id, string nome, string email, string senha)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                throw new ServiceException("Usuário não encontrado.");

            entity.SetNome(nome);
            entity.SetEmail(email);
            entity.SetSenha(senha);

            await _repo.UpdateAsync(entity);
        }
    }
}
