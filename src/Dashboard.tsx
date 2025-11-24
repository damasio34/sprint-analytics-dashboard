import React, { useState, useEffect } from 'react';
import { 
  BarChart, Bar, LineChart, Line, PieChart, Pie, Cell,
  XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer,
  RadarChart, Radar, PolarGrid, PolarAngleAxis, PolarRadiusAxis,
  AreaChart, Area
} from 'recharts';
import {
  TrendingUp, TrendingDown, AlertTriangle, CheckCircle,
  Clock, Users, Target, Activity, Download, Upload,
  Calendar, Award, Zap, AlertCircle
} from 'lucide-react';
import { SprintAnalytics } from './analytics';
import { DashboardData, SprintSnapshot } from './types';
import { TeamView, InsightsView } from './DashboardComponents';
import html2canvas from 'html2canvas';
import jsPDF from 'jspdf';

const COLORS = ['#3b82f6', '#10b981', '#f59e0b', '#ef4444', '#8b5cf6', '#ec4899'];
const STATUS_COLORS: { [key: string]: string } = {
  'backlog': '#6b7280',
  'todo': '#3b82f6',
  'in_progress': '#f59e0b',
  'in_review': '#8b5cf6',
  'blocked': '#ef4444',
  'done': '#10b981',
  'cancelled': '#78716c'
};

