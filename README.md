# Capylender.API

Sistema de agendamento e reservas para serviços, desenvolvido em .NET 8, com SQL Server (Docker), Entity Framework Core e arquitetura moderna (DTOs, Services, Repositories, AutoMapper).

## Tecnologias Utilizadas
- .NET 8
- Entity Framework Core
- SQL Server (via Docker)
- AutoMapper
- JWT para autenticação
- MailKit para envio de e-mails
- Hangfire para jobs em background
- Swagger para documentação da API

## Principais Funcionalidades
- Cadastro e gerenciamento de usuários, clientes, profissionais e serviços
- Agendamento de serviços com validação de disponibilidade e conflitos
- Painel administrativo completo (CRUD, ativação/desativação, alteração de roles, bloqueio, reset de senha)
- Relatórios, dashboard, exportação de dados (CSV/JSON), buscas avançadas, filtros, paginação e ordenação
- Automação de e-mails (lembretes, confirmações, pesquisas de satisfação)
- Feedback de clientes integrado ao painel administrativo
- Logs de auditoria automáticos

## Como Rodar Localmente

### 1. Clonar o repositório
```bash
git clone <url-do-repositorio>
cd Capylender.API
```

### 2. Subir o banco de dados SQL Server com Docker
```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=SuaSenhaForte123" -p 1433:1433 --name capylender-sql -d mcr.microsoft.com/mssql/server:2022-latest
```

### 3. Configurar a connection string
Edite o arquivo `appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=CapylenderDb;User Id=sa;Password=SuaSenhaForte123;TrustServerCertificate=True;"
  }
}
```

### 4. Aplicar as migrations
```bash
dotnet ef database update
```

### 5. Rodar a aplicação localmente
```bash
dotnet run
```
A API estará disponível em: `https://localhost:5001` ou `http://localhost:5000`

### 6. Rodar a aplicação com Docker

#### Build da imagem
```bash
docker build -t capylender-api .
```

#### Executar o container
```bash
docker run -d -p 8080:8080 -p 8081:8081 --env-file .env --name capylender-api capylender-api
```

> **Dica:** Crie um arquivo `.env` com as variáveis de ambiente necessárias, como a connection string e configurações de e-mail.

### 7. Acessar o Swagger
Acesse `http://localhost:8080/swagger` para explorar e testar os endpoints da API.

## Exemplo de Variáveis de Ambiente (`.env`)
```env
ConnectionStrings__DefaultConnection=Server=host.docker.internal,1433;Database=CapylenderDb;User Id=sa;Password=SuaSenhaForte123;TrustServerCertificate=True;
MailSettings__SmtpServer=smtp.seuprovedor.com
MailSettings__SmtpPort=587
MailSettings__SmtpUser=usuario@dominio.com
MailSettings__SmtpPass=sua_senha
Jwt__Key=uma-chave-secreta-bem-grande
Jwt__Issuer=Capylender
Jwt__Audience=CapylenderUsers
```

## Exemplos de Endpoints

### Autenticação (Login)
`POST /api/auth/login`
```json
{
  "email": "admin@capylender.com",
  "senha": "123456"
}
```
**Resposta:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6..."
}
```

## Usuário Admin Padrão (Seed)

Ao rodar as migrations, um usuário administrador é criado automaticamente:
- **Email:** admin@capylender.com
- **Senha:** 123456
- **Role:** Admin

Use esse usuário para acessar o painel administrativo e testar permissões.

### Cadastro de Cliente
`POST /api/clientes`
```json
{
  "nome": "João Silva",
  "email": "joao@exemplo.com",
  "telefone": "11999999999"
}
```

### Agendamento de Serviço
`POST /api/agendamentos`
```json
{
  "clienteId": 1,
  "profissionalId": 2,
  "servicoId": 3,
  "dataHoraInicio": "2024-06-01T14:00:00",
  "dataHoraFim": "2024-06-01T15:00:00",
  "observacoes": "Corte de cabelo com lavagem"
}
```

### Exemplo de uso do JWT
Após o login, inclua o token JWT no header das requisições protegidas:
```
Authorization: Bearer <seu_token_jwt>
```

### Exportação de Dados
`GET /api/admin/exportar?formato=csv`

### Dashboard e Relatórios
`GET /api/admin/dashboard`

## Troubleshooting
- **Erro de conexão com o banco:** Verifique se o container do SQL Server está rodando e se a senha está correta.
- **Problemas com e-mail:** Confirme as variáveis SMTP e se a porta está liberada.
- **Problemas com JWT:** Certifique-se de que a chave e issuer estão iguais no backend e no frontend.
- **Permissões negadas:** Verifique o perfil do usuário e se o token está válido.

## Estrutura do Projeto
- **Controllers/**: Endpoints da API
- **Models/**: Entidades do domínio
- **DTOs/**: Objetos de transferência de dados
- **Repositories/**: Acesso a dados
- **Services/**: Regras de negócio
- **Mappings/**: Configurações do AutoMapper
- **Migrations/**: Migrations do EF Core

## Contribuição
Pull requests são bem-vindos! Para grandes mudanças, abra uma issue primeiro para discutir o que você gostaria de modificar.