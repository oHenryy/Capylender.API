using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Capylender.API.Data;
using Capylender.API.Mappings;
using Capylender.API.Repositories;
using Capylender.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configuração do DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuração do AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// Registro dos Repositories
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IServicoRepository, ServicoRepository>();
builder.Services.AddScoped<IDisponibilidadeRepository, DisponibilidadeRepository>();

// Registro dos Services
builder.Services.AddScoped(typeof(IGenericService<,,>), typeof(GenericService<,,>));
builder.Services.AddScoped<IServicoService, ServicoService>();
builder.Services.AddScoped<IDisponibilidadeService, DisponibilidadeService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IJwtService, JwtService>();

// Registro dos jobs do Hangfire
builder.Services.AddScoped<LembreteAgendamentoJob>();
builder.Services.AddScoped<PesquisaSatisfacaoJob>();

// Configuração do Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Capylender API",
        Version = "v1",
        Description = "API para sistema de agendamento e reservas",
        Contact = new OpenApiContact
        {
            Name = "Suporte Capylender",
            Email = "suporte@capylender.com"
        }
    });

    // Agrupar endpoints por controller
    c.TagActionsBy(api => new[] { api.GroupName ?? api.ActionDescriptor.RouteValues["controller"] });
    c.DocInclusionPredicate((docName, apiDesc) => true);

    // Adicionar autenticação JWT (quando implementar)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!))
    };
});

// Configuração do Hangfire
builder.Services.AddHangfire(x =>
    x.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfireServer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Capylender API v1");
        c.RoutePrefix = string.Empty; // Para servir o Swagger na raiz
    });
    // Painel do Hangfire
    app.UseHangfireDashboard("/hangfire");
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Agendar job recorrente de lembrete de agendamento
using (var scope = app.Services.CreateScope())
{
    var job = scope.ServiceProvider.GetRequiredService<LembreteAgendamentoJob>();
    RecurringJob.AddOrUpdate(
        "lembrete-agendamento",
        () => job.EnviarLembretesAsync(),
        "*/30 * * * *" // a cada 30 minutos
    );

    var pesquisaJob = scope.ServiceProvider.GetRequiredService<PesquisaSatisfacaoJob>();
    RecurringJob.AddOrUpdate(
        "pesquisa-satisfacao",
        () => pesquisaJob.EnviarPesquisasAsync(),
        "0 * * * *" // a cada hora
    );
}

app.Run();
