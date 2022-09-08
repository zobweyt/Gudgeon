using Discord;
using Discord.Addons.Hosting;
using Discord.WebSocket;
using Fergun.Interactive;
using Gudgeon.Services;

var host = Host.CreateDefaultBuilder()
    .ConfigureDiscordHost((context, config) =>
    {
        config.SocketConfig = new DiscordSocketConfig
        {
            LogLevel = LogSeverity.Debug,
            AlwaysDownloadUsers = true,
            GatewayIntents = GatewayIntents.All,
            MessageCacheSize = 200
        };
    
        config.Token = context.Configuration["Token"];
    })
    .UseInteractionService((context, config) =>
    {
        config.LogLevel = LogSeverity.Debug;
        config.UseCompiledLambda = true;
    })
    .ConfigureServices((context, services) =>
    {
        services
        .AddHostedService<InteractionHandler>()
        .AddSingleton<InteractiveService>();
    })
    .Build();

await host.RunAsync();