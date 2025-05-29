using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Capylender.API.Data;
using Capylender.API.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace Capylender.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/admin/usuarios
    [HttpGet("usuarios")]
    public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
    {
        return Ok(await _context.Usuarios.ToListAsync());
    }

    // PATCH: api/admin/usuarios/{id}/role
    [HttpPatch("usuarios/{id}/role")]
    public async Task<IActionResult> AlterarRole(int id, [FromBody] string novaRole)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null)
            return NotFound();
        usuario.Role = novaRole;
        await _context.SaveChangesAsync();
        await RegistrarLog(User.Identity?.Name ?? "admin", $"Alteração de role do usuário {usuario.Email} para {novaRole}");
        return NoContent();
    }

    // GET: api/admin/relatorios
    [HttpGet("relatorios")]
    public async Task<IActionResult> GetRelatorios()
    {
        var totalClientes = await _context.Clientes.CountAsync();
        var totalProfissionais = await _context.Profissionais.CountAsync();
        var totalServicos = await _context.Servicos.CountAsync();
        var totalAgendamentos = await _context.Agendamentos.CountAsync();
        return Ok(new
        {
            totalClientes,
            totalProfissionais,
            totalServicos,
            totalAgendamentos
        });
    }

    // PATCH: api/admin/clientes/{id}/status
    [HttpPatch("clientes/{id}/status")]
    public async Task<IActionResult> AlterarStatusCliente(int id, [FromBody] bool ativo)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente == null)
            return NotFound();
        cliente.Ativo = ativo;
        await _context.SaveChangesAsync();
        await RegistrarLog(User.Identity?.Name ?? "admin", $"Alteração de status do cliente {cliente.Email} para {(ativo ? "Ativo" : "Inativo")}");
        return NoContent();
    }

    // PATCH: api/admin/profissionais/{id}/status
    [HttpPatch("profissionais/{id}/status")]
    public async Task<IActionResult> AlterarStatusProfissional(int id, [FromBody] bool ativo)
    {
        var profissional = await _context.Profissionais.FindAsync(id);
        if (profissional == null)
            return NotFound();
        profissional.Ativo = ativo;
        await _context.SaveChangesAsync();
        await RegistrarLog(User.Identity?.Name ?? "admin", $"Alteração de status do profissional {profissional.Email} para {(ativo ? "Ativo" : "Inativo")}");
        return NoContent();
    }

    // PATCH: api/admin/servicos/{id}/status
    [HttpPatch("servicos/{id}/status")]
    public async Task<IActionResult> AlterarStatusServico(int id, [FromBody] bool ativo)
    {
        var servico = await _context.Servicos.FindAsync(id);
        if (servico == null)
            return NotFound();
        servico.Ativo = ativo;
        await _context.SaveChangesAsync();
        await RegistrarLog(User.Identity?.Name ?? "admin", $"Alteração de status do serviço {servico.Nome} para {(ativo ? "Ativo" : "Inativo")}");
        return NoContent();
    }

    // DELETE: api/admin/usuarios/{id}
    [HttpDelete("usuarios/{id}")]
    public async Task<IActionResult> RemoverUsuario(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null)
            return NotFound();
        _context.Usuarios.Remove(usuario);
        await _context.SaveChangesAsync();
        await RegistrarLog(User.Identity?.Name ?? "admin", $"Remoção do usuário {usuario.Email}");
        return NoContent();
    }

    // DELETE: api/admin/clientes/{id}
    [HttpDelete("clientes/{id}")]
    public async Task<IActionResult> RemoverCliente(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente == null)
            return NotFound();
        _context.Clientes.Remove(cliente);
        await _context.SaveChangesAsync();
        await RegistrarLog(User.Identity?.Name ?? "admin", $"Remoção do cliente {cliente.Email}");
        return NoContent();
    }

    // DELETE: api/admin/profissionais/{id}
    [HttpDelete("profissionais/{id}")]
    public async Task<IActionResult> RemoverProfissional(int id)
    {
        var profissional = await _context.Profissionais.FindAsync(id);
        if (profissional == null)
            return NotFound();
        _context.Profissionais.Remove(profissional);
        await _context.SaveChangesAsync();
        await RegistrarLog(User.Identity?.Name ?? "admin", $"Remoção do profissional {profissional.Email}");
        return NoContent();
    }

    // DELETE: api/admin/servicos/{id}
    [HttpDelete("servicos/{id}")]
    public async Task<IActionResult> RemoverServico(int id)
    {
        var servico = await _context.Servicos.FindAsync(id);
        if (servico == null)
            return NotFound();
        _context.Servicos.Remove(servico);
        await _context.SaveChangesAsync();
        await RegistrarLog(User.Identity?.Name ?? "admin", $"Remoção do serviço {servico.Nome}");
        return NoContent();
    }

    // GET: api/admin/usuarios/busca
    [HttpGet("usuarios/busca")]
    public async Task<IActionResult> BuscarUsuarios(
        [FromQuery] string? nome = null,
        [FromQuery] string? email = null,
        [FromQuery] string? role = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string orderBy = "Id",
        [FromQuery] string orderDir = "asc")
    {
        var query = _context.Usuarios.AsQueryable();
        if (!string.IsNullOrEmpty(nome))
            query = query.Where(u => u.Nome.Contains(nome));
        if (!string.IsNullOrEmpty(email))
            query = query.Where(u => u.Email.Contains(email));
        if (!string.IsNullOrEmpty(role))
            query = query.Where(u => u.Role == role);
        // Ordenação dinâmica
        query = orderBy.ToLower() switch
        {
            "nome" => orderDir == "desc" ? query.OrderByDescending(u => u.Nome) : query.OrderBy(u => u.Nome),
            "email" => orderDir == "desc" ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
            "role" => orderDir == "desc" ? query.OrderByDescending(u => u.Role) : query.OrderBy(u => u.Role),
            _ => orderDir == "desc" ? query.OrderByDescending(u => u.Id) : query.OrderBy(u => u.Id)
        };
        var total = await query.CountAsync();
        var usuarios = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return Ok(new { total, page, pageSize, data = usuarios });
    }

    // GET: api/admin/clientes/busca
    [HttpGet("clientes/busca")]
    public async Task<IActionResult> BuscarClientes(
        [FromQuery] string? nome = null,
        [FromQuery] bool? ativo = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string orderBy = "Id",
        [FromQuery] string orderDir = "asc")
    {
        var query = _context.Clientes.AsQueryable();
        if (!string.IsNullOrEmpty(nome))
            query = query.Where(c => c.Nome.Contains(nome));
        if (ativo.HasValue)
            query = query.Where(c => c.Ativo == ativo.Value);
        query = orderBy.ToLower() switch
        {
            "nome" => orderDir == "desc" ? query.OrderByDescending(c => c.Nome) : query.OrderBy(c => c.Nome),
            "email" => orderDir == "desc" ? query.OrderByDescending(c => c.Email) : query.OrderBy(c => c.Email),
            _ => orderDir == "desc" ? query.OrderByDescending(c => c.Id) : query.OrderBy(c => c.Id)
        };
        var total = await query.CountAsync();
        var clientes = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return Ok(new { total, page, pageSize, data = clientes });
    }

    // GET: api/admin/profissionais/busca
    [HttpGet("profissionais/busca")]
    public async Task<IActionResult> BuscarProfissionais(
        [FromQuery] string? nome = null,
        [FromQuery] bool? ativo = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string orderBy = "Id",
        [FromQuery] string orderDir = "asc")
    {
        var query = _context.Profissionais.AsQueryable();
        if (!string.IsNullOrEmpty(nome))
            query = query.Where(p => p.Nome.Contains(nome));
        if (ativo.HasValue)
            query = query.Where(p => p.Ativo == ativo.Value);
        query = orderBy.ToLower() switch
        {
            "nome" => orderDir == "desc" ? query.OrderByDescending(p => p.Nome) : query.OrderBy(p => p.Nome),
            "email" => orderDir == "desc" ? query.OrderByDescending(p => p.Email) : query.OrderBy(p => p.Email),
            _ => orderDir == "desc" ? query.OrderByDescending(p => p.Id) : query.OrderBy(p => p.Id)
        };
        var total = await query.CountAsync();
        var profissionais = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return Ok(new { total, page, pageSize, data = profissionais });
    }

    // GET: api/admin/servicos/busca
    [HttpGet("servicos/busca")]
    public async Task<IActionResult> BuscarServicos(
        [FromQuery] string? nome = null,
        [FromQuery] bool? ativo = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string orderBy = "Id",
        [FromQuery] string orderDir = "asc")
    {
        var query = _context.Servicos.AsQueryable();
        if (!string.IsNullOrEmpty(nome))
            query = query.Where(s => s.Nome.Contains(nome));
        if (ativo.HasValue)
            query = query.Where(s => s.Ativo == ativo.Value);
        query = orderBy.ToLower() switch
        {
            "nome" => orderDir == "desc" ? query.OrderByDescending(s => s.Nome) : query.OrderBy(s => s.Nome),
            _ => orderDir == "desc" ? query.OrderByDescending(s => s.Id) : query.OrderBy(s => s.Id)
        };
        var total = await query.CountAsync();
        var servicos = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return Ok(new { total, page, pageSize, data = servicos });
    }

    // GET: api/admin/relatorios/detalhado
    [HttpGet("relatorios/detalhado")]
    public async Task<IActionResult> GetRelatorioDetalhado()
    {
        var totalClientesAtivos = await _context.Clientes.CountAsync(c => c.Ativo);
        var totalClientesInativos = await _context.Clientes.CountAsync(c => !c.Ativo);
        var totalProfissionaisAtivos = await _context.Profissionais.CountAsync(p => p.Ativo);
        var totalProfissionaisInativos = await _context.Profissionais.CountAsync(p => !p.Ativo);
        var totalServicosAtivos = await _context.Servicos.CountAsync(s => s.Ativo);
        var totalServicosInativos = await _context.Servicos.CountAsync(s => !s.Ativo);
        var ultimosAgendamentos = await _context.Agendamentos
            .OrderByDescending(a => a.DataCriacao)
            .Take(10)
            .ToListAsync();
        var agendamentosPorDia = await _context.Agendamentos
            .GroupBy(a => a.DataHoraInicio.Date)
            .Select(g => new { Data = g.Key, Total = g.Count() })
            .OrderByDescending(g => g.Data)
            .Take(7)
            .ToListAsync();
        return Ok(new
        {
            totalClientesAtivos,
            totalClientesInativos,
            totalProfissionaisAtivos,
            totalProfissionaisInativos,
            totalServicosAtivos,
            totalServicosInativos,
            ultimosAgendamentos,
            agendamentosPorDia
        });
    }

    // GET: api/admin/usuarios/exportar
    [HttpGet("usuarios/exportar")]
    public async Task<IActionResult> ExportarUsuariosCsv()
    {
        var usuarios = await _context.Usuarios.ToListAsync();
        var csv = "Id,Nome,Email,Role\n" + string.Join("\n", usuarios.Select(u => $"{u.Id},\"{u.Nome}\",{u.Email},{u.Role}"));
        return File(System.Text.Encoding.UTF8.GetBytes(csv), "text/csv", "usuarios.csv");
    }

    // GET: api/admin/clientes/exportar
    [HttpGet("clientes/exportar")]
    public async Task<IActionResult> ExportarClientesCsv()
    {
        var clientes = await _context.Clientes.ToListAsync();
        var csv = "Id,Nome,Email,Telefone,CPF,Ativo\n" + string.Join("\n", clientes.Select(c => $"{c.Id},\"{c.Nome}\",{c.Email},{c.Telefone},{c.CPF},{c.Ativo}"));
        return File(System.Text.Encoding.UTF8.GetBytes(csv), "text/csv", "clientes.csv");
    }

    // GET: api/admin/profissionais/exportar
    [HttpGet("profissionais/exportar")]
    public async Task<IActionResult> ExportarProfissionaisCsv()
    {
        var profissionais = await _context.Profissionais.ToListAsync();
        var csv = "Id,Nome,Email,Telefone,Ativo\n" + string.Join("\n", profissionais.Select(p => $"{p.Id},\"{p.Nome}\",{p.Email},{p.Telefone},{p.Ativo}"));
        return File(System.Text.Encoding.UTF8.GetBytes(csv), "text/csv", "profissionais.csv");
    }

    // GET: api/admin/servicos/exportar
    [HttpGet("servicos/exportar")]
    public async Task<IActionResult> ExportarServicosCsv()
    {
        var servicos = await _context.Servicos.ToListAsync();
        var csv = "Id,Nome,Descricao,Preco,DuracaoMinutos,Ativo,ProfissionalId\n" + string.Join("\n", servicos.Select(s => $"{s.Id},\"{s.Nome}\",\"{s.Descricao}\",{s.Preco},{s.DuracaoMinutos},{s.Ativo},{s.ProfissionalId}"));
        return File(System.Text.Encoding.UTF8.GetBytes(csv), "text/csv", "servicos.csv");
    }

    // GET: api/admin/agendamentos/exportar
    [HttpGet("agendamentos/exportar")]
    public async Task<IActionResult> ExportarAgendamentosCsv(
        [FromQuery] DateTime? inicio = null,
        [FromQuery] DateTime? fim = null,
        [FromQuery] bool? confirmado = null,
        [FromQuery] bool? cancelado = null,
        [FromQuery] int? profissionalId = null,
        [FromQuery] int? clienteId = null)
    {
        var query = _context.Agendamentos
            .Include(a => a.Cliente)
            .Include(a => a.Profissional)
            .Include(a => a.Servico)
            .AsQueryable();
        if (inicio.HasValue)
            query = query.Where(a => a.DataHoraInicio >= inicio.Value);
        if (fim.HasValue)
            query = query.Where(a => a.DataHoraFim <= fim.Value);
        if (confirmado.HasValue)
            query = query.Where(a => a.Confirmado == confirmado.Value);
        if (cancelado.HasValue)
            query = query.Where(a => a.Cancelado == cancelado.Value);
        if (profissionalId.HasValue)
            query = query.Where(a => a.ProfissionalId == profissionalId.Value);
        if (clienteId.HasValue)
            query = query.Where(a => a.ClienteId == clienteId.Value);
        var agendamentos = await query.ToListAsync();
        var csv = "Id,Cliente,Profissional,Servico,DataHoraInicio,DataHoraFim,Confirmado,Cancelado,DataCriacao\n" +
            string.Join("\n", agendamentos.Select(a => $"{a.Id},\"{a.Cliente?.Nome}\",\"{a.Profissional?.Nome}\",\"{a.Servico?.Nome}\",{a.DataHoraInicio:yyyy-MM-dd HH:mm},{a.DataHoraFim:yyyy-MM-dd HH:mm},{a.Confirmado},{a.Cancelado},{a.DataCriacao:yyyy-MM-dd HH:mm}"));
        return File(System.Text.Encoding.UTF8.GetBytes(csv), "text/csv", "agendamentos.csv");
    }

    // GET: api/admin/usuarios/exportar-json
    [HttpGet("usuarios/exportar-json")]
    public async Task<IActionResult> ExportarUsuariosJson()
    {
        var usuarios = await _context.Usuarios.ToListAsync();
        return Ok(usuarios);
    }

    // GET: api/admin/clientes/exportar-json
    [HttpGet("clientes/exportar-json")]
    public async Task<IActionResult> ExportarClientesJson()
    {
        var clientes = await _context.Clientes.ToListAsync();
        return Ok(clientes);
    }

    // GET: api/admin/profissionais/exportar-json
    [HttpGet("profissionais/exportar-json")]
    public async Task<IActionResult> ExportarProfissionaisJson()
    {
        var profissionais = await _context.Profissionais.ToListAsync();
        return Ok(profissionais);
    }

    // GET: api/admin/servicos/exportar-json
    [HttpGet("servicos/exportar-json")]
    public async Task<IActionResult> ExportarServicosJson()
    {
        var servicos = await _context.Servicos.ToListAsync();
        return Ok(servicos);
    }

    // GET: api/admin/agendamentos/exportar-json
    [HttpGet("agendamentos/exportar-json")]
    public async Task<IActionResult> ExportarAgendamentosJson(
        [FromQuery] DateTime? inicio = null,
        [FromQuery] DateTime? fim = null,
        [FromQuery] bool? confirmado = null,
        [FromQuery] bool? cancelado = null,
        [FromQuery] int? profissionalId = null,
        [FromQuery] int? clienteId = null)
    {
        var query = _context.Agendamentos
            .Include(a => a.Cliente)
            .Include(a => a.Profissional)
            .Include(a => a.Servico)
            .AsQueryable();
        if (inicio.HasValue)
            query = query.Where(a => a.DataHoraInicio >= inicio.Value);
        if (fim.HasValue)
            query = query.Where(a => a.DataHoraFim <= fim.Value);
        if (confirmado.HasValue)
            query = query.Where(a => a.Confirmado == confirmado.Value);
        if (cancelado.HasValue)
            query = query.Where(a => a.Cancelado == cancelado.Value);
        if (profissionalId.HasValue)
            query = query.Where(a => a.ProfissionalId == profissionalId.Value);
        if (clienteId.HasValue)
            query = query.Where(a => a.ClienteId == clienteId.Value);
        var agendamentos = await query.ToListAsync();
        return Ok(agendamentos);
    }

    // GET: api/admin/dashboard
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboardResumo()
    {
        // Agendamentos por mês (últimos 12 meses)
        var hoje = DateTime.UtcNow;
        var agendamentosPorMes = await _context.Agendamentos
            .Where(a => a.DataHoraInicio > hoje.AddMonths(-12))
            .GroupBy(a => new { a.DataHoraInicio.Year, a.DataHoraInicio.Month })
            .Select(g => new {
                Ano = g.Key.Year,
                Mes = g.Key.Month,
                Total = g.Count()
            })
            .OrderBy(g => g.Ano).ThenBy(g => g.Mes)
            .ToListAsync();

        // Serviços mais agendados
        var servicosMaisAgendados = await _context.Agendamentos
            .GroupBy(a => a.ServicoId)
            .Select(g => new {
                ServicoId = g.Key,
                NomeServico = g.First().Servico.Nome,
                Total = g.Count()
            })
            .OrderByDescending(g => g.Total)
            .Take(5)
            .ToListAsync();

        // Clientes mais ativos
        var clientesMaisAtivos = await _context.Agendamentos
            .GroupBy(a => a.ClienteId)
            .Select(g => new {
                ClienteId = g.Key,
                NomeCliente = g.First().Cliente.Nome,
                Total = g.Count()
            })
            .OrderByDescending(g => g.Total)
            .Take(5)
            .ToListAsync();

        // Profissionais mais requisitados
        var profissionaisMaisRequisitados = await _context.Agendamentos
            .GroupBy(a => a.ProfissionalId)
            .Select(g => new {
                ProfissionalId = g.Key,
                NomeProfissional = g.First().Profissional.Nome,
                Total = g.Count()
            })
            .OrderByDescending(g => g.Total)
            .Take(5)
            .ToListAsync();

        // Satisfação geral
        var feedbacks = await _context.FeedbacksAgendamento.ToListAsync();
        var satisfacaoGeral = feedbacks.Any() ? feedbacks.Average(f => f.Nota) : 0;

        return Ok(new
        {
            agendamentosPorMes,
            servicosMaisAgendados,
            clientesMaisAtivos,
            profissionaisMaisRequisitados,
            satisfacaoGeral
        });
    }

    // PATCH: api/admin/usuarios/{id}/reset-senha
    [HttpPatch("usuarios/{id}/reset-senha")]
    public async Task<IActionResult> ResetarSenhaUsuario(int id, [FromBody] string novaSenha)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null)
            return NotFound();
        usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(novaSenha);
        await _context.SaveChangesAsync();
        await RegistrarLog(User.Identity?.Name ?? "admin", $"Reset de senha do usuário {usuario.Email}");
        return NoContent();
    }

    // PATCH: api/admin/usuarios/{id}/bloqueio
    [HttpPatch("usuarios/{id}/bloqueio")]
    public async Task<IActionResult> BloquearUsuario(int id, [FromBody] bool bloquear)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null)
            return NotFound();
        usuario.Bloqueado = bloquear;
        await _context.SaveChangesAsync();
        await RegistrarLog(User.Identity?.Name ?? "admin", $"{(bloquear ? "Bloqueio" : "Desbloqueio")} do usuário {usuario.Email}");
        return NoContent();
    }

    // GET: api/admin/logs
    [HttpGet("logs")]
    public async Task<IActionResult> GetLogsAuditoria(
        [FromQuery] string? usuario = null,
        [FromQuery] string? acao = null,
        [FromQuery] DateTime? inicio = null,
        [FromQuery] DateTime? fim = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string orderDir = "desc")
    {
        var query = _context.LogsAuditoria.AsQueryable();
        if (!string.IsNullOrEmpty(usuario))
            query = query.Where(l => l.Usuario.Contains(usuario));
        if (!string.IsNullOrEmpty(acao))
            query = query.Where(l => l.Acao.Contains(acao));
        if (inicio.HasValue)
            query = query.Where(l => l.DataHora >= inicio.Value);
        if (fim.HasValue)
            query = query.Where(l => l.DataHora <= fim.Value);
        query = orderDir == "asc" ? query.OrderBy(l => l.DataHora) : query.OrderByDescending(l => l.DataHora);
        var total = await query.CountAsync();
        var logs = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return Ok(new { total, page, pageSize, data = logs });
    }

    // GET: api/admin/notificacoes
    [HttpGet("notificacoes")]
    public async Task<IActionResult> GetNotificacoesAdmin()
    {
        var seteDiasAtras = DateTime.UtcNow.AddDays(-7);
        var cancelamentosRecentes = await _context.Agendamentos.CountAsync(a => a.Cancelado && a.DataCancelamento >= seteDiasAtras);
        var notificacoes = new List<string>();
        if (cancelamentosRecentes > 5)
            notificacoes.Add($"Atenção: {cancelamentosRecentes} agendamentos foram cancelados nos últimos 7 dias.");
        // Outras notificações podem ser adicionadas aqui
        return Ok(notificacoes);
    }

    // GET: api/admin/feedbacks
    [HttpGet("feedbacks")]
    public async Task<IActionResult> GetFeedbacks(
        [FromQuery] int? nota = null,
        [FromQuery] int? servicoId = null,
        [FromQuery] int? profissionalId = null,
        [FromQuery] int? clienteId = null,
        [FromQuery] DateTime? inicio = null,
        [FromQuery] DateTime? fim = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string orderDir = "desc")
    {
        var query = _context.FeedbacksAgendamento
            .Include(f => f.Agendamento)
                .ThenInclude(a => a.Servico)
            .Include(f => f.Agendamento)
                .ThenInclude(a => a.Profissional)
            .Include(f => f.Agendamento)
                .ThenInclude(a => a.Cliente)
            .AsQueryable();
        if (nota.HasValue)
            query = query.Where(f => f.Nota == nota.Value);
        if (servicoId.HasValue)
            query = query.Where(f => f.Agendamento.ServicoId == servicoId.Value);
        if (profissionalId.HasValue)
            query = query.Where(f => f.Agendamento.ProfissionalId == profissionalId.Value);
        if (clienteId.HasValue)
            query = query.Where(f => f.Agendamento.ClienteId == clienteId.Value);
        if (inicio.HasValue)
            query = query.Where(f => f.DataRegistro >= inicio.Value);
        if (fim.HasValue)
            query = query.Where(f => f.DataRegistro <= fim.Value);
        query = orderDir == "asc" ? query.OrderBy(f => f.DataRegistro) : query.OrderByDescending(f => f.DataRegistro);
        var total = await query.CountAsync();
        var feedbacks = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return Ok(new { total, page, pageSize, data = feedbacks });
    }

    // GET: api/admin/feedbacks/relatorio
    [HttpGet("feedbacks/relatorio")]
    public async Task<IActionResult> GetRelatorioFeedbacks()
    {
        var feedbacks = await _context.FeedbacksAgendamento
            .Include(f => f.Agendamento)
                .ThenInclude(a => a.Servico)
            .Include(f => f.Agendamento)
                .ThenInclude(a => a.Profissional)
            .ToListAsync();
        var mediaGeral = feedbacks.Any() ? feedbacks.Average(f => f.Nota) : 0;
        var mediaPorServico = feedbacks
            .GroupBy(f => f.Agendamento.Servico.Nome)
            .Select(g => new { Servico = g.Key, Media = g.Average(f => f.Nota), Total = g.Count() })
            .OrderByDescending(g => g.Total)
            .ToList();
        var mediaPorProfissional = feedbacks
            .GroupBy(f => f.Agendamento.Profissional.Nome)
            .Select(g => new { Profissional = g.Key, Media = g.Average(f => f.Nota), Total = g.Count() })
            .OrderByDescending(g => g.Total)
            .ToList();
        var feedbacksRecentes = feedbacks
            .OrderByDescending(f => f.DataRegistro)
            .Take(10)
            .ToList();
        return Ok(new
        {
            mediaGeral,
            mediaPorServico,
            mediaPorProfissional,
            feedbacksRecentes
        });
    }

    private async Task RegistrarLog(string usuario, string acao, string? detalhes = null)
    {
        _context.LogsAuditoria.Add(new LogAuditoria
        {
            Usuario = usuario,
            Acao = acao,
            Detalhes = detalhes
        });
        await _context.SaveChangesAsync();
    }
} 