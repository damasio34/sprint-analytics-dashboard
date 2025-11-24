# 🎉 Sprint Analytics Dashboard - Projeto Completo

## ✅ O Que Foi Criado

Um **Dashboard BI Completo** para análise de sprints e performance de equipes ágeis, com:

### 📊 Funcionalidades Principais

1. **Visão Geral da Sprint**
   - Taxa de conclusão de tarefas
   - Pontos entregues vs comprometidos
   - Velocidade da equipe
   - Cycle time e lead time médios
   - Gráficos de distribuição (status, prioridade, tipo)
   - Métricas de qualidade e bloqueios

2. **Análise Individual por Membro**
   - Performance detalhada de cada desenvolvedor
   - Radar de performance (5 dimensões)
   - Comparação entre membros
   - Tendências de conclusão
   - Taxa de utilização e carga
   - Identificação de sobrecarga

3. **Insights Inteligentes**
   - Detecção automática de problemas
   - Classificação por severidade (1-5)
   - Recomendações personalizadas
   - Alertas de compromisso não atingido
   - Identificação de retrabalho
   - Análise de bloqueios

4. **Análises Específicas**
   ✅ **Quanto tempo leva uma tarefa de 3 pontos?**
   ✅ **Quantas vezes tarefas voltam de status?**
   ✅ **Quanto tempo fica em cada estado?**
   ✅ **Tarefas comprometidas vs entregues?**
   ✅ **Tempo médio para completar uma tarefa?**

5. **Exportação de Relatórios**
   - Geração de PDF do dashboard
   - Snapshot do estado atual
   - Versionamento de sprints

---

## 🚀 Como Executar

### Passo 1: Extrair o ZIP

```bash
unzip sprint-analytics-dashboard.zip
cd sprint-analytics-dashboard
```

### Passo 2: Executar

**Linux/Mac:**
```bash
./start.sh
```

**Windows:**
```cmd
start.bat
```

Ou clique duas vezes em `start.bat`

### Passo 3: Acessar

Abra seu navegador em:
```
http://localhost:3000
```

---

## 📁 Estrutura de Arquivos

```
sprint-analytics-dashboard/
├── 📄 README.md              # Documentação completa
├── 📄 QUICK_START.md         # Guia rápido de início
├── 🚀 start.sh              # Script Linux/Mac
├── 🚀 start.bat             # Script Windows
├── 🐳 Dockerfile            # Container Docker
├── 🐳 docker-compose.yml    # Orquestração
│
├── 📁 data/                 # Snapshots JSON
│   ├── snapshots.json       # Lista de snapshots
│   └── sprint-2024-01.json  # Exemplo completo
│
└── 📁 src/                  # Código React + TypeScript
    ├── Dashboard.tsx        # Componente principal
    ├── DashboardComponents.tsx  # TeamView + InsightsView
    ├── analytics.ts         # Motor de análise
    ├── types.ts            # Tipos TypeScript
    └── ...
```

---

## 🎯 Principais Métricas Implementadas

### ⏱️ Tempo
- **Cycle Time**: Tempo do início ao fim
- **Lead Time**: Tempo da criação ao fim
- **Tempo por Pontuação**: Média para 1, 2, 3, 5, 8 pontos
- **Tempo por Status**: Duração em cada estado
- **Tempo Bloqueado**: Horas em blocked

### 📈 Entrega
- **Taxa de Conclusão**: % completadas
- **Velocidade**: Pontos entregues
- **Compromisso**: % do prometido entregue
- **Pontos Completados**: Total entregue

### 🎯 Qualidade
- **Taxa de Retrabalho**: % que retornaram
- **Tarefas Retornadas**: Quantidade absoluta
- **Return Rate por Membro**: Individual

### 👥 Equipe
- **Utilização**: Carga vs capacidade
- **Performance Radar**: 5 dimensões
- **Comparação entre Membros**: Side-by-side

---

## 💡 Insights Automáticos Gerados

O sistema analisa os dados e gera insights como:

### ⚠️ Taxa de Conclusão Baixa
> "Apenas 65% das tarefas foram concluídas. Meta: 80%+"
> 
> **Recomendações:**
> - Revisar planejamento
> - Identificar impedimentos
> - Reduzir comprometimento

### 🔴 Alto Retrabalho
> "18% das tarefas retornaram para status anteriores"
> 
> **Recomendações:**
> - Melhorar Definition of Done
> - Aumentar cobertura de testes
> - Revisões mais rigorosas

### ⚡ Sobrecarga
> "2 membros com carga acima de 120%"
> 
> **Recomendações:**
> - Redistribuir tarefas
> - Revisar capacidade
> - Identificar gargalos

---

## 📊 Visualizações Incluídas

### Gráficos
- 📊 **Pie Charts**: Distribuição por status e tipo
- 📊 **Bar Charts**: Tempo por pontuação, tarefas por prioridade
- 📊 **Line Charts**: Tendências de conclusão
- 📊 **Radar Charts**: Performance individual
- 📊 **Area Charts**: Burndown (preparado)

### KPIs Cards
- Taxa de conclusão
- Pontos entregues
- Velocidade
- Cycle time médio

