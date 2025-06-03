using System;
using Domain.Entities;
using Xunit;

namespace Domain.Tests
{
    public class UsuarioTests
    {
        [Fact]
        public void CriarUsuario_DeveGerarIdEHASHSenha()
        {
            // Arrange
            var nome = "Usuário Teste";
            var email = "user@teste.com";
            var senha = "senhaUser";

            // Act
            var usuario = new Usuario(nome, email, senha);

            // Assert
            Assert.NotEqual(Guid.Empty, usuario.Id);
            Assert.Equal(nome, usuario.Nome);
            Assert.Equal(email.ToLower(), usuario.Email);
            Assert.NotNull(usuario.SenhaHash);
            Assert.True(usuario.VerificarSenha(senha));
            Assert.False(usuario.VerificarSenha("outraSenha"));
        }
    }
}