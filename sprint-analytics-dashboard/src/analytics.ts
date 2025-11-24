// analytics.ts - Motor de análise de métricas

import { 
  SprintSnapshot, 
  SprintMetrics, 
  MemberMetrics, 
  Task, 
  TaskStatus,
  Insight,
  TrendData,
  DashboardData
} from './types';
import { differenceInHours, differenceInDays, parseISO } from 'date-fns';

export class SprintAnalytics {
  private snapshot: SprintSnapshot;

  constructor(snapshot: SprintSnapshot) {
    this.snapshot = snapshot;
  }

  public analyze(): DashboardData {
    const sprintMetrics = this.calculateSprintMetrics();
    const memberMetrics = this.calculateMemberMetrics();
    const insights = this.generateInsights(sprintMetrics, memberMetrics);
    const trends = this.calculateTrends();

    return {
      snapshot: this.snapshot,
      sprintMetrics,
      memberMetrics,
      insights,
      trends
    };
  }

  private calculateSprintMetrics(): SprintMetrics {
    const tasks = this.snapshot.tasks;
    const completedTasks = tasks.filter(t => t.status === 'done');
    const totalPoints = tasks.reduce((sum, t) => sum + t.points, 0);
    const completedPoints = completedTasks.reduce((sum, t) => sum + t.points, 0);

    // Tarefas que voltaram de status (retrabalho)
    const tasksReturned = tasks.filter(task => 
      this.hasTaskReturned(task)
    ).length;

    // Tempo médio de ciclo (do início ao fim)
    const cycleTimesInHours = completedTasks
      .filter(t => t.startedAt && t.completedAt)
      .map(t => differenceInHours(parseISO(t.completedAt!), parseISO(t.startedAt!)));
    
    const averageCycleTime = cycleTimesInHours.length > 0
      ? cycleTimesInHours.reduce((a, b) => a + b, 0) / cycleTimesInHours.length
      : 0;

    // Tempo médio de lead time (da criação ao fim)
    const leadTimesInHours = completedTasks
      .map(t => differenceInHours(parseISO(t.completedAt!), parseISO(t.createdAt)));
    
    const averageLeadTime = leadTimesInHours.length > 0
      ? leadTimesInHours.reduce((a, b) => a + b, 0) / leadTimesInHours.length
      : 0;

    // Tempo por pontuação
    const timeByPoints: SprintMetrics['timeByPoints'] = {};
    completedTasks.forEach(task => {
      if (task.startedAt && task.completedAt) {
        const time = differenceInHours(parseISO(task.completedAt), parseISO(task.startedAt));
        if (!timeByPoints[task.points]) {
          timeByPoints[task.points] = { averageTime: 0, count: 0, tasks: [] };
        }
        timeByPoints[task.points].tasks.push(task);
        timeByPoints[task.points].count++;
      }
    });

    // Calcular médias
    Object.keys(timeByPoints).forEach(points => {
      const data = timeByPoints[parseInt(points)];
      const times = data.tasks
        .filter(t => t.startedAt && t.completedAt)
        .map(t => differenceInHours(parseISO(t.completedAt!), parseISO(t.startedAt!)));
      data.averageTime = times.length > 0 ? times.reduce((a, b) => a + b, 0) / times.length : 0;
    });

    // Tempo por status
    const timeByStatus = this.calculateTimeByStatus(tasks);

    // Tempo bloqueado
    const blockedTime = tasks.reduce((total, task) => {
      const blockedDuration = task.statusHistory
        .filter(h => h.from === 'blocked')
        .reduce((sum, h) => sum + (h.duration || 0), 0);
      return total + blockedDuration;
    }, 0);

    // Tarefas comprometidas (todas que não estavam em backlog no início da sprint)
    const committedTasks = tasks.filter(t => t.status !== 'backlog').length;

    return {
      totalTasks: tasks.length,
      completedTasks: completedTasks.length,
      completionRate: (completedTasks.length / tasks.length) * 100,
      totalPoints,
      completedPoints,
      pointsCompletionRate: (completedPoints / totalPoints) * 100,
      committedTasks,
      deliveredTasks: completedTasks.length,
      commitmentAchievement: (completedTasks.length / committedTasks) * 100,
      averageCycleTime,
      averageLeadTime,
      velocity: completedPoints,
      tasksReturned,
      returnRate: (tasksReturned / tasks.length) * 100,
      blockedTime,
      timeByPoints,
      timeByStatus,
      tasksByStatus: this.groupByStatus(tasks),
      tasksByPriority: this.groupByPriority(tasks),
      tasksByType: this.groupByType(tasks)
    };
  }