### Comparações
- Membros side-by-side
- Cycle time comparativo
- Tarefas completadas
- Utilização

---

## 🎨 Design e UX

- ✨ **Gradientes Modernos**: Visual profissional
- 🎨 **Cores Semânticas**: Verde=sucesso, Vermelho=perigo
- 📱 **Responsivo**: Funciona em mobile/tablet/desktop
- ⚡ **Transições Suaves**: Hover effects
- 🔍 **Tooltips Informativos**: Detalhes on-hover
- 📊 **Gráficos Interativos**: Recharts com tooltips

---

## 🔧 Tecnologias Utilizadas

### Frontend
- ⚛️ React 18
- 📘 TypeScript
- ⚡ Vite (build rápido)
- 🎨 Tailwind CSS
- 📊 Recharts (gráficos)
- 📅 date-fns (datas)
- 🖼️ html2canvas (screenshots)
- 📄 jsPDF (exportação)
- 🎯 Lucide React (ícones)

### Infraestrutura
- 🐳 Docker
- 🐳 Docker Compose
- 🔧 Node.js 18

---

## 📝 Formato do Snapshot JSON

```json
{
  "id": "sprint-2024-01",
  "name": "Sprint Nome",
  "startDate": "2024-01-08T00:00:00Z",
  "endDate": "2024-01-22T00:00:00Z",
  "goal": "Objetivo da sprint",
  "team": [
    {
      "id": "dev-001",
      "name": "Nome Desenvolvedor",
      "capacity": 40
    }
  ],
  "tasks": [
    {
      "id": "TASK-001",
      "title": "Título",
      "assignee": "Nome",
      "points": 3,
      "status": "done",
      "priority": "high",
      "type": "feature",
      "createdAt": "2024-01-08T09:00:00Z",
      "startedAt": "2024-01-08T14:00:00Z",
      "completedAt": "2024-01-12T16:30:00Z",
      "statusHistory": [
        {
          "from": "todo",
          "to": "in_progress",
          "changedAt": "2024-01-08T14:00:00Z",
          "duration": 5
        }
      ]
    }
  ]
}
```

**Campos Importantes:**
- `statusHistory`: Histórico completo de mudanças
- `duration`: Tempo em horas no estado anterior
- `points`: Pontuação da tarefa (1, 2, 3, 5, 8, etc.)
- `capacity`: Horas disponíveis do membro

---

## 🎯 Casos de Uso

### 👨‍💼 Scrum Master
- Monitorar saúde da sprint diariamente
- Identificar bloqueios rapidamente
- Preparar retrospectivas com dados
- Acompanhar compromisso

### 📋 Product Owner
- Verificar velocidade para planning
- Entender impactos na entrega
- Planejar próximas sprints
- Validar estimativas

### 👨‍💻 Desenvolvedor
- Visualizar carga pessoal
- Comparar com equipe
- Identificar melhorias
- Acompanhar progresso

### 👔 Gestor
- Avaliar performance geral
- Identificar necessidade de treinamento
- Tomar decisões baseadas em dados
- Gerar relatórios executivos

---

## ✅ Checklist de Features

- [x] Dashboard com 3 visões (Geral, Time, Insights)
- [x] Seleção de snapshots JSON
- [x] KPIs principais em cards
- [x] Gráficos interativos (8+ tipos)
- [x] Análise individual por membro
- [x] Radar de performance
- [x] Insights automáticos com severidade
- [x] Recomendações de melhoria
- [x] Tempo médio por pontuação
- [x] Detecção de retrabalho
- [x] Análise de bloqueios
- [x] Compromisso vs Entrega
- [x] Exportação para PDF
- [x] Docker + Docker Compose
- [x] Scripts de execução (Linux/Mac/Windows)
- [x] Documentação completa
- [x] Exemplo de snapshot funcional

---

## 📦 Arquivos Entregues

1. **sprint-analytics-dashboard.zip** (31KB)
   - Projeto completo pronto para executar
   
2. **QUICK_START.md**
   - Guia rápido de 3 passos

---

## 🚀 Próximos Passos Sugeridos

1. ✅ **Executar o projeto** (3 minutos)
2. ✅ **Explorar o exemplo** incluído
3. ✅ **Criar seu próprio snapshot** JSON
4. ✅ **Adicionar múltiplas sprints** para comparação
5. ✅ **Customizar** cores/temas conforme necessário
6. ✅ **Integrar** com Jira/Azure DevOps (futuro)

---

## 💬 Comandos Úteis

```bash
# Ver logs
docker-compose logs -f

# Parar
docker-compose down

# Reiniciar
docker-compose restart

# Rebuild
docker-compose build
```

---

## 🎓 Aprendizados do Projeto

Este dashboard demonstra:
- ✅ Análise avançada de dados de sprint
- ✅ Visualizações ricas com Recharts
- ✅ TypeScript para type safety
- ✅ React hooks avançados
- ✅ Containerização com Docker
- ✅ UI/UX moderna com Tailwind
- ✅ Exportação de relatórios
- ✅ Arquitetura escalável

---

**🎉 Projeto 100% funcional e pronto para uso!**

Execute `./start.sh` ou `start.bat` e comece a analisar suas sprints agora mesmo! 🚀
