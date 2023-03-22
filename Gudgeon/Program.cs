using Microsoft.EntityFrameworkCore;
using Discord.Addons.Hosting;
using Discord.WebSocket;
using Gudgeon.Services;

var host = Host.CreateDefaultBuilder()
    .ConfigureDiscordHost((context, config) =>
    {
        config.SocketConfig = new DiscordSocketConfig
        {
            LogLevel = LogSeverity.Debug,
            AlwaysDownloadUsers = true,
            GatewayIntents = GatewayIntents.All,
            MessageCacheSize = 200,
            LogGatewayIntentWarnings = false
        };

        string? token = context.Configuration["Token"];

        if (string.IsNullOrEmpty(token))
            throw new InvalidOperationException("The bot token has not been specified.");

        config.Token = token;
    })
    .UseInteractionService((context, config) =>
    {
        config.LogLevel = LogSeverity.Debug;
        config.UseCompiledLambda = true;
    })
    .ConfigureServices((context, services) =>
    {
        services.AddHostedService<InteractionHandler>();
        services.AddHostedService<AutoroleService>();

        InteractiveConfig interactiveConfig = new()
        {
            LogLevel = LogSeverity.Debug,
            DeferStopPaginatorInteractions = true,
            DefaultTimeout = TimeSpan.FromMinutes(5)
        };

        services.AddSingleton(interactiveConfig);
        services.AddSingleton<InteractiveService>();

        string? connectionString = context.Configuration.GetConnectionString("Default");

        if (string.IsNullOrEmpty(connectionString))
            throw new InvalidOperationException("The database connection string must be specified.");

        ServerVersion serverVersion = ServerVersion.AutoDetect(connectionString);

        services.AddDbContext<GudgeonDbContext>(options => options.UseMySql(connectionString, serverVersion));
    })
    .Build();

await host.RunAsync();