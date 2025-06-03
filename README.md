# GlobalSolution.DotNet

## Descrição do Projeto

**GlobalSolution.DotNet** é uma aplicação web construída em .NET 8 seguindo Clean Architecture e Domain-Driven Design (DDD). O objetivo é expor três entidades principais (Adm, Usuário e SafeHouse) por meio de uma API RESTful, com recursos de autenticação, documentação via Swagger, rate limiting em memória e um endpoint de previsão de alertas usando ML.NET. A separação em camadas torna o projeto modular, testável e de fácil manutenção.

**Estrutura da solução**  
```
GlobalSolution.DotNet.sln
│
├── src
│   ├── Api              ← Projeto ASP.NET Core Web API
│   ├── Application      ← Camada de Application (serviços, DTOs, interfaces)
│   ├── Domain           ← Camada de Domain (entidades e interfaces de repositório)
│   └── Infrastructure   ← Camada de Infra (EF Core, Oracle, RateLimit, ML.NET)
│   
│
└── tests
    ├── Domain.Tests       ← Testes de entidade (xUnit)
    ├── Application.Tests  ← Testes de serviços (xUnit + Moq)
    └── Api.Tests          ← Testes de controllers (xUnit + Moq)
```

---

## Tecnologias Utilizadas

- **.NET 8 / C# 11**  
- **ASP.NET Core Web API**  
- **Entity Framework Core 9** com provider **Oracle.EntityFrameworkCore**  
- **Clean Architecture** & **DDD**  
- **Repository Pattern** (interfaces no Domain, implementações no Infrastructure)  
- **Dependency Injection** nativo do ASP.NET Core  
- **Rate Limiting** via pacote `AspNetCoreRateLimit` (configuração em memória)  
- **Swagger** (`Swashbuckle.AspNetCore`)  
- **ML.NET** (classe `AlertModelBuilder` + serviço `IAlertPredictionService`)  
- **xUnit** para testes unitários e de integração  
- **Moq** para mockar repositórios e serviços nos testes  
- **Oracle Database** (provider EF Core)  
- **POO** (Programação Orientada a Objetos)  

---

## Como Executar o Projeto

### Pré-requisitos

1. **.NET 8 SDK** instalado (`dotnet --version` deve retornar algo ≥ 8.0).  
2. **Oracle Database** acessível (ex.: Oracle XE local ou Oracle Cloud).  
3. Ferramenta para gerenciar Oracle (SQL Developer, DBeaver etc.).

### 1. Configurar `appsettings.json`

No projeto `src/Api`, abra **`appsettings.json`** e defina a connection string do Oracle:

```jsonc
{
  "ConnectionStrings": {
    "FiapOracleConnection": "Data Source=//oracle.fiap.com.br:1521/orcl;User Id=SeuRM;Password=SuaSenha;"
  },
  "IpRateLimiting": {
    "EnableEndpointRateLimiting": true,
    "StackBlockedRequests": false,
    "RealIpHeader": "X-Real-IP",
    "ClientIdHeader": "X-ClientId",
    "HttpStatusCode": 429,
    "GeneralRules": [
      {
        "Endpoint": "*",
        "Period": "1m",
        "Limit": 60
      }
    ]
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  }
}
```

- **`FiapOracleConnection`**: ajuste host, porta (1521), service name (`orcl`), usuário e senha do seu Oracle.  
- **Rate Limiting**: neste exemplo, todas as rotas permitem até 60 requisições por minuto.

### 2. Gerar e aplicar migrações

1. Abra terminal na raiz da solução e vá para `src/Infrastructure`:

   ```bash
   cd src/Infrastructure
   ```

2. Adicione a migration (assembly de migrations = `Infrastructure`):

   ```bash
   dotnet ef migrations add CriarTabelasOracle      --project Infrastructure.csproj      --startup-project ../Api/Api.csproj
   ```

3. Aplique no banco Oracle:

   ```bash
   dotnet ef database update      --project Infrastructure.csproj      --startup-project ../Api/Api.csproj
   ```

   Isso cria as tabelas **Adms**, **Usuarios** e **SafeHouses** no Oracle.

### 3. Executar a API

1. No Rider, olhe para o canto superior direito e clique no run do `API`

---

## Documentação dos Endpoints

Todos os controllers usam `[Route("api/[controller]")]`. A base de cada rota é o nome do controller (união do prefixo `/api`). Abaixo, lista completa:

### 1. **AdmController**  
**Base:** `/api/Adm`

- **GET /api/Adm**  
  - Retorna lista de todos os ADMs.  
  - **Resposta**:  
    - `200 OK` + array de `AdmDto`.

- **GET /api/Adm/{id}**  
  - Busca ADM por `id`.  
  - **Resposta**:  
    - `200 OK` + `AdmDto`  
    - `404 Not Found` (lança `ServiceException` se não existir)

