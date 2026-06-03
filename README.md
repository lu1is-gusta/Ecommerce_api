# Ecommerce API

API RESTful de e-commerce desenvolvida com .NET 10 e Clean Architecture. Permite o gerenciamento completo de pedidos, produtos e compradores, seguindo boas práticas de arquitetura de software com foco em qualidade de código, organização e manutenibilidade.

---

## Tecnologias Utilizadas

| Categoria | Tecnologia |
|---|---|
| Plataforma | .NET 10, C# |
| API | ASP.NET Core Minimal APIs |
| Banco de dados | SQL Server 2022 + Entity Framework Core 9 |
| Validação | FluentValidation |
| Documentação | Swagger / OpenAPI (Swashbuckle) |
| Versionamento de API | Asp.Versioning (segmento de URL) |
| Logs | Serilog (Console + arquivo rotativo) |
| Observabilidade | OpenTelemetry (OTLP) + Jaeger |
| Health Checks | AspNetCore.HealthChecks.SqlServer |
| Testes unitários | xUnit + Moq |
| Testes de integração | xUnit + Microsoft.AspNetCore.Mvc.Testing + EF InMemory |
| Containers | Docker + Docker Compose |

### Arquitetura

O projeto segue os princípios de **Clean Architecture**, dividido em quatro camadas:

```
EcommerceApi.Domain          → Entidades, enums, exceções de domínio
EcommerceApi.Application     → Casos de uso, interfaces, validações
EcommerceApi.Infrastructure  → EF Core, repositórios, migrations
EcommerceApi.Api             → Endpoints, middleware, observabilidade, OpenAPI
```

---

## Pré-requisitos e Instalação

### Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker](https://www.docker.com/) e [Docker Compose](https://docs.docker.com/compose/) (para execução em container)
- SQL Server LocalDB (para execução local sem Docker) — incluído no Visual Studio ou instalável via [SQL Server Express LocalDB](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb)

### Clonando o repositório

```bash
git clone <url-do-repositorio>
cd EcommerceApi
```

### Restaurar dependências

```bash
dotnet restore
```

---

## Rodando a Aplicação

### Opção 1 — Local (sem Docker)

A configuração padrão usa **SQL Server LocalDB**, que não requer instalação de servidor separado.

A string de conexão padrão em `appsettings.json` aponta para:

```
Server=(localdb)\MSSQLLocalDB;Database=EcommerceDb;Trusted_Connection=True;...
```

#### Executar a API

```bash
dotnet run --project src/EcommerceApi.Api
```

A aplicação sobe em `http://localhost:5080`. As migrations são aplicadas automaticamente na inicialização.

#### Executar em modo Development com hot reload

```bash
dotnet watch --project src/EcommerceApi.Api
```

#### Alterar a string de conexão (opcional)

Para apontar para um SQL Server diferente, defina a variável de ambiente antes de rodar:

```bash
export ConnectionStrings__Default="Server=meu-servidor;Database=EcommerceDb;User Id=sa;Password=senha;TrustServerCertificate=True"
dotnet run --project src/EcommerceApi.Api
```

---

### Opção 2 — Docker Compose (recomendado)

Sobe a API, o SQL Server 2022 Express e o Jaeger (observabilidade) em conjunto.

#### Subir todos os serviços

```bash
docker compose up
```

#### Subir em background (modo detached)

```bash
docker compose up -d
```

#### Rebuildar a imagem da API e subir

```bash
docker compose up --build
```

#### Forçar rebuild sem cache

```bash
docker compose build --no-cache
docker compose up
```

#### Parar os serviços

```bash
docker compose down
```

#### Parar e remover os volumes (apaga o banco de dados)

```bash
docker compose down -v
```

#### Serviços e portas expostas

| Serviço | Porta local | Descrição |
|---|---|---|
| `api` | `5080` | Ecommerce API |
| `sqlserver` | `1433` | SQL Server 2022 Express |
| `jaeger` | `16686` | Jaeger UI (tracing) |
| `jaeger` | `4317` | Receptor OTLP gRPC (interno) |

