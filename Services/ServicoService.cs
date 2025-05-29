using AutoMapper;
using Capylender.API.Models;
using Capylender.API.Models.DTOs;
using Capylender.API.Repositories;

namespace Capylender.API.Services;

public interface IServicoService : IGenericService<Servico, ServicoDto, ServicoCreateUpdateDto>
{
    Task<IEnumerable<ServicoDto>> GetServicosByProfissionalAsync(int profissionalId);
    Task<IEnumerable<ServicoDto>> GetServicosAtivosAsync();
}

public class ServicoService : GenericService<Servico, ServicoDto, ServicoCreateUpdateDto>, IServicoService
{
    private readonly IServicoRepository _servicoRepository;

    public ServicoService(IServicoRepository repository, IMapper mapper) 
        : base(repository, mapper)
    {
        _servicoRepository = repository;
    }

    public async Task<IEnumerable<ServicoDto>> GetServicosByProfissionalAsync(int profissionalId)
    {
        var servicos = await _servicoRepository.GetServicosByProfissionalAsync(profissionalId);
        return _mapper.Map<IEnumerable<ServicoDto>>(servicos);
    }

    public async Task<IEnumerable<ServicoDto>> GetServicosAtivosAsync()
    {
        var servicos = await _servicoRepository.GetServicosAtivosAsync();
        return _mapper.Map<IEnumerable<ServicoDto>>(servicos);
    }
} 