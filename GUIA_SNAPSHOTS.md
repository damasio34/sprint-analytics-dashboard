# 📋 Guia de Criação de Snapshots

## Como Criar Seus Próprios Snapshots JSON

### Estrutura Básica

```json
{
  "id": "identificador-unico",
  "name": "Nome da Sprint",
  "startDate": "2024-01-08T00:00:00Z",
  "endDate": "2024-01-22T00:00:00Z",
  "goal": "Objetivo principal da sprint",
  "metadata": {
    "capturedAt": "2024-01-22T18:00:00Z",
    "version": "1.0"
  },
  "team": [ /* array de membros */ ],
  "tasks": [ /* array de tarefas */ ]
}
```

---

## 📝 Campos Obrigatórios

### Sprint

| Campo | Tipo | Descrição | Exemplo |
|-------|------|-----------|---------|
| id | string | Identificador único | "sprint-2024-01" |
| name | string | Nome da sprint | "Sprint 01/2024" |
| startDate | ISO 8601 | Data início | "2024-01-08T00:00:00Z" |
| endDate | ISO 8601 | Data fim | "2024-01-22T00:00:00Z" |
| goal | string | Objetivo | "Implementar autenticação" |

### Team Member

| Campo | Tipo | Descrição | Exemplo |
|-------|------|-----------|---------|
| id | string | Identificador | "dev-001" |
| name | string | Nome completo | "João Silva" |
| email | string | Email | "joao@empresa.com" |
| role | string | Cargo | "Senior Developer" |
| capacity | number | Horas disponíveis | 40 |

### Task

| Campo | Tipo | Descrição | Valores/Exemplo |
|-------|------|-----------|-----------------|
| id | string | Identificador | "TASK-001" |
| title | string | Título | "Implementar login" |
| assignee | string | Responsável | "João Silva" |
| points | number | Pontuação | 1, 2, 3, 5, 8, 13 |
| status | string | Estado atual | Ver lista abaixo |
| priority | string | Prioridade | low, medium, high, urgent |
| type | string | Tipo | feature, bug, improvement, technical_debt |
| createdAt | ISO 8601 | Data criação | "2024-01-08T09:00:00Z" |
| startedAt | ISO 8601 | Data início (opcional) | "2024-01-08T14:00:00Z" |
| completedAt | ISO 8601 | Data conclusão (opcional) | "2024-01-12T16:30:00Z" |
| sprint | string | Nome da sprint | "Sprint 01/2024" |
| statusHistory | array | Histórico estados | Ver abaixo |

**Status possíveis:**
- `backlog`: No backlog
- `todo`: A fazer
- `in_progress`: Em progresso
- `in_review`: Em revisão
- `blocked`: Bloqueada
- `done`: Concluída
- `cancelled`: Cancelada

### Status History Entry

| Campo | Tipo | Descrição | Exemplo |
|-------|------|-----------|---------|
| from | string | Status anterior | "todo" |
| to | string | Novo status | "in_progress" |
| changedAt | ISO 8601 | Data mudança | "2024-01-08T14:00:00Z" |
| changedBy | string | Quem mudou | "João Silva" |
| duration | number | Horas no estado anterior | 5.5 |

---

## 🎯 Exemplo Mínimo (Tarefa Simples)

```json
{
  "id": "task-simple",
  "title": "Tarefa Simples",
  "assignee": "Dev 1",
  "points": 3,
  "status": "done",
  "priority": "medium",
  "type": "feature",
  "createdAt": "2024-01-08T09:00:00Z",
  "startedAt": "2024-01-08T10:00:00Z",
  "completedAt": "2024-01-09T15:00:00Z",
  "sprint": "Sprint 01",
  "statusHistory": [
    {
      "from": "todo",
      "to": "in_progress",
      "changedAt": "2024-01-08T10:00:00Z",
      "changedBy": "Dev 1",
      "duration": 1
    },
    {
      "from": "in_progress",
      "to": "done",
      "changedAt": "2024-01-09T15:00:00Z",
      "changedBy": "Dev 1",
      "duration": 29
    }
  ]
}
```

---

## 📊 Exemplo Completo (Tarefa com Retrabalho)

```json
{
  "id": "TASK-005",
  "title": "Implementar API de Pagamento",
  "description": "Integração com gateway de pagamento",
  "assignee": "Maria Santos",
  "points": 5,
  "status": "done",
  "priority": "high",
  "type": "feature",
  "createdAt": "2024-01-08T09:00:00Z",
  "startedAt": "2024-01-10T09:00:00Z",
  "completedAt": "2024-01-18T17:00:00Z",
  "sprint": "Sprint 01/2024",
  "labels": ["payment", "integration", "backend"],
  "comments": [
    {
      "author": "Tech Lead",
      "text": "Lembrar de adicionar testes de integração",
      "createdAt": "2024-01-10T10:00:00Z"
    }
  ],
  "statusHistory": [
    {
      "from": "backlog",
      "to": "todo",
      "changedAt": "2024-01-08T09:00:00Z",
      "changedBy": "Product Owner",
      "duration": 0
    },
    {
      "from": "todo",
      "to": "in_progress",
      "changedAt": "2024-01-10T09:00:00Z",
      "changedBy": "Maria Santos",
      "duration": 48
    },
    {
      "from": "in_progress",
      "to": "in_review",
      "changedAt": "2024-01-15T16:00:00Z",
      "changedBy": "Maria Santos",
      "duration": 127
    },
    {
      "from": "in_review",
      "to": "in_progress",
      "changedAt": "2024-01-16T10:00:00Z",
      "changedBy": "Code Reviewer",
      "duration": 18,
      "comment": "Ajustes necessários na validação"
    },
    {
      "from": "in_progress",
      "to": "in_review",
      "changedAt": "2024-01-17T14:00:00Z",
      "changedBy": "Maria Santos",
      "duration": 28
    },
    {
      "from": "in_review",
      "to": "done",
      "changedAt": "2024-01-18T17:00:00Z",
      "changedBy": "Code Reviewer",
      "duration": 27
    }
  ]
}
```

