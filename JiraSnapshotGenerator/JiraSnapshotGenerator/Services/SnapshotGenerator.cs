using System.Text;
using JiraSnapshotGenerator.Models;
using JiraSnapshotGenerator.Models.Dashboard;
using Newtonsoft.Json;
using Serilog;
using System.Diagnostics;

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
        var totalTime = Stopwatch.StartNew();

        Log.Information("[SNAPSHOT] Iniciando geração de snapshot");
        Log.Information("[SNAPSHOT] JQL: {Jql}", jql);
        Log.Information("[SNAPSHOT] Sprint: {SprintName} (ID: {SprintId})", _settings.SprintSettings.SprintName, _settings.SprintSettings.SprintId);
        Log.Information("[SNAPSHOT] Período: {StartDate} até {EndDate}", _settings.SprintSettings.StartDate, _settings.SprintSettings.EndDate);

        Console.WriteLine("🚀 Iniciando geração de snapshot...");
        Console.WriteLine($"📋 JQL: {jql}");
        Console.WriteLine();

        try
        {
            Log.Debug("[SNAPSHOT] Iniciando coleta de dados do Jira");
            var issues = await _jiraClient.GetAllIssuesWithChangelogAsync(jql);
            Log.Information("[SNAPSHOT] Dados coletados do Jira: {IssueCount} issues", issues.Count);

            Console.WriteLine();
            Console.WriteLine("🔄 Convertendo dados do Jira para formato do Dashboard...");
            Log.Information("[SNAPSHOT] Iniciando conversão dos dados");

            var conversionTime = Stopwatch.StartNew();
            var snapshot = _converter.Convert(issues);
            conversionTime.Stop();

            Log.Information("[SNAPSHOT] Conversão concluída em {ElapsedMs}ms", conversionTime.ElapsedMilliseconds);
            Log.Information("[SNAPSHOT] Dados convertidos:");
            Log.Information("[SNAPSHOT] - Sprint: {SprintName}", snapshot.Sprint.Name);
            Log.Information("[SNAPSHOT] - Time: {TeamMemberCount} membros", snapshot.Team.Count);
            Log.Information("[SNAPSHOT] - Tasks: {TaskCount} tarefas", snapshot.Tasks.Count);
            Log.Information("[SNAPSHOT] - Story Points total: {TotalPoints}", snapshot.Tasks.Sum(t => t.Points));

            // Estatísticas por status
            var statusStats = snapshot.Tasks.GroupBy(t => t.Status)
                .Select(g => new { Status = g.Key, Count = g.Count(), Points = g.Sum(t => t.Points) })
                .OrderByDescending(s => s.Count);

            Log.Information("[SNAPSHOT] Distribuição por status:");
            foreach (var stat in statusStats)
            {
                Log.Information("[SNAPSHOT] - {Status}: {Count} tasks, {Points} pontos", stat.Status, stat.Count, stat.Points);
            }

            totalTime.Stop();
            Log.Information("[SNAPSHOT] Geração de snapshot concluída com sucesso em {TotalElapsedMs}ms ({ElapsedSeconds:F1}s)",
                totalTime.ElapsedMilliseconds, totalTime.ElapsedMilliseconds / 1000.0);

            Console.WriteLine($"✅ Snapshot gerado com sucesso!");
            Console.WriteLine($"   📊 Sprint: {snapshot.Sprint.Name}");
            Console.WriteLine($"   👥 Time: {snapshot.Team.Count} membros");
            Console.WriteLine($"   📝 Tasks: {snapshot.Tasks.Count} tarefas");
            Console.WriteLine();

            return snapshot;
        }
        catch (Exception ex)
        {
            totalTime.Stop();
            Log.Error(ex, "[SNAPSHOT] Erro durante geração do snapshot após {ElapsedMs}ms", totalTime.ElapsedMilliseconds);
            Log.Error("[SNAPSHOT] JQL que causou erro: {Jql}", jql);
            Log.Error("[SNAPSHOT] Configurações: BaseUrl={BaseUrl}, Username={Username}, Project={ProjectKey}",
                _settings.JiraSettings.BaseUrl, _settings.JiraSettings.Username, _settings.JiraSettings.ProjectKey);
            throw;
        }
    }

    public async System.Threading.Tasks.Task SaveSnapshotAsync(SprintSnapshot snapshot, string? customFilename = null)
    {
        var saveTime = Stopwatch.StartNew();
        var outputDir = _settings.OutputSettings.OutputDirectory;
        var filename = customFilename ?? $"{_settings.SprintSettings.SprintId}.json";
        var fullPath = Path.Combine(outputDir, filename);

        Log.Information("[SNAPSHOT] Salvando snapshot");
        Log.Information("[SNAPSHOT] Diretório de saída: {OutputDirectory}", outputDir);
        Log.Information("[SNAPSHOT] Nome do arquivo: {Filename}", filename);
        Log.Information("[SNAPSHOT] Caminho completo: {FullPath}", fullPath);
        Log.Debug("[SNAPSHOT] Pretty print: {PrettyPrint}", _settings.OutputSettings.PrettyPrint);

        try
        {
            // Criar diretório se não existir
            Directory.CreateDirectory(outputDir);
            Log.Debug("[SNAPSHOT] Diretório criado/verificado: {OutputDirectory}", outputDir);

            // Serializar snapshot
            var serializationTime = Stopwatch.StartNew();
            var json = JsonConvert.SerializeObject(
                snapshot,
                _settings.OutputSettings.PrettyPrint
                    ? Formatting.Indented
                    : Formatting.None
            );
            serializationTime.Stop();

            Log.Debug("[SNAPSHOT] Serialização JSON concluída em {ElapsedMs}ms - Tamanho: {JsonSize} bytes",
                serializationTime.ElapsedMilliseconds, json.Length);

            // Salvar arquivo
            var writeTime = Stopwatch.StartNew();
            await File.WriteAllTextAsync(fullPath, json, Encoding.UTF8);
            writeTime.Stop();

            Log.Information("[SNAPSHOT] Arquivo salvo com sucesso em {ElapsedMs}ms", writeTime.ElapsedMilliseconds);
            Log.Information("[SNAPSHOT] Tamanho do arquivo: {FileSize} bytes", json.Length);

            Console.WriteLine($"💾 Snapshot salvo em: {fullPath}");

            // Gerar/Atualizar snapshots.json
            if (_settings.OutputSettings.GenerateSnapshotsJson)
            {
                Log.Debug("[SNAPSHOT] Atualizando índice de snapshots");
                await UpdateSnapshotsIndexAsync(filename);
            }
            else
            {
                Log.Debug("[SNAPSHOT] Geração de snapshots.json desabilitada");
            }

            saveTime.Stop();
            Log.Information("[SNAPSHOT] Salvamento concluído com sucesso em {TotalElapsedMs}ms", saveTime.ElapsedMilliseconds);
        }
        catch (UnauthorizedAccessException ex)
        {
            Log.Error(ex, "[SNAPSHOT] Erro de permissão ao salvar arquivo: {FullPath}", fullPath);
            throw;
        }
        catch (DirectoryNotFoundException ex)
        {
            Log.Error(ex, "[SNAPSHOT] Diretório não encontrado: {OutputDirectory}", outputDir);
            throw;
        }
        catch (IOException ex)
        {
            Log.Error(ex, "[SNAPSHOT] Erro de I/O ao salvar arquivo: {FullPath}", fullPath);
            throw;
        }
        catch (Exception ex)
        {
            saveTime.Stop();
            Log.Error(ex, "[SNAPSHOT] Erro inesperado ao salvar snapshot após {ElapsedMs}ms", saveTime.ElapsedMilliseconds);
            throw;
        }
    }

    private async System.Threading.Tasks.Task UpdateSnapshotsIndexAsync(string newSnapshot)
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
        Log.Information("[SNAPSHOT] Índice de snapshots atualizado com sucesso");
        Log.Information("[SNAPSHOT] Total de snapshots: {TotalSnapshots}", index.Snapshots.Count);

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