A API aguarda o SQL Server ficar saudável antes de iniciar (healthcheck configurado no `docker-compose.yml`).

---

## Visualização de Logs

### Logs no terminal (durante execução local ou com Docker)

```bash
# Logs em tempo real de todos os serviços Docker
docker compose logs -f

# Logs apenas da API
docker compose logs -f api

# Últimas 100 linhas dos logs da API
docker compose logs --tail=100 api
```

### Logs em arquivo

Os logs são gravados em arquivos rotativos diários no diretório `logs/`:

```
logs/log-20260101.txt
logs/log-20260102.txt
...
```

Cada arquivo é retido por 7 dias. Os logs são enriquecidos com `MachineName` e `ThreadId`.

Para acompanhar o arquivo de log em tempo real (execução local):

```bash
tail -f logs/log-$(date +%Y%m%d).txt
```

---

## Observabilidade — Tracing com Jaeger

A API envia traces via **OpenTelemetry** (protocolo OTLP) para o Jaeger. São instrumentadas automaticamente as requisições HTTP e as queries do Entity Framework Core.

### Acessar o Jaeger UI

Com os serviços Docker em execução, abra no navegador:

```
http://localhost:16686
```

No Jaeger UI, selecione o serviço **`ecommerce-api`** e clique em **Find Traces** para visualizar os traces distribuídos de cada requisição, incluindo as queries SQL executadas pelo EF Core.

---

## Documentação da API — Swagger

O Swagger está disponível apenas no ambiente `Development`:

```
http://localhost:5080/swagger
```

Todos os endpoints estão documentados com descrições, parâmetros, respostas esperadas e exemplos de erros. Os enums são exibidos como strings para facilitar a leitura.

---

## Health Checks

| Endpoint | Descrição |
|---|---|
| `GET /health/live` | Liveness check — verifica se o processo está rodando. Não depende de serviços externos. |
| `GET /health/ready` | Readiness check — verifica a conectividade com o SQL Server. Retorna `503` se o banco estiver indisponível. |

```bash
curl http://localhost:5080/health/live
curl http://localhost:5080/health/ready
```

---

## Executando os Testes

### Testes unitários

Testam os casos de uso e as regras de domínio de forma isolada, usando Moq para simular os repositórios.

```bash
dotnet test tests/EcommerceApi.UnitTests
```

### Testes de integração

Sobem a aplicação real via `WebApplicationFactory` com banco de dados **InMemory**, testando os endpoints HTTP de ponta a ponta.

```bash
dotnet test tests/EcommerceApi.IntegrationTests
```

### Rodar todos os testes

```bash
dotnet test
```

### Rodar todos os testes com relatório de cobertura

```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Rodar testes com saída detalhada

```bash
dotnet test --verbosity normal
```

---

## Migrations (Entity Framework Core)

As migrations são aplicadas automaticamente ao iniciar a aplicação. Para executá-las manualmente ou gerenciá-las:

### Aplicar migrations pendentes

```bash
dotnet ef database update --project src/EcommerceApi.Infrastructure --startup-project src/EcommerceApi.Api
```

### Criar uma nova migration

```bash
dotnet ef migrations add NomeDaMigration \
  --project src/EcommerceApi.Infrastructure \
  --startup-project src/EcommerceApi.Api
```

### Listar migrations existentes

```bash
dotnet ef migrations list \
  --project src/EcommerceApi.Infrastructure \
  --startup-project src/EcommerceApi.Api
```

### Reverter para uma migration específica

```bash
dotnet ef database update NomeDaMigration \
  --project src/EcommerceApi.Infrastructure \
  --startup-project src/EcommerceApi.Api
