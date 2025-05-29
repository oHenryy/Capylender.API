using AutoMapper;
using Capylender.API.Models;
using Capylender.API.Models.DTOs;
using Capylender.API.Repositories;

namespace Capylender.API.Services;

public interface IDisponibilidadeService : IGenericService<Disponibilidade, DisponibilidadeDto, DisponibilidadeCreateUpdateDto>
{
    Task<IEnumerable<DisponibilidadeDto>> GetDisponibilidadesByProfissionalAsync(int profissionalId);
    Task<IEnumerable<DisponibilidadeDto>> GetDisponibilidadesByPeriodoAsync(DateTime inicio, DateTime fim);
    Task<IEnumerable<DisponibilidadeDto>> GetDisponibilidadesByProfissionalAndPeriodoAsync(int profissionalId, DateTime inicio, DateTime fim);
}

public class DisponibilidadeService : GenericService<Disponibilidade, DisponibilidadeDto, DisponibilidadeCreateUpdateDto>, IDisponibilidadeService
{
    private readonly IDisponibilidadeRepository _disponibilidadeRepository;

    public DisponibilidadeService(IDisponibilidadeRepository repository, IMapper mapper) 
        : base(repository, mapper)
    {
        _disponibilidadeRepository = repository;
    }

    public async Task<IEnumerable<DisponibilidadeDto>> GetDisponibilidadesByProfissionalAsync(int profissionalId)
    {
        var disponibilidades = await _disponibilidadeRepository.GetDisponibilidadesByProfissionalAsync(profissionalId);
        return _mapper.Map<IEnumerable<DisponibilidadeDto>>(disponibilidades);
    }

    public async Task<IEnumerable<DisponibilidadeDto>> GetDisponibilidadesByPeriodoAsync(DateTime inicio, DateTime fim)
    {
        var disponibilidades = await _disponibilidadeRepository.GetDisponibilidadesByPeriodoAsync(inicio, fim);
        return _mapper.Map<IEnumerable<DisponibilidadeDto>>(disponibilidades);
    }

    public async Task<IEnumerable<DisponibilidadeDto>> GetDisponibilidadesByProfissionalAndPeriodoAsync(int profissionalId, DateTime inicio, DateTime fim)
    {
        var disponibilidades = await _disponibilidadeRepository.GetDisponibilidadesByProfissionalAndPeriodoAsync(profissionalId, inicio, fim);
        return _mapper.Map<IEnumerable<DisponibilidadeDto>>(disponibilidades);
    }
} 