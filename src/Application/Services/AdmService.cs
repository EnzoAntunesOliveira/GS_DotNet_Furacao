using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class AdmService : IAdmService
    {
        private readonly IAdmRepository _repo;

        public AdmService(IAdmRepository repo)
        {
            _repo = repo;
        }

        public async Task<Adm> AuthenticateAsync(string email, string senha)
        {
            var admin = await _repo.GetByEmailAsync(email.Trim().ToLower());
            if (admin == null || !admin.VerificarSenha(senha))
                throw new ServiceException("Email ou senha inválidos.");

            return admin;
        }

        public async Task<Adm> CreateAsync(string nome, string email, string senha)
        {
            var existing = await _repo.GetByEmailAsync(email.Trim().ToLower());
            if (existing != null)
                throw new ServiceException("Já existe um ADM com esse e-mail.");

            var entity = new Adm(nome, email, senha);
            await _repo.AddAsync(entity);
            return entity;
        }

        public async Task DeleteAsync(Guid id)
        {
            var exists = await _repo.ExistsAsync(id);
            if (!exists)
                throw new ServiceException("ADM não encontrado.");

            await _repo.DeleteAsync(id);
        }

        public async Task<IEnumerable<Adm>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<Adm> GetByIdAsync(Guid id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                throw new ServiceException("ADM não encontrado.");

            return entity;
        }

        public async Task UpdateAsync(Guid id, string nome, string email, string senha)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                throw new ServiceException("ADM não encontrado.");

            entity.SetNome(nome);
            entity.SetEmail(email);
            entity.SetSenha(senha);

            await _repo.UpdateAsync(entity);
        }
    }
}
