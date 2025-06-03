using System;

namespace Application.DTOs.SafeHouse
{
    public class SafeHouseDto
    {
        public Guid Id { get; set; }
        public string CEP { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
    }
}