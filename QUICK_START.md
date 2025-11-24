# 🚀 Quick Start - Corporate Framework

## Conteúdo do Pacote

Este ZIP contém um **Framework Corporativo completo** implementado em .NET 8 com:

✅ **Clean Architecture** + **DDD** + **CQRS**  
✅ **Exemplo funcional** de domínio (Orders/Pedidos)  
✅ **Testes automatizados** com xUnit  
✅ **Documentação completa** e guias de uso  
✅ **Postman Collection** para testes da API  

## 📋 Pré-requisitos

- **.NET 8.0 SDK** ([Download](https://dotnet.microsoft.com/download/dotnet/8.0))
- **Visual Studio 2022** ou **VS Code** (opcional)
- **Postman** (opcional, para testes)

## ⚡ Início Rápido (3 passos)

### 1. Extrair e Restaurar

```bash
# Extrair o ZIP
unzip corporate-framework.zip
cd corporate-framework

# Restaurar pacotes NuGet
dotnet restore
```

### 2. Compilar

```bash
# Compilar toda a solução
dotnet build
```

### 3. Executar

```bash
# Navegar para a API
cd src/CorporateFramework.API

# Executar a aplicação
dotnet run

# A API estará disponível em: https://localhost:5001
# Swagger (documentação interativa): https://localhost:5001
```

## 🧪 Executar Testes

```bash
# Na raiz do projeto
dotnet test
```

## 📚 Estrutura do Projeto

```
corporate-framework/
├── README.md                           ← Documentação principal
├── ARCHITECTURE.md                     ← Detalhes da arquitetura
├── USAGE_GUIDE.md                      ← Guia de uso completo
├── CorporateFramework.postman_collection.json  ← Collection do Postman
├── CorporateFramework.sln              ← Solução Visual Studio
├── src/
│   ├── CorporateFramework.Domain/      ← Camada de Domínio (Core)
│   ├── CorporateFramework.Application/ ← Camada de Aplicação (Use Cases)
│   ├── CorporateFramework.Infrastructure/ ← Infraestrutura
│   └── CorporateFramework.API/         ← API REST
└── tests/
    └── CorporateFramework.Tests/       ← Testes automatizados
```

## 🎯 Testando a API

### Opção 1: Via Swagger
1. Abra https://localhost:5001 no navegador
2. Use a interface interativa do Swagger para testar os endpoints

### Opção 2: Via Postman
1. Importe o arquivo `CorporateFramework.postman_collection.json`
2. Configure a variável `baseUrl` para `https://localhost:5001`
3. Execute as requisições da collection

### Opção 3: Via cURL

```bash
# Criar um pedido
curl -X POST https://localhost:5001/api/orders \
  -H "Content-Type: application/json" \
  -d '{
    "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "items": [
      {
        "productId": "3fa85f64-5717-4562-b3fc-2c963f66afa7",
        "productName": "Notebook Dell",
        "unitPrice": 3500.00,
        "currency": "BRL",
        "quantity": 2
      }
    ],
    "createdBy": "user@example.com"
  }' | jq

# Listar todos os pedidos
curl https://localhost:5001/api/orders | jq
```

## 📖 Documentação

### 1. **README.md**
Documentação principal com visão geral do framework

### 2. **ARCHITECTURE.md**
Explicação detalhada da arquitetura, padrões e princípios

### 3. **USAGE_GUIDE.md**
Guia passo-a-passo de como usar e estender o framework

## 🎓 Principais Conceitos Implementados

### Clean Architecture
- Separação em camadas com dependências bem definidas
- Domínio no centro, independente de frameworks

### Domain-Driven Design (DDD)
- **Entities:** Classes com identidade única
- **Value Objects:** Objetos imutáveis comparados por valor (Money, Email)
- **Aggregates:** Order como agregado raiz
- **Domain Events:** Comunicação entre agregados
- **Repositories:** Abstração de persistência

### CQRS Pattern
- **Commands:** Operações de escrita (Create, Update, Delete)
- **Queries:** Operações de leitura (Get, List, Search)
- Handlers separados para cada responsabilidade

### Padrões de Projeto
- Repository Pattern
- Unit of Work Pattern
- Mediator Pattern (MediatR)
- Factory Pattern
- Strategy Pattern (validadores)

### Princípios SOLID
- **S**ingle Responsibility
- **O**pen/Closed
- **L**iskov Substitution
- **I**nterface Segregation
- **D**ependency Inversion

## 🔧 Configuração

O projeto usa **InMemory Database** por padrão, não requer configuração adicional.

Para usar SQL Server, edite `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=CorporateFrameworkDb;Trusted_Connection=true;"
  }
}
```

E atualize `Program.cs`:

```csharp
// Trocar UseInMemoryDatabase por UseSqlServer
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

## 🆘 Solução de Problemas

### Erro: "Unable to bind to https://localhost:5001"
**Solução:** A porta pode estar em uso. Altere em `launchSettings.json` ou execute:
```bash
dotnet run --urls "http://localhost:5050"
```

### Erro: "The type or namespace name 'MediatR' could not be found"
**Solução:** Restaure os pacotes NuGet:
```bash
dotnet restore
```

### Erro ao executar testes
**Solução:** Compile o projeto primeiro:
```bash
dotnet build
dotnet test
```

## 📞 Suporte e Mais Informações

- **Documentação completa:** Consulte `README.md`
- **Arquitetura detalhada:** Consulte `ARCHITECTURE.md`
- **Guia de uso:** Consulte `USAGE_GUIDE.md`
- **Exemplos de código:** Explore os arquivos no diretório `src/`

## 🎯 Próximos Passos

1. ✅ Execute o projeto e explore o Swagger
2. ✅ Rode os testes para entender o comportamento
3. ✅ Leia o `USAGE_GUIDE.md` para aprender a estender
4. ✅ Implemente seu próprio domínio seguindo os exemplos
5. ✅ Adapte o framework às necessidades da sua empresa

---

**Desenvolvido com ❤️ seguindo as melhores práticas de engenharia de software**
