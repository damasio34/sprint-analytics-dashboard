using Newtonsoft.Json;

namespace JiraSnapshotGenerator.Models.Jira;

public class JiraSearchResponse
{
    [JsonProperty("expand")]
    public string? Expand { get; set; }

    [JsonProperty("startAt")]
    public int StartAt { get; set; }

    [JsonProperty("maxResults")]
    public int MaxResults { get; set; }

    [JsonProperty("total")]
    public int Total { get; set; }

    [JsonProperty("issues")]
    public List<JiraIssue> Issues { get; set; } = new();
}

public class JiraIssue
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("key")]
    public string Key { get; set; } = string.Empty;

    [JsonProperty("self")]
    public string Self { get; set; } = string.Empty;

    [JsonProperty("fields")]
    public JiraFields Fields { get; set; } = new();

    [JsonProperty("changelog")]
    public JiraChangelog? Changelog { get; set; }
}

public class JiraFields
{
    [JsonProperty("summary")]
    public string? Summary { get; set; }

    [JsonProperty("description")]
    public string? Description { get; set; }

    [JsonProperty("issuetype")]
    public JiraIssueType? IssueType { get; set; }

    [JsonProperty("status")]
    public JiraStatus? Status { get; set; }

    [JsonProperty("priority")]
    public JiraPriority? Priority { get; set; }

    [JsonProperty("assignee")]
    public JiraUser? Assignee { get; set; }

    [JsonProperty("creator")]
    public JiraUser? Creator { get; set; }

    [JsonProperty("created")]
    public DateTime Created { get; set; }

    [JsonProperty("updated")]
    public DateTime Updated { get; set; }

    [JsonProperty("resolutiondate")]
    public DateTime? ResolutionDate { get; set; }

    // Campo customizado para Story Points
    [JsonProperty("customfield_10122")]
    public double? StoryPoints { get; set; }

    // Campo customizado para Sprint
    [JsonProperty("customfield_10751")]
    public object? Sprint { get; set; }
}

public class JiraIssueType
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("subtask")]
    public bool Subtask { get; set; }
}

public class JiraStatus
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("statusCategory")]
    public JiraStatusCategory? StatusCategory { get; set; }
}

public class JiraStatusCategory
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("key")]
    public string Key { get; set; } = string.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;
}

public class JiraPriority
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;
}

public class JiraUser
{
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("key")]
    public string Key { get; set; } = string.Empty;

    [JsonProperty("emailAddress")]
    public string? EmailAddress { get; set; }

    [JsonProperty("displayName")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonProperty("active")]
    public bool Active { get; set; }
}

public class JiraChangelog
{
    [JsonProperty("startAt")]
    public int StartAt { get; set; }

    [JsonProperty("maxResults")]
    public int MaxResults { get; set; }

    [JsonProperty("total")]
    public int Total { get; set; }

    [JsonProperty("histories")]
    public List<JiraHistory> Histories { get; set; } = new();
}

public class JiraHistory
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("author")]
    public JiraUser? Author { get; set; }

    [JsonProperty("created")]
    public DateTime Created { get; set; }

    [JsonProperty("items")]
    public List<JiraChangeItem> Items { get; set; } = new();
}

public class JiraChangeItem
{
    [JsonProperty("field")]
    public string Field { get; set; } = string.Empty;

    [JsonProperty("fieldtype")]
    public string FieldType { get; set; } = string.Empty;

    [JsonProperty("from")]
    public string? From { get; set; }

    [JsonProperty("fromString")]
    public string? FromString { get; set; }

    [JsonProperty("to")]
    public string? To { get; set; }

    [JsonProperty("toString")]
    public string? ToString { get; set; }
}
