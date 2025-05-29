using AutoMapper;
using Capylender.API.Models;
using Capylender.API.Models.DTOs;

namespace Capylender.API.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // Cliente mappings
        CreateMap<Cliente, ClienteDto>();
        CreateMap<ClienteCreateUpdateDto, Cliente>();

        // Profissional mappings
        CreateMap<Profissional, ProfissionalDto>();
        CreateMap<ProfissionalCreateUpdateDto, Profissional>();

        // Servico mappings
        CreateMap<Servico, ServicoDto>()
            .ForMember(dest => dest.ProfissionalNome, opt => opt.MapFrom(src => src.Profissional.Nome));
        CreateMap<ServicoCreateUpdateDto, Servico>();

        // Disponibilidade mappings
        CreateMap<Disponibilidade, DisponibilidadeDto>()
            .ForMember(dest => dest.ProfissionalNome, opt => opt.MapFrom(src => src.Profissional.Nome));
        CreateMap<DisponibilidadeCreateUpdateDto, Disponibilidade>();
    }
} 