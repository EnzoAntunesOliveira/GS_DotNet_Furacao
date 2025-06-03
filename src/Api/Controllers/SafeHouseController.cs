using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.SafeHouse;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SafeHouseController : ControllerBase
    {
        private readonly ISafeHouseService _service;

        public SafeHouseController(ISafeHouseService service)
        {
            _service = service;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SafeHouseDto>>> GetAll()
        {
            var entities = await _service.GetAllAsync();

            var dtos = entities
                .Select(e => new SafeHouseDto
                {
                    Id = e.Id,
                    CEP = e.CEP,
                    Numero = e.Numero,
                    Complemento = e.Complemento
                })
                .ToList();

            return Ok(dtos);
        }
        
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<SafeHouseDto>> GetById(Guid id)
        {
            var e = await _service.GetByIdAsync(id);

            var dto = new SafeHouseDto
            {
                Id = e.Id,
                CEP = e.CEP,
                Numero = e.Numero,
                Complemento = e.Complemento
            };

            return Ok(dto);
        }
        
        [HttpPost]
        public async Task<ActionResult<SafeHouseDto>> Create([FromBody] CreateSafeHouseDto createDto)
        {
            var e = await _service.CreateAsync(createDto.CEP, createDto.Numero, createDto.Complemento);

            var dto = new SafeHouseDto
            {
                Id = e.Id,
                CEP = e.CEP,
                Numero = e.Numero,
                Complemento = e.Complemento
            };

            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }
        
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSafeHouseDto updateDto)
        {
            if (id != updateDto.Id)
                return BadRequest("Id da rota diferente do id no corpo.");

            await _service.UpdateAsync(updateDto.Id, updateDto.CEP, updateDto.Numero, updateDto.Complemento);
            return NoContent();
        }
        
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
