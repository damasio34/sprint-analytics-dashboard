using JiraSnapshotGenerator.Models;
using JiraSnapshotGenerator.Models.Dashboard;
using JiraSnapshotGenerator.Models.Jira;

namespace JiraSnapshotGenerator.Services;

public class SnapshotConverter
{
    private readonly AppSettings _settings;

    public SnapshotConverter(AppSettings settings)
    {
        _settings = settings;
    }

    public SprintSnapshot Convert(List<JiraIssue> jiraIssues)
    {
        var snapshot = new SprintSnapshot
        {
            Sprint = CreateSprint(),
            Team = CreateTeam(),
            Tasks = ConvertTasks(jiraIssues)
        };

        snapshot.Sprint.Metadata.TotalIssues = snapshot.Tasks.Count;
        return snapshot;
    }

    private Sprint CreateSprint()
    {
        return new Sprint
        {
            Id = _settings.SprintSettings.SprintId,
            Name = _settings.SprintSettings.SprintName,
            StartDate = _settings.SprintSettings.StartDate.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            EndDate = _settings.SprintSettings.EndDate.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            Goal = _settings.SprintSettings.Goal,
            Metadata = new SprintMetadata
            {
                Source = "Jira",
                GeneratedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                JiraProject = _settings.JiraSettings.ProjectKey
            }
        };
    }

    private List<TeamMember> CreateTeam()
    {
        return _settings.TeamSettings.Members.Select((m, index) => new TeamMember
        {
            Id = $"member-{index + 1}",
            Name = m.Name,
            Email = m.Email,
            Role = m.Role,
            Capacity = m.Capacity
        }).ToList();
    }

    private List<Models.Dashboard.Task> ConvertTasks(List<JiraIssue> jiraIssues)
    {
        var tasks = new List<Models.Dashboard.Task>();

        foreach (var issue in jiraIssues)
        {
            tasks.Add(ConvertTask(issue));
        }

        return tasks;
    }

    private Models.Dashboard.Task ConvertTask(JiraIssue issue)
    {
        var task = new Models.Dashboard.Task
        {
            Id = issue.Key,
            Title = issue.Fields.Summary ?? "Sem título",
            Description = issue.Fields.Description,
            Assignee = GetAssigneeId(issue.Fields.Assignee),
            Points = GetStoryPoints(issue.Fields.StoryPoints),
            Status = MapStatus(issue.Fields.Status?.Name ?? "Open"),
            Priority = MapPriority(issue.Fields.Priority?.Name ?? "Major"),
            Type = MapType(issue.Fields.IssueType?.Name ?? "Task"),
            CreatedAt = issue.Fields.Created.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            StartedAt = GetStartedAt(issue),
            CompletedAt = GetCompletedAt(issue),
            StatusHistory = BuildStatusHistory(issue),
            Metadata = new TaskMetadata
            {
                JiraKey = issue.Key,
                JiraLink = issue.Self,
                JiraType = issue.Fields.IssueType?.Name ?? "Unknown",
                JiraPriority = issue.Fields.Priority?.Name ?? "Unknown"
            }
        };

        return task;
    }

    private string? GetAssigneeId(JiraUser? assignee)
    {
        if (assignee == null) return null;

        var member = _settings.TeamSettings.Members
            .FirstOrDefault(m => m.JiraUsername.Equals(assignee.Name, StringComparison.OrdinalIgnoreCase));

        return member != null 
            ? $"member-{_settings.TeamSettings.Members.IndexOf(member) + 1}" 
            : null;
    }

    private int GetStoryPoints(double? storyPoints)
    {
        if (!storyPoints.HasValue) return 0;

        // Arredondar para inteiro
        var points = (int)Math.Round(storyPoints.Value);

        // Validar se é um valor de Fibonacci válido
        var validPoints = new[] { 0, 1, 2, 3, 5, 8, 13 };
        if (!validPoints.Contains(points))
        {
            // Encontrar o valor mais próximo
            points = validPoints.OrderBy(p => Math.Abs(p - points)).First();
        }

        return points;
    }

    private string MapStatus(string jiraStatus)
    {
        if (_settings.MappingSettings.StatusMapping.TryGetValue(jiraStatus, out var mappedStatus))
        {
            return mappedStatus;
        }

        // Fallback baseado em categoria
        return jiraStatus.ToLower() switch
        {
            var s when s.Contains("progress") => "in_progress",
            var s when s.Contains("review") => "in_review",
            var s when s.Contains("test") => "in_review",
            var s when s.Contains("done") || s.Contains("closed") || s.Contains("resolved") => "done",
            var s when s.Contains("block") => "blocked",
            _ => "todo"
        };
    }

