using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class SafeHouseService : ISafeHouseService
    {
        private readonly ISafeHouseRepository _repo;

        public SafeHouseService(ISafeHouseRepository repo)
        {
            _repo = repo;
        }

        public async Task<SafeHouse> CreateAsync(string cep, string numero, string complemento)
        {
            var entity = new SafeHouse(cep, numero, complemento);
            await _repo.AddAsync(entity);
            return entity;
        }

        public async Task DeleteAsync(Guid id)
        {
            var exists = await _repo.ExistsAsync(id);
            if (!exists)
                throw new ServiceException("SafeHouse não encontrado.");

            await _repo.DeleteAsync(id);
        }

        public async Task<IEnumerable<SafeHouse>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<SafeHouse> GetByIdAsync(Guid id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                throw new ServiceException("SafeHouse não encontrado.");

            return entity;
        }

        public async Task UpdateAsync(Guid id, string cep, string numero, string complemento)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                throw new ServiceException("SafeHouse não encontrado.");

            entity.SetCEP(cep);
            entity.SetNumero(numero);
            entity.SetComplemento(complemento);

            await _repo.UpdateAsync(entity);
        }
    }
}