export default function Dashboard() {
  const [snapshots, setSnapshots] = useState<string[]>([]);
  const [selectedSnapshot, setSelectedSnapshot] = useState<string>('');
  const [dashboardData, setDashboardData] = useState<DashboardData | null>(null);
  const [loading, setLoading] = useState(false);
  const [view, setView] = useState<'overview' | 'team' | 'insights'>('overview');

  useEffect(() => {
    loadAvailableSnapshots();
  }, []);

  const loadAvailableSnapshots = async () => {
    try {
      // In a real scenario, this would fetch from an API
      // For now, we'll simulate with local files
      const response = await fetch('/data/snapshots.json');
      if (response.ok) {
        const data = await response.json();
        setSnapshots(data.snapshots);
      } else {
        // Fallback to example
        setSnapshots(['sprint-2024-01.json']);
      }
    } catch (error) {
      // Fallback
      setSnapshots(['sprint-2024-01.json']);
    }
  };

  const loadSnapshot = async (filename: string) => {
    setLoading(true);
    try {
      const response = await fetch(`/data/${filename}`);
      const snapshot: SprintSnapshot = await response.json();
      
      const analytics = new SprintAnalytics(snapshot);
      const data = analytics.analyze();
      
      setDashboardData(data);
      setSelectedSnapshot(filename);
    } catch (error) {
      console.error('Error loading snapshot:', error);
      alert('Erro ao carregar o snapshot. Verifique se o arquivo existe.');
    } finally {
      setLoading(false);
    }
  };

  const exportReport = async () => {
    if (!dashboardData) return;

    const element = document.getElementById('dashboard-content');
    if (!element) return;

    const canvas = await html2canvas(element, {
      scale: 2,
      logging: false,
      useCORS: true
    });

    const imgData = canvas.toDataURL('image/png');
    const pdf = new jsPDF('p', 'mm', 'a4');
    const imgWidth = 210;
    const imgHeight = (canvas.height * imgWidth) / canvas.width;
    
    pdf.addImage(imgData, 'PNG', 0, 0, imgWidth, imgHeight);
    pdf.save(`sprint-report-${dashboardData.snapshot.name}-${new Date().toISOString().split('T')[0]}.pdf`);
  };

  if (!dashboardData) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-gray-900 via-blue-900 to-gray-900 p-8">
        <div className="max-w-7xl mx-auto">
          <div className="text-center mb-12">
            <h1 className="text-5xl font-bold text-white mb-4">Sprint Analytics Dashboard</h1>
            <p className="text-xl text-gray-300">Business Intelligence para Gestão de Sprints</p>
          </div>

          <div className="bg-white rounded-2xl shadow-2xl p-8">
            <h2 className="text-2xl font-bold text-gray-800 mb-6">Selecione um Snapshot</h2>
            
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
              {snapshots.map(snapshot => (
                <button
                  key={snapshot}
                  onClick={() => loadSnapshot(snapshot)}
                  className="bg-gradient-to-r from-blue-500 to-blue-600 hover:from-blue-600 hover:to-blue-700 text-white p-6 rounded-xl shadow-lg transform transition hover:scale-105"
                >
                  <div className="flex items-center justify-center mb-3">
                    <Upload className="w-8 h-8" />
                  </div>
                  <div className="font-semibold text-lg">{snapshot.replace('.json', '')}</div>
                  <div className="text-sm text-blue-100 mt-2">Clique para carregar</div>
                </button>
              ))}
            </div>

            {snapshots.length === 0 && (
              <div className="text-center text-gray-500 py-12">
                <Upload className="w-16 h-16 mx-auto mb-4 opacity-50" />
                <p className="text-lg">Nenhum snapshot disponível</p>
                <p className="text-sm mt-2">Adicione arquivos JSON na pasta /data</p>
              </div>
            )}
          </div>
        </div>
      </div>
    );
  }

  const metrics = dashboardData.sprintMetrics;
  const snapshot = dashboardData.snapshot;

  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-900 via-blue-900 to-gray-900 p-8">
      <div className="max-w-7xl mx-auto" id="dashboard-content">
        {/* Header */}
        <div className="mb-8">
          <div className="flex justify-between items-start mb-6">
            <div>
              <h1 className="text-4xl font-bold text-white mb-2">{snapshot.name}</h1>
              <p className="text-gray-300 text-lg">{snapshot.goal}</p>
              <div className="flex gap-4 mt-3 text-sm text-gray-400">
                <span className="flex items-center gap-1">
                  <Calendar className="w-4 h-4" />
                  {new Date(snapshot.startDate).toLocaleDateString('pt-BR')} - {new Date(snapshot.endDate).toLocaleDateString('pt-BR')}
                </span>
                <span className="flex items-center gap-1">
                  <Users className="w-4 h-4" />
                  {snapshot.team.length} membros
                </span>
              </div>
            </div>
            
            <div className="flex gap-3">
              <button
                onClick={() => setSelectedSnapshot('')}
                className="bg-gray-700 hover:bg-gray-600 text-white px-6 py-3 rounded-lg flex items-center gap-2 transition"
              >
                <Upload className="w-5 h-5" />
                Mudar Sprint
              </button>
              <button
                onClick={exportReport}
                className="bg-green-600 hover:bg-green-700 text-white px-6 py-3 rounded-lg flex items-center gap-2 transition"
              >
                <Download className="w-5 h-5" />
                Exportar Relatório
              </button>
            </div>
          </div>

          {/* View Tabs */}
          <div className="flex gap-2 bg-gray-800 p-1 rounded-lg inline-flex">
            <button
              onClick={() => setView('overview')}
              className={`px-6 py-2 rounded-lg transition ${
                view === 'overview' ? 'bg-blue-600 text-white' : 'text-gray-300 hover:bg-gray-700'
              }`}
            >
              Visão Geral
            </button>
            <button
              onClick={() => setView('team')}
              className={`px-6 py-2 rounded-lg transition ${
                view === 'team' ? 'bg-blue-600 text-white' : 'text-gray-300 hover:bg-gray-700'
              }`}
            >
              Time
            </button>
            <button
              onClick={() => setView('insights')}
              className={`px-6 py-2 rounded-lg transition ${
                view === 'insights' ? 'bg-blue-600 text-white' : 'text-gray-300 hover:bg-gray-700'
              }`}
            >
              Insights
            </button>
          </div>
        </div>

        {/* KPI Cards */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
          <KPICard
            title="Taxa de Conclusão"
            value={`${metrics.completionRate.toFixed(1)}%`}
            subtitle={`${metrics.completedTasks}/${metrics.totalTasks} tarefas`}
            icon={<Target className="w-8 h-8" />}
            trend={metrics.completionRate >= 80 ? 'up' : 'down'}
            color="blue"
          />
          <KPICard
            title="Pontos Entregues"
            value={metrics.completedPoints.toString()}
            subtitle={`${metrics.pointsCompletionRate.toFixed(1)}% do total`}
            icon={<Award className="w-8 h-8" />}
            trend={metrics.pointsCompletionRate >= 80 ? 'up' : 'down'}
            color="green"
          />
          <KPICard
            title="Velocidade"
            value={metrics.velocity.toString()}
            subtitle="pontos por sprint"
            icon={<Zap className="w-8 h-8" />}
            color="purple"
          />
          <KPICard
            title="Cycle Time Médio"
            value={`${(metrics.averageCycleTime / 24).toFixed(1)}d`}
            subtitle={`${metrics.averageCycleTime.toFixed(0)}h total`}
            icon={<Clock className="w-8 h-8" />}
            color="orange"
          />
        </div>

        {view === 'overview' && <OverviewView data={dashboardData} />}
        {view === 'team' && <TeamView data={dashboardData} />}
        {view === 'insights' && <InsightsView data={dashboardData} />}
      </div>
    </div>
  );
}

