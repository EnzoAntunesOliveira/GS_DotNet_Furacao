using System;
using Domain.Entities;
using Xunit;

namespace Domain.Tests
{
    public class AdmTests
    {
        [Fact]
        public void CriarAdm_DeveGerarIdEHASHSenha()
        {
            // Arrange
            var nome = "Admin Teste";
            var email = "admin@teste.com";
            var senha = "senha123";

            // Act
            var adm = new Adm(nome, email, senha);

            // Assert
            Assert.NotEqual(Guid.Empty, adm.Id);                   
            Assert.Equal(nome, adm.Nome);
            Assert.Equal(email.ToLower(), adm.Email);               
            Assert.NotNull(adm.SenhaHash);                           
            Assert.NotEqual(senha, adm.SenhaHash);                   
            Assert.True(adm.VerificarSenha(senha));                 
            Assert.False(adm.VerificarSenha("senhaErrada"));        
        }

        [Fact]
        public void AtualizarCampos_DeveAlterarNomeEmailSenha()
        {
            // Arrange
            var adm = new Adm("Antigo", "antigo@ex.com", "senha123");
            var novoNome = "Novo Nome";
            var novoEmail = "novo@ex.com";
            var novaSenha = "novaSenha456";

            // Act
            adm.SetNome(novoNome);
            adm.SetEmail(novoEmail);
            adm.SetSenha(novaSenha);

            // Assert
            Assert.Equal(novoNome, adm.Nome);
            Assert.Equal(novoEmail.ToLower(), adm.Email);
            Assert.True(adm.VerificarSenha(novaSenha));
        }
    }
}