  private calculateMemberMetrics(): MemberMetrics[] {
    return this.snapshot.team.map(member => {
      const memberTasks = this.snapshot.tasks.filter(t => t.assignee === member.name);
      const completedTasks = memberTasks.filter(t => t.status === 'done');
      const inProgressTasks = memberTasks.filter(t => t.status === 'in_progress');

      const cycleTimesInHours = completedTasks
        .filter(t => t.startedAt && t.completedAt)
        .map(t => differenceInHours(parseISO(t.completedAt!), parseISO(t.startedAt!)));
      
      const averageCycleTime = cycleTimesInHours.length > 0
        ? cycleTimesInHours.reduce((a, b) => a + b, 0) / cycleTimesInHours.length
        : 0;

      const leadTimesInHours = completedTasks
        .map(t => differenceInHours(parseISO(t.completedAt!), parseISO(t.createdAt)));
      
      const averageLeadTime = leadTimesInHours.length > 0
        ? leadTimesInHours.reduce((a, b) => a + b, 0) / leadTimesInHours.length
        : 0;

      const tasksReturned = memberTasks.filter(task => this.hasTaskReturned(task)).length;
      const currentLoad = inProgressTasks.reduce((sum, t) => sum + t.points, 0);
      const capacity = member.capacity || 40; // default 40 horas

      // Trend de conclusão diária
      const completionTrend = this.calculateCompletionTrend(completedTasks);

      return {
        memberId: member.id,
        memberName: member.name,
        tasksCompleted: completedTasks.length,
        pointsCompleted: completedTasks.reduce((sum, t) => sum + t.points, 0),
        tasksInProgress: inProgressTasks.length,
        averageCycleTime,
        averageLeadTime,
        tasksReturned,
        returnRate: memberTasks.length > 0 ? (tasksReturned / memberTasks.length) * 100 : 0,
        currentLoad,
        capacity,
        utilizationRate: (currentLoad / capacity) * 100,
        tasksByStatus: this.groupByStatus(memberTasks),
        tasksByType: this.groupByType(memberTasks),
        completionTrend
      };
    });
  }

  private hasTaskReturned(task: Task): boolean {
    const history = task.statusHistory;
    for (let i = 1; i < history.length; i++) {
      const statuses: TaskStatus[] = ['backlog', 'todo', 'in_progress', 'in_review', 'done'];
      const fromIndex = statuses.indexOf(history[i - 1].to);
      const toIndex = statuses.indexOf(history[i].to);
      
      if (toIndex < fromIndex && history[i].to !== 'blocked') {
        return true; // Voltou para um status anterior
      }
    }
    return false;
  }

  private calculateTimeByStatus(tasks: Task[]): SprintMetrics['timeByStatus'] {
    const result: SprintMetrics['timeByStatus'] = {};

    tasks.forEach(task => {
      task.statusHistory.forEach(change => {
        const status = change.from;
        if (!result[status]) {
          result[status] = { averageTime: 0, totalTime: 0, count: 0 };
        }
        if (change.duration) {
          result[status].totalTime += change.duration;
          result[status].count++;
        }
      });
    });

    // Calcular médias
    Object.keys(result).forEach(status => {
      const data = result[status];
      data.averageTime = data.count > 0 ? data.totalTime / data.count : 0;
    });

    return result;
  }

  private groupByStatus(tasks: Task[]): { [status: string]: number } {
    return tasks.reduce((acc, task) => {
      acc[task.status] = (acc[task.status] || 0) + 1;
      return acc;
    }, {} as { [status: string]: number });
  }

