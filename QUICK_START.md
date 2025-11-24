# 🚀 Guia Rápido - Sprint Analytics Dashboard

## Início em 3 Passos

### 1️⃣ Instalar Docker

#### Windows/Mac
Baixe e instale o Docker Desktop:
https://www.docker.com/products/docker-desktop

#### Linux (Ubuntu/Debian)
```bash
sudo apt-get update
sudo apt-get install docker.io docker-compose
sudo usermod -aG docker $USER
# Faça logout e login novamente
```

### 2️⃣ Executar o Dashboard

#### Linux/Mac
```bash
./start.sh
```

#### Windows
```cmd
start.bat
```

Ou clique duas vezes no arquivo `start.bat`

### 3️⃣ Acessar

Abra seu navegador em:
```
http://localhost:3000
```

---

## 📊 Como Usar

### Carregar um Snapshot

1. Na tela inicial, você verá os snapshots disponíveis
2. Clique em um snapshot para carregar (ex: `sprint-2024-01`)
3. O dashboard será carregado com todos os dados e análises

### Navegar entre Abas

O dashboard possui 3 abas principais:

#### 🎯 Visão Geral
- Métricas gerais da sprint
- Gráficos de distribuição
- Análise de tempo por pontuação
- Métricas de qualidade

#### 👥 Time
- Clique em um membro para ver detalhes
- Compare performance entre membros
- Veja radar de performance individual
- Analise tendências de conclusão

#### 💡 Insights
- Veja insights automáticos gerados
- Identifique problemas críticos
- Receba recomendações de melhoria
- Planeje próximos passos

### Exportar Relatório

1. Navegue pela aba desejada
2. Clique em "Exportar Relatório" (botão verde no topo)
3. Um PDF será gerado com snapshot do dashboard

---

## 📁 Adicionar Seus Snapshots

### Passo 1: Crie o Arquivo JSON

Crie um arquivo na pasta `data/` com seus dados:

```
data/minha-sprint.json
```

Use o formato do arquivo `sprint-2024-01.json` como exemplo.

### Passo 2: Atualize a Lista

Edite o arquivo `data/snapshots.json` e adicione seu arquivo:

```json
{
  "snapshots": [
    "sprint-2024-01.json",
    "minha-sprint.json"
  ]
}
```

### Passo 3: Recarregue a Página

Atualize o navegador (F5) e seu novo snapshot aparecerá na lista!

---

## 🔍 Principais Métricas Explicadas

### Cycle Time
Tempo desde quando a tarefa foi **iniciada** até ser **completada**.
- Mede a eficiência da execução
- Ideal: < 3 dias para tarefas de 3 pontos

### Lead Time
Tempo desde quando a tarefa foi **criada** até ser **completada**.
- Mede o tempo total no sistema
- Inclui tempo de espera

### Velocidade
Total de pontos completados na sprint.
- Indica capacidade de entrega
- Use para planejar próximas sprints

### Taxa de Retrabalho
% de tarefas que voltaram para status anterior.
- Indica problemas de qualidade
- Ideal: < 10%

### Utilização
Carga atual vs capacidade do membro.
- 100% = utilizando toda capacidade
- > 120% = sobrecarregado

---

## 🎯 Dicas de Uso

### Para Scrum Masters
1. **Daily**: Verifique tarefas bloqueadas e membros sobrecarregados
2. **Mid-sprint**: Analise se o compromisso será atingido
3. **Retrospectiva**: Use insights para guiar discussões

### Para Product Owners
1. Verifique a velocidade para planejar próximas sprints
2. Analise o tempo por pontuação para melhorar estimativas
3. Use o compromisso vs entrega para ajustar scope

### Para Gestores
1. Compare performance entre sprints
2. Identifique necessidades de treinamento
3. Gere relatórios PDF para stakeholders

---

## ⚙️ Comandos Úteis

### Ver Logs
```bash
docker-compose logs -f
```

### Parar Dashboard
```bash
docker-compose down
```

### Reiniciar Dashboard
```bash
docker-compose restart
```

### Rebuild (após mudanças)
```bash
docker-compose down
docker-compose build
docker-compose up -d
```

---

## ❓ Problemas Comuns

### "Docker não encontrado"
**Solução**: Instale o Docker Desktop ou docker.io

### "Porta 3000 em uso"
**Solução**: Pare o serviço que está usando a porta ou edite `docker-compose.yml` para usar outra porta

### "Snapshot não carrega"
**Solução**: Verifique se o JSON está válido em https://jsonlint.com

### "Página em branco"
**Solução**: 
1. Verifique os logs: `docker-compose logs`
2. Reinicie: `docker-compose restart`

---

## 📞 Suporte

Para mais ajuda, consulte o README.md completo ou abra uma issue no repositório.

---

**Pronto para começar? Execute `./start.sh` (Linux/Mac) ou `start.bat` (Windows)!** 🚀
