// DashboardComponents.tsx - Team and Insights Views

import React, { useState } from 'react';
import {
  BarChart, Bar, LineChart, Line, RadarChart, Radar,
  XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer,
  PolarGrid, PolarAngleAxis, PolarRadiusAxis
} from 'recharts';
import {
  User, TrendingUp, TrendingDown, AlertTriangle, CheckCircle,
  Clock, Target, Award, AlertCircle, Info, Zap
} from 'lucide-react';
import { DashboardData, MemberMetrics, Insight } from './types';

export function TeamView({ data }: { data: DashboardData }) {
  const [selectedMember, setSelectedMember] = useState<string | null>(null);
  
  const memberMetrics = data.memberMetrics;
  const selectedMetrics = selectedMember 
    ? memberMetrics.find(m => m.memberId === selectedMember)
    : null;

  // Team comparison data
  const teamComparisonData = memberMetrics.map(m => ({
    name: m.memberName.split(' ')[0],
    completed: m.tasksCompleted,
    points: m.pointsCompleted,
    cycleTime: (m.averageCycleTime / 24).toFixed(1),
    utilization: m.utilizationRate.toFixed(0)
  }));

  return (
    <div className="space-y-6">
      {/* Team Members Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        {memberMetrics.map(member => (
          <button
            key={member.memberId}
            onClick={() => setSelectedMember(
              selectedMember === member.memberId ? null : member.memberId
            )}
            className={`bg-white rounded-xl shadow-lg p-6 text-left transition transform hover:scale-105 ${
              selectedMember === member.memberId ? 'ring-4 ring-blue-500' : ''
            }`}
          >
            <div className="flex items-center gap-3 mb-4">
              <div className="w-12 h-12 bg-gradient-to-br from-blue-500 to-purple-500 rounded-full flex items-center justify-center">
                <User className="w-6 h-6 text-white" />
              </div>
              <div>
                <div className="font-bold text-gray-800">{member.memberName}</div>
                <div className="text-sm text-gray-500">{member.tasksCompleted} tarefas</div>
              </div>
            </div>

            <div className="space-y-2">
              <div className="flex justify-between text-sm">
                <span className="text-gray-600">Pontos:</span>
                <span className="font-semibold text-gray-800">{member.pointsCompleted}</span>
              </div>
              <div className="flex justify-between text-sm">
                <span className="text-gray-600">Utilização:</span>
                <span className={`font-semibold ${
                  member.utilizationRate > 100 ? 'text-red-600' : 'text-green-600'
                }`}>
                  {member.utilizationRate.toFixed(0)}%
                </span>
              </div>
              <div className="flex justify-between text-sm">
                <span className="text-gray-600">Cycle Time:</span>
                <span className="font-semibold text-gray-800">
                  {(member.averageCycleTime / 24).toFixed(1)}d
                </span>
              </div>
            </div>

            {member.tasksReturned > 0 && (
              <div className="mt-3 pt-3 border-t border-gray-200">
                <div className="flex items-center gap-2 text-sm text-orange-600">
                  <AlertTriangle className="w-4 h-4" />
                  <span>{member.tasksReturned} retrabalho(s)</span>
                </div>
              </div>
            )}
          </button>
        ))}
      </div>

      {/* Team Comparison Charts */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Tasks Completed Comparison */}
        <div className="bg-white rounded-xl shadow-lg p-6">
          <h3 className="text-xl font-bold text-gray-800 mb-4">Tarefas Completadas por Membro</h3>
          <ResponsiveContainer width="100%" height={300}>
            <BarChart data={teamComparisonData}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="name" />
              <YAxis />
              <Tooltip />
              <Legend />
              <Bar dataKey="completed" fill="#3b82f6" name="Tarefas" />
              <Bar dataKey="points" fill="#10b981" name="Pontos" />
            </BarChart>
          </ResponsiveContainer>
        </div>

        {/* Cycle Time Comparison */}
        <div className="bg-white rounded-xl shadow-lg p-6">
          <h3 className="text-xl font-bold text-gray-800 mb-4">Cycle Time Médio (dias)</h3>
          <ResponsiveContainer width="100%" height={300}>
            <BarChart data={teamComparisonData}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="name" />
              <YAxis />
              <Tooltip />
              <Bar dataKey="cycleTime" fill="#f59e0b" name="Cycle Time (dias)" />
            </BarChart>
          </ResponsiveContainer>
        </div>
      </div>

      {/* Individual Member Details */}
      {selectedMetrics && (
        <div className="bg-white rounded-xl shadow-lg p-6">
          <div className="flex items-center gap-3 mb-6">
            <div className="w-16 h-16 bg-gradient-to-br from-blue-500 to-purple-500 rounded-full flex items-center justify-center">
              <User className="w-8 h-8 text-white" />
            </div>
            <div>
              <h2 className="text-2xl font-bold text-gray-800">{selectedMetrics.memberName}</h2>
              <p className="text-gray-600">Análise Individual Detalhada</p>
            </div>
          </div>

          <div className="grid grid-cols-2 md:grid-cols-4 gap-4 mb-6">
            <MetricBox
              label="Tarefas Completadas"
              value={selectedMetrics.tasksCompleted}
              icon={<CheckCircle className="w-5 h-5" />}
              color="green"
            />
            <MetricBox
              label="Pontos Entregues"
              value={selectedMetrics.pointsCompleted}
              icon={<Award className="w-5 h-5" />}
              color="blue"
            />
            <MetricBox
              label="Em Progresso"
              value={selectedMetrics.tasksInProgress}
              icon={<Target className="w-5 h-5" />}
              color="orange"
            />
            <MetricBox
              label="Retrabalhos"
              value={selectedMetrics.tasksReturned}
              icon={<AlertTriangle className="w-5 h-5" />}
              color="red"
            />
          </div>

          <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
            {/* Performance Radar */}
            <div>
              <h3 className="text-lg font-bold text-gray-800 mb-4">Performance Radar</h3>
              <ResponsiveContainer width="100%" height={300}>
                <RadarChart data={[
                  {
                    metric: 'Produtividade',
                    value: Math.min(100, (selectedMetrics.pointsCompleted / data.sprintMetrics.velocity) * 100)
                  },
                  {
                    metric: 'Velocidade',
                    value: Math.max(0, 100 - (selectedMetrics.averageCycleTime / data.sprintMetrics.averageCycleTime) * 50)
                  },
                  {
                    metric: 'Qualidade',
                    value: Math.max(0, 100 - selectedMetrics.returnRate * 2)
                  },
                  {
                    metric: 'Eficiência',
                    value: Math.min(100, selectedMetrics.utilizationRate)
                  },
                  {
                    metric: 'Consistência',
                    value: selectedMetrics.completionTrend.length > 0 ? 80 : 50
                  }
                ]}>
                  <PolarGrid />
                  <PolarAngleAxis dataKey="metric" />
                  <PolarRadiusAxis angle={90} domain={[0, 100]} />
                  <Radar name={selectedMetrics.memberName} dataKey="value" stroke="#3b82f6" fill="#3b82f6" fillOpacity={0.6} />
                  <Tooltip />
                </RadarChart>
              </ResponsiveContainer>
            </div>

            {/* Completion Trend */}
            <div>
              <h3 className="text-lg font-bold text-gray-800 mb-4">Tendência de Conclusão</h3>
              {selectedMetrics.completionTrend.length > 0 ? (
                <ResponsiveContainer width="100%" height={300}>
                  <LineChart data={selectedMetrics.completionTrend}>
                    <CartesianGrid strokeDasharray="3 3" />
                    <XAxis dataKey="date" />
                    <YAxis />
                    <Tooltip />
                    <Line type="monotone" dataKey="completed" stroke="#3b82f6" strokeWidth={2} />
                  </LineChart>
                </ResponsiveContainer>
              ) : (
                <div className="h-[300px] flex items-center justify-center text-gray-500">
                  Dados insuficientes para gráfico de tendência
                </div>
              )}
            </div>
          </div>

          {/* Task Distribution */}
          <div className="mt-6 grid grid-cols-2 gap-6">
            <div>
              <h3 className="text-lg font-bold text-gray-800 mb-3">Distribuição por Status</h3>
              <div className="space-y-2">
                {Object.entries(selectedMetrics.tasksByStatus).map(([status, count]) => (
                  <div key={status} className="flex justify-between items-center p-2 bg-gray-50 rounded">
                    <span className="text-sm text-gray-700 capitalize">{status.replace('_', ' ')}</span>
                    <span className="font-semibold text-gray-800">{count}</span>
                  </div>
                ))}
              </div>
            </div>

            <div>
              <h3 className="text-lg font-bold text-gray-800 mb-3">Distribuição por Tipo</h3>
              <div className="space-y-2">
                {Object.entries(selectedMetrics.tasksByType).map(([type, count]) => (
                  <div key={type} className="flex justify-between items-center p-2 bg-gray-50 rounded">
                    <span className="text-sm text-gray-700 capitalize">{type.replace('_', ' ')}</span>
                    <span className="font-semibold text-gray-800">{count}</span>
                  </div>
                ))}
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

export function InsightsView({ data }: { data: DashboardData }) {
  const insights = data.insights;
  
  const getInsightIcon = (type: string) => {
    switch (type) {
      case 'warning': return <AlertTriangle className="w-6 h-6" />;
      case 'danger': return <AlertCircle className="w-6 h-6" />;
      case 'success': return <CheckCircle className="w-6 h-6" />;
      default: return <Info className="w-6 h-6" />;
    }
  };

  const getInsightColor = (type: string) => {
    switch (type) {
      case 'warning': return 'orange';
      case 'danger': return 'red';
      case 'success': return 'green';
      default: return 'blue';
    }
  };

  const colorClasses = {
    orange: 'bg-orange-100 border-orange-500 text-orange-900',
    red: 'bg-red-100 border-red-500 text-red-900',
    green: 'bg-green-100 border-green-500 text-green-900',
    blue: 'bg-blue-100 border-blue-500 text-blue-900'
  };

  const iconColorClasses = {
    orange: 'text-orange-600',
    red: 'text-red-600',
    green: 'text-green-600',
    blue: 'text-blue-600'
  };

  // Group insights by category
  const insightsByCategory = insights.reduce((acc, insight) => {
    if (!acc[insight.category]) {
      acc[insight.category] = [];
    }
    acc[insight.category].push(insight);
    return acc;
  }, {} as { [key: string]: Insight[] });

  return (
    <div className="space-y-6">
      {/* Summary Cards */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
        <div className="bg-white rounded-xl shadow-lg p-6">
          <div className="text-center">
            <div className="text-4xl font-bold text-gray-800">{insights.length}</div>
            <div className="text-gray-600 mt-2">Total de Insights</div>
          </div>
        </div>
        <div className="bg-white rounded-xl shadow-lg p-6">
          <div className="text-center">
            <div className="text-4xl font-bold text-red-600">
              {insights.filter(i => i.type === 'danger').length}
            </div>
            <div className="text-gray-600 mt-2">Críticos</div>
          </div>
        </div>
        <div className="bg-white rounded-xl shadow-lg p-6">
          <div className="text-center">
            <div className="text-4xl font-bold text-orange-600">
              {insights.filter(i => i.type === 'warning').length}
            </div>
            <div className="text-gray-600 mt-2">Avisos</div>
          </div>
        </div>
        <div className="bg-white rounded-xl shadow-lg p-6">
          <div className="text-center">
            <div className="text-4xl font-bold text-green-600">
              {insights.filter(i => i.type === 'success').length}
            </div>
            <div className="text-gray-600 mt-2">Sucessos</div>
          </div>
        </div>
      </div>

      {/* Insights by Category */}
      {Object.entries(insightsByCategory).map(([category, categoryInsights]) => (
        <div key={category} className="bg-white rounded-xl shadow-lg p-6">
          <h2 className="text-2xl font-bold text-gray-800 mb-4 capitalize flex items-center gap-2">
            <Zap className="w-6 h-6 text-blue-600" />
            {category.replace('_', ' ')}
          </h2>

          <div className="space-y-4">
            {categoryInsights.map(insight => {
              const color = getInsightColor(insight.type);
              return (
                <div
                  key={insight.id}
                  className={`border-l-4 rounded-lg p-6 ${colorClasses[color as keyof typeof colorClasses]}`}
                >
                  <div className="flex items-start gap-4">
                    <div className={iconColorClasses[color as keyof typeof iconColorClasses]}>
                      {getInsightIcon(insight.type)}
                    </div>
                    <div className="flex-1">
                      <div className="flex items-center gap-3 mb-2">
                        <h3 className="text-lg font-bold">{insight.title}</h3>
                        <div className="flex gap-1">
                          {[...Array(5)].map((_, i) => (
                            <div
                              key={i}
                              className={`w-2 h-2 rounded-full ${
                                i < insight.severity ? 'bg-current' : 'bg-gray-300'
                              }`}
                            />
                          ))}
                        </div>
                      </div>
                      <p className="text-sm mb-3">{insight.description}</p>

                      {insight.recommendations && insight.recommendations.length > 0 && (
                        <div className="mt-4">
                          <div className="font-semibold text-sm mb-2">Recomendações:</div>
                          <ul className="list-disc list-inside space-y-1 text-sm">
                            {insight.recommendations.map((rec, index) => (
                              <li key={index}>{rec}</li>
                            ))}
                          </ul>
                        </div>
                      )}
                    </div>
                  </div>
                </div>
              );
            })}
          </div>
        </div>
      ))}

      {/* Action Items */}
      <div className="bg-gradient-to-r from-blue-600 to-purple-600 rounded-xl shadow-lg p-8 text-white">
        <h2 className="text-2xl font-bold mb-4">Próximos Passos Recomendados</h2>
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div className="bg-white bg-opacity-20 rounded-lg p-4">
            <div className="flex items-center gap-2 mb-2">
              <Target className="w-5 h-5" />
              <div className="font-semibold">Curto Prazo (Esta Sprint)</div>
            </div>
            <ul className="text-sm space-y-1 list-disc list-inside">
              <li>Redistribuir tarefas de membros sobrecarregados</li>
              <li>Resolver bloqueios pendentes</li>
              <li>Revisar tarefas com alto tempo de ciclo</li>
            </ul>
          </div>
          <div className="bg-white bg-opacity-20 rounded-lg p-4">
            <div className="flex items-center gap-2 mb-2">
              <TrendingUp className="w-5 h-5" />
              <div className="font-semibold">Longo Prazo (Próximas Sprints)</div>
            </div>
            <ul className="text-sm space-y-1 list-disc list-inside">
              <li>Melhorar processo de estimativa</li>
              <li>Implementar Definition of Done mais rigoroso</li>
              <li>Aumentar cobertura de code review</li>
            </ul>
          </div>
        </div>
      </div>
    </div>
  );
}

function MetricBox({ label, value, icon, color }: any) {
  const colorClasses = {
    green: 'text-green-600',
    blue: 'text-blue-600',
    orange: 'text-orange-600',
    red: 'text-red-600'
  };

  return (
    <div className="bg-gray-50 rounded-lg p-4">
      <div className={`${colorClasses[color]} mb-2`}>{icon}</div>
      <div className="text-2xl font-bold text-gray-800">{value}</div>
      <div className="text-sm text-gray-600">{label}</div>
    </div>
  );
}

