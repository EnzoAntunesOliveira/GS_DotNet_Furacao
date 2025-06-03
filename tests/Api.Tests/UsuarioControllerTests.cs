using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Controllers;
using Api.DTOs;
using Application.DTOs.Usuario;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Api.Tests
{
    public class UsuarioControllerTests
    {
        private readonly Mock<IUsuarioService> _serviceMock;
        private readonly UsuarioController _controller;

        public UsuarioControllerTests()
        {
            _serviceMock = new Mock<IUsuarioService>();
            _controller = new UsuarioController(_serviceMock.Object);
        }

        [Fact]
        public async Task GetAll_QuandoExistiremUsuarios_DeveRetornarOkComListaDeUsuarioDto()
        {
            // Arrange
            var listaEntidades = new List<Usuario>
            {
                new Usuario("User A", "a@ex.com", "senhaA"),
                new Usuario("User B", "b@ex.com", "senhaB")
            };
            _serviceMock
                .Setup(s => s.GetAllAsync())
                .ReturnsAsync(listaEntidades);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<UsuarioDto>>(okResult.Value);
            dtos.Count().Should().Be(2);
            dtos.Should().Contain(u => u.Email == "a@ex.com" && u.Nome == "User A");
            dtos.Should().Contain(u => u.Email == "b@ex.com" && u.Nome == "User B");
        }

        [Fact]
        public async Task GetById_ComIdValido_DeveRetornarOkComUsuarioDto()
        {
            // Arrange
            var id = Guid.NewGuid();
            var entidade = new Usuario("UserX", "x@ex.com", "senhaX");
            typeof(Usuario).GetProperty("Id")!.SetValue(entidade, id);

            _serviceMock
                .Setup(s => s.GetByIdAsync(id))
                .ReturnsAsync(entidade);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<UsuarioDto>(okResult.Value);
            dto.Id.Should().Be(id);
            dto.Email.Should().Be("x@ex.com");
            dto.Nome.Should().Be("UserX");
        }

        [Fact]
        public async Task GetById_ComIdInexistente_DevePropagarServiceException()
        {
            // Arrange
            var id = Guid.NewGuid();
            _serviceMock
                .Setup(s => s.GetByIdAsync(id))
                .ThrowsAsync(new ServiceException("Usuário não encontrado."));

            // Act & Assert
            await Assert.ThrowsAsync<ServiceException>(() => _controller.GetById(id));
        }

        [Fact]
        public async Task Create_ComDadosValidos_DeveRetornarCreatedAtActionComUsuarioDto()
        {
            // Arrange
            var createDto = new CreateUsuarioDto
            {
                Nome = "NovoUser",
                Email = "novo@ex.com",
                Senha = "senha123"
            };
            var entidade = new Usuario("NovoUser", "novo@ex.com", "senha123");
            _serviceMock
                .Setup(s => s.CreateAsync("NovoUser", "novo@ex.com", "senha123"))
                .ReturnsAsync(entidade);

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var createdAt = Assert.IsType<CreatedAtActionResult>(result.Result);
            createdAt.ActionName.Should().Be(nameof(UsuarioController.GetById));
            var dto = Assert.IsType<UsuarioDto>(createdAt.Value);
            dto.Email.Should().Be("novo@ex.com");
            dto.Nome.Should().Be("NovoUser");
        }

        [Fact]
        public async Task Create_QuandoServiceLancarServiceException_DevePropagarException()
        {
            // Arrange
            var createDto = new CreateUsuarioDto
            {
                Nome = "Existente",
                Email = "existente@ex.com",
                Senha = "senha"
            };
            _serviceMock
                .Setup(s => s.CreateAsync("Existente", "existente@ex.com", "senha"))
                .ThrowsAsync(new ServiceException("Já existe um usuário com esse e-mail."));

            // Act & Assert
            await Assert.ThrowsAsync<ServiceException>(() => _controller.Create(createDto));
        }

        [Fact]
        public async Task Update_ComIdDiferenteDoDto_DeveRetornarBadRequest()
        {
            // Arrange
            var idRota = Guid.NewGuid();
            var updateDto = new UpdateUsuarioDto
            {
                Id = Guid.NewGuid(), 
                Nome = "Outro",
                Email = "outro@ex.com",
                Senha = "senha"
            };

            // Act
            var result = await _controller.Update(idRota, updateDto);

            // Assert
            var badReq = Assert.IsType<BadRequestObjectResult>(result);
            badReq.Value.Should().Be("Id da rota diferente do id no corpo.");
        }

        [Fact]
        public async Task Update_ComIdValido_DeveRetornarNoContent()
        {
            // Arrange
            var id = Guid.NewGuid();
            var updateDto = new UpdateUsuarioDto
            {
                Id = id,
                Nome = "UserAtualizado",
                Email = "upd@ex.com",
                Senha = "novaSenha"
            };
            _serviceMock
                .Setup(s => s.UpdateAsync(id, "UserAtualizado", "upd@ex.com", "novaSenha"))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(id, updateDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Update_QuandoServiceLancarServiceException_DevePropagarException()
        {
            // Arrange
            var id = Guid.NewGuid();
            var updateDto = new UpdateUsuarioDto
            {
                Id = id,
                Nome = "NaoEx",
                Email = "no@ex.com",
                Senha = "senha"
            };
            _serviceMock
                .Setup(s => s.UpdateAsync(id, "NaoEx", "no@ex.com", "senha"))
                .ThrowsAsync(new ServiceException("Usuário não encontrado."));

            // Act & Assert
            await Assert.ThrowsAsync<ServiceException>(() => _controller.Update(id, updateDto));
        }

        [Fact]
        public async Task Delete_ComIdValido_DeveRetornarNoContent()
        {
            // Arrange
            var id = Guid.NewGuid();
            _serviceMock
                .Setup(s => s.DeleteAsync(id))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_QuandoServiceLancarServiceException_DevePropagarException()
        {
            // Arrange
            var id = Guid.NewGuid();
            _serviceMock
                .Setup(s => s.DeleteAsync(id))
                .ThrowsAsync(new ServiceException("Usuário não encontrado."));

            // Act & Assert
            await Assert.ThrowsAsync<ServiceException>(() => _controller.Delete(id));
        }

        [Fact]
        public async Task Authenticate_ComCredenciaisValidas_DeveRetornarOkComUsuarioDto()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "auth@ex.com",
                Senha = "senha123"
            };
            var entidade = new Usuario("AuthUser", "auth@ex.com", "senha123");
            _serviceMock
                .Setup(s => s.AuthenticateAsync("auth@ex.com", "senha123"))
                .ReturnsAsync(entidade);

            // Act
            var result = await _controller.Authenticate(loginDto);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<UsuarioDto>(ok.Value);
            dto.Email.Should().Be("auth@ex.com");
            dto.Nome.Should().Be("AuthUser");
        }

        [Fact]
        public async Task Authenticate_QuandoServiceLancarServiceException_DevePropagarException()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "no@ex.com",
                Senha = "errada"
            };
            _serviceMock
                .Setup(s => s.AuthenticateAsync("no@ex.com", "errada"))
                .ThrowsAsync(new ServiceException("Email ou senha inválidos."));

            // Act & Assert
            await Assert.ThrowsAsync<ServiceException>(() => _controller.Authenticate(loginDto));
        }
    }
}
