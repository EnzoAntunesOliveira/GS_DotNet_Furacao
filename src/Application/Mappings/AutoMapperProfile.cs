using AutoMapper;
using Application.DTOs.ADM;
using Application.DTOs.SafeHouse;
using Application.DTOs.Usuario;

using Domain.Entities;

namespace Application.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
           
            CreateMap<Adm, AdmDto>();
            CreateMap<CreateAdmDto, Adm>();
            CreateMap<UpdateAdmDto, Adm>();
            
            CreateMap<SafeHouse, SafeHouseDto>();
            CreateMap<CreateSafeHouseDto, SafeHouse>();
            CreateMap<UpdateSafeHouseDto, SafeHouse>();
            
            CreateMap<Usuario, UsuarioDto>();
            CreateMap<CreateUsuarioDto, Usuario>();
            CreateMap<UpdateUsuarioDto, Usuario>();
        }
    }
}