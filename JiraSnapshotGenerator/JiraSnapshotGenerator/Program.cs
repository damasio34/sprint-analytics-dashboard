using JiraSnapshotGenerator.Models;
using JiraSnapshotGenerator.Services;
using Microsoft.Extensions.Configuration;
using Spectre.Console;

namespace JiraSnapshotGenerator;

class Program
{
    static async Task<int> Main(string[] args)
    {
        try
        {
            AnsiConsole.Write(
                new FigletText("Jira Snapshot")
                    .LeftJustified()
                    .Color(Color.Blue));

            AnsiConsole.MarkupLine("[blue]Dashboard BI - Gerador de Snapshots do Jira[/]");
            AnsiConsole.MarkupLine("[grey]Versão 1.0.0[/]");
            AnsiConsole.WriteLine();

            // Carregar configurações
            var settings = LoadSettings();

            if (settings == null)
            {
                AnsiConsole.MarkupLine("[red]❌ Erro ao carregar configurações![/]");
                AnsiConsole.MarkupLine("[yellow]⚠️  Verifique se o arquivo appsettings.json existe.[/]");
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
                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[green]O que você deseja fazer?[/]")
                        .AddChoices(
                            "🚀 Gerar snapshot com configurações padrão",
                            "🔧 Gerar snapshot com JQL customizado",
                            "⚙️  Mostrar configurações atuais",
                            "📚 Ajuda",
                            "❌ Sair"
                        ));

                switch (choice)
                {
                    case "🚀 Gerar snapshot com configurações padrão":
                        await GenerateDefaultSnapshot(settings);
                        break;

                    case "🔧 Gerar snapshot com JQL customizado":
                        await GenerateCustomSnapshot(settings);
                        break;

                    case "⚙️  Mostrar configurações atuais":
                        ShowCurrentSettings(settings);
                        break;

                    case "📚 Ajuda":
                        ShowHelp();
                        break;

                    case "❌ Sair":
                        AnsiConsole.MarkupLine("[blue]👋 Até logo![/]");
                        return 0;
                }

                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine("[grey]Pressione qualquer tecla para continuar...[/]");
                Console.ReadKey(true);
                Console.Clear();
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
            return 1;
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
            AnsiConsole.MarkupLine($"[red]Erro ao carregar configurações: {ex.Message}[/]");
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

        if (errors.Any())
        {
            AnsiConsole.MarkupLine("[red]❌ Configurações inválidas:[/]");
            foreach (var error in errors)
            {
                AnsiConsole.MarkupLine($"[yellow]   • {error}[/]");
            }
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[blue]💡 Edite o arquivo appsettings.json com suas configurações.[/]");
            return false;
        }

        return true;
    }

    static async Task GenerateDefaultSnapshot(AppSettings settings)
    {
        AnsiConsole.Clear();
        AnsiConsole.Rule("[blue]Gerar Snapshot Padrão[/]");
        AnsiConsole.WriteLine();

        var generator = new SnapshotGenerator(settings);

        try
        {
            var snapshot = await AnsiConsole.Status()
                .Spinner(Spinner.Known.Dots)
                .StartAsync("Gerando snapshot...", async ctx =>
                {
                    return await generator.GenerateSnapshotAsync();
                });

            generator.PrintSummary(snapshot);

            var shouldSave = AnsiConsole.Confirm("💾 Deseja salvar este snapshot?", true);

            if (shouldSave)
            {
                await generator.SaveSnapshotAsync(snapshot);
                
                AnsiConsole.MarkupLine("[green]✅ Snapshot salvo com sucesso![/]");
                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine("[blue]📋 Próximos passos:[/]");
                AnsiConsole.MarkupLine($"   1. Copie os arquivos da pasta [yellow]{settings.OutputSettings.OutputDirectory}[/]");
                AnsiConsole.MarkupLine("   2. Cole na pasta [yellow]data/[/] do dashboard");
                AnsiConsole.MarkupLine("   3. Recarregue o dashboard no navegador");
            }
        }
        catch (HttpRequestException ex)
        {
            AnsiConsole.MarkupLine($"[red]❌ Erro de conexão com o Jira:[/]");
            AnsiConsole.MarkupLine($"[yellow]{ex.Message}[/]");
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[blue]💡 Verifique:[/]");
            AnsiConsole.MarkupLine("   • Se a URL do Jira está correta");
            AnsiConsole.MarkupLine("   • Se suas credenciais estão corretas");
            AnsiConsole.MarkupLine("   • Se você tem acesso à rede/VPN");
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
        }
    }

    static async Task GenerateCustomSnapshot(AppSettings settings)
    {
        AnsiConsole.Clear();
        AnsiConsole.Rule("[blue]Gerar Snapshot Customizado[/]");
        AnsiConsole.WriteLine();

        AnsiConsole.MarkupLine("[yellow]Digite o JQL customizado:[/]");
        AnsiConsole.MarkupLine("[grey]Exemplo: project=CROSS AND sprint=\"Sprint 1\" AND status=Done[/]");
        AnsiConsole.WriteLine();

        var jql = AnsiConsole.Ask<string>("JQL:", settings.JiraSettings.DefaultJql);

        AnsiConsole.WriteLine();
        
        var sprintName = AnsiConsole.Ask<string>(
            "Nome da Sprint:", 
            settings.SprintSettings.SprintName);

        var sprintId = AnsiConsole.Ask<string>(
            "ID do Snapshot (nome do arquivo):", 
            settings.SprintSettings.SprintId);

        // Atualizar temporariamente as configurações
        settings.SprintSettings.SprintName = sprintName;
        settings.SprintSettings.SprintId = sprintId;

        var generator = new SnapshotGenerator(settings);

        try
        {
            var snapshot = await AnsiConsole.Status()
                .Spinner(Spinner.Known.Dots)
                .StartAsync("Gerando snapshot...", async ctx =>
                {
                    return await generator.GenerateSnapshotAsync(jql);
                });

            generator.PrintSummary(snapshot);

            var shouldSave = AnsiConsole.Confirm("💾 Deseja salvar este snapshot?", true);

            if (shouldSave)
            {
                var filename = $"{sprintId}.json";
                await generator.SaveSnapshotAsync(snapshot, filename);
                
                AnsiConsole.MarkupLine("[green]✅ Snapshot salvo com sucesso![/]");
            }
        }
        catch (HttpRequestException ex)
        {
            AnsiConsole.MarkupLine($"[red]❌ Erro de conexão com o Jira:[/]");
            AnsiConsole.MarkupLine($"[yellow]{ex.Message}[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
        }
    }

    static void ShowCurrentSettings(AppSettings settings)
    {
        AnsiConsole.Clear();
        AnsiConsole.Rule("[blue]Configurações Atuais[/]");
        AnsiConsole.WriteLine();

        var table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("[yellow]Configuração[/]")
            .AddColumn("[green]Valor[/]");

        table.AddRow("🔗 URL Jira", settings.JiraSettings.BaseUrl);
        table.AddRow("👤 Usuário", settings.JiraSettings.Username);
        table.AddRow("🔑 Senha/Token", new string('*', 20));
        table.AddRow("📁 Projeto", settings.JiraSettings.ProjectKey);
        table.AddRow("🏃 Sprint", settings.SprintSettings.SprintName);
        table.AddRow("📅 Período", $"{settings.SprintSettings.StartDate:yyyy-MM-dd} até {settings.SprintSettings.EndDate:yyyy-MM-dd}");
        table.AddRow("👥 Membros", settings.TeamSettings.Members.Count.ToString());
        table.AddRow("💾 Output", settings.OutputSettings.OutputDirectory);

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();

        AnsiConsole.MarkupLine("[blue]JQL Padrão:[/]");
        AnsiConsole.MarkupLine($"[grey]{settings.JiraSettings.DefaultJql}[/]");
        AnsiConsole.WriteLine();

        AnsiConsole.MarkupLine("[blue]Membros do Time:[/]");
        foreach (var member in settings.TeamSettings.Members)
        {
            AnsiConsole.MarkupLine($"   • [yellow]{member.Name}[/] ({member.Role}) - {member.Capacity}h - [grey]@{member.JiraUsername}[/]");
        }
    }

    static void ShowHelp()
    {
        AnsiConsole.Clear();
        AnsiConsole.Rule("[blue]Ajuda - Como Usar[/]");
        AnsiConsole.WriteLine();

        var panel1 = new Panel(
            "[yellow]1.[/] Configure suas credenciais do Jira no [blue]appsettings.json[/]\n" +
            "[yellow]2.[/] Configure os membros do seu time\n" +
            "[yellow]3.[/] Ajuste o JQL padrão conforme sua necessidade\n" +
            "[yellow]4.[/] Execute o programa e escolha uma opção")
        {
            Header = new PanelHeader("🚀 Primeiros Passos"),
            Border = BoxBorder.Rounded
        };
        AnsiConsole.Write(panel1);
        AnsiConsole.WriteLine();

        var panel2 = new Panel(
            "[blue]BaseUrl:[/] URL completa do seu Jira (ex: http://jira.empresa.com:8080/jira)\n" +
            "[blue]Username:[/] Seu usuário do Jira\n" +
            "[blue]Password:[/] Sua senha OU token de API do Jira\n" +
            "[blue]ProjectKey:[/] Chave do projeto (ex: CROSS, PROJ, etc)\n" +
            "[blue]DefaultJql:[/] Consulta JQL padrão para buscar issues")
        {
            Header = new PanelHeader("⚙️  Configurações Importantes"),
            Border = BoxBorder.Rounded
        };
        AnsiConsole.Write(panel2);
        AnsiConsole.WriteLine();

        var panel3 = new Panel(
            "[yellow]customfield_10122:[/] Campo de Story Points (varia por instalação)\n" +
            "[yellow]customfield_10751:[/] Campo de Sprint (varia por instalação)\n\n" +
            "[grey]💡 Para descobrir IDs de campos customizados:[/]\n" +
            "[grey]   GET /rest/api/2/field - lista todos os campos[/]")
        {
            Header = new PanelHeader("🔧 Campos Customizados"),
            Border = BoxBorder.Rounded
        };
        AnsiConsole.Write(panel3);
        AnsiConsole.WriteLine();

        var panel4 = new Panel(
            "[green]1.[/] Após gerar o snapshot, copie os arquivos de [yellow]./output/[/]\n" +
            "[green]2.[/] Cole na pasta [yellow]data/[/] do seu Dashboard BI\n" +
            "[green]3.[/] Certifique-se que [blue]snapshots.json[/] foi atualizado\n" +
            "[green]4.[/] Recarregue o dashboard no navegador (F5)")
        {
            Header = new PanelHeader("📊 Usando no Dashboard"),
            Border = BoxBorder.Rounded
        };
        AnsiConsole.Write(panel4);
        AnsiConsole.WriteLine();

        var panel5 = new Panel(
            "[yellow]Buscar issues de uma sprint específica:[/]\n" +
            "[blue]project=CROSS AND sprint=\"Sprint 112\"[/]\n\n" +
            "[yellow]Buscar por período:[/]\n" +
            "[blue]project=CROSS AND resolved >= \"2025-01-01\" AND resolved <= \"2025-01-31\"[/]\n\n" +
            "[yellow]Buscar por tipo e status:[/]\n" +
            "[blue]project=CROSS AND issuetype=Bug AND status=Done[/]\n\n" +
            "[yellow]Combinar múltiplos critérios:[/]\n" +
            "[blue]project=CROSS AND assignee=currentUser() AND status IN (\"In Progress\", Done)[/]")
        {
            Header = new PanelHeader("📝 Exemplos de JQL"),
            Border = BoxBorder.Rounded
        };
        AnsiConsole.Write(panel5);
    }
}
