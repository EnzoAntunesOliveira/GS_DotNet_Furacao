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
    public class SafeHouseServiceTests
    {
        private readonly Mock<ISafeHouseRepository> _repoMock;
        private readonly SafeHouseService _service;

        public SafeHouseServiceTests()
        {
            _repoMock = new Mock<ISafeHouseRepository>();
            _service = new SafeHouseService(_repoMock.Object);
        }

        [Fact]
        public async Task CreateAsync_DeveRetornarNovaSafeHouse()
        {
            // Arrange
            _repoMock
                .Setup(r => r.AddAsync(It.IsAny<SafeHouse>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateAsync("01001-000", "123", "Ap 1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("01001-000", result.CEP);
            Assert.Equal("123", result.Numero);
            Assert.Equal("Ap 1", result.Complemento);
            _repoMock.Verify(r => r.AddAsync(It.IsAny<SafeHouse>()), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_IdInvalido_DeveLancarServiceException()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repoMock
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync((SafeHouse?)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ServiceException>(() =>
                _service.GetByIdAsync(id));
            Assert.Equal("SafeHouse não encontrado.", ex.Message);
        }

        [Fact]
        public async Task GetByIdAsync_IdValido_DeveRetornarSafeHouse()
        {
            // Arrange
            var id = Guid.NewGuid();
            var entity = new SafeHouse("01001-000", "123", "Ap 1");
            typeof(SafeHouse).GetProperty("Id")!.SetValue(entity, id);

            _repoMock
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync(entity);

            // Act
            var result = await _service.GetByIdAsync(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("01001-000", result.CEP);
        }

        [Fact]
        public async Task GetAllAsync_DeveRetornarLista()
        {
            // Arrange
            var list = new List<SafeHouse>
            {
                new SafeHouse("01001-000", "1", "Comp A"),
                new SafeHouse("02002-111", "2", "Comp B")
            };
            _repoMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(list);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.Collection(result,
                s => Assert.Equal("01001-000", s.CEP),
                s => Assert.Equal("02002-111", s.CEP));
        }

        [Fact]
        public async Task UpdateAsync_IdInvalido_DeveLancarServiceException()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repoMock
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync((SafeHouse?)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ServiceException>(() =>
                _service.UpdateAsync(id, "00000-000", "99", "Novo"));
            Assert.Equal("SafeHouse não encontrado.", ex.Message);
        }

        [Fact]
        public async Task UpdateAsync_IdValido_DeveChamarUpdateNoRepo()
        {
            // Arrange
            var id = Guid.NewGuid();
            var entity = new SafeHouse("01001-000", "1", "Comp A");
            typeof(SafeHouse).GetProperty("Id")!.SetValue(entity, id);

            _repoMock
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync(entity);
            _repoMock
                .Setup(r => r.UpdateAsync(entity))
                .Returns(Task.CompletedTask);

            // Act
            await _service.UpdateAsync(id, "99999-999", "42", "Novo Comp");

            // Assert
            _repoMock.Verify(r => r.UpdateAsync(It.Is<SafeHouse>(s =>
                s.Id == id &&
                s.CEP == "99999-999" &&
                s.Numero == "42" &&
                s.Complemento == "Novo Comp"
            )), Times.Once);
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
                _service.DeleteAsync(id));
            Assert.Equal("SafeHouse não encontrado.", ex.Message);
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
            await _service.DeleteAsync(id);

            // Assert
            _repoMock.Verify(r => r.DeleteAsync(id), Times.Once);
        }
    }
}
