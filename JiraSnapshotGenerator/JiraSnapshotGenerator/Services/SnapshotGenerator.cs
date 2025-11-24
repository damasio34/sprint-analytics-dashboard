using System.Text;
using JiraSnapshotGenerator.Models;
using JiraSnapshotGenerator.Models.Dashboard;
using Newtonsoft.Json;

namespace JiraSnapshotGenerator.Services;

public class SnapshotGenerator
{
    private readonly AppSettings _settings;
    private readonly JiraClient _jiraClient;
    private readonly SnapshotConverter _converter;

    public SnapshotGenerator(AppSettings settings)
    {
        _settings = settings;
        _jiraClient = new JiraClient(settings.JiraSettings);
        _converter = new SnapshotConverter(settings);
    }

    public async Task<SprintSnapshot> GenerateSnapshotAsync(string? customJql = null)
    {
        var jql = customJql ?? _settings.JiraSettings.DefaultJql;
        
        Console.WriteLine("🚀 Iniciando geração de snapshot...");
        Console.WriteLine($"📋 JQL: {jql}");
        Console.WriteLine();

        var issues = await _jiraClient.GetAllIssuesWithChangelogAsync(jql);
        
        Console.WriteLine();
        Console.WriteLine("🔄 Convertendo dados do Jira para formato do Dashboard...");
        
        var snapshot = _converter.Convert(issues);
        
        Console.WriteLine($"✅ Snapshot gerado com sucesso!");
        Console.WriteLine($"   📊 Sprint: {snapshot.Sprint.Name}");
        Console.WriteLine($"   👥 Time: {snapshot.Team.Count} membros");
        Console.WriteLine($"   📝 Tasks: {snapshot.Tasks.Count} tarefas");
        Console.WriteLine();

        return snapshot;
    }

    public async Task SaveSnapshotAsync(SprintSnapshot snapshot, string? customFilename = null)
    {
        var outputDir = _settings.OutputSettings.OutputDirectory;
        Directory.CreateDirectory(outputDir);

        var filename = customFilename ?? $"{_settings.SprintSettings.SprintId}.json";
        var fullPath = Path.Combine(outputDir, filename);

        var json = JsonConvert.SerializeObject(
            snapshot,
            _settings.OutputSettings.PrettyPrint 
                ? Formatting.Indented 
                : Formatting.None
        );

        await File.WriteAllTextAsync(fullPath, json, Encoding.UTF8);

        Console.WriteLine($"💾 Snapshot salvo em: {fullPath}");

        // Gerar/Atualizar snapshots.json
        if (_settings.OutputSettings.GenerateSnapshotsJson)
        {
            await UpdateSnapshotsIndexAsync(filename);
        }
    }

    private async Task UpdateSnapshotsIndexAsync(string newSnapshot)
    {
        var outputDir = _settings.OutputSettings.OutputDirectory;
        var indexPath = Path.Combine(outputDir, "snapshots.json");

        SnapshotsIndex index;

        if (File.Exists(indexPath))
        {
            var existingJson = await File.ReadAllTextAsync(indexPath, Encoding.UTF8);
            index = JsonConvert.DeserializeObject<SnapshotsIndex>(existingJson) ?? new SnapshotsIndex();
        }
        else
        {
            index = new SnapshotsIndex();
        }

        // Adicionar novo snapshot se não existir
        if (!index.Snapshots.Contains(newSnapshot))
        {
            index.Snapshots.Add(newSnapshot);
            index.Snapshots.Sort(); // Ordenar alfabeticamente
        }

        var json = JsonConvert.SerializeObject(
            index,
            _settings.OutputSettings.PrettyPrint 
                ? Formatting.Indented 
                : Formatting.None
        );

        await File.WriteAllTextAsync(indexPath, json, Encoding.UTF8);

        Console.WriteLine($"📑 Índice atualizado: {indexPath}");
        Console.WriteLine($"   Total de snapshots: {index.Snapshots.Count}");
        Console.WriteLine();
    }

    public void PrintSummary(SprintSnapshot snapshot)
    {
        Console.WriteLine("=" .PadRight(60, '='));
        Console.WriteLine("📊 RESUMO DO SNAPSHOT");
        Console.WriteLine("=" .PadRight(60, '='));
        Console.WriteLine();

        // Sprint Info
        Console.WriteLine($"🏃 Sprint: {snapshot.Sprint.Name}");
        Console.WriteLine($"   ID: {snapshot.Sprint.Id}");
        Console.WriteLine($"   Início: {snapshot.Sprint.StartDate}");
        Console.WriteLine($"   Fim: {snapshot.Sprint.EndDate}");
        Console.WriteLine($"   Goal: {snapshot.Sprint.Goal}");
        Console.WriteLine();

        // Team Info
        Console.WriteLine($"👥 Time ({snapshot.Team.Count} membros):");
        foreach (var member in snapshot.Team)
        {
            Console.WriteLine($"   • {member.Name} ({member.Role}) - {member.Capacity}h");
        }
        Console.WriteLine();

        // Tasks Stats
        var totalTasks = snapshot.Tasks.Count;
        var completedTasks = snapshot.Tasks.Count(t => t.Status == "done");
        var inProgressTasks = snapshot.Tasks.Count(t => t.Status == "in_progress");
        var blockedTasks = snapshot.Tasks.Count(t => t.Status == "blocked");
        var totalPoints = snapshot.Tasks.Sum(t => t.Points);
        var completedPoints = snapshot.Tasks.Where(t => t.Status == "done").Sum(t => t.Points);

        Console.WriteLine($"📝 Tarefas:");
        Console.WriteLine($"   Total: {totalTasks}");
        Console.WriteLine($"   ✅ Concluídas: {completedTasks} ({(totalTasks > 0 ? (completedTasks * 100.0 / totalTasks):0):F1}%)");
        Console.WriteLine($"   🔄 Em progresso: {inProgressTasks}");
        Console.WriteLine($"   🚫 Bloqueadas: {blockedTasks}");
        Console.WriteLine();

        Console.WriteLine($"📊 Story Points:");
        Console.WriteLine($"   Total: {totalPoints}");
        Console.WriteLine($"   ✅ Entregues: {completedPoints} ({(totalPoints > 0 ? (completedPoints * 100.0 / totalPoints):0):F1}%)");
        Console.WriteLine();

        // Por tipo
        var byType = snapshot.Tasks.GroupBy(t => t.Type).Select(g => new { Type = g.Key, Count = g.Count() });
        Console.WriteLine($"🏷️  Por Tipo:");
        foreach (var type in byType)
        {
            Console.WriteLine($"   • {type.Type}: {type.Count}");
        }
        Console.WriteLine();

        // Por prioridade
        var byPriority = snapshot.Tasks.GroupBy(t => t.Priority).Select(g => new { Priority = g.Key, Count = g.Count() });
        Console.WriteLine($"⚡ Por Prioridade:");
        foreach (var priority in byPriority)
        {
            Console.WriteLine($"   • {priority.Priority}: {priority.Count}");
        }
        Console.WriteLine();

        Console.WriteLine("=" .PadRight(60, '='));
        Console.WriteLine();
    }
}
