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
    public class UsuarioServiceTests
    {
        private readonly Mock<IUsuarioRepository> _repoMock;
        private readonly UsuarioService _usuarioService;

        public UsuarioServiceTests()
        {
            _repoMock = new Mock<IUsuarioRepository>();
            _usuarioService = new UsuarioService(_repoMock.Object);
        }

        [Fact]
        public async Task CreateAsync_EmailNovo_DeveRetornarUsuario()
        {
            // Arrange
            _repoMock
                .Setup(r => r.GetByEmailAsync("novo@ex.com"))
                .ReturnsAsync((Usuario?)null);
            _repoMock
                .Setup(r => r.AddAsync(It.IsAny<Usuario>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _usuarioService.CreateAsync("Usu Teste", "novo@ex.com", "senha123");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("novo@ex.com", result.Email);
            Assert.Equal("Usu Teste", result.Nome);
            Assert.True(result.VerificarSenha("senha123"));
            _repoMock.Verify(r => r.AddAsync(It.IsAny<Usuario>()), Times.Once);
        }

        [Fact]
        public async Task AuthenticateAsync_EmailInvalido_DeveLancarServiceException()
        {
            // Arrange
            _repoMock
                .Setup(r => r.GetByEmailAsync("nao@ex.com"))
                .ReturnsAsync((Usuario?)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ServiceException>(() =>
                _usuarioService.AuthenticateAsync("nao@ex.com", "senha"));
            Assert.Equal("Email ou senha inválidos.", ex.Message);
        }

        [Fact]
        public async Task AuthenticateAsync_SenhaIncorreta_DeveLancarServiceException()
        {
            // Arrange
            var userEntity = new Usuario("Usu X", "ux@ex.com", "senhaCorreta");
            _repoMock
                .Setup(r => r.GetByEmailAsync("ux@ex.com"))
                .ReturnsAsync(userEntity);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ServiceException>(() =>
                _usuarioService.AuthenticateAsync("ux@ex.com", "senhaErrada"));
            Assert.Equal("Email ou senha inválidos.", ex.Message);
        }

        [Fact]
        public async Task AuthenticateAsync_CredenciaisValidas_DeveRetornarUsuario()
        {
            // Arrange
            var userEntity = new Usuario("Usu OK", "ok@ex.com", "senha123");
            _repoMock
                .Setup(r => r.GetByEmailAsync("ok@ex.com"))
                .ReturnsAsync(userEntity);

            // Act
            var result = await _usuarioService.AuthenticateAsync("ok@ex.com", "senha123");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("ok@ex.com", result.Email);
        }

        [Fact]
        public async Task GetByIdAsync_IdInvalido_DeveLancarServiceException()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repoMock
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync((Usuario?)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ServiceException>(() =>
                _usuarioService.GetByIdAsync(id));
            Assert.Equal("Usuário não encontrado.", ex.Message);
        }

        [Fact]
        public async Task UpdateAsync_IdInvalido_DeveLancarServiceException()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repoMock
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync((Usuario?)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ServiceException>(() =>
                _usuarioService.UpdateAsync(id, "Novo", "novo@ex.com", "senha"));
            Assert.Equal("Usuário não encontrado.", ex.Message);
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
                _usuarioService.DeleteAsync(id));
            Assert.Equal("Usuário não encontrado.", ex.Message);
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
            await _usuarioService.DeleteAsync(id);

            // Assert
            _repoMock.Verify(r => r.DeleteAsync(id), Times.Once);
        }
    }
}
