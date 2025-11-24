# 📊 Sprint Analytics Dashboard

## Dashboard BI Completo para Análise de Sprints e Performance de Equipe

Sistema de Business Intelligence avançado para monitoramento, análise e insights sobre o desempenho de equipes ágeis. Visualize métricas detalhadas, identifique gargalos e tome decisões baseadas em dados.

![Dashboard Preview](https://via.placeholder.com/800x400/1e3a8a/ffffff?text=Sprint+Analytics+Dashboard)

---

## 🎯 Funcionalidades Principais

### 📈 Métricas Gerais da Sprint
- ✅ Taxa de conclusão de tarefas
- ✅ Pontos entregues vs comprometidos
- ✅ Velocidade da equipe (velocity)
- ✅ Cycle time e lead time médios
- ✅ Burndown chart
- ✅ Distribuição de tarefas por status, prioridade e tipo

### 👥 Análise Individual por Membro
- ✅ Tarefas completadas e pontos entregues
- ✅ Taxa de utilização e carga atual
- ✅ Performance radar (5 dimensões)
- ✅ Tendência de conclusão ao longo do tempo
- ✅ Identificação de gargalos e sobrecargas
- ✅ Comparação entre membros da equipe

### 💡 Insights Inteligentes
- ✅ Identificação automática de problemas
- ✅ Recomendações de melhoria
- ✅ Alertas de retrabalho e bloqueios
- ✅ Análise de compromisso vs entrega
- ✅ Detecção de tarefas fora do padrão
- ✅ Classificação por severidade

### 📊 Análises Detalhadas
- ✅ **Tempo por Pontuação**: Quanto tempo leva uma tarefa de 3 pontos?
- ✅ **Tarefas que Retornam**: Quantas vezes tarefas voltam de status?
- ✅ **Tempo por Estado**: Quanto tempo ficam em cada status?
- ✅ **Bloqueios**: Tempo total em estado bloqueado
- ✅ **Compromisso vs Entrega**: Sprint commitment achievement
- ✅ **Qualidade**: Taxa de retrabalho e bugs

### 📄 Exportação de Relatórios
- ✅ Geração de PDF com todos os dados
- ✅ Snapshot do estado atual
- ✅ Histórico versionado de sprints

---

## 🚀 Início Rápido

### Pré-requisitos

- **Docker** e **Docker Compose** instalados
  - Windows/Mac: [Docker Desktop](https://www.docker.com/products/docker-desktop)
  - Linux: `sudo apt-get install docker.io docker-compose`

### Instalação e Execução

#### No Linux/Mac:
```bash
# 1. Executar o script
./start.sh

# 2. Acessar o dashboard
# http://localhost:3000
```

#### No Windows:
```cmd
REM 1. Executar o script
start.bat

REM 2. Acessar o dashboard
REM http://localhost:3000
```

### Parar o Dashboard

```bash
# Linux/Mac
docker-compose down

# Windows
docker-compose down
```

---

## 📁 Estrutura do Projeto

```
sprint-analytics-dashboard/
├── data/                          # Snapshots JSON das sprints
│   ├── snapshots.json            # Lista de snapshots disponíveis
│   └── sprint-2024-01.json       # Exemplo de snapshot
├── src/                          # Código-fonte React + TypeScript
│   ├── Dashboard.tsx             # Componente principal
│   ├── DashboardComponents.tsx   # TeamView e InsightsView
│   ├── analytics.ts              # Motor de análise de métricas
│   ├── types.ts                  # Definições TypeScript
│   ├── main.tsx                  # Entry point
│   └── index.css                 # Estilos Tailwind
├── docker-compose.yml            # Configuração Docker
├── Dockerfile                    # Imagem Docker
├── start.sh                      # Script de início (Linux/Mac)
├── start.bat                     # Script de início (Windows)
├── package.json                  # Dependências Node.js
└── README.md                     # Esta documentação
```

---

## 📝 Formato do Snapshot JSON

Os snapshots representam o estado da sprint em um momento específico. Exemplo:

```json
{
  "id": "sprint-2024-01",
  "name": "Sprint 01/2024 - Q1",
  "startDate": "2024-01-08T00:00:00Z",
  "endDate": "2024-01-22T00:00:00Z",
  "goal": "Implementar funcionalidades de autenticação",
  "metadata": {
    "capturedAt": "2024-01-22T18:00:00Z",
    "version": "1.0"
  },
  "team": [
    {
      "id": "dev-001",
      "name": "João Silva",
      "email": "joao@empresa.com",
      "role": "Senior Developer",
      "capacity": 40
    }
  ],
  "tasks": [
    {
      "id": "TASK-001",
      "title": "Implementar login JWT",
      "assignee": "João Silva",
      "points": 5,
      "status": "done",
      "priority": "high",
      "type": "feature",
      "createdAt": "2024-01-08T09:00:00Z",
      "startedAt": "2024-01-08T14:00:00Z",
      "completedAt": "2024-01-12T16:30:00Z",
      "sprint": "Sprint 01/2024",
      "statusHistory": [
        {
          "from": "todo",
          "to": "in_progress",
          "changedAt": "2024-01-08T14:00:00Z",
          "changedBy": "João Silva",
          "duration": 5
        }
      ]
    }
  ]
}
```

### Campos Importantes:

**Task:**
- `id`: Identificador único
- `points`: Pontuação da tarefa (1, 2, 3, 5, 8, etc.)
- `status`: Estado atual (backlog, todo, in_progress, in_review, blocked, done, cancelled)
- `priority`: Prioridade (low, medium, high, urgent)
- `type`: Tipo (feature, bug, improvement, technical_debt)
- `statusHistory`: Array com mudanças de status e duração em cada estado

---

## 🎨 Telas e Visualizações

### 1. Visão Geral (Overview)
- KPIs principais (cards coloridos)
- Gráfico de pizza: Distribuição por status
- Gráfico de barras: Tempo médio por pontuação
- Gráfico de barras: Tarefas por prioridade
- Gráfico de pizza: Tarefas por tipo
- Métricas de compromisso vs entrega
- Métricas de qualidade (retrabalho, bloqueios)

### 2. Time (Team View)
- Cards de membros com métricas individuais
- Gráfico comparativo: Tarefas completadas
- Gráfico comparativo: Cycle time
- Radar de performance (5 dimensões)
- Gráfico de tendência de conclusão
- Distribuição de tarefas por status e tipo

### 3. Insights
- Cards de resumo (total, críticos, avisos, sucessos)
- Insights agrupados por categoria:
  - Performance
  - Qualidade
  - Compromisso
  - Time
- Cada insight inclui:
  - Severidade (1-5 estrelas)
  - Descrição do problema
  - Recomendações de ação
- Próximos passos recomendados

---

## 🔍 Métricas Calculadas

### Métricas de Tempo
- **Cycle Time**: Tempo do início ao fim da tarefa
- **Lead Time**: Tempo da criação até conclusão
- **Tempo por Pontuação**: Média de tempo para cada pontuação
- **Tempo por Status**: Tempo médio em cada estado
- **Tempo Bloqueado**: Total de horas em blocked

### Métricas de Entrega
- **Taxa de Conclusão**: % de tarefas completadas
- **Velocidade**: Pontos completados na sprint
- **Compromisso vs Entrega**: % do comprometido que foi entregue
- **Pontos Completados**: Total de pontos entregues

### Métricas de Qualidade
- **Taxa de Retrabalho**: % de tarefas que retornaram
- **Tarefas Retornadas**: Número de tarefas que voltaram de status
- **Return Rate**: % de retrabalho por membro

### Métricas de Equipe
- **Utilização**: Carga atual vs capacidade
- **Carga Atual**: Pontos em progresso
- **Capacidade**: Horas disponíveis

---

## 🎓 Exemplos de Insights Gerados

### ⚠️ Performance
> **Taxa de Conclusão Abaixo do Esperado**
> Apenas 65% das tarefas foram concluídas. Meta: 80%+
> 
> Recomendações:
> - Revisar o planejamento da sprint
> - Identificar impedimentos frequentes
> - Reduzir o comprometimento de tarefas

### 🔴 Qualidade
> **Alto Índice de Retrabalho**
> 18% das tarefas retornaram para status anteriores
> 
> Recomendações:
> - Melhorar definição de pronto (DoD)
> - Aumentar cobertura de testes
> - Implementar revisões de código mais rigorosas

### ⚡ Time
> **Membros da Equipe Sobrecarregados**
> 2 membro(s) com carga acima de 120% da capacidade
> 
> Recomendações:
> - Redistribuir tarefas
> - Revisar capacidade do time
> - Identificar gargalos

---

## 💻 Desenvolvimento Local (Sem Docker)

Se preferir rodar sem Docker:

```bash
# Instalar dependências
npm install

# Executar em modo desenvolvimento
npm run dev

# Acessar
# http://localhost:3000
```

---

## 📊 Adicionando Novos Snapshots

1. **Crie o arquivo JSON** na pasta `data/` com a estrutura correta
   ```
   data/sprint-2024-02.json
   ```

2. **Atualize a lista** em `data/snapshots.json`:
   ```json
   {
     "snapshots": [
       "sprint-2024-01.json",
       "sprint-2024-02.json"
     ]
   }
   ```

3. **Recarregue o dashboard** - o novo snapshot aparecerá automaticamente

---

## 🛠️ Tecnologias Utilizadas

- **React 18** - Framework UI
- **TypeScript** - Type safety
- **Vite** - Build tool rápido
- **Recharts** - Biblioteca de gráficos
- **Tailwind CSS** - Estilização
- **date-fns** - Manipulação de datas
- **jsPDF** + **html2canvas** - Exportação de relatórios
- **Lucide React** - Ícones modernos
- **Docker** - Containerização

---

## 📈 Roadmap / Melhorias Futuras

- [ ] Comparação entre múltiplas sprints
- [ ] Filtros avançados (data, membro, tipo)
- [ ] Exportação para Excel
- [ ] Gráfico de burndown em tempo real
- [ ] Integração com Jira/Azure DevOps
- [ ] Previsão de conclusão com ML
- [ ] Alertas em tempo real
- [ ] Dashboard customizável
- [ ] Tema dark mode

---

## 🤝 Contribuindo

Contribuições são bem-vindas! Para contribuir:

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

---

## 📄 Licença

Este projeto é licenciado sob a licença MIT.

---

## 💬 Suporte

Para dúvidas ou problemas:
- Abra uma issue no repositório
- Entre em contato com a equipe de desenvolvimento

---

## 🎯 Casos de Uso

### Para Scrum Masters
- Monitorar a saúde da sprint
- Identificar bloqueios rapidamente
- Preparar retrospectivas com dados
- Acompanhar o compromisso da equipe

### Para Product Owners
- Verificar velocidade e previsibilidade
- Entender o que impacta a entrega
- Planejar próximas sprints com base em dados

### Para Desenvolvedores
- Visualizar carga de trabalho
- Comparar performance com a equipe
- Identificar áreas de melhoria pessoal

### Para Gestores
- Avaliar performance da equipe
- Identificar necessidades de treinamento
- Tomar decisões baseadas em dados
- Gerar relatórios executivos

---

**Desenvolvido com ❤️ para times ágeis que querem melhorar continuamente**
