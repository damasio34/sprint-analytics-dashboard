namespace JiraSnapshotGenerator.Models;

public class AppSettings
{
    public JiraSettings JiraSettings { get; set; } = new();
    public SprintSettings SprintSettings { get; set; } = new();
    public TeamSettings TeamSettings { get; set; } = new();
    public MappingSettings MappingSettings { get; set; } = new();
    public OutputSettings OutputSettings { get; set; } = new();
}

public class JiraSettings
{
    public string BaseUrl { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ProjectKey { get; set; } = string.Empty;
    public string DefaultJql { get; set; } = string.Empty;
    public int MaxResults { get; set; } = 1000;
}

public class SprintSettings
{
    public string SprintName { get; set; } = string.Empty;
    public string SprintId { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Goal { get; set; } = string.Empty;
}

public class TeamSettings
{
    public int DefaultCapacity { get; set; } = 40;
    public List<TeamMemberConfig> Members { get; set; } = new();
}

public class TeamMemberConfig
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = "Developer";
    public int Capacity { get; set; } = 40;
    public string JiraUsername { get; set; } = string.Empty;
}

public class MappingSettings
{
    public string StoryPointsField { get; set; } = "customfield_10122";
    public string SprintField { get; set; } = "customfield_10751";
    public Dictionary<string, string> StatusMapping { get; set; } = new();
    public Dictionary<string, string> PriorityMapping { get; set; } = new();
    public Dictionary<string, string> TypeMapping { get; set; } = new();
}

public class OutputSettings
{
    public string OutputDirectory { get; set; } = "./output";
    public bool GenerateSnapshotsJson { get; set; } = true;
    public bool PrettyPrint { get; set; } = true;
}
