# Transactions API

Uma API RESTful para gerenciamento de transações com autenticação JWT.

## Tecnologias Utilizadas

- .NET 8
- Entity Framework Core
- PostgreSQL
- JWT Authentication
- AutoMapper
- Swagger
- BCrypt.Net-Next
- CSVHelper

## Design Patterns e Boas Práticas

O projeto foi desenvolvido seguindo padrões de design e boas práticas que facilitam a manutenção, escalabilidade e testabilidade do código:

### Arquitetura em Camadas
O sistema é organizado em camadas com responsabilidades bem definidas:
- **Controllers**: Responsáveis por receber requisições HTTP e retornar respostas apropriadas
- **Services**: Implementam a lógica de negócio da aplicação
- **Repositories**: Gerenciam o acesso aos dados
- **Models**: Representam as entidades do domínio

### Repository Pattern
Implementado para abstrair e encapsular o acesso aos dados, permitindo uma separação clara entre a lógica de negócio e a camada de persistência.

### Unit of Work Pattern
Utilizado para garantir a consistência das transações de banco de dados, agrupando operações relacionadas em uma única unidade atômica de trabalho.

### Dependency Injection
Aplicado para reduzir o acoplamento entre os componentes do sistema, facilitar testes unitários e melhorar a manutenibilidade do código.

### DTO Pattern (Data Transfer Objects)
Implementado para transferir dados entre as camadas do sistema, evitando a exposição direta das entidades e melhorando a segurança.

### Middleware Pattern
Utilizado para processar requisições HTTP, implementando funcionalidades como autenticação, autorização e tratamento de exceções.

### Entity Mapping
Implementado com AutoMapper para facilitar a conversão entre entidades do domínio e DTOs, reduzindo código repetitivo.

## Pré-requisitos

- [.NET SDK](https://dotnet.microsoft.com/download) (versão 8.0 ou superior)
- [PostgreSQL](https://www.postgresql.org/download/)
- IDE de sua preferência (Visual Studio, VS Code, JetBrains Rider)

## Configuração

### 1. Banco de Dados

Certifique-se de ter o PostgreSQL instalado e rodando. Você precisará configurar a string de conexão no arquivo `appsettings.json`.

### 2. Configuração do appsettings.json

Crie ou atualize o arquivo `appsettings.json` na raiz do projeto com as seguintes configurações:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=transactions_db;Username=seu_usuario;Password=sua_senha"
  },
  "JwtSettings": {
    "SecretKey": "sua_chave_secreta_aqui_use_uma_chave_forte_com_pelo_menos_32_caracteres",
    "Issuer": "TransactionsAPI",
    "Audience": "TransactionsAPIUser",
    "ExpiryMinutes": 60
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

Substitua `seu_usuario` e `sua_senha` pelas suas credenciais do PostgreSQL. Defina uma chave secreta forte para o JWT.

## Executando o Projeto

### Usando Visual Studio

1. Abra a solução no Visual Studio
2. Restaure os pacotes NuGet (botão direito na solução > Restaurar Pacotes NuGet)
3. Pressione F5 ou clique em "Iniciar" para executar o projeto
4. O Swagger deve abrir automaticamente no navegador

### Usando CLI do .NET

1. Clone o repositório:
   ```bash
   git clone [URL_DO_REPOSITORIO]
   cd [NOME_DA_PASTA]
   ```

2. Restaure os pacotes:
   ```bash
   dotnet restore
   ```

3. Execute as migrations (opcional, pois o programa as aplica automaticamente na inicialização):
   ```bash
   dotnet ef database update
   ```

4. Execute o projeto:
   ```bash
   dotnet run
   ```

5. Acesse o Swagger para testar a API:
   ```
   https://localhost:7176/swagger
   ```
   (A porta pode variar dependendo da configuração)

## Autenticação

A API utiliza autenticação JWT. Para acessar endpoints protegidos:

1. Registre um usuário ou faça login para obter um token JWT
2. Inclua o token no cabeçalho de autorização:
   ```
   Authorization: Bearer [seu_token_jwt]
   ```

## Endpoints Principais

- **/api/users/register** - Registrar novo usuário
- **/api/users/login** - Fazer login e obter token JWT
- **/api/transactions** - CRUD de transações (requer autenticação)

## Serviços Externos

A API integra com o serviço ViaCEP para validação de endereços.

## Solução de Problemas

- Se encontrar erros de conexão com o banco de dados, verifique se o PostgreSQL está rodando e se a string de conexão está correta.
- Para problemas com JWT, verifique se a chave secreta tem pelo menos 32 caracteres.
- Consulte os logs para mais detalhes sobre possíveis erros.
