namespace Capylender.API.Services;

public interface IGenericService<TEntity, TDto, TCreateUpdateDto> where TEntity : class
{
    Task<IEnumerable<TDto>> GetAllAsync();
    Task<TDto?> GetByIdAsync(int id);
    Task<TDto> CreateAsync(TCreateUpdateDto dto);
    Task UpdateAsync(int id, TCreateUpdateDto dto);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
} 