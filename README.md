# FGC Users API

Microsserviço responsável pelo cadastro e autenticação de usuários da plataforma **FIAP Cloud Games**.

## Responsabilidades

- Cadastro de novos usuários (role `User`)
- Autenticação via JWT
- Publicação do evento `UserCreatedEvent` no RabbitMQ após criação de usuário
- Seed automático do usuário administrador na inicialização

## Endpoints

| Método | Rota | Autenticação | Descrição |
|--------|------|-------------|-----------|
| `POST` | `/users` | Não | Cadastra novo usuário |
| `POST` | `/auth/login` | Não | Autentica e retorna JWT |

### POST /users

```json
{
  "name": "João Silva",
  "email": "joao@email.com",
  "password": "Senha@123"
}
```

Retorna `201 Created` com o `id` do usuário criado.

Regras de senha: mínimo 8 caracteres, pelo menos 1 letra, 1 número e 1 caractere especial.

### POST /auth/login

```json
{
  "email": "joao@email.com",
  "password": "Senha@123"
}
```

Retorna `200 OK`:

```json
{
  "token": "<jwt>"
}
```

### Usuário admin padrão (seed)

| Campo | Valor |
|-------|-------|
| Email | `admin@fcg.com` |
| Senha | `Admin@123` |

## Variáveis de ambiente

| Variável | Descrição | Exemplo |
|----------|-----------|---------|
| `ConnectionStrings__DefaultConnection` | Connection string PostgreSQL | `Host=localhost;Database=usersdb;Username=postgres;Password=postgres` |
| `Jwt__Key` | Chave secreta JWT (mín. 32 chars) | `CHAVE_SUPER_SECRETA_MIN_32_CHARS!!` |
| `Jwt__Issuer` | Issuer do token JWT | `FCG.Users.Api` |
| `Jwt__Audience` | Audience do token JWT | `FCG.Users.Api` |
| `RabbitMQ__Host` | Host do RabbitMQ | `localhost` |
| `RabbitMQ__Username` | Usuário RabbitMQ | `guest` |
| `RabbitMQ__Password` | Senha RabbitMQ | `guest` |

## Executar localmente

### Pré-requisitos

- .NET SDK 8.0
- PostgreSQL rodando na porta 5432
- RabbitMQ rodando na porta 5672

```bash
# Restaurar dependências e rodar
dotnet run --project FGC.Users.Api
```

A API estará disponível em `https://localhost:5001` (ou `http://localhost:5000`).

As migrações e o seed do admin são aplicados automaticamente na inicialização.

O Swagger estará disponível em `/swagger` no ambiente de desenvolvimento.

## Executar com Docker

```bash
# Subir todos os serviços (API + PostgreSQL + RabbitMQ)
docker compose up --build
```

A API ficará disponível em `http://localhost:5000`.

O painel de administração do RabbitMQ estará em `http://localhost:15672` (guest/guest).

## Arquitetura

```
FGC.Users.Api           → Controllers, Middlewares, Program.cs
FGC.Users.Application   → Services, DTOs, Validators, Events
FGC.Users.Infrastructure → DbContext, Repositories, Migrations, Seed
FGC.Users.Domain        → Entities, Value Objects, Enums, Interfaces
```

## Eventos publicados

| Evento | Exchange | Quando |
|--------|----------|--------|
| `UserCreatedEvent` | `user-created` (MassTransit padrão) | Ao criar novo usuário |