  private groupByPriority(tasks: Task[]): { [priority: string]: number } {
    return tasks.reduce((acc, task) => {
      acc[task.priority] = (acc[task.priority] || 0) + 1;
      return acc;
    }, {} as { [priority: string]: number });
  }

  private groupByType(tasks: Task[]): { [type: string]: number } {
    return tasks.reduce((acc, task) => {
      acc[task.type] = (acc[task.type] || 0) + 1;
      return acc;
    }, {} as { [type: string]: number });
  }

  private calculateCompletionTrend(completedTasks: Task[]): Array<{ date: string; completed: number }> {
    const dailyCompletion: { [date: string]: number } = {};

    completedTasks.forEach(task => {
      if (task.completedAt) {
        const date = parseISO(task.completedAt).toISOString().split('T')[0];
        dailyCompletion[date] = (dailyCompletion[date] || 0) + 1;
      }
    });

    return Object.entries(dailyCompletion)
      .map(([date, completed]) => ({ date, completed }))
      .sort((a, b) => a.date.localeCompare(b.date));
  }

  private calculateTrends(): TrendData {
    const tasks = this.snapshot.tasks;
    const completedTasks = tasks.filter(t => t.status === 'done');

    // Daily completion
    const dailyCompletion = this.calculateDailyCompletion(completedTasks);

    // Burndown chart
    const burndown = this.calculateBurndown();

    // Status distribution over time
    const statusDistribution = this.calculateStatusDistribution();

    return {
      dailyCompletion,
      statusDistribution,
      burndown,
      velocity: [{ sprint: this.snapshot.name, points: completedTasks.reduce((s, t) => s + t.points, 0) }]
    };
  }

  private calculateDailyCompletion(completedTasks: Task[]): Array<{ date: string; completed: number; points: number }> {
    const daily: { [date: string]: { completed: number; points: number } } = {};

    completedTasks.forEach(task => {
      if (task.completedAt) {
        const date = parseISO(task.completedAt).toISOString().split('T')[0];
        if (!daily[date]) {
          daily[date] = { completed: 0, points: 0 };
        }
        daily[date].completed++;
        daily[date].points += task.points;
      }
    });

    return Object.entries(daily)
      .map(([date, data]) => ({ date, ...data }))
      .sort((a, b) => a.date.localeCompare(b.date));
  }

  private calculateBurndown(): Array<{ date: string; remaining: number; ideal: number }> {
    const startDate = parseISO(this.snapshot.startDate);
    const endDate = parseISO(this.snapshot.endDate);
    const totalPoints = this.snapshot.tasks.reduce((sum, t) => sum + t.points, 0);
    const totalDays = differenceInDays(endDate, startDate);
    const idealBurnRate = totalPoints / totalDays;

    const result: Array<{ date: string; remaining: number; ideal: number }> = [];
    
    // Simplified burndown - would need daily snapshots for accurate data
    for (let day = 0; day <= totalDays; day++) {
      const date = new Date(startDate);
      date.setDate(date.getDate() + day);
      const dateStr = date.toISOString().split('T')[0];
      
      result.push({
        date: dateStr,
        remaining: totalPoints - (idealBurnRate * day),
        ideal: totalPoints - (idealBurnRate * day)
      });
    }

    return result;
  }

  private calculateStatusDistribution(): Array<{ date: string; [status: string]: number }> {
    // Simplified - would need historical data for accurate distribution
    const current: any = {
      date: new Date().toISOString().split('T')[0]
    };

    this.snapshot.tasks.forEach(task => {
      current[task.status] = (current[task.status] || 0) + 1;
    });

    return [current];
  }

