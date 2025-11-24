using System.Net.Http.Headers;
using System.Text;
using JiraSnapshotGenerator.Models;
using JiraSnapshotGenerator.Models.Jira;
using Newtonsoft.Json;

namespace JiraSnapshotGenerator.Services;

public class JiraClient
{
    private readonly HttpClient _httpClient;
    private readonly JiraSettings _settings;

    public JiraClient(JiraSettings settings)
    {
        _settings = settings;
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(_settings.BaseUrl)
        };

        // Configurar autenticação Basic
        var authToken = Convert.ToBase64String(
            Encoding.ASCII.GetBytes($"{_settings.Username}:{_settings.Password}")
        );
        _httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Basic", authToken);
        _httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json")
        );
    }

    public async Task<JiraSearchResponse> SearchIssuesAsync(string jql, int startAt = 0, int maxResults = 50)
    {
        var fields = "summary,description,issuetype,status,priority,assignee,creator,created,updated,resolutiondate,customfield_10122,customfield_10751";
        var url = $"/rest/api/2/search?jql={Uri.EscapeDataString(jql)}&fields={fields}&startAt={startAt}&maxResults={maxResults}";

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<JiraSearchResponse>(content) 
            ?? throw new InvalidOperationException("Failed to deserialize search response");
    }

    public async Task<JiraIssue> GetIssueWithChangelogAsync(string issueKey)
    {
        var url = $"/rest/api/2/issue/{issueKey}?expand=changelog";

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<JiraIssue>(content) 
            ?? throw new InvalidOperationException($"Failed to deserialize issue {issueKey}");
    }

    public async Task<List<JiraIssue>> GetAllIssuesAsync(string jql)
    {
        var allIssues = new List<JiraIssue>();
        var startAt = 0;
        var batchSize = 50; // Jira recomenda batches menores

        while (true)
        {
            var response = await SearchIssuesAsync(jql, startAt, batchSize);
            allIssues.AddRange(response.Issues);

            if (allIssues.Count >= response.Total)
            {
                break;
            }

            startAt += batchSize;
        }

        return allIssues;
    }

    public async Task<List<JiraIssue>> GetAllIssuesWithChangelogAsync(string jql)
    {
        Console.WriteLine($"🔍 Buscando issues com JQL: {jql}");
        
        var issues = await GetAllIssuesAsync(jql);
        Console.WriteLine($"✅ Encontradas {issues.Count} issues");

        var issuesWithChangelog = new List<JiraIssue>();
        var processed = 0;

        foreach (var issue in issues)
        {
            processed++;
            Console.WriteLine($"📥 [{processed}/{issues.Count}] Buscando changelog de {issue.Key}...");

            try
            {
                var detailedIssue = await GetIssueWithChangelogAsync(issue.Key);
                issuesWithChangelog.Add(detailedIssue);
                
                // Delay para não sobrecarregar o servidor
                await Task.Delay(100);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️  Erro ao buscar changelog de {issue.Key}: {ex.Message}");
                // Adiciona a issue mesmo sem changelog completo
                issuesWithChangelog.Add(issue);
            }
        }

        Console.WriteLine($"✅ Changelog coletado de {issuesWithChangelog.Count} issues");
        return issuesWithChangelog;
    }
}