    private string MapPriority(string jiraPriority)
    {
        if (_settings.MappingSettings.PriorityMapping.TryGetValue(jiraPriority, out var mappedPriority))
        {
            return mappedPriority;
        }

        return jiraPriority.ToLower() switch
        {
            "blocker" or "critical" => "urgent",
            "major" => "high",
            "minor" => "medium",
            "trivial" => "low",
            _ => "medium"
        };
    }

    private string MapType(string jiraType)
    {
        if (_settings.MappingSettings.TypeMapping.TryGetValue(jiraType, out var mappedType))
        {
            return mappedType;
        }

        return jiraType.ToLower() switch
        {
            var t when t.Contains("bug") => "bug",
            var t when t.Contains("improve") => "improvement",
            var t when t.Contains("feature") => "feature",
            var t when t.Contains("technical") => "technical_debt",
            _ => "feature"
        };
    }

    private string? GetStartedAt(JiraIssue issue)
    {
        if (issue.Changelog == null) return null;

        var startTransition = issue.Changelog.Histories
            .SelectMany(h => h.Items.Select(i => new { History = h, Item = i }))
            .Where(x => x.Item.Field == "status" && 
                       (x.Item.ToString == "In Progress" || 
                        x.Item.ToString?.Contains("Progress") == true))
            .OrderBy(x => x.History.Created)
            .FirstOrDefault();

        return startTransition?.History.Created.ToString("yyyy-MM-ddTHH:mm:ssZ");
    }

    private string? GetCompletedAt(JiraIssue issue)
    {
        if (issue.Fields.ResolutionDate.HasValue)
        {
            return issue.Fields.ResolutionDate.Value.ToString("yyyy-MM-ddTHH:mm:ssZ");
        }

        if (issue.Changelog == null) return null;

        var doneTransition = issue.Changelog.Histories
            .SelectMany(h => h.Items.Select(i => new { History = h, Item = i }))
            .Where(x => x.Item.Field == "status" && 
                       (x.Item.ToString == "Closed" || 
                        x.Item.ToString == "Resolved" ||
                        x.Item.ToString == "Done"))
            .OrderByDescending(x => x.History.Created)
            .FirstOrDefault();

        return doneTransition?.History.Created.ToString("yyyy-MM-ddTHH:mm:ssZ");
    }

    private List<StatusChange> BuildStatusHistory(JiraIssue issue)
    {
        var history = new List<StatusChange>();

        if (issue.Changelog == null) return history;

        var statusChanges = issue.Changelog.Histories
            .SelectMany(h => h.Items
                .Where(i => i.Field == "status")
                .Select(i => new
                {
                    History = h,
                    Item = i
                }))
            .OrderBy(x => x.History.Created)
            .ToList();

        DateTime? previousTime = issue.Fields.Created;
        string? previousStatus = null;

        foreach (var change in statusChanges)
        {
            var from = MapStatus(change.Item.FromString ?? "Open");
            var to = MapStatus(change.Item.ToString ?? "Open");
            var changedAt = change.History.Created;

            // Calcular duração no status anterior
            var duration = previousTime.HasValue 
                ? (changedAt - previousTime.Value).TotalHours 
                : 0;

            if (previousStatus != null)
            {
                history.Add(new StatusChange
                {
                    From = previousStatus,
                    To = from,
                    ChangedAt = previousTime!.Value.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    ChangedBy = change.History.Author?.DisplayName ?? "Unknown",
                    Duration = Math.Round(duration, 2)
                });
            }

            history.Add(new StatusChange
            {
                From = from,
                To = to,
                ChangedAt = changedAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                ChangedBy = change.History.Author?.DisplayName ?? "Unknown",
                Duration = 0 // Será calculado na próxima iteração
            });

            previousTime = changedAt;
            previousStatus = to;
        }

        // Adicionar duração do status atual
        if (history.Count > 0 && previousTime.HasValue)
        {
            var lastChange = history.Last();
            var currentDuration = (DateTime.UtcNow - previousTime.Value).TotalHours;
            lastChange.Duration = Math.Round(currentDuration, 2);
        }

        return history;
    }
}
