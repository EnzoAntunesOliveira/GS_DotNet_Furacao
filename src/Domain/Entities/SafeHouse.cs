namespace Domain.Entities
{
    using System;
    using Domain.Exceptions;

    public class SafeHouse
    {
        public Guid Id { get; private set; }
        public string CEP { get; private set; }
        public string Numero { get; private set; }
        public string Complemento { get; private set; }
        
        protected SafeHouse() { }
        
        public SafeHouse(string cep, string numero, string complemento)
        {
            Id = Guid.NewGuid();
            SetCEP(cep);
            SetNumero(numero);
            SetComplemento(complemento);
        }

        public void SetCEP(string cep)
        {
            if (string.IsNullOrWhiteSpace(cep))
                throw new DomainException("CEP é obrigatório.");
            CEP = cep.Trim();
        }

        public void SetNumero(string numero)
        {
            if (string.IsNullOrWhiteSpace(numero))
                throw new DomainException("Número é obrigatório.");

            Numero = numero.Trim();
        }

        public void SetComplemento(string complemento)
        {
            Complemento = complemento?.Trim() ?? string.Empty;
        }
    }
}