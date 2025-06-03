namespace Domain.Entities
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using Domain.Exceptions;

    public class Adm
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public string Email { get; private set; }
        public string SenhaHash { get; private set; }
        
        protected Adm() { }
        
        public Adm(string nome, string email, string senha)
        {
            Id = Guid.NewGuid();
            SetNome(nome);
            SetEmail(email);
            SetSenha(senha);
        }

        public void SetNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new DomainException("Nome é obrigatório.");

            Nome = nome.Trim();
        }

        public void SetEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new DomainException("Email é obrigatório.");

            Email = email.Trim().ToLower();
        }

        public void SetSenha(string senha)
        {
            if (string.IsNullOrWhiteSpace(senha) || senha.Length < 6)
                throw new DomainException("Senha deve ter ao menos 6 caracteres.");

            SenhaHash = HashPassword(senha);
        }

        private string HashPassword(string senha)
        {
            using var sha256 = SHA256.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(senha);
            byte[] hash = sha256.ComputeHash(bytes);
            return Convert.ToHexString(hash);
        }

        public bool VerificarSenha(string senhaParaTestar)
        {
            if (string.IsNullOrEmpty(senhaParaTestar))
                return false;

            return SenhaHash == HashPassword(senhaParaTestar);
        }
    }
}