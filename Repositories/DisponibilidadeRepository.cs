using Microsoft.EntityFrameworkCore;
using Capylender.API.Data;
using Capylender.API.Models;

namespace Capylender.API.Repositories;

public interface IDisponibilidadeRepository : IGenericRepository<Disponibilidade>
{
    Task<IEnumerable<Disponibilidade>> GetDisponibilidadesByProfissionalAsync(int profissionalId);
    Task<IEnumerable<Disponibilidade>> GetDisponibilidadesByPeriodoAsync(DateTime inicio, DateTime fim);
    Task<IEnumerable<Disponibilidade>> GetDisponibilidadesByProfissionalAndPeriodoAsync(int profissionalId, DateTime inicio, DateTime fim);
}

public class DisponibilidadeRepository : GenericRepository<Disponibilidade>, IDisponibilidadeRepository
{
    public DisponibilidadeRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Disponibilidade>> GetDisponibilidadesByProfissionalAsync(int profissionalId)
    {
        return await _dbSet
            .Where(d => d.ProfissionalId == profissionalId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Disponibilidade>> GetDisponibilidadesByPeriodoAsync(DateTime inicio, DateTime fim)
    {
        return await _dbSet
            .Where(d => d.DataInicio >= inicio && d.DataFim <= fim)
            .ToListAsync();
    }

    public async Task<IEnumerable<Disponibilidade>> GetDisponibilidadesByProfissionalAndPeriodoAsync(int profissionalId, DateTime inicio, DateTime fim)
    {
        return await _dbSet
            .Where(d => d.ProfissionalId == profissionalId && d.DataInicio >= inicio && d.DataFim <= fim)
            .ToListAsync();
    }
} 