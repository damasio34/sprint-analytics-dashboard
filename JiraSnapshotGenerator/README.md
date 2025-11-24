# 🚀 Jira Snapshot Generator

Ferramenta de linha de comando em .NET para gerar snapshots JSON a partir do Jira, prontos para importação no **Sprint Analytics Dashboard**.

## 📋 Índice

- [Visão Geral](#-visão-geral)
- [Pré-requisitos](#-pré-requisitos)
- [Instalação](#-instalação)
- [Configuração](#️-configuração)
- [Como Usar](#-como-usar)
- [Campos Customizados](#-campos-customizados)
- [Mapeamentos](#-mapeamentos)
- [Troubleshooting](#-troubleshooting)

---

## 🎯 Visão Geral

Esta ferramenta conecta-se ao Jira via API REST, coleta informações de issues (incluindo changelog completo), e gera arquivos JSON no formato esperado pelo Dashboard BI de Análise de Sprints.

### Funcionalidades

✅ **Conexão com Jira** - Autenticação Basic Auth (usuário/senha ou token)  
✅ **Busca Flexível** - JQL customizável para filtrar issues  
✅ **Changelog Completo** - Coleta histórico de mudanças de status  
✅ **Mapeamento Inteligente** - Converte status, prioridades e tipos do Jira para formato do dashboard  
✅ **Cálculo de Duração** - Calcula tempo em cada status automaticamente  
✅ **Validação de Dados** - Story points ajustados para valores Fibonacci válidos  
✅ **Interface Amigável** - Menu interativo com Spectre.Console  
✅ **Geração Automática** - Cria e atualiza `snapshots.json` automaticamente  

---

## 🔧 Pré-requisitos

- **.NET 8.0 SDK** ou superior
- Acesso ao Jira (usuário + senha/token)
- Permissões para ler issues e changelog

### Instalar .NET 8.0

**Windows:**
```powershell
winget install Microsoft.DotNet.SDK.8
```

**Linux (Ubuntu/Debian):**
```bash
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 8.0
```

**macOS:**
```bash
brew install dotnet@8
```

---

## 📥 Instalação

### 1. Clone ou Extraia o Projeto

```bash
cd JiraSnapshotGenerator
```

### 2. Restaurar Dependências

```bash
dotnet restore
```

### 3. Build do Projeto

```bash
dotnet build
```

---

## ⚙️ Configuração

### Arquivo `appsettings.json`

Edite o arquivo `appsettings.json` com suas configurações:

```json
{
  "JiraSettings": {
    "BaseUrl": "http://jira.sua-empresa.com:8080/jira",
    "Username": "seu-usuario",
    "Password": "sua-senha-ou-token",
    "ProjectKey": "PROJ",
    "DefaultJql": "project=PROJ AND issuetype in (Bug, Task) AND status=Done",
    "MaxResults": 1000
  },
  "SprintSettings": {
    "SprintName": "Sprint 01/2025",
    "SprintId": "sprint-2025-01",
    "StartDate": "2025-01-01T00:00:00Z",
    "EndDate": "2025-01-31T23:59:59Z",
    "Goal": "Entregas de Janeiro"
  },
  "TeamSettings": {
    "DefaultCapacity": 40,
    "Members": [
      {
        "Name": "João Silva",
        "Email": "joao.silva@empresa.com",
        "Role": "Developer",
        "Capacity": 40,
        "JiraUsername": "jsilva"
      }
    ]
  }
}
```

### 📝 Configurações Importantes

| Campo | Descrição | Exemplo |
|-------|-----------|---------|
| `BaseUrl` | URL completa do Jira | `http://jira.empresa.com:8080/jira` |
| `Username` | Usuário do Jira | `usuario` |
| `Password` | Senha ou API Token | `senha123` ou `ATATT3x...` |
| `ProjectKey` | Chave do projeto | `CROSS`, `PROJ`, etc |
| `DefaultJql` | JQL padrão para busca | Ver exemplos abaixo |
| `JiraUsername` | Username no Jira (para mapeamento) | Mesmo que aparece em `assignee` |

### 🔑 Autenticação

**Opção 1: Senha**
```json
"Username": "usuario",
"Password": "minha-senha"
```

**Opção 2: API Token (Recomendado)**
```json
"Username": "usuario",
"Password": "ATATT3xFfGF0..."
```

Para gerar um token de API:
1. Acesse o Jira → Perfil → Segurança
2. Crie um novo API Token
3. Copie e cole no campo `Password`

---

## 🚀 Como Usar

### Executar o Programa

```bash
cd JiraSnapshotGenerator
dotnet run
```

### Menu Interativo

Você verá um menu com opções:

```
🚀 Gerar snapshot com configurações padrão
🔧 Gerar snapshot com JQL customizado
⚙️  Mostrar configurações atuais
📚 Ajuda
❌ Sair
```

### Opção 1: Snapshot Padrão

1. Selecione **"Gerar snapshot com configurações padrão"**
2. Aguarde a coleta de dados
3. Revise o resumo exibido
4. Confirme o salvamento

### Opção 2: JQL Customizado

1. Selecione **"Gerar snapshot com JQL customizado"**
2. Digite o JQL desejado (ex: `project=CROSS AND sprint="Sprint 112"`)
3. Informe o nome da sprint
4. Informe o ID do arquivo (ex: `sprint-2025-02`)
5. Aguarde a geração
6. Confirme o salvamento

### Saída

Os arquivos são salvos em `./output/`:

```
output/
├── sprint-2025-01.json      # Snapshot gerado
└── snapshots.json           # Índice (atualizado automaticamente)
```

### Importar no Dashboard

1. Copie os arquivos de `output/`
2. Cole na pasta `data/` do dashboard
3. Recarregue o dashboard (F5)
4. Selecione o novo snapshot

---

## 🔧 Campos Customizados

O Jira permite campos customizados que variam por instalação. Os mais comuns:

### Descobrir IDs de Campos

**Via API:**
```bash
curl -u usuario:senha \
  http://jira.empresa.com:8080/jira/rest/api/2/field
```

**Via Postman:**
```
GET http://jira.empresa.com:8080/jira/rest/api/2/field
```

### Campos Típicos

| Campo | ID Comum | Como Configurar |
|-------|----------|-----------------|
| Story Points | `customfield_10122` | `MappingSettings.StoryPointsField` |
| Sprint | `customfield_10751` | `MappingSettings.SprintField` |
| Epic Link | `customfield_10014` | Adicione conforme necessário |

### Atualizar no `appsettings.json`

```json
"MappingSettings": {
  "StoryPointsField": "customfield_10122",
  "SprintField": "customfield_10751"
}
```

---

## 🗺️ Mapeamentos

A ferramenta mapeia valores do Jira para o formato do dashboard:

### Status

| Jira | Dashboard |
|------|-----------|
| Open, Reopened | `todo` |
| In Progress | `in_progress` |
| In Review, Ready to Test | `in_review` |
| Resolved, Closed, Done | `done` |
| Blocked | `blocked` |

**Customizar:**
```json
"StatusMapping": {
  "Backlog": "todo",
  "Development": "in_progress",
  "Testing": "in_review"
}
```

### Prioridade

| Jira | Dashboard |
|------|-----------|
| Blocker, Critical | `urgent` |
| Major | `high` |
| Minor | `medium` |
| Trivial | `low` |

**Customizar:**
```json
"PriorityMapping": {
  "Highest": "urgent",
  "High": "high",
  "Medium": "medium",
  "Low": "low",
  "Lowest": "low"
}
```

### Tipo

| Jira | Dashboard |
|------|-----------|
| Bug | `bug` |
| Improvement | `improvement` |
| New Feature, Task | `feature` |
| Technical task | `technical_debt` |

**Customizar:**
```json
"TypeMapping": {
  "Story": "feature",
  "Epic": "feature",
  "Sub-task": "feature"
}
```

---

## 📝 Exemplos de JQL

### Por Sprint

```jql
project=CROSS AND sprint="Sprint 112"
```

### Por Período

```jql
project=CROSS AND resolved >= "2025-01-01" AND resolved <= "2025-01-31"
```

### Por Status

```jql
project=CROSS AND status IN (Done, Closed) AND resolved >= startOfMonth()
```

### Por Assignee

```jql
project=CROSS AND assignee=currentUser() AND status=Done
```

### Por Tipo

```jql
project=CROSS AND issuetype IN (Bug, "New Feature") AND statusCategory=Done
```

### Complexo

```jql
project=CROSS 
  AND sprint IN openSprints() 
  AND issuetype NOT IN (Epic, Sub-task) 
  AND status != Cancelled
```

---

## 🐛 Troubleshooting

### ❌ Erro: "Could not connect to Jira"

**Causas:**
- URL incorreta
- Credenciais inválidas
- Firewall/VPN

**Soluções:**
```bash
# Testar conexão
curl -u usuario:senha http://jira.empresa.com:8080/jira/rest/api/2/myself

# Verificar conectividade
ping jira.empresa.com

# Testar com navegador
# Abra: http://jira.empresa.com:8080/jira
```

### ❌ Erro: "Unauthorized"

**Causas:**
- Credenciais incorretas
- Token expirado
- Falta de permissões

**Soluções:**
1. Gere um novo API Token
2. Verifique se tem permissão de leitura no projeto
3. Teste login no navegador

### ❌ Erro: "No issues found"

**Causas:**
- JQL muito restritivo
- Projeto vazio
- Permissões insuficientes

**Soluções:**
```jql
# Teste com JQL mais simples
project=CROSS

# Verifique se existem issues
project=CROSS AND created >= -30d
```

### ❌ Erro: "Field customfield_XXXXX does not exist"

**Causas:**
- ID de campo incorreto
- Campo não existe na instalação

**Soluções:**
1. Liste todos os campos:
   ```bash
   curl -u user:pass http://jira.../rest/api/2/field
   ```
2. Encontre o ID correto de Story Points
3. Atualize `StoryPointsField` no `appsettings.json`

### ⚠️ Story Points com Valores Estranhos

A ferramenta ajusta automaticamente para Fibonacci válido:

| Jira | Dashboard |
|------|-----------|
| 1.5 | 2 |
| 4 | 3 ou 5 (mais próximo) |
| 10 | 8 ou 13 (mais próximo) |

### 📊 Membros Não Aparecem

**Causas:**
- `JiraUsername` não corresponde ao `assignee` do Jira

**Soluções:**
1. Verifique o username correto:
   ```json
   // No JSON de resposta do Jira
   "assignee": {
     "name": "jsilva",  // Use este valor
     "displayName": "João Silva"
   }
   ```

2. Atualize `appsettings.json`:
   ```json
   {
     "Name": "João Silva",
     "JiraUsername": "jsilva"  // Deve corresponder exatamente
   }
   ```

---

## 🎓 Dicas Avançadas

### Executar em Modo Release

```bash
dotnet run --configuration Release
```

### Gerar Executável

```bash
dotnet publish -c Release -r win-x64 --self-contained
# Executável em: bin/Release/net8.0/win-x64/publish/
```

### Automatizar com Script

**PowerShell:**
```powershell
# gerar-snapshot.ps1
cd C:\JiraSnapshotGenerator
dotnet run -- --auto
Copy-Item output\*.json C:\dashboard\data\
```

**Bash:**
```bash
#!/bin/bash
# gerar-snapshot.sh
cd /home/user/JiraSnapshotGenerator
dotnet run -- --auto
cp output/*.json /var/www/dashboard/data/
```

### Agendar Execução

**Windows (Task Scheduler):**
1. Abra Agendador de Tarefas
2. Criar Tarefa Básica
3. Ação: `powershell.exe -File C:\gerar-snapshot.ps1`
4. Agende (diário, semanal, etc)

**Linux (Cron):**
```bash
# Executar diariamente às 8h
0 8 * * * /home/user/gerar-snapshot.sh
```

---

## 📚 Arquitetura

```
JiraSnapshotGenerator/
├── Models/
│   ├── AppSettings.cs       # Configurações da aplicação
│   ├── JiraModels.cs        # Modelos da API do Jira
│   └── DashboardModels.cs   # Modelos do formato Dashboard
├── Services/
│   ├── JiraClient.cs        # Cliente HTTP para Jira API
│   ├── SnapshotConverter.cs # Conversor Jira → Dashboard
│   └── SnapshotGenerator.cs # Orquestrador principal
├── Program.cs               # Ponto de entrada + Menu
└── appsettings.json         # Configurações
```

---

## 🤝 Contribuindo

1. Fork o projeto
2. Crie uma branch (`git checkout -b feature/nova-funcionalidade`)
3. Commit suas mudanças (`git commit -m 'Adiciona nova funcionalidade'`)
4. Push para a branch (`git push origin feature/nova-funcionalidade`)
5. Abra um Pull Request

---

## 📄 Licença

MIT License - veja LICENSE para detalhes

---

## 📞 Suporte

- 📧 Email: suporte@empresa.com
- 📚 Wiki: [link-para-wiki]
- 🐛 Issues: [link-para-issues]

---

## ✅ Checklist de Primeira Execução

- [ ] .NET 8.0 SDK instalado
- [ ] Projeto baixado e extraído
- [ ] `dotnet restore` executado
- [ ] `appsettings.json` configurado com credenciais
- [ ] Membros do time adicionados
- [ ] JQL testado e validado
- [ ] `dotnet run` executado com sucesso
- [ ] Snapshot gerado em `output/`
- [ ] Arquivos copiados para dashboard
- [ ] Dashboard atualizado e funcionando

---

**🎉 Pronto! Você está pronto para gerar snapshots do Jira automaticamente!**
