using JiraSnapshotGenerator.Models;
using JiraSnapshotGenerator.Services;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace JiraSnapshotGenerator;

class Program
{
    static async Task<int> Main(string[] args)
    {
        // Configurar Serilog
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information)
            .WriteTo.File("logs/jira-snapshot-.txt",
                rollingInterval: RollingInterval.Day,
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Debug)
            .WriteTo.File("logs/jira-requests-.txt",
                rollingInterval: RollingInterval.Day,
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Debug,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}")
            .Enrich.FromLogContext()
            .CreateLogger();

        try
        {
            Log.Information("=== Jira Snapshot Generator ===");
            Log.Information("Dashboard BI - Gerador de Snapshots do Jira");
            Log.Information("Versão 1.0.0");
            Console.WriteLine();

            // Carregar configurações
            var settings = LoadSettings();

            if (settings == null)
            {
                Log.Error("❌ Erro ao carregar configurações!");
                Log.Warning("⚠️  Verifique se o arquivo appsettings.json existe.");
                return 1;
            }

            // Validar configurações
            if (!ValidateSettings(settings))
            {
                return 1;
            }

            // Menu principal
            while (true)
            {
                Console.WriteLine("O que você deseja fazer?");
                Console.WriteLine("1. 🚀 Gerar snapshot com configurações padrão");
                Console.WriteLine("2. 🔧 Gerar snapshot com JQL customizado");
                Console.WriteLine("3. ⚙️  Mostrar configurações atuais");
                Console.WriteLine("4. 📚 Ajuda");
                Console.WriteLine("5. ❌ Sair");
                Console.Write("Digite sua escolha (1-5): ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await GenerateDefaultSnapshot(settings);
                        break;

                    case "2":
                        await GenerateCustomSnapshot(settings);
                        break;

                    case "3":
                        ShowCurrentSettings(settings);
                        break;

                    case "4":
                        ShowHelp();
                        break;

                    case "5":
                        Log.Information("👋 Até logo!");
                        return 0;

                    default:
                        Log.Warning("Opção inválida. Digite um número de 1 a 5.");
                        break;
                }

                Console.WriteLine();
                Console.WriteLine("Pressione qualquer tecla para continuar...");
                Console.ReadKey(true);
                Console.Clear();
            }
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Erro fatal na aplicação");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    static AppSettings? LoadSettings()
    {
        try
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var settings = new AppSettings();
            configuration.Bind(settings);

            return settings;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro ao carregar configurações: {ErrorMessage}", ex.Message);
            return null;
        }
    }