- **POST /api/Adm**  
  - Cria um novo ADM.  
  - **Corpo (JSON)**:
    ```json
    {
      "nome": "Fulano",
      "email": "fulano@ex.com",
      "senha": "123456"
    }
    ```
  - **Resposta**:  
    - `201 Created` + `AdmDto`  
    - `400 Bad Request` (lança `ServiceException` se email já existe)

- **PUT /api/Adm/{id}**  
  - Atualiza um ADM (id da rota deve bater com id no body).  
  - **Corpo (JSON)**:
    ```json
    {
      "id": "GUID-do-adm",
      "nome": "NomeAtualizado",
      "email": "novo@ex.com",
      "senha": "novaSenha"
    }
    ```
  - **Resposta**:  
    - `204 No Content`  
    - `400 Bad Request` (se `id` rota ≠ `id` body)  
    - `404 Not Found` (lança `ServiceException` se não existir)

- **DELETE /api/Adm/{id}**  
  - Remove um ADM por `id`.  
  - **Resposta**:  
    - `204 No Content`  
    - `404 Not Found` (lança `ServiceException` se não existir)

- **POST /api/Adm/authenticate**  
  - Autentica um ADM (login).  
  - **Corpo (JSON)**:
    ```json
    {
      "email": "fulano@ex.com",
      "senha": "123456"
    }
    ```
  - **Resposta**:  
    - `200 OK` + `AdmDto`  
    - `400 Bad Request` (lança `ServiceException` se credenciais inválidas)

**DTOs usados:**  
- `CreateAdmDto { string Nome; string Email; string Senha; }`  
- `UpdateAdmDto { Guid Id; string Nome; string Email; string Senha; }`  
- `LoginDto { string Email; string Senha; }`  
- `AdmDto { Guid Id; string Nome; string Email; }`

---

### 2. **UsuarioController**  
**Base:** `/api/Usuario`

- **GET /api/Usuario**  
  - Retorna lista de todos os Usuários.  
  - **Resposta**:  
    - `200 OK` + array de `UsuarioDto`.

- **GET /api/Usuario/{id}**  
  - Busca Usuário por `id`.  
  - **Resposta**:  
    - `200 OK` + `UsuarioDto`  
    - `404 Not Found` (lança `ServiceException` se não existir)

- **POST /api/Usuario**  
  - Cria um novo Usuário.  
  - **Corpo (JSON)**:
    ```json
    {
      "nome": "Beltrano",
      "email": "beltrano@ex.com",
      "senha": "abc123"
    }
    ```
  - **Resposta**:  
    - `201 Created` + `UsuarioDto`  
    - `400 Bad Request` (lança `ServiceException` se email já existe)

- **PUT /api/Usuario/{id}**  
  - Atualiza um Usuário (id da rota deve bater com id no body).  
  - **Corpo (JSON)**:
    ```json
    {
      "id": "GUID-do-usuario",
      "nome": "NomeAtualizado",
      "email": "novo@ex.com",
      "senha": "novaSenha"
    }
    ```
  - **Resposta**:  
    - `204 No Content`  
    - `400 Bad Request` (se `id` rota ≠ `id` body)  
    - `404 Not Found` (lança `ServiceException` se não existir)

- **DELETE /api/Usuario/{id}**  
  - Remove um Usuário por `id`.  
  - **Resposta**:  
    - `204 No Content`  
    - `404 Not Found` (lança `ServiceException` se não existir)

- **POST /api/Usuario/authenticate**  
  - Autentica um Usuário (login).  
  - **Corpo (JSON)**:
    ```json
    {
      "email": "beltrano@ex.com",
      "senha": "abc123"
    }
    ```
  - **Resposta**:  
    - `200 OK` + `UsuarioDto`  
    - `400 Bad Request` (lança `ServiceException` se credenciais inválidas)

**DTOs usados:**  
- `CreateUsuarioDto { string Nome; string Email; string Senha; }`  
- `UpdateUsuarioDto { Guid Id; string Nome; string Email; string Senha; }`  
- `LoginDto { string Email; string Senha; }`  
- `UsuarioDto { Guid Id; string Nome; string Email; }`

---

### 3. **SafeHouseController**  
**Base:** `/api/SafeHouse`

- **GET /api/SafeHouse**  
  - Retorna lista de todas as SafeHouses.  
  - **Resposta**:  
    - `200 OK` + array de `SafeHouseDto`.

- **GET /api/SafeHouse/{id}**  
  - Busca SafeHouse por `id`.  
  - **Resposta**:  
    - `200 OK` + `SafeHouseDto`  
    - `404 Not Found` (lança `ServiceException` se não existir)

