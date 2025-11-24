# ⚡ Guia de Início Rápido - 5 Minutos

## 🎯 Objetivo

Gerar seu primeiro snapshot do Jira em menos de 5 minutos.

---

## 📋 Pré-requisitos

- [ ] .NET 8.0 SDK instalado
- [ ] Acesso ao Jira (usuário + senha/token)
- [ ] Conhecer a chave do seu projeto (ex: CROSS, PROJ)

---

## 🚀 Passo 1: Instalar .NET 8.0 (se necessário)

**Windows:**
```powershell
winget install Microsoft.DotNet.SDK.8
```

**Linux:**
```bash
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 8.0
```

**macOS:**
```bash
brew install dotnet@8
```

**Verificar instalação:**
```bash
dotnet --version
# Deve retornar 8.x.x
```

---

## 📁 Passo 2: Preparar o Projeto

```bash
# Entrar na pasta
cd JiraSnapshotGenerator

# Restaurar dependências
dotnet restore
```

---

## ⚙️ Passo 3: Configurar Credenciais

Edite `JiraSnapshotGenerator/appsettings.json`:

### Configuração Mínima

```json
{
  "JiraSettings": {
    "BaseUrl": "http://seu-jira.com:8080/jira",
    "Username": "seu-usuario",
    "Password": "sua-senha-ou-token",
    "ProjectKey": "PROJ",
    "DefaultJql": "project=PROJ AND resolved >= \"2025-01-01\""
  },
  "TeamSettings": {
    "Members": [
      {
        "Name": "Seu Nome",
        "Email": "seu.email@empresa.com",
        "Role": "Developer",
        "Capacity": 40,
        "JiraUsername": "seu-usuario-jira"
      }
    ]
  }
}
```

### ✅ Checklist de Configuração

- [ ] `BaseUrl` aponta para seu Jira
- [ ] `Username` e `Password` estão corretos
- [ ] `ProjectKey` é a chave do seu projeto
- [ ] `JiraUsername` corresponde ao username no Jira
- [ ] Pelo menos 1 membro configurado

---

## ▶️ Passo 4: Executar

```bash
cd JiraSnapshotGenerator
dotnet run
```

Você verá:

```
     _  _                ____                        _           _   
    | |(_)_ __ __ _     / ___| _ __   __ _ _ __  ___| |__   ___ | |_ 
 _  | || | '__/ _` |    \___ \| '_ \ / _` | '_ \/ __| '_ \ / _ \| __|
| |_| || | | | (_| |     ___) | | | | (_| | |_) \__ \ | | | (_) | |_ 
 \___/ |_|_|  \__,_|    |____/|_| |_|\__,_| .__/|___/_| |_|\___/ \__|
                                           |_|                        

Dashboard BI - Gerador de Snapshots do Jira
Versão 1.0.0

O que você deseja fazer?
> 🚀 Gerar snapshot com configurações padrão
  🔧 Gerar snapshot com JQL customizado
  ⚙️  Mostrar configurações atuais
  📚 Ajuda
  ❌ Sair
```

---

## 🎮 Passo 5: Gerar Snapshot

1. **Selecione** a primeira opção: `🚀 Gerar snapshot com configurações padrão`
2. **Aguarde** enquanto coleta dados do Jira
3. **Revise** o resumo exibido
4. **Confirme** com `Y` para salvar

Você verá algo como:

```
🔍 Buscando issues com JQL: project=CROSS AND resolved >= "2025-01-01"
✅ Encontradas 61 issues
📥 [1/61] Buscando changelog de CROSS-2828...
...
✅ Changelog coletado de 61 issues

🔄 Convertendo dados do Jira para formato do Dashboard...
✅ Snapshot gerado com sucesso!
   📊 Sprint: Sprint 2025-01
   👥 Time: 2 membros
   📝 Tasks: 61 tarefas

============================================================
📊 RESUMO DO SNAPSHOT
============================================================

🏃 Sprint: Sprint 2025-01
   ID: sprint-2025-01
   ...

💾 Deseja salvar este snapshot? (y/n): y

💾 Snapshot salvo em: ./output/sprint-2025-01.json
📑 Índice atualizado: ./output/snapshots.json
   Total de snapshots: 1

✅ Snapshot salvo com sucesso!
```

---

## 📊 Passo 6: Importar no Dashboard

```bash
# Copiar arquivos gerados
cp output/*.json /caminho/para/dashboard/data/

# OU manualmente:
# 1. Abra a pasta output/
# 2. Copie sprint-2025-01.json e snapshots.json
# 3. Cole em: dashboard/data/
```

---

## 🌐 Passo 7: Visualizar

1. Acesse o dashboard: `http://localhost:3000`
2. Recarregue a página (F5)
3. Selecione o snapshot `sprint-2025-01`
4. 🎉 **Sucesso!** Seu snapshot está carregado

---

## ❓ Problemas Comuns

### ❌ "Could not connect"

**Solução:**
```bash
# Testar conexão
curl http://seu-jira.com:8080/jira/rest/api/2/myself

# Se falhar, verifique:
# - URL está correta?
# - Está conectado à VPN?
# - Firewall bloqueando?
```

### ❌ "Unauthorized"

**Solução:**
1. Verifique username e password
2. Teste login no navegador
3. Ou gere um API Token

**Gerar API Token:**
1. Jira → Perfil → Segurança → API Tokens
2. Criar novo token
3. Copiar e colar no `Password`

### ❌ "No issues found"

**Solução:**
```json
// Teste com JQL mais simples
"DefaultJql": "project=PROJ"

// Ou apenas issues recentes
"DefaultJql": "project=PROJ AND created >= -30d"
```

### ⚠️ Membros não aparecem

**Solução:**
```json
// Verifique se JiraUsername corresponde ao assignee do Jira
{
  "Name": "João Silva",
  "JiraUsername": "jsilva"  // Deve ser EXATAMENTE como aparece no Jira
}
```

---

## 🎓 Próximos Passos

Agora que você gerou seu primeiro snapshot:

1. ✅ **Customize o JQL** para suas necessidades
2. ✅ **Adicione mais membros** do time
3. ✅ **Ajuste os mapeamentos** de status/prioridade
4. ✅ **Gere snapshots periódicos** (diário, semanal)
5. ✅ **Automatize com scripts** (ver README completo)

---

## 📚 Documentação Completa

Para mais detalhes, consulte:
- [README.md](README.md) - Documentação completa
- [appsettings.json](JiraSnapshotGenerator/appsettings.json) - Todas as configurações

---

## 🆘 Precisa de Ajuda?

Execute o programa e escolha:
```
📚 Ajuda
```

Ou consulte a seção de Troubleshooting no README.

---

## ✅ Checklist Final

- [ ] .NET instalado e funcionando
- [ ] Projeto restaurado (`dotnet restore`)
- [ ] Credenciais configuradas
- [ ] Snapshot gerado com sucesso
- [ ] Arquivos em `output/`
- [ ] Arquivos copiados para dashboard
- [ ] Dashboard mostrando dados

---

**🎉 Parabéns! Você gerou seu primeiro snapshot em menos de 5 minutos!**

**Tempo total estimado: 3-5 minutos** ⏱️
