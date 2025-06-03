using System;

namespace Application.DTOs.ADM
{
    public class UpdateAdmDto
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
    }
}