// types.ts - Definições de tipos para o sistema de análise de sprints

export interface Task {
  id: string;
  title: string;
  description: string;
  assignee: string;
  points: number;
  status: TaskStatus;
  priority: Priority;
  type: TaskType;
  createdAt: string;
  startedAt?: string;
  completedAt?: string;
  sprint: string;
  statusHistory: StatusChange[];
  comments?: Comment[];
  labels?: string[];
}

export type TaskStatus = 
  | 'backlog' 
  | 'todo' 
  | 'in_progress' 
  | 'in_review' 
  | 'blocked' 
  | 'done' 
  | 'cancelled';

export type Priority = 'low' | 'medium' | 'high' | 'urgent';
export type TaskType = 'feature' | 'bug' | 'improvement' | 'technical_debt';

export interface StatusChange {
  from: TaskStatus;
  to: TaskStatus;
  changedAt: string;
  changedBy: string;
  duration?: number; // tempo no estado anterior em horas
}

export interface Comment {
  author: string;
  text: string;
  createdAt: string;
}

export interface SprintSnapshot {
  id: string;
  name: string;
  startDate: string;
  endDate: string;
  goal: string;
  tasks: Task[];
  team: TeamMember[];
  metadata: {
    capturedAt: string;
    version: string;
  };
}

export interface TeamMember {
  id: string;
  name: string;
  email: string;
  role: string;
  capacity?: number; // horas disponíveis na sprint
}

// Métricas calculadas
export interface SprintMetrics {
  // Métricas gerais
  totalTasks: number;
  completedTasks: number;
  completionRate: number;
  totalPoints: number;
  completedPoints: number;
  pointsCompletionRate: number;
  
  // Compromisso vs Entrega
  committedTasks: number;
  deliveredTasks: number;
  commitmentAchievement: number;
  
  // Tempo e velocidade
  averageCycleTime: number; // tempo médio do início ao fim
  averageLeadTime: number; // tempo médio da criação ao fim
  velocity: number; // pontos completados
  
  // Qualidade
  tasksReturned: number; // tarefas que voltaram de status
  returnRate: number;
  blockedTime: number; // tempo total em blocked
  
  // Por pontuação
  timeByPoints: {
    [points: number]: {
      averageTime: number;
      count: number;
      tasks: Task[];
    };
  };
  
  // Por status
  timeByStatus: {
    [status: string]: {
      averageTime: number;
      totalTime: number;
      count: number;
    };
  };
  
  // Distribuição
  tasksByStatus: { [status: string]: number };
  tasksByPriority: { [priority: string]: number };
  tasksByType: { [type: string]: number };
}

export interface MemberMetrics {
  memberId: string;
  memberName: string;
  
  // Produtividade
  tasksCompleted: number;
  pointsCompleted: number;
  tasksInProgress: number;
  
  // Tempo
  averageCycleTime: number;
  averageLeadTime: number;
  
  // Qualidade
  tasksReturned: number;
  returnRate: number;
  
  // Carga
  currentLoad: number; // pontos em progresso
  capacity: number;
  utilizationRate: number; // currentLoad / capacity
  
  // Detalhes
  tasksByStatus: { [status: string]: number };
  tasksByType: { [type: string]: number };
  completionTrend: Array<{ date: string; completed: number }>;
}

export interface DashboardData {
  snapshot: SprintSnapshot;
  sprintMetrics: SprintMetrics;
  memberMetrics: MemberMetrics[];
  insights: Insight[];
  trends: TrendData;
}

export interface Insight {
  id: string;
  type: 'warning' | 'success' | 'info' | 'danger';
  category: 'performance' | 'quality' | 'commitment' | 'team';
  title: string;
  description: string;
  severity: number; // 1-5
  recommendations?: string[];
}

export interface TrendData {
  dailyCompletion: Array<{ date: string; completed: number; points: number }>;
  statusDistribution: Array<{ date: string; [status: string]: string | number }>;
  burndown: Array<{ date: string; remaining: number; ideal: number }>;
  velocity: Array<{ sprint: string; points: number }>;
}