// KPI Card Component
function KPICard({ title, value, subtitle, icon, trend, color }: any) {
  const colorClasses = {
    blue: 'from-blue-500 to-blue-600',
    green: 'from-green-500 to-green-600',
    purple: 'from-purple-500 to-purple-600',
    orange: 'from-orange-500 to-orange-600'
  };

  return (
    <div className={`bg-gradient-to-br ${colorClasses[color]} rounded-xl shadow-lg p-6 text-white`}>
      <div className="flex justify-between items-start mb-4">
        <div className="opacity-90">{icon}</div>
        {trend && (
          <div className={`flex items-center gap-1 text-sm ${trend === 'up' ? 'text-green-200' : 'text-red-200'}`}>
            {trend === 'up' ? <TrendingUp className="w-4 h-4" /> : <TrendingDown className="w-4 h-4" />}
          </div>
        )}
      </div>
      <div className="text-3xl font-bold mb-1">{value}</div>
      <div className="text-sm opacity-90">{title}</div>
      <div className="text-xs opacity-75 mt-1">{subtitle}</div>
    </div>
  );
}

// Overview View Component
function OverviewView({ data }: { data: DashboardData }) {
  const metrics = data.sprintMetrics;
  
  // Prepare chart data
  const statusData = Object.entries(metrics.tasksByStatus).map(([status, count]) => ({
    name: status.replace('_', ' ').toUpperCase(),
    value: count,
    color: STATUS_COLORS[status]
  }));

  const pointsData = Object.entries(metrics.timeByPoints).map(([points, data]) => ({
    points: `${points}pts`,
    tempo: (data.averageTime / 24).toFixed(1),
    tarefas: data.count
  }));

  const priorityData = Object.entries(metrics.tasksByPriority).map(([priority, count]) => ({
    name: priority.toUpperCase(),
    value: count
  }));

  const typeData = Object.entries(metrics.tasksByType).map(([type, count]) => ({
    name: type.replace('_', ' ').toUpperCase(),
    value: count
  }));

  return (
    <div className="space-y-6">
      {/* Charts Row 1 */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Status Distribution */}
        <div className="bg-white rounded-xl shadow-lg p-6">
          <h3 className="text-xl font-bold text-gray-800 mb-4">Distribuição por Status</h3>
          <ResponsiveContainer width="100%" height={300}>
            <PieChart>
              <Pie
                data={statusData}
                cx="50%"
                cy="50%"
                labelLine={false}
                label={({ name, percent }) => `${name} ${(percent * 100).toFixed(0)}%`}
                outerRadius={100}
                fill="#8884d8"
                dataKey="value"
              >
                {statusData.map((entry, index) => (
                  <Cell key={`cell-${index}`} fill={entry.color} />
                ))}
              </Pie>
              <Tooltip />
            </PieChart>
          </ResponsiveContainer>
        </div>

        {/* Time by Points */}
        <div className="bg-white rounded-xl shadow-lg p-6">
          <h3 className="text-xl font-bold text-gray-800 mb-4">Tempo Médio por Pontuação (dias)</h3>
          <ResponsiveContainer width="100%" height={300}>
            <BarChart data={pointsData}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="points" />
              <YAxis />
              <Tooltip />
              <Legend />
              <Bar dataKey="tempo" fill="#3b82f6" name="Tempo (dias)" />
            </BarChart>
          </ResponsiveContainer>
          <div className="mt-4 text-sm text-gray-600">
            <strong>Insight:</strong> Tarefas de 3 pontos levam em média {pointsData.find(d => d.points === '3pts')?.tempo || 'N/A'} dias
          </div>
        </div>
      </div>

      {/* Charts Row 2 */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Priority Distribution */}
        <div className="bg-white rounded-xl shadow-lg p-6">
          <h3 className="text-xl font-bold text-gray-800 mb-4">Tarefas por Prioridade</h3>
          <ResponsiveContainer width="100%" height={300}>
            <BarChart data={priorityData}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="name" />
              <YAxis />
              <Tooltip />
              <Bar dataKey="value" fill="#f59e0b" />
            </BarChart>
          </ResponsiveContainer>
        </div>

        {/* Type Distribution */}
        <div className="bg-white rounded-xl shadow-lg p-6">
          <h3 className="text-xl font-bold text-gray-800 mb-4">Tarefas por Tipo</h3>
          <ResponsiveContainer width="100%" height={300}>
            <PieChart>
              <Pie
                data={typeData}
                cx="50%"
                cy="50%"
                labelLine={false}
                label={({ name, percent }) => `${name} ${(percent * 100).toFixed(0)}%`}
                outerRadius={100}
                fill="#8884d8"
                dataKey="value"
              >
                {typeData.map((entry, index) => (
                  <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                ))}
              </Pie>
              <Tooltip />
            </PieChart>
          </ResponsiveContainer>
        </div>
      </div>

      {/* Commitment vs Delivery */}
      <div className="bg-white rounded-xl shadow-lg p-6">
        <h3 className="text-xl font-bold text-gray-800 mb-4">Compromisso vs Entrega</h3>
        <div className="grid grid-cols-3 gap-6">
          <div className="text-center">
            <div className="text-4xl font-bold text-blue-600">{metrics.committedTasks}</div>
            <div className="text-gray-600 mt-2">Tarefas Comprometidas</div>
          </div>
          <div className="text-center">
            <div className="text-4xl font-bold text-green-600">{metrics.deliveredTasks}</div>
            <div className="text-gray-600 mt-2">Tarefas Entregues</div>
          </div>
          <div className="text-center">
            <div className={`text-4xl font-bold ${metrics.commitmentAchievement >= 80 ? 'text-green-600' : 'text-red-600'}`}>
              {metrics.commitmentAchievement.toFixed(1)}%
            </div>
            <div className="text-gray-600 mt-2">Taxa de Atingimento</div>
          </div>
        </div>
      </div>

      {/* Quality Metrics */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <div className="bg-white rounded-xl shadow-lg p-6">
          <div className="flex items-center gap-3 mb-3">
            <AlertTriangle className="w-6 h-6 text-orange-500" />
            <h3 className="text-lg font-bold text-gray-800">Retrabalho</h3>
          </div>
          <div className="text-3xl font-bold text-orange-600 mb-2">{metrics.tasksReturned}</div>
          <div className="text-sm text-gray-600">Tarefas que retornaram ({metrics.returnRate.toFixed(1)}%)</div>
        </div>

        <div className="bg-white rounded-xl shadow-lg p-6">
          <div className="flex items-center gap-3 mb-3">
            <AlertCircle className="w-6 h-6 text-red-500" />
            <h3 className="text-lg font-bold text-gray-800">Tempo Bloqueado</h3>
          </div>
          <div className="text-3xl font-bold text-red-600 mb-2">{(metrics.blockedTime / 24).toFixed(1)}d</div>
          <div className="text-sm text-gray-600">{metrics.blockedTime.toFixed(0)} horas totais</div>
        </div>

        <div className="bg-white rounded-xl shadow-lg p-6">
          <div className="flex items-center gap-3 mb-3">
            <Activity className="w-6 h-6 text-blue-500" />
            <h3 className="text-lg font-bold text-gray-800">Lead Time Médio</h3>
          </div>
          <div className="text-3xl font-bold text-blue-600 mb-2">{(metrics.averageLeadTime / 24).toFixed(1)}d</div>
          <div className="text-sm text-gray-600">Da criação até conclusão</div>
        </div>
      </div>
    </div>
  );
}

// Team View Component (continuará no próximo arquivo)