- **POST /api/SafeHouse**  
  - Cria uma nova SafeHouse.  
  - **Corpo (JSON)**:
    ```json
    {
      "cep": "01001-000",
      "numero": "123",
      "complemento": "Apto 45",
      "usuarioId": "GUID-do-usuario"
    }
    ```
    > Observação: o controller ignora `usuarioId` no construtor, mas o DTO exige esse campo.  
  - **Resposta**:  
    - `201 Created` + `SafeHouseDto`  
    - `400 Bad Request` (lança `ServiceException` se houver erro)

- **PUT /api/SafeHouse/{id}**  
  - Atualiza uma SafeHouse (id da rota deve bater com id no body).  
  - **Corpo (JSON)**:
    ```json
    {
      "id": "GUID-da-safehouse",
      "cep": "02002-222",
      "numero": "456",
      "complemento": "Casa Nova"
    }
    ```
  - **Resposta**:  
    - `204 No Content`  
    - `400 Bad Request` (se `id` rota ≠ `id` body)  
    - `404 Not Found` (lança `ServiceException` se não existir)

- **DELETE /api/SafeHouse/{id}**  
  - Remove uma SafeHouse por `id`.  
  - **Resposta**:  
    - `204 No Content`  
    - `404 Not Found` (lança `ServiceException` se não existir)

**DTOs usados:**  
- `CreateSafeHouseDto { string CEP; string Numero; string Complemento; Guid UsuarioId; }`  
- `UpdateSafeHouseDto { Guid Id; string CEP; string Numero; string Complemento; }`  
- `SafeHouseDto { Guid Id; string CEP; string Numero; string Complemento; }`

---

### 4. **AlertController**  
**Base:** `/api/Alert`

- **POST /api/Alert/predict**  
  - Recebe três valores (features) e retorna uma severidade (float).  
  - **Corpo (JSON)**:
    ```json
    {
      "feature1": 1.0,
      "feature2": 2.0,
      "feature3": 3.0
    }
    ```
  - **Resposta**:  
    - `200 OK` + `{ float }`

**DTO usado:**  
- `AlertPredictionDto { float Feature1; float Feature2; float Feature3; }`

---

## Instruções de Testes

A solução inclui três projetos de teste:

1. **Domain.Tests** – Testes de entidades  
2. **Application.Tests** – Testes de serviços  
3. **Api.Tests** – Testes de controllers

### 1. Domain.Tests

- Foca em testes de domínio:  
  - Criação de entidades (`Adm`, `Usuario`, `SafeHouse`).  
  - Verificação de hash de senha e métodos de atualização internos.  
- Para executar:
  ```bash
  cd tests/Domain.Tests
  dotnet test
  ```

### 2. Application.Tests

- Verifica lógica dos serviços (`AdmService`, `UsuarioService`, `SafeHouseService`), com **Moq** para os repositórios.  
- Cenários incluídos:  
  - **Criação** (e-mail já existe → `ServiceException`; e-mail novo → sucesso).  
  - **Autenticação** (credenciais inválidas → `ServiceException`; válidas → retorna entidade).  
  - **Busca por ID** (não existe → `ServiceException`; existe → retorna entidade).  
  - **Atualização/Remoção** (cenários de sucesso e erro).  
- Para executar:
  ```bash
  cd tests/Application.Tests
  dotnet test
  ```

### 3. Api.Tests

- Testes de controllers (`AdmController`, `UsuarioController`, `SafeHouseController`, `AlertController`) com **Moq** para os serviços.  
- Verifica respostas HTTP:  
  - `200 OK` com corpo (lista ou DTO)  
  - `201 Created` com DTO retornado  
  - `204 No Content` para atualizações e deleções bem-sucedidas  
  - Propagação de `ServiceException` quando o serviço lança erro (tratado como exceção no teste)  
- Para executar:
  ```bash
  cd tests/Api.Tests
  dotnet test
  ```

> **Dica**: Na raiz da solução, rodar `dotnet test` executa todos os três projetos de teste.
---
## Integrantes
- #### Enzo Antunes Oliveira RM553185
- #### Arhtur Fenili RM552752
- #### Vinicio Rapahel RM553813
---

## Observações Finais

- **Clean Architecture & DDD** garantem separação clara entre camadas e regras de negócio.  
- **Controller → Service → Repository → DbContext**: fluxo de chamada.  
- **Swagger** fornece documentação automática e interativa em modo de desenvolvimento.  
- **Rate Limiting** impede abusos, retornando `429 Too Many Requests` após 60 chamadas/minuto.  
- **ML.NET** no `AlertPredictionService` usa o modelo treinado por `Infrastructure/ML/AlertModelBuilder`.  
- É possível trocar facilmente o provider de banco (por ex. SQL Server, SQLite) ajustando a chamada `UseOracle` no `Program.cs`.

---

> **Obrigado por usar o GlobalSolution.DotNet!**  
> Para dúvidas ou contribuições, abra uma issue ou envie um pull request.
