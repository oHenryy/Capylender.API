using Microsoft.EntityFrameworkCore;
using Capylender.API.Data;
using Capylender.API.Models;

namespace Capylender.API.Repositories;

public interface IServicoRepository : IGenericRepository<Servico>
{
    Task<IEnumerable<Servico>> GetServicosByProfissionalAsync(int profissionalId);
    Task<IEnumerable<Servico>> GetServicosAtivosAsync();
}

public class ServicoRepository : GenericRepository<Servico>, IServicoRepository
{
    public ServicoRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<Servico?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(s => s.Profissional)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public override async Task<IEnumerable<Servico>> GetAllAsync()
    {
        return await _dbSet
            .Include(s => s.Profissional)
            .ToListAsync();
    }

    public async Task<IEnumerable<Servico>> GetServicosByProfissionalAsync(int profissionalId)
    {
        return await _dbSet
            .Include(s => s.Profissional)
            .Where(s => s.ProfissionalId == profissionalId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Servico>> GetServicosAtivosAsync()
    {
        return await _dbSet
            .Include(s => s.Profissional)
            .Where(s => s.Ativo)
            .ToListAsync();
    }
} 