using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Exceptions;
using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Moq;
using Xunit;

namespace Application.Tests
{
    public class AdmServiceTests
    {
        private readonly Mock<IAdmRepository> _repoMock;
        private readonly AdmService _admService;

        public AdmServiceTests()
        {
            _repoMock = new Mock<IAdmRepository>();
            _admService = new AdmService(_repoMock.Object);
        }

        [Fact]
        public async Task CreateAsync_QuandoEmailExistente_DeveLancarServiceException()
        {
            // Arrange
            var existingAdm = new Adm("Admin Existente", "admin@ex.com", "senha123");
            _repoMock
                .Setup(r => r.GetByEmailAsync("admin@ex.com"))
                .ReturnsAsync(existingAdm);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ServiceException>(() =>
                _admService.CreateAsync("Novo Admin", "admin@ex.com", "novaSenha"));
            Assert.Equal("Já existe um ADM com esse e-mail.", exception.Message);
        }

        [Fact]
        public async Task CreateAsync_QuandoEmailNovo_DeveRetornarNovaEntidadeAdm()
        {
            // Arrange
            _repoMock
                .Setup(r => r.GetByEmailAsync("novo@ex.com"))
                .ReturnsAsync((Adm?)null);
            _repoMock
                .Setup(r => r.AddAsync(It.IsAny<Adm>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _admService.CreateAsync("Novo Admin", "novo@ex.com", "senha123");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Novo Admin", result.Nome);
            Assert.Equal("novo@ex.com", result.Email);
            Assert.True(result.VerificarSenha("senha123"));
            _repoMock.Verify(r => r.AddAsync(It.IsAny<Adm>()), Times.Once);
        }

        [Fact]
        public async Task AuthenticateAsync_EmailInexistente_DeveLancarServiceException()
        {
            // Arrange
            _repoMock
                .Setup(r => r.GetByEmailAsync("naoexiste@ex.com"))
                .ReturnsAsync((Adm?)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ServiceException>(() =>
                _admService.AuthenticateAsync("naoexiste@ex.com", "qualquer"));
            Assert.Equal("Email ou senha inválidos.", ex.Message);
        }

        [Fact]
        public async Task AuthenticateAsync_SenhaIncorreta_DeveLancarServiceException()
        {
            // Arrange
            var admEntity = new Adm("Admin Teste", "adm@teste.com", "senhaCorreta");
            _repoMock
                .Setup(r => r.GetByEmailAsync("adm@teste.com"))
                .ReturnsAsync(admEntity);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ServiceException>(() =>
                _admService.AuthenticateAsync("adm@teste.com", "senhaErrada"));
            Assert.Equal("Email ou senha inválidos.", ex.Message);
        }

        [Fact]
        public async Task AuthenticateAsync_CredenciaisValidas_DeveRetornarAdm()
        {
            // Arrange
            var admEntity = new Adm("Admin OK", "admok@teste.com", "senha123");
            _repoMock
                .Setup(r => r.GetByEmailAsync("admok@teste.com"))
                .ReturnsAsync(admEntity);

            // Act
            var result = await _admService.AuthenticateAsync("admok@teste.com", "senha123");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("admok@teste.com", result.Email);
            Assert.Equal("Admin OK", result.Nome);
        }

        [Fact]
        public async Task GetByIdAsync_IdInvalido_DeveLancarServiceException()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repoMock
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync((Adm?)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ServiceException>(() =>
                _admService.GetByIdAsync(id));
            Assert.Equal("ADM não encontrado.", ex.Message);
        }

        [Fact]
        public async Task UpdateAsync_IdInvalido_DeveLancarServiceException()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repoMock
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync((Adm?)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ServiceException>(() =>
                _admService.UpdateAsync(id, "Novo", "novo@ex.com", "senha"));
            Assert.Equal("ADM não encontrado.", ex.Message);
        }

        [Fact]
        public async Task DeleteAsync_IdInvalido_DeveLancarServiceException()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repoMock
                .Setup(r => r.ExistsAsync(id))
                .ReturnsAsync(false);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ServiceException>(() =>
                _admService.DeleteAsync(id));
            Assert.Equal("ADM não encontrado.", ex.Message);
        }

        [Fact]
        public async Task DeleteAsync_IdValido_DeveChamarDeleteNoRepo()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repoMock
                .Setup(r => r.ExistsAsync(id))
                .ReturnsAsync(true);
            _repoMock
                .Setup(r => r.DeleteAsync(id))
                .Returns(Task.CompletedTask);

            // Act
            await _admService.DeleteAsync(id);

            // Assert
            _repoMock.Verify(r => r.DeleteAsync(id), Times.Once);
        }
    }
}
