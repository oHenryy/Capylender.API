using Microsoft.EntityFrameworkCore;
using Capylender.API.Models;
using BCrypt.Net;

namespace Capylender.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Cliente> Clientes { get; set; } = null!;
    public DbSet<Profissional> Profissionais { get; set; } = null!;
    public DbSet<Servico> Servicos { get; set; } = null!;
    public DbSet<Disponibilidade> Disponibilidades { get; set; } = null!;
    public DbSet<Agendamento> Agendamentos { get; set; } = null!;
    public DbSet<Usuario> Usuarios { get; set; } = null!;
    public DbSet<LogAuditoria> LogsAuditoria { get; set; } = null!;
    public DbSet<FeedbackAgendamento> FeedbacksAgendamento { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurações de índices
        modelBuilder.Entity<Agendamento>()
            .HasIndex(a => new { a.ProfissionalId, a.DataHoraInicio, a.DataHoraFim });

        modelBuilder.Entity<Cliente>()
            .HasIndex(c => c.Email)
            .IsUnique();

        modelBuilder.Entity<Cliente>()
            .HasIndex(c => c.CPF)
            .IsUnique();

        modelBuilder.Entity<Profissional>()
            .HasIndex(p => p.Email)
            .IsUnique();

        // Configuração de precisão para o campo Preco
        modelBuilder.Entity<Servico>()
            .Property(s => s.Preco)
            .HasPrecision(10, 2);

        // Configurações do Serviço
        modelBuilder.Entity<Servico>()
            .HasOne(s => s.Profissional)
            .WithMany()
            .HasForeignKey(s => s.ProfissionalId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configurações da Disponibilidade
        modelBuilder.Entity<Disponibilidade>()
            .HasOne(d => d.Profissional)
            .WithMany()
            .HasForeignKey(d => d.ProfissionalId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configurações do Agendamento
        modelBuilder.Entity<Agendamento>()
            .HasOne(a => a.Cliente)
            .WithMany(c => c.Agendamentos)
            .HasForeignKey(a => a.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Agendamento>()
            .HasOne(a => a.Profissional)
            .WithMany()
            .HasForeignKey(a => a.ProfissionalId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Agendamento>()
            .HasOne(a => a.Servico)
            .WithMany(s => s.Agendamentos)
            .HasForeignKey(a => a.ServicoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Seed de usuário admin mockado
        var senhaHash = BCrypt.Net.BCrypt.HashPassword("123456");
        modelBuilder.Entity<Usuario>().HasData(new Usuario
        {
            Id = 1,
            Nome = "Administrador",
            Email = "admin@capylender.com",
            SenhaHash = senhaHash,
            Role = "Admin",
            Bloqueado = false
        });
    }
} 