    static bool ValidateSettings(AppSettings settings)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(settings.JiraSettings.BaseUrl))
            errors.Add("URL base do Jira não configurada");

        if (string.IsNullOrWhiteSpace(settings.JiraSettings.Username))
            errors.Add("Usuário do Jira não configurado");

        if (string.IsNullOrWhiteSpace(settings.JiraSettings.Password))
            errors.Add("Senha/Token do Jira não configurado");

        if (settings.TeamSettings.Members.Count == 0)
            errors.Add("Nenhum membro do time configurado");

        if (errors.Count > 0)
        {
            Log.Error("❌ Configurações inválidas:");
            foreach (var error in errors)
            {
                Log.Warning("   • {Error}", error);
            }
            Console.WriteLine();
            Log.Information("💡 Edite o arquivo appsettings.json com suas configurações.");
            return false;
        }

        return true;
    }

    static async Task GenerateDefaultSnapshot(AppSettings settings)
    {
        Console.Clear();
        Log.Information("=== Gerar Snapshot Padrão ===");
        Console.WriteLine();

        var generator = new SnapshotGenerator(settings);

        try
        {
            Log.Information("Gerando snapshot...");
            var snapshot = await generator.GenerateSnapshotAsync();

            generator.PrintSummary(snapshot);

            Console.Write("💾 Deseja salvar este snapshot? (S/n): ");
            var response = Console.ReadLine();
            var shouldSave = string.IsNullOrWhiteSpace(response) || response.ToUpper().StartsWith("S");

            if (shouldSave)
            {
                await generator.SaveSnapshotAsync(snapshot);

                Log.Information("✅ Snapshot salvo com sucesso!");
                Console.WriteLine();
                Log.Information("📋 Próximos passos:");
                Log.Information("   1. Copie os arquivos da pasta {OutputDirectory}", settings.OutputSettings.OutputDirectory);
                Log.Information("   2. Cole na pasta data/ do dashboard");
                Log.Information("   3. Recarregue o dashboard no navegador");
            }
        }
        catch (HttpRequestException ex)
        {
            Log.Error("❌ Erro de conexão com o Jira:");
            Log.Error(ex.Message);
            Console.WriteLine();
            Log.Information("💡 Verifique:");
            Log.Information("   • Se a URL do Jira está correta");
            Log.Information("   • Se suas credenciais estão corretas");
            Log.Information("   • Se você tem acesso à rede/VPN");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro durante a geração do snapshot");
        }
    }

    static async Task GenerateCustomSnapshot(AppSettings settings)
    {
        Console.Clear();
        Log.Information("=== Gerar Snapshot Customizado ===");
        Console.WriteLine();

        Log.Information("Digite o JQL customizado:");
        Log.Information("Exemplo: project=CROSS AND sprint=\"Sprint 1\" AND status=Done");
        Console.WriteLine();

        Console.Write($"JQL ({settings.JiraSettings.DefaultJql}): ");
        var jql = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(jql))
            jql = settings.JiraSettings.DefaultJql;

        Console.WriteLine();

        Console.Write($"Nome da Sprint ({settings.SprintSettings.SprintName}): ");
        var sprintName = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(sprintName))
            sprintName = settings.SprintSettings.SprintName;

        Console.Write($"ID do Snapshot ({settings.SprintSettings.SprintId}): ");
        var sprintId = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(sprintId))
            sprintId = settings.SprintSettings.SprintId;

        // Atualizar temporariamente as configurações
        settings.SprintSettings.SprintName = sprintName;
        settings.SprintSettings.SprintId = sprintId;

        var generator = new SnapshotGenerator(settings);

        try
        {
            Log.Information("Gerando snapshot...");
            var snapshot = await generator.GenerateSnapshotAsync(jql);

            generator.PrintSummary(snapshot);

            Console.Write("💾 Deseja salvar este snapshot? (S/n): ");
            var response = Console.ReadLine();
            var shouldSave = string.IsNullOrWhiteSpace(response) || response.ToUpper().StartsWith("S");

            if (shouldSave)
            {
                var filename = $"{sprintId}.json";
                await generator.SaveSnapshotAsync(snapshot, filename);

                Log.Information("✅ Snapshot salvo com sucesso!");
            }
        }
        catch (HttpRequestException ex)
        {
            Log.Error("❌ Erro de conexão com o Jira:");
            Log.Error(ex.Message);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro durante a geração do snapshot customizado");
        }
    }

    static void ShowCurrentSettings(AppSettings settings)
    {
        Console.Clear();
        Log.Information("=== Configurações Atuais ===");
        Console.WriteLine();

        Console.WriteLine("┌────────────────────────────────────────┬──────────────────────────────────────┐");
        Console.WriteLine("│ Configuração                           │ Valor                                │");
        Console.WriteLine("├────────────────────────────────────────┼──────────────────────────────────────┤");
        Console.WriteLine($"│ 🔗 URL Jira                           │ {settings.JiraSettings.BaseUrl,-36} │");
        Console.WriteLine($"│ 👤 Usuário                            │ {settings.JiraSettings.Username,-36} │");
        Console.WriteLine($"│ 🔑 Senha/Token                        │ {new string('*', 20),-36} │");
        Console.WriteLine($"│ 📁 Projeto                            │ {settings.JiraSettings.ProjectKey,-36} │");
        Console.WriteLine($"│ 🏃 Sprint                             │ {settings.SprintSettings.SprintName,-36} │");
        Console.WriteLine($"│ 📅 Período                            │ {$"{settings.SprintSettings.StartDate:yyyy-MM-dd} até {settings.SprintSettings.EndDate:yyyy-MM-dd}",-36} │");
        Console.WriteLine($"│ 👥 Membros                            │ {settings.TeamSettings.Members.Count.ToString(),-36} │");
        Console.WriteLine($"│ 💾 Output                             │ {settings.OutputSettings.OutputDirectory,-36} │");
        Console.WriteLine("└────────────────────────────────────────┴──────────────────────────────────────┘");
        Console.WriteLine();

        Log.Information("JQL Padrão:");
        Console.WriteLine($"   {settings.JiraSettings.DefaultJql}");
        Console.WriteLine();

        Log.Information("Membros do Time:");
        foreach (var member in settings.TeamSettings.Members)
        {
            Console.WriteLine($"   • {member.Name} ({member.Role}) - {member.Capacity}h - @{member.JiraUsername}");
        }
    }

    static void ShowHelp()
    {
        Console.Clear();
        Log.Information("=== Ajuda - Como Usar ===");
        Console.WriteLine();

        Console.WriteLine("🚀 Primeiros Passos");
        Console.WriteLine("──────────────────────────────────────────────────────────────────");
        Console.WriteLine("1. Configure suas credenciais do Jira no appsettings.json");
        Console.WriteLine("2. Configure os membros do seu time");
        Console.WriteLine("3. Ajuste o JQL padrão conforme sua necessidade");
        Console.WriteLine("4. Execute o programa e escolha uma opção");
        Console.WriteLine();

        Console.WriteLine("⚙️  Configurações Importantes");
        Console.WriteLine("──────────────────────────────────────────────────────────────────");
        Console.WriteLine("BaseUrl: URL completa do seu Jira (ex: http://jira.empresa.com:8080/jira)");
        Console.WriteLine("Username: Seu usuário do Jira");
        Console.WriteLine("Password: Sua senha OU token de API do Jira");
        Console.WriteLine("ProjectKey: Chave do projeto (ex: CROSS, PROJ, etc)");
        Console.WriteLine("DefaultJql: Consulta JQL padrão para buscar issues");
        Console.WriteLine();

        Console.WriteLine("🔧 Campos Customizados");
        Console.WriteLine("──────────────────────────────────────────────────────────────────");
        Console.WriteLine("customfield_10122: Campo de Story Points (varia por instalação)");
        Console.WriteLine("customfield_10751: Campo de Sprint (varia por instalação)");
        Console.WriteLine();
        Console.WriteLine("💡 Para descobrir IDs de campos customizados:");
        Console.WriteLine("   GET /rest/api/2/field - lista todos os campos");
        Console.WriteLine();

        Console.WriteLine("📊 Usando no Dashboard");
        Console.WriteLine("──────────────────────────────────────────────────────────────────");
        Console.WriteLine("1. Após gerar o snapshot, copie os arquivos de ./output/");
        Console.WriteLine("2. Cole na pasta data/ do seu Dashboard BI");
        Console.WriteLine("3. Certifique-se que snapshots.json foi atualizado");
        Console.WriteLine("4. Recarregue o dashboard no navegador (F5)");
        Console.WriteLine();

        Console.WriteLine("📝 Exemplos de JQL");
        Console.WriteLine("──────────────────────────────────────────────────────────────────");
        Console.WriteLine("Buscar issues de uma sprint específica:");
        Console.WriteLine("   project=CROSS AND sprint=\"Sprint 112\"");
        Console.WriteLine();
        Console.WriteLine("Buscar por período:");
        Console.WriteLine("   project=CROSS AND resolved >= \"2025-01-01\" AND resolved <= \"2025-01-31\"");
        Console.WriteLine();
        Console.WriteLine("Buscar por tipo e status:");
        Console.WriteLine("   project=CROSS AND issuetype=Bug AND status=Done");
        Console.WriteLine();
        Console.WriteLine("Combinar múltiplos critérios:");
        Console.WriteLine("   project=CROSS AND assignee=currentUser() AND status IN (\"In Progress\", Done)");
    }
}