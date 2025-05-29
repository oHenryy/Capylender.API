using AutoMapper;
using Capylender.API.Repositories;

namespace Capylender.API.Services;

public class GenericService<TEntity, TDto, TCreateUpdateDto> : IGenericService<TEntity, TDto, TCreateUpdateDto>
    where TEntity : class
{
    protected readonly IGenericRepository<TEntity> _repository;
    protected readonly IMapper _mapper;

    public GenericService(IGenericRepository<TEntity> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public virtual async Task<IEnumerable<TDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<TDto>>(entities);
    }

    public virtual async Task<TDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity != null ? _mapper.Map<TDto>(entity) : default;
    }

    public virtual async Task<TDto> CreateAsync(TCreateUpdateDto dto)
    {
        var entity = _mapper.Map<TEntity>(dto);
        var createdEntity = await _repository.AddAsync(entity);
        return _mapper.Map<TDto>(createdEntity);
    }

    public virtual async Task UpdateAsync(int id, TCreateUpdateDto dto)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
            throw new KeyNotFoundException($"Entity with id {id} not found");

        _mapper.Map(dto, entity);
        await _repository.UpdateAsync(entity);
    }

    public virtual async Task DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
            throw new KeyNotFoundException($"Entity with id {id} not found");

        await _repository.DeleteAsync(entity);
    }

    public virtual async Task<bool> ExistsAsync(int id)
    {
        return await _repository.ExistsAsync(id);
    }
} 