```

> **Nota:** a variável de ambiente `ECOMMERCE_CONNECTION` pode ser usada para sobrescrever a string de conexão ao rodar comandos EF fora do contexto da aplicação:
> ```bash
> export ECOMMERCE_CONNECTION="Server=...;Database=EcommerceDb;..."
> ```

---

## Features / Funcionalidades

### Pedidos (`/api/v1/orders`)

| Método | Endpoint | Descrição |
|---|---|---|
| `POST` | `/api/v1/orders` | Cria um novo pedido |
| `GET` | `/api/v1/orders` | Lista pedidos com filtros e paginação |
| `GET` | `/api/v1/orders/{id}` | Busca um pedido pelo ID |
| `PUT` | `/api/v1/orders/{id}` | Atualiza os itens de um pedido |
| `PUT` | `/api/v1/orders/{id}/cancel` | Cancela um pedido |
| `DELETE` | `/api/v1/orders/{id}` | Remove um pedido |

**Filtros disponíveis em `GET /api/v1/orders`:**
- `status` — filtra por status (`Started`, `Processed`, `Shipped`, `Cancelled`)
- `buyerId` — filtra pelo ID do comprador
- `from` / `to` — intervalo de datas de criação
- `page` / `pageSize` — paginação

**Ciclo de vida do pedido:**

```
Iniciado (Started)
    │
    ├──[cancelar]──→ Cancelado (Cancelled)
    │
    ▼
Processado (Processed)
    │
    ├──[cancelar]──→ Cancelado (Cancelled)
    │
    ▼
Enviado (Shipped)
```

**Regras de negócio:**
- Um pedido deve ter um comprador e pelo menos um item
- Apenas pedidos com status `Started` podem ter seus itens alterados
- Apenas pedidos com status `Started` ou `Processed` podem ser cancelados
- O preço unitário dos itens é fixado no momento da criação, com base no catálogo de produtos

---

### Produtos (`/api/v1/products`)

| Método | Endpoint | Descrição |
|---|---|---|
| `POST` | `/api/v1/products` | Cria um novo produto |
| `GET` | `/api/v1/products` | Lista produtos com filtros e paginação |
| `GET` | `/api/v1/products/{id}` | Busca um produto pelo ID |
| `PUT` | `/api/v1/products/{id}` | Atualiza nome e preço de um produto |
| `DELETE` | `/api/v1/products/{id}` | Remove um produto |

**Filtros disponíveis em `GET /api/v1/products`:**
- `name` — filtra por nome (busca parcial)
- `minPrice` / `maxPrice` — intervalo de preço
- `page` / `pageSize` — paginação

**Dados iniciais (seed):** o banco é populado com 3 produtos na migration inicial: Keyboard, Mouse e Monitor.

---

### Compradores (`/api/v1/buyers`)

| Método | Endpoint | Descrição |
|---|---|---|
| `POST` | `/api/v1/buyers` | Cria um novo comprador |
| `GET` | `/api/v1/buyers` | Lista compradores com filtros e paginação |
| `GET` | `/api/v1/buyers/{id}` | Busca um comprador pelo ID |
| `PUT` | `/api/v1/buyers/{id}` | Atualiza nome e e-mail de um comprador |
| `DELETE` | `/api/v1/buyers/{id}` | Remove um comprador (somente se não tiver pedidos) |

**Filtros disponíveis em `GET /api/v1/buyers`:**
- `name` — filtra por nome (busca parcial)
- `email` — filtra por e-mail (busca parcial)
- `page` / `pageSize` — paginação

**Regra de negócio:** um comprador não pode ser excluído enquanto possuir pedidos associados (retorna `422 Unprocessable Entity`).

---

### Tratamento de Erros

A API retorna respostas no formato [RFC 9457 Problem Details](https://www.rfc-editor.org/rfc/rfc9457) para todos os erros:

| Status | Situação |
|---|---|
| `400 Bad Request` | Falha de validação (FluentValidation) |
| `404 Not Found` | Recurso não encontrado |
| `422 Unprocessable Entity` | Violação de regra de domínio |
| `500 Internal Server Error` | Erro inesperado |
