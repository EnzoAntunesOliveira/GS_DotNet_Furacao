using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Controllers;
using Api.DTOs;
using Application.DTOs.ADM;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Api.Tests
{
    public class AdmControllerTests
    {
        private readonly Mock<IAdmService> _serviceMock;
        private readonly AdmController _controller;

        public AdmControllerTests()
        {
            _serviceMock = new Mock<IAdmService>();
            _controller = new AdmController(_serviceMock.Object);
        }

        [Fact]
        public async Task GetAll_QuandoExistiremAdms_DeveRetornarOkComListaDeAdmDto()
        {
            // Arrange
            var listaEntidades = new List<Adm>
            {
                new Adm("Admin A", "a@ex.com", "senhaA"),
                new Adm("Admin B", "b@ex.com", "senhaB")
            };
            _serviceMock
                .Setup(s => s.GetAllAsync())
                .ReturnsAsync(listaEntidades);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<AdmDto>>(okResult.Value);
            dtos.Count().Should().Be(2);
            dtos.Should().Contain(a => a.Email == "a@ex.com" && a.Nome == "Admin A");
            dtos.Should().Contain(a => a.Email == "b@ex.com" && a.Nome == "Admin B");
        }

        [Fact]
        public async Task GetById_ComIdValido_DeveRetornarOkComAdmDto()
        {
            // Arrange
            var id = Guid.NewGuid();
            var entidade = new Adm("Admin X", "x@ex.com", "senhaX");
            typeof(Adm).GetProperty("Id")!.SetValue(entidade, id);

            _serviceMock
                .Setup(s => s.GetByIdAsync(id))
                .ReturnsAsync(entidade);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<AdmDto>(okResult.Value);
            dto.Id.Should().Be(id);
            dto.Email.Should().Be("x@ex.com");
            dto.Nome.Should().Be("Admin X");
        }

        [Fact]
        public async Task GetById_ComIdInexistente_DevePropagarServiceException()
        {
            // Arrange
            var id = Guid.NewGuid();
            _serviceMock
                .Setup(s => s.GetByIdAsync(id))
                .ThrowsAsync(new ServiceException("ADM não encontrado."));

            // Act & Assert
            await Assert.ThrowsAsync<ServiceException>(() => _controller.GetById(id));
        }

        [Fact]
        public async Task Create_ComDadosValidos_DeveRetornarCreatedAtActionComAdmDto()
        {
            // Arrange
            var createDto = new CreateAdmDto
            {
                Nome = "NovoAdmin",
                Email = "novo@ex.com",
                Senha = "senha123"
            };
            var entidade = new Adm("NovoAdmin", "novo@ex.com", "senha123");
            _serviceMock
                .Setup(s => s.CreateAsync("NovoAdmin", "novo@ex.com", "senha123"))
                .ReturnsAsync(entidade);

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var createdAt = Assert.IsType<CreatedAtActionResult>(result.Result);
            createdAt.ActionName.Should().Be(nameof(AdmController.GetById));
            var dto = Assert.IsType<AdmDto>(createdAt.Value);
            dto.Email.Should().Be("novo@ex.com");
            dto.Nome.Should().Be("NovoAdmin");
        }

        [Fact]
        public async Task Create_QuandoServiceLancarServiceException_DevePropagarException()
        {
            // Arrange
            var createDto = new CreateAdmDto
            {
                Nome = "DupAdmin",
                Email = "dup@ex.com",
                Senha = "senha"
            };
            _serviceMock
                .Setup(s => s.CreateAsync("DupAdmin", "dup@ex.com", "senha"))
                .ThrowsAsync(new ServiceException("Já existe um ADM com esse e-mail."));

            // Act & Assert
            await Assert.ThrowsAsync<ServiceException>(() => _controller.Create(createDto));
        }

        [Fact]
        public async Task Update_ComIdDiferenteDoDto_DeveRetornarBadRequest()
        {
            // Arrange
            var idRota = Guid.NewGuid();
            var updateDto = new UpdateAdmDto
            {
                Id = Guid.NewGuid(), 
                Nome = "Qualquer",
                Email = "qualquer@ex.com",
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
            var updateDto = new UpdateAdmDto
            {
                Id = id,
                Nome = "AdminAtualizado",
                Email = "upd@ex.com",
                Senha = "novaSenha"
            };
            _serviceMock
                .Setup(s => s.UpdateAsync(id, "AdminAtualizado", "upd@ex.com", "novaSenha"))
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
            var updateDto = new UpdateAdmDto
            {
                Id = id,
                Nome = "NaoExiste",
                Email = "no@ex.com",
                Senha = "senha"
            };
            _serviceMock
                .Setup(s => s.UpdateAsync(id, "NaoExiste", "no@ex.com", "senha"))
                .ThrowsAsync(new ServiceException("ADM não encontrado."));

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
                .ThrowsAsync(new ServiceException("ADM não encontrado."));

            // Act & Assert
            await Assert.ThrowsAsync<ServiceException>(() => _controller.Delete(id));
        }

        [Fact]
        public async Task Authenticate_ComCredenciaisValidas_DeveRetornarOkComAdmDto()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "auth@ex.com",
                Senha = "senha123"
            };
            var entidade = new Adm("AuthAdmin", "auth@ex.com", "senha123");
            _serviceMock
                .Setup(s => s.AuthenticateAsync("auth@ex.com", "senha123"))
                .ReturnsAsync(entidade);

            // Act
            var result = await _controller.Authenticate(loginDto);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<AdmDto>(ok.Value);
            dto.Email.Should().Be("auth@ex.com");
            dto.Nome.Should().Be("AuthAdmin");
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