**Nota:** Esta tarefa mostra retrabalho (voltou de in_review para in_progress)

---

## 🔧 Dicas para Calcular Duration

A `duration` é o tempo em **horas** que a tarefa ficou no estado **anterior**.

### Exemplo:

```
todo → in_progress (10h00)
  duration no "todo" = 0 (acabou de criar)

in_progress → in_review (16h00 do dia seguinte)
  duration no "in_progress" = 30 horas (1 dia + 6 horas)

in_review → done (17h00 do mesmo dia)
  duration no "in_review" = 25 horas
```

### Cálculo Rápido:

```javascript
// JavaScript/Node.js
const startDate = new Date("2024-01-08T10:00:00Z");
const endDate = new Date("2024-01-09T16:00:00Z");
const durationInHours = (endDate - startDate) / (1000 * 60 * 60);
// Result: 30
```

---

## 📋 Template de Sprint Vazio

Use este template para criar sua sprint:

```json
{
  "id": "sprint-YYYY-MM",
  "name": "Sprint XX/YYYY",
  "startDate": "YYYY-MM-DDT00:00:00Z",
  "endDate": "YYYY-MM-DDT00:00:00Z",
  "goal": "Objetivo da sprint",
  "metadata": {
    "capturedAt": "YYYY-MM-DDTHH:MM:SSZ",
    "version": "1.0"
  },
  "team": [
    {
      "id": "dev-001",
      "name": "Nome Completo",
      "email": "email@empresa.com",
      "role": "Cargo",
      "capacity": 40
    }
  ],
  "tasks": [
    {
      "id": "TASK-001",
      "title": "Título da tarefa",
      "description": "Descrição detalhada",
      "assignee": "Nome do responsável",
      "points": 3,
      "status": "done",
      "priority": "medium",
      "type": "feature",
      "createdAt": "YYYY-MM-DDTHH:MM:SSZ",
      "startedAt": "YYYY-MM-DDTHH:MM:SSZ",
      "completedAt": "YYYY-MM-DDTHH:MM:SSZ",
      "sprint": "Sprint XX/YYYY",
      "labels": ["tag1", "tag2"],
      "statusHistory": [
        {
          "from": "todo",
          "to": "in_progress",
          "changedAt": "YYYY-MM-DDTHH:MM:SSZ",
          "changedBy": "Nome",
          "duration": 0
        }
      ]
    }
  ]
}
```

---

## 🎯 Cenários Comuns

### Sprint com Alta Performance

```json
{
  "tasks": [
    {
      "status": "done",
      "points": 5,
      "statusHistory": [
        {
          "from": "todo",
          "to": "in_progress",
          "duration": 1
        },
        {
          "from": "in_progress",
          "to": "done",
          "duration": 24
        }
      ]
    }
  ]
}
```

**Resultado:**
- ✅ Taxa de conclusão alta
- ✅ Cycle time baixo
- ✅ Sem retrabalho

### Sprint com Problemas

```json
{
  "tasks": [
    {
      "status": "blocked",
      "points": 8,
      "statusHistory": [
        {
          "from": "in_progress",
          "to": "blocked",
          "duration": 72
        }
      ]
    },
    {
      "status": "in_progress",
      "points": 5,
      "statusHistory": [
        {
          "from": "in_review",
          "to": "in_progress",
          "duration": 24
        }
      ]
    }
  ]
}
```

**Resultado:**
- ⚠️ Bloqueios detectados
- ⚠️ Retrabalho identificado
- ⚠️ Insights de alerta

---

## ✅ Validação do JSON

Antes de usar seu snapshot:

1. **Valide a sintaxe**: https://jsonlint.com
2. **Verifique campos obrigatórios**
3. **Teste datas**: Use formato ISO 8601
4. **Confira status**: Use apenas os valores permitidos
5. **Revise duration**: Calcule corretamente

---

## 🚀 Como Usar

1. **Crie o arquivo**: `data/minha-sprint.json`
2. **Adicione à lista**: Edite `data/snapshots.json`
3. **Recarregue**: Atualize o navegador
4. **Analise**: Selecione seu snapshot

---

## 💡 Dicas Avançadas

### Múltiplas Sprints

Crie um arquivo por sprint para comparar:
```
data/
  sprint-2024-01.json
  sprint-2024-02.json
  sprint-2024-03.json
```

### Exportar de Jira/Azure DevOps

Use APIs ou exportações para gerar JSONs:

```python
# Exemplo Python
import json
from jira import JIRA

# Conectar ao Jira
jira = JIRA('https://seu-jira.com', auth=('user', 'pass'))

# Buscar issues da sprint
issues = jira.search_issues('sprint = "Sprint 01"')

# Converter para formato
snapshot = {
    "id": "sprint-01",
    "tasks": [
        {
            "id": issue.key,
            "title": issue.fields.summary,
            "assignee": issue.fields.assignee.displayName,
            "points": issue.fields.customfield_10016,
            "status": issue.fields.status.name,
            # ...
        }
        for issue in issues
    ]
}

# Salvar
with open('sprint-01.json', 'w') as f:
    json.dump(snapshot, f, indent=2)
```

---

**Pronto para criar seus snapshots! 🎉**
