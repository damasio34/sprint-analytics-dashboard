using System.Net.Http.Headers;
using System.Text;
using JiraSnapshotGenerator.Models;
using JiraSnapshotGenerator.Models.Jira;
using Newtonsoft.Json;
using Serilog;
using System.Diagnostics;

namespace JiraSnapshotGenerator.Services;

public class JiraClient
{
    private readonly HttpClient _httpClient;
    private readonly JiraSettings _settings;

    public JiraClient(JiraSettings settings)
    {
        _settings = settings;

        Log.Information("[JIRA] Inicializando cliente Jira");
        Log.Information("[JIRA] URL Base: {BaseUrl}", _settings.BaseUrl);
        Log.Information("[JIRA] Usuário: {Username}", _settings.Username);
        Log.Debug("[JIRA] Max Results: {MaxResults}", _settings.MaxResults);

        try
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_settings.BaseUrl),
                Timeout = TimeSpan.FromMinutes(5) // Timeout de 5 minutos para requests
            };

            Log.Debug("[JIRA] HttpClient criado com timeout de {TimeoutMinutes} minutos", 5);
        }
        catch (UriFormatException ex)
        {
            Log.Error(ex, "[JIRA] URL base inválida: {BaseUrl}", _settings.BaseUrl);
            throw new ArgumentException($"URL base inválida: {_settings.BaseUrl}", nameof(settings), ex);
        }

        // Configurar autenticação Basic
        try
        {
            var authToken = Convert.ToBase64String(
                Encoding.ASCII.GetBytes($"{_settings.Username}:{_settings.Password}")
            );
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", authToken);
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")
            );

            // Adicionar User-Agent para identificação
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("JiraSnapshotGenerator/1.0");

            Log.Debug("[JIRA] Autenticação Basic configurada para usuário: {Username}", _settings.Username);
            Log.Debug("[JIRA] Headers padrão configurados (Accept: application/json, User-Agent: JiraSnapshotGenerator/1.0)");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[JIRA] Erro ao configurar autenticação");
            throw;
        }

        Log.Information("[JIRA] Cliente Jira inicializado com sucesso");
    }

    public async Task<JiraSearchResponse> SearchIssuesAsync(string jql, int startAt = 0, int maxResults = 50)
    {
        var fields = "summary,description,issuetype,status,priority,assignee,creator,created,updated,resolutiondate,customfield_10122,customfield_10751";
        var url = $"/rest/api/2/search?jql={jql}&fields={fields}&startAt={startAt}&maxResults={maxResults}";

        var stopwatch = Stopwatch.StartNew();
        var fullUrl = _httpClient.BaseAddress + url.TrimStart('/');

        Log.Debug("[HTTP] Iniciando requisição para buscar issues");
        Log.Debug("[HTTP] URL: {Url}", fullUrl);
        Log.Debug("[HTTP] JQL: {Jql}", jql);
        Log.Debug("[HTTP] StartAt: {StartAt}, MaxResults: {MaxResults}", startAt, maxResults);

        try
        {
            var response = await _httpClient.GetAsync(url);
            stopwatch.Stop();

            Log.Debug("[HTTP] Resposta recebida em {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
            Log.Debug("[HTTP] Status: {StatusCode} ({StatusDescription})", (int)response.StatusCode, response.StatusCode);

            if (response.Headers?.Any() == true)
            {
                foreach (var header in response.Headers)
                {
                    Log.Debug("[HTTP] Response Header: {HeaderName} = {HeaderValue}", header.Key, string.Join(", ", header.Value));
                }
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Log.Error("[HTTP] Erro na requisição: {StatusCode} {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
                Log.Error("[HTTP] Conteúdo do erro: {ErrorContent}", errorContent);
                response.EnsureSuccessStatusCode(); // Vai gerar a exceção
            }

            var content = await response.Content.ReadAsStringAsync();
            Log.Debug("[HTTP] Tamanho da resposta: {ContentLength} bytes", content.Length);
            Log.Debug("[HTTP] Content-Type: {ContentType}", response.Content.Headers.ContentType?.ToString());

            // Log do início do conteúdo para debug (primeiros 500 caracteres)
            var preview = content.Length > 500 ? content.Substring(0, 500) + "..." : content;
            Log.Debug("[HTTP] Conteúdo (preview): {ContentPreview}", preview);

            var searchResponse = JsonConvert.DeserializeObject<JiraSearchResponse>(content);
            if (searchResponse == null)
            {
                Log.Error("[HTTP] Falha ao deserializar resposta da busca");
                Log.Error("[HTTP] Conteúdo completo: {FullContent}", content);
                throw new InvalidOperationException("Failed to deserialize search response");
            }

            Log.Information("[JIRA] Busca concluída: {IssueCount}/{TotalIssues} issues retornadas", searchResponse.Issues.Count, searchResponse.Total);
            return searchResponse;
        }
        catch (HttpRequestException ex)
        {
            stopwatch.Stop();
            Log.Error(ex, "[HTTP] Erro de rede após {ElapsedMs}ms na URL: {Url}", stopwatch.ElapsedMilliseconds, fullUrl);
            throw;
        }
        catch (TaskCanceledException ex)
        {
            stopwatch.Stop();
            Log.Error(ex, "[HTTP] Timeout após {ElapsedMs}ms na URL: {Url}", stopwatch.ElapsedMilliseconds, fullUrl);
            throw;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            Log.Error(ex, "[HTTP] Erro inesperado após {ElapsedMs}ms na URL: {Url}", stopwatch.ElapsedMilliseconds, fullUrl);
            throw;
        }
    }

    public async Task<JiraIssue> GetIssueWithChangelogAsync(string issueKey)
    {
        var url = $"/rest/api/2/issue/{issueKey}?expand=changelog";
        var stopwatch = Stopwatch.StartNew();
        var fullUrl = _httpClient.BaseAddress + url.TrimStart('/');

        Log.Debug("[HTTP] Buscando issue com changelog: {IssueKey}", issueKey);
        Log.Debug("[HTTP] URL: {Url}", fullUrl);

        try
        {
            var response = await _httpClient.GetAsync(url);
            stopwatch.Stop();

            Log.Debug("[HTTP] Issue {IssueKey} - Resposta em {ElapsedMs}ms - Status: {StatusCode}",
                issueKey, stopwatch.ElapsedMilliseconds, response.StatusCode);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Log.Error("[HTTP] Erro ao buscar issue {IssueKey}: {StatusCode} {ReasonPhrase}",
                    issueKey, response.StatusCode, response.ReasonPhrase);
                Log.Error("[HTTP] Conteúdo do erro: {ErrorContent}", errorContent);
                response.EnsureSuccessStatusCode();
            }

            var content = await response.Content.ReadAsStringAsync();
            Log.Debug("[HTTP] Issue {IssueKey} - Tamanho da resposta: {ContentLength} bytes",
                issueKey, content.Length);

            var issue = JsonConvert.DeserializeObject<JiraIssue>(content);
            if (issue == null)
            {
                Log.Error("[HTTP] Falha ao deserializar issue {IssueKey}", issueKey);
                Log.Error("[HTTP] Conteúdo: {Content}", content);
                throw new InvalidOperationException($"Failed to deserialize issue {issueKey}");
            }

            Log.Debug("[JIRA] Issue {IssueKey} carregada com sucesso - Changelog entries: {ChangelogCount}",
                issueKey, issue.Changelog?.Histories?.Count ?? 0);
            return issue;
        }
        catch (HttpRequestException ex)
        {
            stopwatch.Stop();
            Log.Error(ex, "[HTTP] Erro de rede ao buscar issue {IssueKey} após {ElapsedMs}ms",
                issueKey, stopwatch.ElapsedMilliseconds);
            throw;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            Log.Error(ex, "[HTTP] Erro inesperado ao buscar issue {IssueKey} após {ElapsedMs}ms",
                issueKey, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }

    public async Task<List<JiraIssue>> GetAllIssuesAsync(string jql)
    {
        Log.Information("[JIRA] Iniciando busca de todas as issues com JQL: {Jql}", jql);
        var allIssues = new List<JiraIssue>();
        var startAt = 0;
        var batchSize = 1000; // Usando batch maior conforme solicitado
        var batchCount = 0;
        var totalTime = Stopwatch.StartNew();

        while (true)
        {
            batchCount++;
            Log.Debug("[JIRA] Processando batch {BatchNumber} (startAt={StartAt}, maxResults={MaxResults})",
                batchCount, startAt, batchSize);

            var response = await SearchIssuesAsync(jql, startAt, batchSize);
            allIssues.AddRange(response.Issues);

            Log.Information("[JIRA] Batch {BatchNumber}: {CurrentCount}/{TotalIssues} issues coletadas",
                batchCount, allIssues.Count, response.Total);

            if (allIssues.Count >= response.Total)
            {
                break;
            }

            startAt += batchSize;

            // Delay entre batches para não sobrecarregar o servidor
            if (batchCount > 1)
            {
                await Task.Delay(200);
            }
        }

        totalTime.Stop();
        Log.Information("[JIRA] Busca concluída: {TotalIssues} issues em {BatchCount} batches ({ElapsedMs}ms total)",
            allIssues.Count, batchCount, totalTime.ElapsedMilliseconds);

        return allIssues;
    }

    public async Task<List<JiraIssue>> GetAllIssuesWithChangelogAsync(string jql)
    {
        Log.Information("[JIRA] Iniciando coleta de issues com changelog");
        Log.Information("[JIRA] JQL: {Jql}", jql);

        var totalTime = Stopwatch.StartNew();
        var issues = await GetAllIssuesAsync(jql);
        Log.Information("[JIRA] Encontradas {IssueCount} issues para processar changelog", issues.Count);

        if (issues.Count == 0)
        {
            Log.Warning("[JIRA] Nenhuma issue encontrada com o JQL fornecido");
            return new List<JiraIssue>();
        }

        var issuesWithChangelog = new List<JiraIssue>();
        var processed = 0;
        var errors = 0;
        var successfulChangelogs = 0;

        Console.WriteLine($"\n🔄 Processando changelog de {issues.Count} issues...");

        foreach (var issue in issues)
        {
            processed++;
            var progress = (processed * 100.0 / issues.Count);
            Console.Write($"\r📥 [{processed}/{issues.Count}] ({progress:F1}%) Processando {issue.Key}...");

            Log.Debug("[JIRA] [{Current}/{Total}] Processando issue {IssueKey}", processed, issues.Count, issue.Key);

            try
            {
                var detailedIssue = await GetIssueWithChangelogAsync(issue.Key);
                issuesWithChangelog.Add(detailedIssue);
                successfulChangelogs++;

                // Delay para não sobrecarregar o servidor
                await Task.Delay(150);
            }
            catch (Exception ex)
            {
                errors++;
                Log.Warning(ex, "[JIRA] Erro ao buscar changelog de {IssueKey}: {ErrorMessage}", issue.Key, ex.Message);
                Console.WriteLine($"\n⚠️  Erro ao buscar changelog de {issue.Key}: {ex.Message}");

                // Adiciona a issue mesmo sem changelog completo
                issuesWithChangelog.Add(issue);
                Log.Information("[JIRA] Issue {IssueKey} adicionada sem changelog completo", issue.Key);
            }
        }

        totalTime.Stop();
        Console.WriteLine($"\n✅ Processamento concluído!");

        Log.Information("[JIRA] Coleta de changelog concluída:");
        Log.Information("[JIRA] - Total de issues: {TotalIssues}", issues.Count);
        Log.Information("[JIRA] - Changelogs coletados com sucesso: {SuccessfulChangelogs}", successfulChangelogs);
        Log.Information("[JIRA] - Erros durante coleta: {Errors}", errors);
        Log.Information("[JIRA] - Tempo total: {ElapsedMs}ms ({ElapsedSeconds:F1}s)", totalTime.ElapsedMilliseconds, totalTime.ElapsedMilliseconds / 1000.0);
        Log.Information("[JIRA] - Tempo médio por issue: {AverageMs:F1}ms", (double)totalTime.ElapsedMilliseconds / issues.Count);

        return issuesWithChangelog;
    }
}
