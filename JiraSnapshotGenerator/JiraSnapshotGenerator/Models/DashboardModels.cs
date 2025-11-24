using Newtonsoft.Json;

namespace JiraSnapshotGenerator.Models.Dashboard;

public class SprintSnapshot
{
    [JsonProperty("sprint")]
    public Sprint Sprint { get; set; } = new();

    [JsonProperty("team")]
    public List<TeamMember> Team { get; set; } = new();

    [JsonProperty("tasks")]
    public List<Task> Tasks { get; set; } = new();
}

public class Sprint
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("startDate")]
    public string StartDate { get; set; } = string.Empty;

    [JsonProperty("endDate")]
    public string EndDate { get; set; } = string.Empty;

    [JsonProperty("goal")]
    public string Goal { get; set; } = string.Empty;

    [JsonProperty("metadata")]
    public SprintMetadata Metadata { get; set; } = new();
}

public class SprintMetadata
{
    [JsonProperty("source")]
    public string Source { get; set; } = "Jira";

    [JsonProperty("generatedAt")]
    public string GeneratedAt { get; set; } = string.Empty;

    [JsonProperty("totalIssues")]
    public int TotalIssues { get; set; }

    [JsonProperty("jiraProject")]
    public string JiraProject { get; set; } = string.Empty;
}

public class TeamMember
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;

    [JsonProperty("role")]
    public string Role { get; set; } = "Developer";

    [JsonProperty("capacity")]
    public int Capacity { get; set; } = 40;
}

public class Task
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;

    [JsonProperty("description")]
    public string? Description { get; set; }

    [JsonProperty("assignee")]
    public string? Assignee { get; set; }

    [JsonProperty("points")]
    public int Points { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; } = "todo";

    [JsonProperty("priority")]
    public string Priority { get; set; } = "medium";

    [JsonProperty("type")]
    public string Type { get; set; } = "feature";

    [JsonProperty("createdAt")]
    public string CreatedAt { get; set; } = string.Empty;

    [JsonProperty("startedAt")]
    public string? StartedAt { get; set; }

    [JsonProperty("completedAt")]
    public string? CompletedAt { get; set; }

    [JsonProperty("statusHistory")]
    public List<StatusChange> StatusHistory { get; set; } = new();

    [JsonProperty("metadata")]
    public TaskMetadata Metadata { get; set; } = new();
}

public class StatusChange
{
    [JsonProperty("from")]
    public string From { get; set; } = string.Empty;

    [JsonProperty("to")]
    public string To { get; set; } = string.Empty;

    [JsonProperty("changedAt")]
    public string ChangedAt { get; set; } = string.Empty;

    [JsonProperty("changedBy")]
    public string ChangedBy { get; set; } = string.Empty;

    [JsonProperty("duration")]
    public double Duration { get; set; }
}

public class TaskMetadata
{
    [JsonProperty("jiraKey")]
    public string JiraKey { get; set; } = string.Empty;

    [JsonProperty("jiraLink")]
    public string JiraLink { get; set; } = string.Empty;

    [JsonProperty("jiraType")]
    public string JiraType { get; set; } = string.Empty;

    [JsonProperty("jiraPriority")]
    public string JiraPriority { get; set; } = string.Empty;
}

public class SnapshotsIndex
{
    [JsonProperty("snapshots")]
    public List<string> Snapshots { get; set; } = new();
}
