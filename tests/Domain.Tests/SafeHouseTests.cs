using System;
using Domain.Entities;
using Xunit;

namespace Domain.Tests
{
    public class SafeHouseTests
    {
        [Fact]
        public void CriarSafeHouse_DeveTerPropriedadesCorretas()
        {
            // Arrange
            var cep = "01001-000";
            var numero = "123";
            var complemento = "Apto 45";
            var usuarioId = Guid.NewGuid();

            // Act
            var safeHouse = new SafeHouse(cep, numero, complemento);

            // Assert
            Assert.NotEqual(Guid.Empty, safeHouse.Id);
            Assert.Equal(cep, safeHouse.CEP);
            Assert.Equal(numero, safeHouse.Numero);
            Assert.Equal(complemento, safeHouse.Complemento); 
        }

        [Fact]
        public void AtualizarEndereco_DeveAlterarPropriedades()
        {
            // Arrange
            var safeHouse = new SafeHouse("01001-000", "123", "Apto 1");
            var novoCep = "02002-111";
            var novoNumero = "456";
            var novoComplemento = "Casa B";

            // Act
            safeHouse.SetCEP(novoCep);
            safeHouse.SetNumero(novoNumero);
            safeHouse.SetComplemento(novoComplemento);

            // Assert
            Assert.Equal(novoCep, safeHouse.CEP);
            Assert.Equal(novoNumero, safeHouse.Numero);
            Assert.Equal(novoComplemento, safeHouse.Complemento);
        }
    }
}