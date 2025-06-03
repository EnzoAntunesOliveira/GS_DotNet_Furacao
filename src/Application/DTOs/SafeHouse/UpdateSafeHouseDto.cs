using System;

namespace Application.DTOs.SafeHouse
{
    public class UpdateSafeHouseDto
    {
        public Guid Id { get; set; }
        public string CEP { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
    }
}