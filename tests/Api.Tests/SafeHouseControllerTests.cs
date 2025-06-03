using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Controllers;
using Application.DTOs.SafeHouse;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Api.Tests
{
    public class SafeHouseControllerTests
    {
        private readonly Mock<ISafeHouseService> _serviceMock;
        private readonly SafeHouseController _controller;

        public SafeHouseControllerTests()
        {
            _serviceMock = new Mock<ISafeHouseService>();
            _controller = new SafeHouseController(_serviceMock.Object);
        }

        [Fact]
        public async Task GetAll_QuandoExistiremSafeHouses_DeveRetornarOkComListaDeSafeHouseDto()
        {
            // Arrange
            var listaEntidades = new List<SafeHouse>
            {
                new SafeHouse("01001-000", "123", "Apto 1"),
                new SafeHouse("02002-111", "456", "Casa 2")
            };
            _serviceMock
                .Setup(s => s.GetAllAsync())
                .ReturnsAsync(listaEntidades);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<SafeHouseDto>>(okResult.Value);
            dtos.Count().Should().Be(2);
            dtos.Should().Contain(s => s.CEP == "01001-000" && s.Numero == "123");
            dtos.Should().Contain(s => s.CEP == "02002-111" && s.Numero == "456");
        }

        [Fact]
        public async Task GetById_ComIdValido_DeveRetornarOkComSafeHouseDto()
        {
            // Arrange
            var id = Guid.NewGuid();
            var entidade = new SafeHouse("03003-333", "789", "Casa X");
            typeof(SafeHouse).GetProperty("Id")!.SetValue(entidade, id);

            _serviceMock
                .Setup(s => s.GetByIdAsync(id))
                .ReturnsAsync(entidade);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<SafeHouseDto>(okResult.Value);
            dto.Id.Should().Be(id);
            dto.CEP.Should().Be("03003-333");
            dto.Numero.Should().Be("789");
            dto.Complemento.Should().Be("Casa X");
        }

        [Fact]
        public async Task GetById_ComIdInexistente_DevePropagarServiceException()
        {
            // Arrange
            var id = Guid.NewGuid();
            _serviceMock
                .Setup(s => s.GetByIdAsync(id))
                .ThrowsAsync(new ServiceException("SafeHouse não encontrado."));

            // Act & Assert
            await Assert.ThrowsAsync<ServiceException>(() => _controller.GetById(id));
        }

        [Fact]
        public async Task Create_ComDadosValidos_DeveRetornarCreatedAtActionComSafeHouseDto()
        {
            // Arrange
            var createDto = new CreateSafeHouseDto
            {
                CEP = "04004-444",
                Numero = "001",
                Complemento = "Test",
            };
            var entidade = new SafeHouse("04004-444", "001", "Test");
            _serviceMock
                .Setup(s => s.CreateAsync("04004-444", "001", "Test"))
                .ReturnsAsync(entidade);

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var createdAt = Assert.IsType<CreatedAtActionResult>(result.Result);
            createdAt.ActionName.Should().Be(nameof(SafeHouseController.GetById));
            var dto = Assert.IsType<SafeHouseDto>(createdAt.Value);
            dto.CEP.Should().Be("04004-444");
            dto.Numero.Should().Be("001");
        }

        [Fact]
        public async Task Create_QuandoServiceLancarServiceException_DevePropagarException()
        {
            // Arrange
            var createDto = new CreateSafeHouseDto
            {
                CEP = "00000-000",
                Numero = "000",
                Complemento = "Nenhum",
            };
            _serviceMock
                .Setup(s => s.CreateAsync("00000-000", "000", "Nenhum"))
                .ThrowsAsync(new ServiceException("Erro ao criar SafeHouse."));

            // Act & Assert
            await Assert.ThrowsAsync<ServiceException>(() => _controller.Create(createDto));
        }

        [Fact]
        public async Task Update_ComIdDiferenteDoDto_DeveRetornarBadRequest()
        {
            // Arrange
            var idRota = Guid.NewGuid();
            var updateDto = new UpdateSafeHouseDto
            {
                Id = Guid.NewGuid(),  
                CEP = "11111-111",
                Numero = "111",
                Complemento = "Errado"
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
            var updateDto = new UpdateSafeHouseDto
            {
                Id = id,
                CEP = "22222-222",
                Numero = "222",
                Complemento = "Atualizado"
            };
            _serviceMock
                .Setup(s => s.UpdateAsync(id, "22222-222", "222", "Atualizado"))
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
            var updateDto = new UpdateSafeHouseDto
            {
                Id = id,
                CEP = "33333-333",
                Numero = "333",
                Complemento = "NaoExiste"
            };
            _serviceMock
                .Setup(s => s.UpdateAsync(id, "33333-333", "333", "NaoExiste"))
                .ThrowsAsync(new ServiceException("SafeHouse não encontrado."));

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
                .ThrowsAsync(new ServiceException("SafeHouse não encontrado."));

            // Act & Assert
            await Assert.ThrowsAsync<ServiceException>(() => _controller.Delete(id));
        }
    }
}