  private generateInsights(sprintMetrics: SprintMetrics, memberMetrics: MemberMetrics[]): Insight[] {
    const insights: Insight[] = [];

    // Insight 1: Taxa de conclusão
    if (sprintMetrics.completionRate < 70) {
      insights.push({
        id: 'completion-rate-low',
        type: 'warning',
        category: 'performance',
        title: 'Taxa de Conclusão Abaixo do Esperado',
        description: `Apenas ${sprintMetrics.completionRate.toFixed(1)}% das tarefas foram concluídas. Meta: 80%+`,
        severity: 4,
        recommendations: [
          'Revisar o planejamento da sprint',
          'Identificar impedimentos frequentes',
          'Reduzir o comprometimento de tarefas'
        ]
      });
    }

    // Insight 2: Compromisso não cumprido
    if (sprintMetrics.commitmentAchievement < 80) {
      insights.push({
        id: 'commitment-not-met',
        type: 'danger',
        category: 'commitment',
        title: 'Compromisso da Sprint Não Atingido',
        description: `${sprintMetrics.commitmentAchievement.toFixed(1)}% do comprometido foi entregue`,
        severity: 5,
        recommendations: [
          'Melhorar estimativas de capacidade',
          'Reduzir work in progress',
          'Identificar causas de atrasos'
        ]
      });
    }

    // Insight 3: Alto índice de retrabalho
    if (sprintMetrics.returnRate > 15) {
      insights.push({
        id: 'high-return-rate',
        type: 'warning',
        category: 'quality',
        title: 'Alto Índice de Retrabalho',
        description: `${sprintMetrics.returnRate.toFixed(1)}% das tarefas retornaram para status anteriores`,
        severity: 4,
        recommendations: [
          'Melhorar definição de pronto (DoD)',
          'Aumentar cobertura de testes',
          'Implementar revisões de código mais rigorosas'
        ]
      });
    }

    // Insight 4: Tarefas de 3 pontos
    const threePointTasks = sprintMetrics.timeByPoints[3];
    if (threePointTasks && threePointTasks.averageTime > 24) {
      insights.push({
        id: 'three-point-tasks-slow',
        type: 'info',
        category: 'performance',
        title: 'Tarefas de 3 Pontos Demorando Mais que o Esperado',
        description: `Tempo médio: ${(threePointTasks.averageTime / 24).toFixed(1)} dias. Esperado: ~1 dia`,
        severity: 3,
        recommendations: [
          'Quebrar tarefas em subtarefas menores',
          'Revisar critérios de pontuação',
          'Identificar bloqueios recorrentes'
        ]
      });
    }

    // Insight 5: Membros sobrecarregados
    const overloadedMembers = memberMetrics.filter(m => m.utilizationRate > 120);
    if (overloadedMembers.length > 0) {
      insights.push({
        id: 'team-overloaded',
        type: 'warning',
        category: 'team',
        title: 'Membros da Equipe Sobrecarregados',
        description: `${overloadedMembers.length} membro(s) com carga acima de 120% da capacidade`,
        severity: 4,
        recommendations: [
          'Redistribuir tarefas',
          'Revisar capacidade do time',
          'Identificar gargalos'
        ]
      });
    }

    // Insight 6: Velocidade positiva
    if (sprintMetrics.pointsCompletionRate > 90) {
      insights.push({
        id: 'excellent-velocity',
        type: 'success',
        category: 'performance',
        title: 'Excelente Velocidade da Sprint!',
        description: `${sprintMetrics.pointsCompletionRate.toFixed(1)}% dos pontos foram entregues`,
        severity: 1,
        recommendations: [
          'Manter o ritmo atual',
          'Documentar práticas bem-sucedidas',
          'Compartilhar aprendizados com a equipe'
        ]
      });
    }

    // Insight 7: Tempo em blocked
    if (sprintMetrics.blockedTime > 100) {
      insights.push({
        id: 'high-blocked-time',
        type: 'danger',
        category: 'quality',
        title: 'Alto Tempo de Bloqueio',
        description: `${(sprintMetrics.blockedTime / 24).toFixed(1)} dias acumulados em tarefas bloqueadas`,
        severity: 5,
        recommendations: [
          'Identificar dependências cedo',
          'Criar plano de contingência',
          'Melhorar comunicação com stakeholders'
        ]
      });
    }

    return insights.sort((a, b) => b.severity - a.severity);
  }
}
