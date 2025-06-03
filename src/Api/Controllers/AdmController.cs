using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.DTOs;
using Application.DTOs.ADM;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdmController : ControllerBase
    {
        private readonly IAdmService _service;

        public AdmController(IAdmService service)
        {
            _service = service;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdmDto>>> GetAll()
        {
            var entities = await _service.GetAllAsync();

            var dtos = entities
                .Select(e => new AdmDto
                {
                    Id = e.Id,
                    Nome = e.Nome,
                    Email = e.Email
                })
                .ToList();

            return Ok(dtos);
        }
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<AdmDto>> GetById(Guid id)
        {
            var e = await _service.GetByIdAsync(id);

            var dto = new AdmDto
            {
                Id = e.Id,
                Nome = e.Nome,
                Email = e.Email
            };

            return Ok(dto);
        }
        
        [HttpPost]
        public async Task<ActionResult<AdmDto>> Create([FromBody] CreateAdmDto createDto)
        {
            var e = await _service.CreateAsync(createDto.Nome, createDto.Email, createDto.Senha);

            var dto = new AdmDto
            {
                Id = e.Id,
                Nome = e.Nome,
                Email = e.Email
            };

            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }
        
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAdmDto updateDto)
        {
            if (id != updateDto.Id)
                return BadRequest("Id da rota diferente do id no corpo.");

            await _service.UpdateAsync(updateDto.Id, updateDto.Nome, updateDto.Email, updateDto.Senha);
            return NoContent();
        }
        
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
        
        [HttpPost("authenticate")]
        public async Task<ActionResult<AdmDto>> Authenticate([FromBody] LoginDto login)
        {
            var e = await _service.AuthenticateAsync(login.Email, login.Senha);

            var dto = new AdmDto
            {
                Id = e.Id,
                Nome = e.Nome,
                Email = e.Email
            };

            return Ok(dto);
        }
    }
}
